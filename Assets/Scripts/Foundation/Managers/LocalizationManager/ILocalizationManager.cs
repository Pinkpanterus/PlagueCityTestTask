namespace Foundation
{
    public interface ILocalizationManager
    {
        ObserverList<IOnLanguageChanged> OnLanguageChanged { get; }

        Language CurrentLanguage { get; set; }

        public string GetString(LocalizedString str);
    }
}
