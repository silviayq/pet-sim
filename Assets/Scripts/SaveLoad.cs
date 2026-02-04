using UnityEngine;

public static class SaveLoad
{
    private const string KEY = "PET_CONFIG_JSON";

    public static void Save(PetConfigData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public static PetConfigData LoadOrDefault()
    {
        if (!PlayerPrefs.HasKey(KEY))
        {
            return new PetConfigData();
        }
        string json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<PetConfigData>(json);
    }
}
