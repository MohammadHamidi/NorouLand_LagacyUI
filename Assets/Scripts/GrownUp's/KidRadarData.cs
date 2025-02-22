using UnityEngine;

[System.Serializable]
public class KidRadarData
{
    public string childName;
    public int childAge;
    public string tagline;

    // Skills (0 to 1 or 0 to 100, depending on your design)
    public float memory;
    public float language;
    public float lifeSkills;
    public float mathAndLogic;
    public float attention;

    // The colored polygon shape for the chart (could be a sprite or a mask)
    public Sprite radarShapeSprite;

    // If true, layout flips for RTL languages (e.g., Farsi)
    public bool isRTL;
}