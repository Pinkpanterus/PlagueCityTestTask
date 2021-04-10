using System.Collections.Generic;
using Foundation;

namespace Game
{
    public interface IGameManager
    {
        ObserverList<IOnDayBegin> OnNewDayBegin { get; }
        ObserverList<IOnDayEnd> OnDayEnd { get; }
        ObserverList<IOnTimerChanged> OnTimerChanged { get; }

        List<Visitor> PreparedVisitors { get; set; }
        List<Visitor> VisitorsInRowThisDay { get; set; }
        List<VisitorAvatar> VisitorsAvatarCreated { get; set; }
        Visitor VisitorInFocus { get; set; }
        
        int Day { get; set; }
        float DayTimeLeft { get; set; }
        
        void CreateVisitorsList(VisitorsComponents_SO visitorsComponentsSo);
        void StartNewDay();
        void EndDay();
        
        void CreateVisitorAvatars();
        void DeleteVisitorAvatarCreated();
        void DestroyVisitorAvatar(VisitorAvatar visitorAvatar);
        void SetVisitorInFocus(Visitor visitor);

        void CreateVisitorAvatarAfterLoadGame();
    }
}