using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class OpenKeyboardOnSelect : MonoBehaviour, ISelectHandler
{
    public VirtualKeyboard keyboard;
    public TMP_InputField input;

    public void OnSelect(BaseEventData eventData)
    {
        if (keyboard != null && input != null)
            keyboard.Open(input);
    }
}
