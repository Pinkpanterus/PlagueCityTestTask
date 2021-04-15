using System.Collections.Generic;
using Foundation;
using UnityEngine;

namespace Game
{
    public interface ISettingsManager
    {
        //SettingsManager.Language CurrentLanguage { get; set; }
        ScriptableObject[] GraficsPresets { get; set; }
        ScriptableObject CurrentGraficsPreset{ get; set; }

        float MusicVolume { get; set; }
        float SoundVolume { get; set; }

        ObserverList<IOnSaveGame> OnSaveGame { get; }
        ObserverList<IOnLoadGame> OnLoadGame{ get; }
        ObserverList<IOnLoadSettings> OnLoadSettings{ get; }

        void SaveGame();
        void LoadGame();
        
        void SaveSettings();
        void LoadSettings();
        void ChangeLanguage(int languageIndex);
        void ChangeGraficPreset(int GraficPresetIndex);
    }
}