using System.Collections.Generic;
using Foundation;

namespace Game
{
    public interface IInterfaceManager
    {
        ObserverList<IOnPauseButtonPress> OnPauseButtonPress { get; }
        ObserverList<IOnNextDayButtonPress> OnNextDayButtonPress { get; }
        ObserverList<IOnSettingsButtonPress> OnSettingsButtonPress { get; }

        void ShowTimer();
        void ShowDay();

        //void ShowEndDayEffect();
        //void ShowNewDayEffect();
    }
}