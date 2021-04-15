using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using Foundation;
using System.IO;
using Game;
using UnityEngine.UI;

namespace Game
{
    public sealed class SettingsManager : AbstractService<ISettingsManager>, ISettingsManager
    {
        // public enum Language
        // {
        //     russian,
        //     english
        // }
        
        public enum GraficPreset
        {
            High,
            Medium,
            Low
        }

        [Inject] ILocalizationManager _localizationManager = default;

        //public Language CurrentLanguage { get; set; }
        
        public ScriptableObject[] graficsPresets;
        public ScriptableObject[] GraficsPresets
        {
            get { return graficsPresets;}
            set { graficsPresets = value; }
        }
        public ScriptableObject CurrentGraficsPreset { get; set; }
        
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }

        public ObserverList<IOnSaveGame> OnSaveGame { get; } = new ObserverList<IOnSaveGame>();
        public ObserverList<IOnLoadGame> OnLoadGame { get; } = new ObserverList<IOnLoadGame>();
        public ObserverList<IOnLoadSettings> OnLoadSettings { get; } = new ObserverList<IOnLoadSettings>();
        
        static private SaveFile _saveFile;
        static private SettingsFile _settingsFile;
        static private string   _saveFilePath;
        static private string   _settingsFilePath;

        [Inject] IGameManager _gameManager =default;
        [Inject] private ISoundManager _soundManager = default;


        void Start()
        {
            _saveFilePath = Application.persistentDataPath + "/Data.save";
            _saveFile = new SaveFile();
            
            _settingsFilePath = Application.persistentDataPath + "/Settings.save";
            _settingsFile = new SettingsFile();

            LoadSettings();
        }

        public void SaveGame()
        {
            _saveFile.SavedDay = _gameManager.Day;
            _saveFile.SavedDayTimeLeft = _gameManager.DayTimeLeft;
            _saveFile.SavedPreparedVisitors = _gameManager.PreparedVisitors;
            _saveFile.SavedVisitorsInRowThisDay = _gameManager.VisitorsInRowThisDay;
           
            string jsonSaveFile = JsonUtility.ToJson(_saveFile, true);

            File.WriteAllText(_saveFilePath, jsonSaveFile);
            
            Debug.Log("Game was saved");
            
            foreach (var observer in OnSaveGame.Enumerate())
                observer.Do();
        }

        public void LoadGame()
        {
            string dataAsJson = File.ReadAllText(_saveFilePath);

            try
            {
                _saveFile = JsonUtility.FromJson<SaveFile>(dataAsJson);

            }
            catch
            {
                Debug.LogWarning("SaveFile was malformed.\n" + dataAsJson);
                return;
            }
            
            _gameManager.DeleteVisitorAvatarCreated();
            
            _gameManager.Day = _saveFile.SavedDay;
            _gameManager.DayTimeLeft = _saveFile.SavedDayTimeLeft;
            _gameManager.PreparedVisitors = _saveFile.SavedPreparedVisitors;
            _gameManager.VisitorsInRowThisDay = _saveFile.SavedVisitorsInRowThisDay;
          
            _gameManager.CreateVisitorAvatarAfterLoadGame();
            
            Debug.Log("Game was loaded");

            foreach (var observer in OnLoadGame.Enumerate())
                observer.Do();
        }
        
        
        public void ChangeGraficPreset(int graficPresetIndex)
        {
            switch (graficPresetIndex)
            {
                case 0:
                    CurrentGraficsPreset = GraficsPresets[graficPresetIndex];
                    Debug.Log("Current Grafic preset is High");
                    break;
                case 1:
                    CurrentGraficsPreset =  GraficsPresets[graficPresetIndex];
                    Debug.Log("Current Grafic preset is Medium");
                    break;
                case 2:
                    CurrentGraficsPreset =  GraficsPresets[graficPresetIndex];
                    Debug.Log("Current Grafic preset is Low");
                    break;
            }

            SaveSettings();
        }

        public void ChangeLanguage(int languageIndex)
        {
            switch (languageIndex)
            {
                case 0:
                    _localizationManager.CurrentLanguage = Language.Russian;
                    //CurrentLanguage = Language.Russian;
                    Debug.Log("Current language is " + Language.Russian);
                    break;
                case 1:
                    _localizationManager.CurrentLanguage = Language.English;
                    //CurrentLanguage = Language.English;
                    Debug.Log("Current language is " + Language.English);
                    break;
            }

            SaveSettings();
        }
        

        public void SaveSettings()
        {
            //_settingsFile.Language = (int)_localizationManager.CurrentLanguage;
            _settingsFile.CurrentLanguage = (int)_localizationManager.CurrentLanguage;
            _settingsFile.GraficsPreset = CurrentGraficsPreset;
            _settingsFile.MusicVolume = MusicVolume;
            _settingsFile.SoundVolume = SoundVolume;
           
            string jsonSaveFile = JsonUtility.ToJson(_settingsFile, true);

            File.WriteAllText(_settingsFilePath, jsonSaveFile);
            
            Debug.Log("Settings was saved");
            
            // foreach (var observer in OnSaveGame.Enumerate())
            //     observer.Do();
        }

        public void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                string dataAsJson = File.ReadAllText(_settingsFilePath);

                try
                {
                    _settingsFile = JsonUtility.FromJson<SettingsFile>(dataAsJson);

                }
                catch
                {
                    Debug.LogWarning("Settings file was malformed.\n" + dataAsJson);
                    return;
                }

                _localizationManager.CurrentLanguage = (Language)_settingsFile.CurrentLanguage;
                CurrentGraficsPreset = _settingsFile.GraficsPreset;
                MusicVolume = _settingsFile.MusicVolume;
                SoundVolume = _settingsFile.SoundVolume;
                
                _soundManager.GetChannel("Music").Volume = MusicVolume;
                _soundManager.GetChannel("Sfx").Volume = SoundVolume;
           
                Debug.Log("Settings was loaded");

                foreach (var observer in OnLoadSettings.Enumerate())
                    observer.Do();
                
                foreach (var observer in _localizationManager.OnLanguageChanged.Enumerate())
                    observer.Do();
            }
            else
            {
                Debug.Log("Settings file is not found");
            }
        }
    }
}

[System.Serializable]
public class SaveFile
{
    public int SavedDay;
    public float SavedDayTimeLeft; // in seconds
    public List<Visitor> SavedPreparedVisitors;
    public List<Visitor> SavedVisitorsInRowThisDay;
}

[System.Serializable]
public class SettingsFile
{
    public float MusicVolume;
    public float SoundVolume;
    //public SettingsManager.Language Language;
    //public Language Language;
    public int CurrentLanguage;
    public ScriptableObject GraficsPreset;
}
