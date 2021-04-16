using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;
using Foundation;
using Random = UnityEngine.Random;

namespace Game
{
    public sealed class GameManager : AbstractService<IGameManager>, IGameManager, IOnUpdate, IOnStateActivate
    {
        [Inject] ISceneState state = default;
        [Inject] VisitorAvatar.Factory _visitorAvatarFactory = default;
        [Inject] private ILocalizationManager _localizationManager = default;
        
        public int VisitorsInRowCount = 5;
        public float SecondsPerDay = 180;
        public VisitorsComponents_SO visitorsComponentsSo;
        

        public ObserverList<IOnDayBegin> OnNewDayBegin { get; } = new ObserverList<IOnDayBegin>();
        public ObserverList<IOnDayEnd> OnDayEnd { get; } = new ObserverList<IOnDayEnd>();
        public ObserverList<IOnTimerChanged> OnTimerChanged { get; } = new ObserverList<IOnTimerChanged>();
        

        private List<Visitor> _preparedVisitors = new List<Visitor>();
        public List<Visitor> PreparedVisitors { get { return _preparedVisitors;} set {_preparedVisitors = value;}}
        

        private List<Visitor> _visitorsInRowThisDay = new List<Visitor>();
        public List<Visitor> VisitorsInRowThisDay { get { return _visitorsInRowThisDay;} set {_visitorsInRowThisDay = value;}}
        
        public Visitor VisitorInFocus { get; set; }
        

        private List<VisitorAvatar> _visitorAvatarsCreated = new List<VisitorAvatar>();
        public List<VisitorAvatar> VisitorsAvatarCreated { get { return _visitorAvatarsCreated;} set {_visitorAvatarsCreated = value;}}
        
        
        private int _day;
        private bool isGameStarted;
        private bool isTimeStoped;
        public int Day { get { return _day;} set {_day = value;}}
        

        private float _dayTimeLeft;
        public float DayTimeLeft { get { return _dayTimeLeft;} set {_dayTimeLeft = value;}}
        
      
        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnActivate);
            Observe(state.OnUpdate);
        }
        
        void IOnStateActivate.Do()
        {
            CreateVisitorsList(visitorsComponentsSo);
        }

        void Start()
        {
            Invoke("StartNewDay", 0.1f);
        }

        void IOnUpdate.Do(float deltaTime)
        {
            if(!isGameStarted || isTimeStoped)
                return;
            
            DayTimeLeft -= deltaTime;
            
            foreach (var it in OnTimerChanged.Enumerate())
                it.Do();
           
            if ( DayTimeLeft <= 0 )
            {
                EndDay();
            }
        }

        public void CreateVisitorsList(VisitorsComponents_SO visitorsComponentsSo)
        {
           var so = visitorsComponentsSo;
           if (so.Names.Length != so.Portraits.Length || so.Portraits.Length != so.Replics.Length ||
               so.Replics.Length != so.Surnames.Length)
               Debug.Log("Visitors component`s length are no equal. Check it!");

           List<NameUnit> _namesList = so.Names.ToList();
           List<LocalizedString> _surnamesList = so.Surnames.ToList();
           List<LocalizedString> _replicsList = so.Replics.ToList();
           List<PortraitUnit> _portraitsList = so.Portraits.ToList();
           List<PortraitUnit> alreadyUsedPortrats = new List<PortraitUnit>();
           
           
           foreach (var portrait in _portraitsList)
           {
               bool isFemale;
               int rndPortrait;
               
               var visitor = new Visitor();

               do
               {
                   rndPortrait = Random.Range(0, _portraitsList.Count);
               } while (alreadyUsedPortrats.Contains(_portraitsList[rndPortrait]));

              visitor.Portrait = _portraitsList[rndPortrait].Portrait;
              isFemale = _portraitsList[rndPortrait].isFemale;
              alreadyUsedPortrats.Add(_portraitsList[rndPortrait]);

               if (isFemale)
               {
                   var _femailNames = _namesList.Where(x => x.isFemale).ToList();
                   var rndNames = Random.Range(0, _femailNames.Count);
                   NameUnit _name = _femailNames[rndNames];
                   visitor.Name = _localizationManager.GetString(_name.Name);
                   _namesList.Remove(_name);
               }
               else
               {
                   var _mailNames = _namesList.Where(x => !x.isFemale).ToList();
                   var rndNames = Random.Range(0, _mailNames.Count);
                   NameUnit _name = _mailNames[rndNames];
                   visitor.Name = _localizationManager.GetString(_name.Name);
                   _namesList.Remove(_name);
               }
               
               var rndSurNames = Random.Range(0, _surnamesList.Count);
          
               if (isFemale)
                   visitor.SurName = _localizationManager.GetString(_surnamesList[rndSurNames])  + "a";
               else
                   visitor.SurName = _localizationManager.GetString(_surnamesList[rndSurNames]);
               _surnamesList.RemoveAt(rndSurNames);


               var rndReplics = Random.Range(0, _replicsList.Count);
               visitor.Replic = _localizationManager.GetString(_replicsList[rndReplics]);
               _replicsList.RemoveAt(rndReplics);

               _preparedVisitors.Add(visitor);
           }
        }

        public void StartNewDay()
        {
            if(!isGameStarted)
                isGameStarted = true;
            
            if(isTimeStoped)
                isTimeStoped = false;
            
            ++Day;
            DayTimeLeft = SecondsPerDay;
            
            CreateVisitorsInRowForDay();

            CreateVisitorAvatars();
            
            foreach (var it in OnNewDayBegin.Enumerate())
                it.Do();
        }

        private void CreateVisitorsInRowForDay()
        {
            if (_preparedVisitors.Count >= VisitorsInRowCount)
            {
                for (int i = 0; i < VisitorsInRowCount; i++)
                {
                    var rnd = Random.Range(0, _preparedVisitors.Count);
                    VisitorsInRowThisDay.Add(_preparedVisitors[rnd]);
                    _preparedVisitors.RemoveAt(rnd);
                }
            }
            else
            {
                for (int i = 0; i < _preparedVisitors.Count; i++)
                {
                    var rnd = Random.Range(0, _preparedVisitors.Count);
                    VisitorsInRowThisDay.Add(_preparedVisitors[rnd]);
                }
            }
        }

        public void EndDay()
        {
            isTimeStoped = true;
            _dayTimeLeft = 0;
            
            VisitorInFocus = null;

            DeleteVisitorAvatarCreated();

            _visitorAvatarsCreated.Clear();
            VisitorsInRowThisDay.Clear();
            
            foreach (var it in OnDayEnd.Enumerate())
                it.Do();
            
        }

        public void CreateVisitorAvatars()
        {
            for (int i = 0; i < VisitorsInRowThisDay.Count; i++)
            {
                var _visitorAvatar = _visitorAvatarFactory.Create();
                _visitorAvatarsCreated.Add(_visitorAvatar);
                
                _visitorAvatar.VisitorInfo = VisitorsInRowThisDay[i];
                _visitorAvatar.Portrait.sprite = VisitorsInRowThisDay[i].Portrait;
                _visitorAvatar.NameText.text = VisitorsInRowThisDay[i].Name.ToString();
                _visitorAvatar.SurnameText.text = VisitorsInRowThisDay[i].SurName;
            }
        }

        public void DestroyVisitorAvatar(VisitorAvatar avatar)
        {
            avatar.OnDespawned();
        }

        public void SetVisitorInFocus(Visitor visitor)
        {
            VisitorInFocus = visitor;
            VisitorsInRowThisDay.Remove(visitor);
        }

        public void DeleteVisitorAvatarCreated()
        {
            if (_visitorAvatarsCreated.Count == 0)
                return;

            foreach (var avatar in _visitorAvatarsCreated)
            {
                DestroyVisitorAvatar(avatar);
            }
        }

        public void CreateVisitorAvatarAfterLoadGame()
        {
            /*foreach (var avatar in _visitorAvatarsCreated)
            {
                DestroyVisitorAvatar(avatar);
            }*/

            CreateVisitorAvatars();
        }
    }
}