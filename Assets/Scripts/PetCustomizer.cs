using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PetCustomizer : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nameInput;
    public TMP_Dropdown speciesDropdown;
    public Image previewImage;

    public Button btnColor1;
    public Button btnColor2;
    public Button btnColor3;
    public Button btnStart;

    [Header("Sprites")]
    public Sprite chickenSprite;
    public Sprite plantSprite;

    private PetConfigData current = new PetConfigData();

    void Start()
    {
        speciesDropdown.ClearOptions();
        speciesDropdown.AddOptions(new System.Collections.Generic.List<string> { "Chicken", "Plant" });
        speciesDropdown.onValueChanged.AddListener(OnSpeciesChanged);

        btnColor1.onClick.AddListener(() => SetColor(ColorUtil.From255(255, 70, 70)));   // 红
        btnColor2.onClick.AddListener(() => SetColor(ColorUtil.From255(70, 120, 255)));  // 蓝
        btnColor3.onClick.AddListener(() => SetColor(ColorUtil.From255(70, 200, 120)));  // 绿

        if (nameInput != null)
            nameInput.onValueChanged.AddListener((s) => current.petName = string.IsNullOrEmpty(s) ? "Buddy" : s);

        btnStart.onClick.AddListener(StartGame);

        // 默认选项（0 = Chicken）
        current.species = Species.Chicken;

        ApplySpecies();
        SetColor(Color.white);
        UpdatePreview();
    }

    void OnSpeciesChanged(int idx)
    {
        current.species = (Species)idx;
        ApplySpecies();
        UpdatePreview();
    }

    void ApplySpecies()
    {
        Sprite s = chickenSprite;
        switch (current.species)
        {
            case Species.Chicken: s = chickenSprite; break;
            case Species.Plant: s = plantSprite; break;
        }

        if (previewImage != null)
            previewImage.sprite = s;
    }

    void SetColor(Color c)
    {
        current.color = c;
        UpdatePreview();
    }

    void UpdatePreview()
    {
        if (previewImage != null)
            previewImage.color = current.color;
    }

    void StartGame()
    {
        if (string.IsNullOrWhiteSpace(current.petName)) current.petName = "Buddy";
        SaveLoad.Save(current);
        SceneManager.LoadScene("PlayScene");
    }
}
