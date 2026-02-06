using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PetRuntime : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer petRenderer;
    public TMP_Text nameText;

    [Header("Stage Sprites (1/2/3)")]
    public Sprite chicken1;
    public Sprite chicken2;
    public Sprite chicken3;

    public Sprite plant1;
    public Sprite plant2;
    public Sprite plant3;

    [Header("Thinking UI (Image)")]
    public Image thinkingImage;
    public Sprite thinkFeedSprite;
    public Sprite thinkCleanSprite;
    public Sprite thinkPlaySprite;
    public Sprite thinkMusicSprite;

    [Header("Action Texts")]
    public TextMeshProUGUI feedBtnText;
    public TextMeshProUGUI cleanBtnText;
    public TextMeshProUGUI playBtnText;

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
    [Range(0, 100)] public float happiness = 0f;

    [Header("Need System (Random)")]
    [Tooltip("Random wait time range (seconds), to keep it irregular.")]
    public Vector2 needIntervalRange = new Vector2(3f, 9f);

    [Tooltip("How long the pet will wait for the player after a need appears (seconds). After this, the need disappears.")]
    public float needTimeout = 6f;

    [Header("Happiness Gain (ONLY when correct)")]
    [Tooltip("Happiness increases only when the player performs the correct action.")]
    public float correctNeedHappinessGain = 12f;

    [Tooltip("Optional penalty when the player performs the wrong action (0 = no penalty).")]
    public float wrongNeedPenalty = 0f;

    public float Hunger01 => Mathf.Clamp01(hunger / 100f);
    public float Cleanliness01 => Mathf.Clamp01(cleanliness / 100f);
    public float Happiness01 => Mathf.Clamp01(happiness / 100f);

    public System.Action OnAnyStatZero;

    public enum NeedType { None, Feed, Clean, Play, Music }
    public NeedType currentNeed = NeedType.None;

    private Species speciesCached;
    private int currentStage = 1;

    private Coroutine needLoopCo;
    private Coroutine timeoutCo;

    private void Start()
    {
        var cfg = SaveLoad.LoadOrDefault();
        speciesCached = cfg.species;

        if (nameText) nameText.text = cfg.petName;

        if (petRenderer != null)
            petRenderer.color = cfg.color;

        HideThinking();

        happiness = 0f;
        ApplyStageSprite(force: true);

        needLoopCo = StartCoroutine(NeedLoop());
    }

    private int GetStageByHappiness()
    {
        float p = Happiness01;
        if (p < 0.33f) return 1;
        if (p < 0.66f) return 2;
        return 3;
    }

    private void ApplyStageSprite(bool force = false)
    {
        int stage = GetStageByHappiness();
        if (!force && stage <= currentStage) return;
        currentStage = stage;

        if (petRenderer == null) return;

        Sprite s = null;
        if (speciesCached == Species.Chicken)
        {
            s = (stage == 1) ? chicken1 : (stage == 2) ? chicken2 : chicken3;
            feedBtnText.text = "Feed";
            cleanBtnText.text = "Wash";
            playBtnText.text = "Pet";
            //Change icon sprite for official
        }
        else
        {
            s = (stage == 1) ? plant1 : (stage == 2) ? plant2 : plant3;
            feedBtnText.text = "Water";
            cleanBtnText.text = "Trim";
            playBtnText.text = "Tend";
        }

        if (s != null) petRenderer.sprite = s;
    }

    private IEnumerator NeedLoop()
    {
        while (true)
        {
            if (happiness >= 100f)
            {
                HideThinking();
                yield return null;
                continue;
            }

            float wait = Random.Range(needIntervalRange.x, needIntervalRange.y);
            yield return new WaitForSeconds(wait);

            if (currentNeed != NeedType.None) continue;

            int r = Random.Range(0, 4);
            switch (r)
            {
                case 0: currentNeed = NeedType.Feed; break;
                case 1: currentNeed = NeedType.Clean; break;
                case 2: currentNeed = NeedType.Play; break;
                case 3: currentNeed = NeedType.Music; break;
            }

            ShowThinking(currentNeed);

            if (timeoutCo != null) StopCoroutine(timeoutCo);
            timeoutCo = StartCoroutine(NeedTimeout());
        }
    }

    private IEnumerator NeedTimeout()
    {
        yield return new WaitForSeconds(needTimeout);

        if (currentNeed != NeedType.None && wrongNeedPenalty > 0f)
        {
            happiness = Mathf.Clamp(happiness - wrongNeedPenalty, 0f, 100f);
            ApplyStageSprite();
        }

        currentNeed = NeedType.None;
        HideThinking();
        timeoutCo = null;
    }

    private void ShowThinking(NeedType need)
    {
        if (!thinkingImage) return;

        thinkingImage.enabled = true;
        switch (need)
        {
            case NeedType.Feed: thinkingImage.sprite = thinkFeedSprite; break;
            case NeedType.Clean: thinkingImage.sprite = thinkCleanSprite; break;
            case NeedType.Play: thinkingImage.sprite = thinkPlaySprite; break;
            case NeedType.Music: thinkingImage.sprite = thinkMusicSprite; break;
        }
    }

    private void HideThinking()
    {
        if (!thinkingImage) return;
        thinkingImage.enabled = false;
        thinkingImage.sprite = null;
    }

    private int TrySatisfyNeed(NeedType action)
    {
        if (currentNeed == NeedType.None) return 0;

        if (currentNeed == action)
        {
            happiness = Mathf.Clamp(happiness + correctNeedHappinessGain, 0f, 100f);

            currentNeed = NeedType.None;
            HideThinking();

            if (timeoutCo != null) { StopCoroutine(timeoutCo); timeoutCo = null; }

            ApplyStageSprite();

            return Mathf.RoundToInt(correctNeedHappinessGain);
        }
        else
        {
            if (wrongNeedPenalty > 0f)
            {
                happiness = Mathf.Clamp(happiness - wrongNeedPenalty, 0f, 100f);
                ApplyStageSprite();
            }

            currentNeed = NeedType.None;
            HideThinking();

            if (timeoutCo != null)
            {
                StopCoroutine(timeoutCo);
                timeoutCo = null;
            }

            return 0;
        }
    }
    public int Feed()
    {
        int gain = TrySatisfyNeed(NeedType.Feed);
        if (gain > 0)
        {
            if (audioSource && eatClip) audioSource.PlayOneShot(eatClip);
            if (heartFX) heartFX.Play();
        }
        return gain;
    }

    public int Clean()
    {
        int gain = TrySatisfyNeed(NeedType.Clean);
        if (gain > 0)
        {
            if (audioSource && washClip) audioSource.PlayOneShot(washClip);
            if (sparkleFX) sparkleFX.Play();
        }
        return gain;
    }

    public int PlayWith()
    {
        int gain = TrySatisfyNeed(NeedType.Play);
        if (gain > 0)
        {
            if (audioSource && happyClip) audioSource.PlayOneShot(happyClip);
            if (heartFX) heartFX.Play();
        }
        return gain;
    }

    public int Music()
    {
        int gain = TrySatisfyNeed(NeedType.Music);
        if (gain > 0)
        {
            if (audioSource && happyClip) audioSource.PlayOneShot(happyClip);
            if (heartFX) heartFX.Play();
        }
        return gain;
    }
}
