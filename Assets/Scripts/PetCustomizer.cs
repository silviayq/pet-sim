using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PetCustomizer : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nameInput;
    public Image previewImage;
    public Button btnStart;

    public Toggle chickenToggle;
    public Toggle plantToggle;

    private bool hasSelected = false;

    public ToggleGroup toggleGroup;

    public GameObject keyboard;

    public TextMeshProUGUI pleaseText;
    public float pleaseTextShowDuration = 1.2f;

    [Header("Preview Sprites (Stage 1)")]
    public Sprite chicken1;
    public Sprite plant1;

    private PetConfigData current = new PetConfigData();

    void Start()
    {
        pleaseText.gameObject.SetActive(false);

        chickenToggle.onValueChanged.AddListener(OnChickenSelected);
        plantToggle.onValueChanged.AddListener(OnPlantSelected);

        if (nameInput != null)
        {
            nameInput.onValueChanged.AddListener((s) =>
                current.petName = string.IsNullOrEmpty(s) ? "Buddy" : s
            );
        }

        btnStart.onClick.AddListener(StartGame);

        current.species = Species.Chicken;

        toggleGroup.enabled = false;

        UpdatePreview();
    }

    void OnChickenSelected(bool isOn)
    {
        if (!isOn) return;

        current.species = Species.Chicken;
        toggleGroup.enabled = true;
        hasSelected = true;
        UpdatePreview();
    }

    void OnPlantSelected(bool isOn)
    {
        if (!isOn) return;

        current.species = Species.Plant;
        toggleGroup.enabled = true;
        hasSelected = true;
        UpdatePreview();
    }

    void UpdatePreview()
    {
        if (previewImage == null) return;
        previewImage.sprite = (current.species == Species.Chicken) ? chicken1 : plant1;
    }

    void StartGame()
    {
        if (!hasSelected)
        {
            StartCoroutine(ShowPleaseText());
            return;
        }

        if (string.IsNullOrWhiteSpace(current.petName))
            current.petName = "Buddy";

        SaveLoad.Save(current);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayStart();

        StartCoroutine(LoadPlaySceneAfterSfx());
    }

    IEnumerator LoadPlaySceneAfterSfx()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("PlayScene");
    }

    IEnumerator ShowPleaseText()
    {
        pleaseText.gameObject.SetActive(true);
        yield return new WaitForSeconds(pleaseTextShowDuration);
        pleaseText.gameObject.SetActive(false);
    }
}
