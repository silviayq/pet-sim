using UnityEngine;
using TMPro;

public class VirtualKeyboard : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_InputField targetInput;
    public GameObject pleaseObject;

    [Header("SFX")]
    public AudioClip typeClip;
    public AudioSource sfxSource;
    [Range(0f, 1f)] public float typeVolume = 1f;
    public float typeCooldown = 0.03f;

    private float _lastTypeTime = -999f;

    public int characterLimit = 12;

    void Awake()
    {
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        sfxSource.playOnAwake = false;
    }

    void PlayTypeSfx()
    {
        if (typeClip == null || sfxSource == null) return;

        if (typeCooldown > 0f && Time.unscaledTime - _lastTypeTime < typeCooldown)
            return;

        _lastTypeTime = Time.unscaledTime;
        sfxSource.PlayOneShot(typeClip, typeVolume);
    }

    public void Open(TMP_InputField input)
    {
        targetInput = input;
        if (panel != null) panel.SetActive(true);
        pleaseObject.SetActive(false);

        if (targetInput != null)
        {
            targetInput.ActivateInputField();
            targetInput.caretPosition = targetInput.text.Length;
        }

        Debug.Log("Open called");
        PlayTypeSfx();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        if (targetInput != null) targetInput.DeactivateInputField();
        targetInput = null;

        PlayTypeSfx();
    }

    public void Type(string s)
    {
        if (targetInput == null) return;
        if (targetInput.text.Length >= characterLimit) return;

        targetInput.text += s;
        targetInput.caretPosition = targetInput.text.Length;
        targetInput.ActivateInputField();

        PlayTypeSfx();
    }

    public void Space()
    {
        if (targetInput.text.Length >= characterLimit) return;
        Type(" ");
    }

    public void ClearAll()
    {
        if (targetInput == null) return;

        targetInput.text = "";
        targetInput.caretPosition = 0;
        targetInput.ActivateInputField(); 

        PlayTypeSfx();
    }

    public void Backspace()
    {
        if (targetInput == null) return;
        if (string.IsNullOrEmpty(targetInput.text)) return;

        targetInput.text = targetInput.text.Substring(0, targetInput.text.Length - 1);
        targetInput.caretPosition = targetInput.text.Length;
        targetInput.ActivateInputField();

        PlayTypeSfx();
    }

    public void Done()
    {
        Close();
    }
}
