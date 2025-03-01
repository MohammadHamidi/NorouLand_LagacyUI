using UnityEngine;

[System.Serializable]
public class KidProfile
{
    public string name;
    public int age;
    public string subtitle;
    public Sprite avatar;
    public Color chartColor = Color.red;
    public float[] skillValues = new float[5]; // Memory, Language, SoftSkills, Math, Attention
    
    // Total users count (optional, for statistics)
    public int totalUsers;
}