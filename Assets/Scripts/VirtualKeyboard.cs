using UnityEngine;
using TMPro;

public class VirtualKeyboard : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;
    public TMP_InputField target;

    public void Open(TMP_InputField input)
    {
        target = input;
        if (panel != null) panel.SetActive(true);

        if (target != null)
        {
            target.ActivateInputField();
            target.caretPosition = target.text.Length;
        }
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        if (target != null) target.DeactivateInputField();
        target = null;
    }

    public void Type(string s)
    {
        if (target == null) return;
        target.text += s;
        target.caretPosition = target.text.Length;
        target.ActivateInputField();
    }

    public void Space() => Type(" ");

    public void Backspace()
    {
        if (target == null) return;
        if (string.IsNullOrEmpty(target.text)) return;

        target.text = target.text.Substring(0, target.text.Length - 1);
        target.caretPosition = target.text.Length;
        target.ActivateInputField();
    }

    public void Clear()
    {
        if (target == null) return;
        target.text = "";
        target.caretPosition = 0;
        target.ActivateInputField();
    }

    public void Done()
    {
        Close();
    }
}
