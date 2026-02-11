using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PetCustomizer : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nameInput;
    public TMP_Dropdown speciesDropdown;
    public Image previewImage;

    //public Button btnColor1;
    //public Button btnColor2;
    //public Button btnColor3;
    public Button btnStart;

    public Toggle chickenToggle;
    public Toggle plantToggle;

    private bool hasSelected = false;

    public ToggleGroup toggleGroup;

    public GameObject keyboard;

    public TextMeshProUGUI pleaseText;
    public float pleaseTextShowDuration;

    [Header("Preview Sprites (Stage 1)")]
    public Sprite chicken1;
    public Sprite plant1;

    private PetConfigData current = new PetConfigData();

    void Start()
    {
        //speciesDropdown.ClearOptions();
        //speciesDropdown.AddOptions(new System.Collections.Generic.List<string> { "Chicken", "Plant" });
        //speciesDropdown.onValueChanged.AddListener(OnSpeciesChanged);

        //btnColor1.onClick.AddListener(() => SetColor(ColorUtil.From255(255, 70, 70)));
        //btnColor2.onClick.AddListener(() => SetColor(ColorUtil.From255(70, 120, 255)));
        //btnColor3.onClick.AddListener(() => SetColor(ColorUtil.From255(70, 200, 120)));

        pleaseText.gameObject.SetActive(false);

        chickenToggle.onValueChanged.AddListener(OnChickenSelected);
        plantToggle.onValueChanged.AddListener(OnPlantSelected);

        btnStart.onClick.AddListener(StartGame);

        //btnStart.gameObject.SetActive(false);

        // default:chicken stage 1
        //I use the placemat for the images
        current.species = Species.Chicken;

        //ApplySpecies();
        //SetColor(Color.white);
        //UpdatePreview();

        toggleGroup.enabled = false;
    }

    void OnChickenSelected(bool isOn)
    {
        if (!isOn) return;

        current.species = Species.Chicken;
        //btnStart.gameObject.SetActive(true);
        toggleGroup.enabled = true;
        hasSelected = true;
    }

    void OnPlantSelected(bool isOn)
    {
        if (!isOn) return;

        current.species = Species.Plant;
        //btnStart.gameObject.SetActive(true);
        toggleGroup.enabled = true;
        hasSelected = true;
    }

    //public void SelectedChicken()
    //{
    //    current.species = Species.Chicken;
    //    Debug.Log("Selected CHICKEN");
    //    btnStart.gameObject.SetActive(true);
    //}

    //public void SelectedPlant()
    //{
    //    current.species = Species.Plant;
    //    Debug.Log("Selected PLANT");
    //    btnStart.gameObject.SetActive(true);
    //}

    void StartGame()
    {
        string enteredName = nameInput.text;

        keyboard.SetActive(false);

        if (!hasSelected)
        {
            pleaseText.text = "Please select a pet!";
            StartCoroutine(PleaseTextShow());
        }
        else if(string.IsNullOrWhiteSpace(enteredName))
        {
            pleaseText.text = "Please enter a name!";
            StartCoroutine(PleaseTextShow());
        }
        else
        {
            current.petName = enteredName;

            SaveLoad.Save(current);
            SceneManager.LoadScene("PlayScene");
        }
    }

    private IEnumerator PleaseTextShow()
    {
        pleaseText.gameObject.SetActive(true);

        yield return new WaitForSeconds(pleaseTextShowDuration);

        pleaseText.gameObject.SetActive(false);
    }

    //void OnSpeciesChanged(int idx)
    //{
    //    current.species = (Species)idx;
    //    ApplySpecies();
    //    UpdatePreview();
    //}

    //void ApplySpecies()
    //{
    //    Sprite s = (current.species == Species.Chicken) ? chicken1 : plant1;
    //    if (previewImage != null) previewImage.sprite = s;
    //}

    //void SetColor(Color c)
    //{
    //    current.color = c;
    //    UpdatePreview();
    //}

    //void UpdatePreview()
    //{
    //    if (previewImage != null) previewImage.color = current.color;
    //}
}
