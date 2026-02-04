using UnityEngine;
using System;

public enum Species { Chicken = 0, Plant = 1 }

[Serializable]
public class PetConfigData
{
    public Species species = Species.Chicken;
    public string petName = "Buddy";
    public Color color = Color.white;
}

public static class ColorUtil
{
    public static Color From255(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1f);
    }
}
