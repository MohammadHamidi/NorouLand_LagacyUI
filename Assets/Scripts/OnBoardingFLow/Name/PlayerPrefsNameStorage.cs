using UnityEngine;

public class PlayerPrefsNameStorage : INameStorage
{
    private const string NAME_KEY = "ChildName";
    
    public void SaveName(string name)
    {
        PlayerPrefs.SetString(NAME_KEY, name);
        PlayerPrefs.Save();
    }
    
    public string LoadName()
    {
        return PlayerPrefs.GetString(NAME_KEY, "");
    }
    
    public void DeleteName()
    {
        PlayerPrefs.DeleteKey(NAME_KEY);
        PlayerPrefs.Save();
    }
    
    public bool HasName()
    {
        return PlayerPrefs.HasKey(NAME_KEY);
    }
}