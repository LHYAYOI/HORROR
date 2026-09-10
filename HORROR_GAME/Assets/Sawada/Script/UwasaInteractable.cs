using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UwasaInteractable : MonoBehaviour
{
    [SerializeField] Button m_button;
    [SerializeField] TMP_Text m_text;

    public void SetButtonEnabled(bool enabled)
    {
        m_button.interactable = enabled;

        m_text.color = enabled ? Color.white : Color.gray;
    }
}
