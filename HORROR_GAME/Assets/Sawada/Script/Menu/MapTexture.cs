using UnityEngine;

public class MapTexture : MonoBehaviour
{
    [SerializeField] Sprite m_maskMap;
    [SerializeField] Sprite m_map;

    public Sprite GetMaskMap()
    {
        return m_maskMap;
    }
    public Sprite GetMap()
    {
        return m_map;
    }
}
