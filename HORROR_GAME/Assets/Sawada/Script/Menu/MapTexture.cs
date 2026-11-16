using UnityEngine;

public class MapTexture : MonoBehaviour
{
    [SerializeField] Sprite m_mapImage;

    public Sprite GetMapImage()
    {
        return m_mapImage;
    }
}
