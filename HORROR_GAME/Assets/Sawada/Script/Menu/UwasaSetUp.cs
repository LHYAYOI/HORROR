using System.Collections.Generic;
using UnityEngine;

public class UwasaSetUp : MonoBehaviour
{
    [Header("噂ボタン")]
    [SerializeField] UwasaInteractable[] m_uwasaInteractable = new UwasaInteractable[6];
    
    private void Start()
    {
        m_uwasaInteractable[0].SetButtonEnabled(MenuFlag.GetBool("Stage1"));
        m_uwasaInteractable[1].SetButtonEnabled(MenuFlag.GetBool("Stage2"));
        m_uwasaInteractable[2].SetButtonEnabled(MenuFlag.GetBool("Stage3"));
        m_uwasaInteractable[3].SetButtonEnabled(MenuFlag.GetBool("Stage4"));
        m_uwasaInteractable[4].SetButtonEnabled(MenuFlag.GetBool("Stage5"));
        m_uwasaInteractable[5].SetButtonEnabled(MenuFlag.GetBool("Stage6"));
    }
}
