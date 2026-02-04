using UnityEngine;
using System;

public enum Species { Dog = 0, Cat = 1, Bird = 2 }

[Serializable]
public class PetConfigData
{
    public Species species = Species.Dog;
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
