using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public PetRuntime pet;

    public TMP_Text timerText;
    public TMP_Text scoreText;

    public StatBar hungerUI;
    public StatBar cleanUI;
    public StatBar happyUI;

    public UnityEngine.UI.Button btnFeed;
    public UnityEngine.UI.Button btnClean;
    public UnityEngine.UI.Button btnPlay;

    public GameObject resultPanel;
    public TMP_Text resultTitleText; 
    public TMP_Text finalScoreText;
    public UnityEngine.UI.Button btnReplay;
    public UnityEngine.UI.Button btnCustomize;

    [Header("Game Rules")]
    public bool useTimerWin = true; 
    public float gameDuration = 60f;  

    private float timeLeft;
    private int score;
    private bool ended;

    void Start()
    {
        timeLeft = gameDuration;
        ended = false;
        if (resultPanel) resultPanel.SetActive(false);

        //if (hungerUI) { hungerUI.SetLabel("Hunger"); hungerUI.SetWords("Full", "Hungry", "Starving"); }
        //if (cleanUI) { cleanUI.SetLabel("Clean"); cleanUI.SetWords("Sparkly", "Messy", "Stinky"); }
        if (happyUI) { happyUI.SetLabel("Happy"); happyUI.SetWords("Joyful", "Okay", "Upset"); }

        if (btnFeed) btnFeed.onClick.AddListener(OnFeed);
        if (btnClean) btnClean.onClick.AddListener(OnClean);
        if (btnPlay) btnPlay.onClick.AddListener(OnPlayWith);

        if (btnReplay) btnReplay.onClick.AddListener(() => {
            SceneManager.LoadScene("CustomizationScene");
        });
        //if (btnCustomize) btnCustomize.onClick.AddListener(() => {
        //    SceneManager.LoadScene("CustomizationScene");
        //});

        if (pet != null)
        {
            pet.OnAnyStatZero += () => { if (!ended) EndGame(false); };
        }

        UpdateUI();
    }

    void Update()
    {
        if (ended) return;

        //happiness>=100 win
        if (pet != null && pet.happiness >= 100f)
        {
            EndGame(true);
            return;
        }

        if (useTimerWin)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                EndGame(true);
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText) timerText.text = useTimerWin ? $"Time: {timeLeft:0}s" : "Free Mode";
        if (scoreText) scoreText.text = $"Score: {score}";

        if (pet != null)
        {
            if (hungerUI) hungerUI.UpdateStat(pet.Hunger01, pet.hunger);
            if (cleanUI) cleanUI.UpdateStat(pet.Cleanliness01, pet.cleanliness);
            if (happyUI) happyUI.UpdateStat(pet.Happiness01, pet.happiness);
        }
    }

    void OnFeed()
    {
        if (ended || pet == null) return;
        int gain = pet.Feed();
        score += gain;
        UpdateUI();
    }

    void OnClean()
    {
        if (ended || pet == null) return;
        int gain = pet.Clean();
        score += gain;
        UpdateUI();
    }

    void OnPlayWith()
    {
        if (ended || pet == null) return;
        int gain = pet.PlayWith();
        score += gain;
        UpdateUI();
    }

    void EndGame(bool win)
    {
        ended = true;
        if (resultPanel) resultPanel.SetActive(true);
        if (resultTitleText) resultTitleText.text = win ? "You Win!" : "Game Over";
        if (finalScoreText) finalScoreText.text = $"Final Score: {score}";
    }
}
