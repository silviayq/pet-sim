using UnityEngine;
using TMPro;

public class PetRuntime : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer petRenderer;
    public TMP_Text nameText;

    [Header("Sprites")]
    public Sprite chickenSprite;
    public Sprite plantSprite;

    [Header("FX/Audio (optional)")]
    public AudioSource audioSource;
    public AudioClip eatClip;
    public AudioClip washClip;
    public AudioClip happyClip;
    public ParticleSystem heartFX;
    public ParticleSystem sparkleFX;

    [Header("Core Stats (0~100)")]
    [Range(0, 100)] public float hunger = 60f;
    [Range(0, 100)] public float cleanliness = 60f;
    [Range(0, 100)] public float happiness = 60f;

    [Header("Natural Decay (per second)")]
    public float hungerDecay = 6f;
    public float cleanlinessDecay = 4f;
    public float happinessDecay = 3f;

    [Header("Action Gains / Costs")]
    public float feedGain = 10f;
    //public float feedCleanCost = 5f;
    //public float overfeedPenalty = 15f;

    public float cleanGain = 10f;
    //public float cleanHappyCost = 5f;

    public float playHappyGain = 10f;
    //public float playHungerCost = 6f;
    //public float playCleanCost = 6f;

    [Header("Overfeed Threshold")]
    [Tooltip(">= already full, if feed, -happy")]
    public float overfeedThreshold = 98f;

    public float Hunger01 => Mathf.Clamp01(hunger / 100f);
    public float Cleanliness01 => Mathf.Clamp01(cleanliness / 100f);
    public float Happiness01 => Mathf.Clamp01(happiness / 100f);

    public System.Action OnAnyStatZero;

    private void Start()
    {
        var cfg = SaveLoad.LoadOrDefault();

        if (nameText) nameText.text = cfg.petName;

        if (petRenderer != null)
        {
            switch (cfg.species)
            {
                case Species.Chicken: petRenderer.sprite = chickenSprite; break;
                case Species.Plant: petRenderer.sprite = plantSprite; break;
            }
            petRenderer.color = cfg.color;
        }
    }

    private void Update()
    {
        //hunger = Mathf.Clamp(hunger - hungerDecay * Time.deltaTime, 0f, 100f);
        //cleanliness = Mathf.Clamp(cleanliness - cleanlinessDecay * Time.deltaTime, 0f, 100f);
        //happiness = Mathf.Clamp(happiness - happinessDecay * Time.deltaTime, 0f, 100f);

        //if (hunger <= 0f || cleanliness <= 0f || happiness <= 0f)
        //{
        //    OnAnyStatZero?.Invoke();
        //}
    }

    public int Feed()
    {
        //int scoreGain = 0;
        //if (hunger >= overfeedThreshold)
        //{
        //    happiness = Mathf.Clamp(happiness - overfeedPenalty, 0f, 100f);
        //    scoreGain = Mathf.RoundToInt(2 + 5 * Happiness01);
        //}
        //else
        //{
        //    hunger = Mathf.Clamp(hunger + feedGain, 0f, 100f);
        //    //cleanliness = Mathf.Clamp(cleanliness - feedCleanCost, 0f, 100f);
        //    scoreGain = Mathf.RoundToInt(8 + 18 * Hunger01);
        //}

        happiness = Mathf.Clamp(happiness + feedGain, 0f, 100f);
        int scoreGain = Mathf.RoundToInt(Happiness01);

        if (audioSource && eatClip) audioSource.PlayOneShot(eatClip);
        if (heartFX) heartFX.Play();
        return scoreGain;
    }

    public int Clean()
    {
        happiness = Mathf.Clamp(happiness + cleanGain, 0f, 100f);
        //happiness = Mathf.Clamp(happiness - cleanHappyCost, 0f, 100f);

        if (audioSource && washClip) audioSource.PlayOneShot(washClip);
        if (sparkleFX) sparkleFX.Play();

        int scoreGain = Mathf.RoundToInt(Happiness01);
        return scoreGain;
    }

    public int PlayWith()
    {
        happiness = Mathf.Clamp(happiness + playHappyGain, 0f, 100f);
        //hunger = Mathf.Clamp(hunger - playHungerCost, 0f, 100f);
        //cleanliness = Mathf.Clamp(cleanliness - playCleanCost, 0f, 100f);

        if (audioSource && happyClip) audioSource.PlayOneShot(happyClip);
        if (heartFX) heartFX.Play();

        int scoreGain = Mathf.RoundToInt(Happiness01);
        return scoreGain;
    }
}
