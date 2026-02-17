using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;

public class PetRuntime : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer petRenderer;
    public TMP_Text chickenName;
    public TMP_Text plantName;

    public TMP_Text nameText;

    public GameObject farmBackground;
    public GameObject gardenBackground;

    public Image feedImage;
    public Sprite foodSprite;
    public Sprite waterSprite;

    public Image tendImage;
    public Sprite petSprite;
    public Sprite trimSprite;

    public GameObject chickenParent;
    public GameObject plantParent;

    public GameObject chickenObject;
    public GameObject plantObject;
    private GameObject petObject;

    public Animator chickenAnimator;
    public Animator plantAnimator;
    private Animator animator;

    public Animator foodBowlAnimator;
    private bool isChicken = false;

    [Header("Thinking Bubble Variables")]
    public GameObject thinkingBubble;
    public Animator thinkingAnimator;
    public Vector3 chickenThinkingPos;
    public Vector3 plantThinkingPos;

    [Header("Thinking UI Variables")]
    public Image thinkingImage;
    public Sprite thinkFoodSprite;
    public Sprite thinkWaterSprite;
    private Sprite thinkFeedSprite;
    public Sprite thinkPetSprite;
    public Sprite thinkTrimSprite;
    private Sprite thinkPlaySprite;
    public Sprite thinkMusicSprite;
    public Vector3 thinkFeedSize;
    public Vector3 thinkTendSize;
    public Vector3 thinkMusicSize;

    [Header("FX/Audio")]
    public AudioSource chickenAudioSource;
    public AudioSource plantAudioSource;
    private AudioSource audioSource;
    public AudioClip eatClip;
    public AudioClip waterClip;
    public AudioClip happyClip;
    public AudioClip trimClip;
    public AudioClip musicClip;
    public AudioClip thinkClip;
    public AudioClip errorClip;
    public ParticleSystem heartFX;
    public ParticleSystem sparkleFX;

    [Header("Core Stats (0~100)")]
    [Range(0, 100)] public float hunger = 60f;
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
    public float Happiness01 => Mathf.Clamp01(happiness / 100f);

    public System.Action OnAnyStatZero;

    public enum NeedType { None, Feed, Play, Music }
    public NeedType currentNeed = NeedType.None;

    private Species speciesCached;
    private int currentStage = 1;

    private Coroutine needLoopCo;
    private Coroutine timeoutCo;

    public float wrongDuration;
    public float shakePosIntensity;
    public int shakePosVibration;

    public SpriteRenderer chickenRenderer;
    public SpriteRenderer plantRenderer;
    public Color wrongColor;

    private void Start()
    {
        farmBackground.SetActive(false);
        gardenBackground.SetActive(false);

        chickenParent.SetActive(false);
        plantParent.SetActive(false);

        var cfg = SaveLoad.LoadOrDefault();
        speciesCached = cfg.species;

        if (nameText) nameText.text = cfg.petName;

        plantName.text = chickenName.text = nameText.text;

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

        int prevStage = currentStage;
        currentStage = stage;

        if (!force && stage > prevStage)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayStageChange();
        }

        if (speciesCached == Species.Chicken)
        {
            farmBackground.SetActive(true);
            chickenParent.SetActive(true);
            animator = chickenAnimator;

            audioSource = chickenAudioSource;
            feedImage.sprite = foodSprite;
            thinkFeedSprite = thinkFoodSprite;
            tendImage.sprite = petSprite;
            thinkPlaySprite = thinkPetSprite;

            petObject = chickenObject;
            petRenderer = chickenRenderer;

            thinkingBubble.transform.localPosition = chickenThinkingPos;

            isChicken = true;
        }
        else
        {
            gardenBackground.SetActive(true);
            plantParent.SetActive(true);
            animator = plantAnimator;

            audioSource = plantAudioSource;
            feedImage.sprite = waterSprite;
            thinkFeedSprite = thinkWaterSprite;
            tendImage.sprite = trimSprite;
            thinkPlaySprite = thinkTrimSprite;


            petObject = plantObject;
            petRenderer = plantRenderer;

            thinkingBubble.transform.localPosition = plantThinkingPos;

            isChicken = false;
        }

        if (stage == 2)
        {
            animator.SetBool("Grow1", true);
        }
        else if (stage == 3)
        {
            animator.SetBool("Grow2", true);
        }
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

            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0: currentNeed = NeedType.Feed; break;
                case 1: currentNeed = NeedType.Play; break;
                case 2: currentNeed = NeedType.Music; break;
            }

            thinkingBubble.SetActive(true);
            StartCoroutine(ThinkingSequence());

            if (timeoutCo != null) StopCoroutine(timeoutCo);
            timeoutCo = StartCoroutine(NeedTimeout());
        }
    }

    private IEnumerator ThinkingSequence()
    {
        thinkingAnimator.SetBool("Thinking", true);
        yield return new WaitForSeconds(1f);
        ShowThinking(currentNeed);
    }

    private IEnumerator NeedTimeout()
    {
        yield return new WaitForSeconds(needTimeout);

        if (currentNeed != NeedType.None && wrongNeedPenalty > 0f)
        {
            happiness = Mathf.Clamp(happiness - wrongNeedPenalty, 0f, 100f);
            Penalty();
            ApplyStageSprite();
        }

        currentNeed = NeedType.None;
        HideThinking();
        timeoutCo = null;
    }

    private void ShowThinking(NeedType need)
    {
        if (!thinkingImage) return;
        audioSource.PlayOneShot(thinkClip);
        thinkingImage.enabled = true;
        switch (need)
        {
            case NeedType.Feed: thinkingImage.transform.localScale = thinkFeedSize; thinkingImage.sprite = thinkFeedSprite; break;
            case NeedType.Play: thinkingImage.transform.localScale = thinkTendSize; thinkingImage.sprite = thinkPlaySprite; break;
            case NeedType.Music: thinkingImage.transform.localScale = thinkMusicSize; thinkingImage.sprite = thinkMusicSprite; break;
        }
    }

    private void HideThinking()
    {
        if (!thinkingImage) return;
        thinkingBubble.SetActive(false);
        thinkingImage.enabled = false;
        thinkingImage.sprite = null;
        thinkingAnimator.SetBool("Thinking", false);
    }

    private int TrySatisfyNeed(NeedType action)
    {
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
            Penalty();

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

    private void Penalty()
    {
        Sequence wrong = DOTween.Sequence();
        wrong.Append(petRenderer.DOColor(wrongColor, 0));
        wrong.Append(petObject.transform.DOShakePosition(wrongDuration, shakePosIntensity, shakePosVibration, 90f, false, false));
        wrong.Append(petRenderer.DOColor(Color.white, 0));
        wrong.Play();
        audioSource.PlayOneShot(errorClip);
    }

    public int Feed()
    {
        int gain = TrySatisfyNeed(NeedType.Feed);
        if (gain > 0)
        {
            if (isChicken)
            {
                animator.SetTrigger("Feed");
                foodBowlAnimator.SetTrigger("Feed");
                audioSource.PlayOneShot(eatClip);
            }
            else
            {
                animator.SetTrigger("Feed");
                audioSource.PlayOneShot(waterClip);
            }
        }
        return gain;
    }

    public int PlayWith()
    {
        int gain = TrySatisfyNeed(NeedType.Play);
        if (gain > 0)
        {
            animator.SetTrigger("Pet");

            if (isChicken)
            {
                audioSource.PlayOneShot(happyClip);
            }
            else
            {
                audioSource.PlayOneShot(trimClip);
            }
            //if (heartFX) heartFX.Play();
        }
        return gain;
    }

    public int Music()
    {
        int gain = TrySatisfyNeed(NeedType.Music);
        if (gain > 0)
        {
            animator.SetTrigger("Dancing");
            audioSource.PlayOneShot(musicClip);
            //if (heartFX) heartFX.Play();
        }
        return gain;
    }
}
