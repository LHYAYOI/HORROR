using System.Collections.Generic;
using UnityEngine;

public class PageList : MonoBehaviour
{
    [SerializeField] List<Sprite> m_uwasa1;
    [SerializeField] List<Sprite> m_uwasa2;
    [SerializeField] List<Sprite> m_uwasa3;
    [SerializeField] List<Sprite> m_uwasa4;
    [SerializeField] List<Sprite> m_uwasa5;
    [SerializeField] List<Sprite> m_uwasa6;

    // 噂の指定ページを取得する
    public Sprite GetUwasaPage(int uwasaNumber, int pageNumber)
    {
        switch (uwasaNumber)
        {
            case 1:
                if (pageNumber < 0 || m_uwasa1.Count <= pageNumber) return null;
                return m_uwasa1[pageNumber];
            case 2:
                if (pageNumber < 0 || m_uwasa2.Count <= pageNumber) return null;
                return m_uwasa2[pageNumber];
            case 3:
                if (pageNumber < 0 || m_uwasa3.Count <= pageNumber) return null;
                return m_uwasa3[pageNumber];
            case 4:
                if (pageNumber < 0 || m_uwasa4.Count <= pageNumber) return null;
                return m_uwasa4[pageNumber];
            case 5:
                if (pageNumber < 0 || m_uwasa5.Count <= pageNumber) return null;
                return m_uwasa5[pageNumber];
            case 6:
                if (pageNumber < 0 || m_uwasa6.Count <= pageNumber) return null;
                return m_uwasa6[pageNumber];
            default:
                return null;
        }
    }
    public int GetUwasaPageCount(int uwasaNumber)
    {
        switch (uwasaNumber)
        {
            case 1:
                return m_uwasa1.Count;
            case 2:
                return m_uwasa2.Count;
            case 3:
                return m_uwasa3.Count;
            case 4:
                return m_uwasa4.Count;
            case 5:
                return m_uwasa5.Count;
            case 6:
                return m_uwasa6.Count;
            default:
                return 0;
        }
    }
}
