namespace LocalizationSystem
{
    #region Localization Interfaces and Implementations

// Abstract interface for localization
public interface ILocalization
{
    // Returns the language name (or code)
    string Language { get; }

    // Returns the localized value for the given key
    string GetLocalizedValue(string key);

    // Loads the localization data (could be from file, database, etc.)
    void LoadLocalizationData();
}

// English localization implementation

// Farsi localization implementation

#endregion

#region Localization Manager

#endregion

}