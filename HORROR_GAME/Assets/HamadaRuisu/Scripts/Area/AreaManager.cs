using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [SerializeField] MapData m_MapData;

    public MapData GetMapData()
    {
        return m_MapData;
    }
}
