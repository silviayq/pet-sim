using UnityEngine;
using UnityEngine.UI;

public class ToggleVisualSwitch : MonoBehaviour
{
    public Toggle toggle;

    private ColorBlock originalColors;

    void Start()
    {
        originalColors = toggle.colors;

        toggle.onValueChanged.AddListener(UpdateVisual);
        UpdateVisual(toggle.isOn);
    }

    void UpdateVisual(bool isOn)
    {
        ColorBlock cb = toggle.colors;

        Color normal = originalColors.normalColor;

        if (isOn)
            normal.a = 1f;
        else
            normal.a = 210f / 255f;

        cb.normalColor = normal;

        toggle.colors = cb;
    }
}
