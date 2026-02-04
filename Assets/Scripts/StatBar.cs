using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBar : MonoBehaviour
{
    [Header("Wiring")]
    public Slider slider;
    public TMP_Text labelText;
    public TMP_Text valueText;  
    public TMP_Text stateText; 
    public Image fillImage; 

    [Header("Labels")]
    public string label = "Stat"; 

    [Header("Thresholds (0~1)")]
    [Tooltip(">= goodHigh aa“good”（green）")]
    public float goodHigh = 0.7f;
    [Tooltip("< warnLow as“danger”（red）")]
    public float warnLow = 0.3f;

    [Header("Colors")]
    public Color goodColor = new Color(0.3f, 0.8f, 0.4f); 
    public Color midColor = new Color(0.95f, 0.8f, 0.2f); 
    public Color badColor = new Color(0.9f, 0.3f, 0.3f);  

    [Header("State Words")]
    public string goodWord = "良好";
    public string midWord = "一般";
    public string badWord = "危险";

    void Reset()
    {
        if (!slider) slider = GetComponentInChildren<Slider>();
        if (!labelText) labelText = transform.Find("LabelText")?.GetComponent<TMP_Text>();
        if (!valueText) valueText = transform.Find("ValueText")?.GetComponent<TMP_Text>();
        if (!stateText) stateText = transform.Find("StateText")?.GetComponent<TMP_Text>();
        if (!fillImage && slider)
            fillImage = slider.fillRect ? slider.fillRect.GetComponent<Image>() : null;
    }

    void Awake()
    {
        if (slider) { slider.minValue = 0f; slider.maxValue = 1f; }
        if (labelText) labelText.text = label;
    }

    public void UpdateStat(float norm, float abs)
    {
        if (slider) slider.value = Mathf.Clamp01(norm);
        if (valueText) valueText.text = Mathf.RoundToInt(Mathf.Clamp(abs, 0f, 100f)).ToString();

        string word; Color col;
        if (norm >= goodHigh) { word = goodWord; col = goodColor; }
        else if (norm < warnLow) { word = badWord; col = badColor; }
        else { word = midWord; col = midColor; }

        if (stateText) stateText.text = word;
        if (fillImage) fillImage.color = col;
    }

    public void SetLabel(string newLabel)
    {
        label = newLabel;
        if (labelText) labelText.text = newLabel;
    }

    public void SetWords(string good, string mid, string bad)
    {
        goodWord = good; midWord = mid; badWord = bad;
    }
}
