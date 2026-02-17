using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public PetRuntime pet;

    public TMP_Text timerText;
    public TMP_Text scoreText;

    public StatBar hungerUI;
    public StatBar happyUI;

    public UnityEngine.UI.Button btnFeed;
    public UnityEngine.UI.Button btnPlay;
    public UnityEngine.UI.Button btnMusic;

    [Header("Result UI")]
    public GameObject blackScreen;
    public float blackDuration;
    public GameObject resultPanel;
    public TMP_Text resultTitleText;
    public TMP_Text finalScoreText;
    public UnityEngine.UI.Button btnReplay;
    public UnityEngine.UI.Button btnCustomize;

    [Header("Hide On Result")]
    public GameObject petRoot;
    public GameObject btnRoot;
    public GameObject[] extraHideOnResult;

    [Header("Game Rules")]
    public bool useTimerWin = true;
    public float gameDuration = 60f;

    private float timeLeft;
    private int score;
    private bool ended;

    void Start()
    {
        blackScreen.SetActive(false);

        timeLeft = gameDuration;
        ended = false;

        if (resultPanel) resultPanel.SetActive(false);

        if (happyUI) { happyUI.SetLabel("Happy"); happyUI.SetWords("Joyful", "Okay", "Upset"); }

        if (btnFeed) btnFeed.onClick.AddListener(OnFeed);
        if (btnPlay) btnPlay.onClick.AddListener(OnPlayWith);
        if (btnMusic) btnMusic.onClick.AddListener(OnMusic);

        if (btnReplay) btnReplay.onClick.AddListener(() =>
        {
            if (AudioManager.Instance) AudioManager.Instance.PlayClick();
            SceneManager.LoadScene("CustomizationScene");
        });

        if (btnCustomize) btnCustomize.onClick.AddListener(() =>
        {
            if (AudioManager.Instance) AudioManager.Instance.PlayClick();
            SceneManager.LoadScene("CustomizationScene");
        });

        if (pet != null)
        {
            pet.OnAnyStatZero += () => { if (!ended) EndGame(false); };
        }

        UpdateUI();
    }

    void Update()
    {
        if (ended)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("CustomizationScene");
            }
            return;
        }

        if (pet != null && pet.happiness >= 100f)
        {
            StartCoroutine(WaitForActionToEnd());
            return;
        }

        //if (useTimerWin)
        //{
        //    timeLeft -= Time.deltaTime;
        //    if (timeLeft <= 0f)
        //    {
        //        timeLeft = 0f;
        //        EndGame(true);
        //    }
        //}

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText) timerText.text = useTimerWin ? $"Time: {timeLeft:0}s" : "Free Mode";
        if (scoreText) scoreText.text = $"Score: {score}";

        if (pet != null)
        {
            if (hungerUI) hungerUI.UpdateStat(pet.Hunger01, pet.hunger);
            if (happyUI) happyUI.UpdateStat(pet.Happiness01, pet.happiness);
        }
    }

    void OnFeed()
    {
        if (ended || pet == null) return;

        //if (AudioManager.Instance) AudioManager.Instance.PlayClick();
        int gain = pet.Feed();
        score += gain;
        UpdateUI();
        EventSystem.current.SetSelectedGameObject(null);
    }

    void OnPlayWith()
    {
        if (ended || pet == null) return;

        //if (AudioManager.Instance) AudioManager.Instance.PlayClick();
        int gain = pet.PlayWith();
        score += gain;
        UpdateUI();
        EventSystem.current.SetSelectedGameObject(null);
    }

    void OnMusic()
    {
        if (ended || pet == null) return;

        //if (AudioManager.Instance) AudioManager.Instance.PlayClick();
        int gain = pet.Music();
        score += gain;
        UpdateUI();
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator WaitForActionToEnd()
    {
        yield return new WaitForSeconds(1f);
        EndGame(true);
    }

    void EndGame(bool win)
    {
        ended = true;

        if (petRoot) petRoot.SetActive(false);
        if (btnRoot) btnRoot.SetActive(false);
        if (extraHideOnResult != null)
        {
            foreach (var go in extraHideOnResult)
                if (go) go.SetActive(false);
        }

        blackScreen.SetActive(true);

        //if (win && AudioManager.Instance) AudioManager.Instance.PlayWin();

        if (win && AudioManager.Instance) AudioManager.Instance.StopBGM();
        StartCoroutine(ShowBlackScreen());
        //if (resultTitleText) resultTitleText.text = win ? "You Win!" : "Game Over";
        //if (finalScoreText) finalScoreText.text = $"Final Score: {score}";
    }
    private IEnumerator ShowBlackScreen()
    {
        yield return new WaitForSeconds(blackDuration);
        if (AudioManager.Instance) AudioManager.Instance.PlayBGM();
        blackScreen.SetActive(false);
        if (resultPanel) resultPanel.SetActive(true);
    }
}
