using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip bgm;
    public AudioClip click;
    public AudioClip stageChange;
    public AudioClip start;
    public AudioClip win;

    [Header("Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgm == null) return;

        if (bgmSource.clip != bgm)
            bgmSource.clip = bgm;

        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PlayClick()
    {
        if (click) sfxSource.PlayOneShot(click);
    }

    public void PlayStageChange()
    {
        if (stageChange) sfxSource.PlayOneShot(stageChange);
    }

    public void PlayStart()
    {
        if (start) sfxSource.PlayOneShot(start);
    }

    public void PlayWin()
    {
        if (win) sfxSource.PlayOneShot(win);
    }
}
