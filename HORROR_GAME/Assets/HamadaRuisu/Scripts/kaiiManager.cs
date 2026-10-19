using UnityEngine;
using System.Collections.Generic;

public class kaiiManager : MonoBehaviour
{
    [SerializeField] List<KaiControler> m_kaiiList;

    public bool IsAllCaptured() 
    {
        foreach (KaiControler kaii in m_kaiiList) 
        {
            if (kaii == null)
            {
                continue;
            }

            if (!kaii.WasShot()) 
            {
                return false;
            }
        }

        return true;
    }
}
