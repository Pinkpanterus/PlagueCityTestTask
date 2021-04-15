using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Foundation;
using ModestTree;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    sealed class InterfaceManager : AbstractService<IInterfaceManager>, IInterfaceManager, IOnDayBegin, IOnDayEnd, IOnTimerChanged,IOnLoadGame, IOnLoadSettings
    {
        [Inject] ILocalizationManager _localizationManager = default;
        [Inject] IGameManager _gameManager = default;
        [Inject] ISettingsManager _settingsManager = default;
        [Inject] ITimeScaleManager _timeScaleManager = default;
        TimeScaleHandle timeScaleHandle;
        
        [Inject] ISceneStateManager _iSceneStateManager = default;
        [Inject] ISoundManager _soundManager = default;
        
        public SceneState _settingsMenuState;

        [Header("Cabinet interface")]
        public Button PauseButton;
        public Button NextDayButton;
        public Button SettingsButton;
        public Button AppQuitButton;
        
        public Text TimerText;
        public Text DayNumberText;
        
        public Image FadeImage;

        [Header("Settings interface")] 
        public Dropdown LanguageSelector;
        public Dropdown GraficPresetSelector;
        public Slider MusicVolumeSlider;
        public Slider SoundVolumeSlider;
        public Button SaveButton;
        public Button LoadButton;

        [Header("Sound")] 
        public AudioClip ButtonClickSound;
        public AudioClip BackGroundMusic;
        

        
        public ObserverList<IOnPauseButtonPress> OnPauseButtonPress { get; } = new ObserverList<IOnPauseButtonPress>();
        public ObserverList<IOnNextDayButtonPress> OnNextDayButtonPress { get; } = new ObserverList<IOnNextDayButtonPress>();
        public ObserverList<IOnSettingsButtonPress> OnSettingsButtonPress { get; } = new ObserverList<IOnSettingsButtonPress>();
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(_gameManager.OnNewDayBegin);
            Observe(_gameManager.OnDayEnd);
            Observe(_gameManager.OnTimerChanged);
            Observe(_settingsManager.OnLoadGame);
            Observe(_settingsManager.OnLoadSettings);

            void PlayButtonClickSound()
            {
                _soundManager.Sfx.Play(ButtonClickSound);
            }

            SaveButton.onClick.AddListener(() =>
            {
                _settingsManager.SaveGame();
                PlayButtonClickSound();
            });
            
            LoadButton.onClick.AddListener(() =>
            {
                _settingsManager.LoadGame();
                PlayButtonClickSound();
            });
            
            MusicVolumeSlider.onValueChanged.AddListener (delegate {MusicVolumeChanged();});
            SoundVolumeSlider.onValueChanged.AddListener (delegate {SoundVolumeChanged();});
            
            var finishPositionX = SettingsButton.transform.localPosition.x;
            SettingsButton.transform.DOLocalMoveX(finishPositionX, 1).From(finishPositionX - 200); //не стал длительность и оффсет в переменные выносить, здесь не важно

            
            NextDayButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                    
                foreach (var it in OnNextDayButtonPress.Enumerate())
                    it.Do();
                _gameManager.EndDay();
            }) ;
            
            PauseButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                
                float scale = 0.0f;
                
                if (timeScaleHandle == null)
                    timeScaleHandle = new TimeScaleHandle();

                if (Time.timeScale > 0.0f)
                    _timeScaleManager.BeginTimeScale(timeScaleHandle, scale);
                else
                    _timeScaleManager.EndTimeScale(timeScaleHandle);
                
                foreach (var it in OnPauseButtonPress.Enumerate())
                    it.Do();
            });
            
            AppQuitButton.onClick.AddListener(()=>
            {
                Application.Quit();
                PlayButtonClickSound();
            });
            
            SettingsButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                
                _iSceneStateManager.Push(_settingsMenuState);

                foreach (var it in OnSettingsButtonPress.Enumerate())
                    it.Do();
            });
            
            LanguageSelector.GetComponent<Dropdown>().onValueChanged.AddListener(delegate {
               _settingsManager.ChangeLanguage(LanguageSelector.GetComponent<Dropdown>().value);
               //PlayButtonClickSound();
            });
            
            GraficPresetSelector.GetComponent<Dropdown>().onValueChanged.AddListener(delegate {
                _settingsManager.ChangeGraficPreset(GraficPresetSelector.GetComponent<Dropdown>().value);
                //PlayButtonClickSound();
            });
        }

        private void SoundVolumeChanged()
        {
            _settingsManager.SoundVolume = SoundVolumeSlider.value;
            _settingsManager.SaveSettings();
            _soundManager.GetChannel("Sfx").Volume = _settingsManager.SoundVolume;
        }

        private void MusicVolumeChanged()
        {
            _settingsManager.MusicVolume = MusicVolumeSlider.value;
            _settingsManager.SaveSettings();
            _soundManager.GetChannel("Music").Volume = _settingsManager.MusicVolume;
        }

        public void ShowTimer()
        {
            int min = Mathf.FloorToInt(_gameManager.DayTimeLeft / 60);
            int sec = Mathf.FloorToInt(_gameManager.DayTimeLeft % 60);
            TimerText.text = min.ToString("00") + ":" + sec.ToString("00");
        }

        public void ShowDay()
        {
            DayNumberText.text = _gameManager.Day.ToString();
        }

        async Task IOnDayBegin.Do()
        {
            ShowDay();
            await FadeImage.GetComponent<Image>().DOFade(0.0f, 1.0f).AsyncWaitForCompletion();
        }

        async Task IOnDayEnd.Do()
        {
            await FadeImage.GetComponent<Image>().DOFade(1.0f, 1.0f).AsyncWaitForCompletion();
            _gameManager.StartNewDay(); // test
        }

        void IOnTimerChanged.Do()
        {
            ShowTimer();
        }

        void IOnLoadGame.Do()
        {
            ShowDay();
        }

        void RestoreSettingInMenuAfterLoad()
        {
            //LanguageSelector.value =(int)_settingsManager.CurrentLanguage;
            LanguageSelector.value =(int)_localizationManager.CurrentLanguage;
            GraficPresetSelector.value = _settingsManager.GraficsPresets.IndexOf(_settingsManager.CurrentGraficsPreset);
            MusicVolumeSlider.value = _settingsManager.MusicVolume;
            SoundVolumeSlider.value = _settingsManager.SoundVolume;
        }

        void IOnLoadSettings.Do()
        {
            RestoreSettingInMenuAfterLoad();
        }

    }
}
