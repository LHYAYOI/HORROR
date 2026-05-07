using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "Scriptable Objects/AreaData")]
public class AreaData : ScriptableObject
{
    [SerializeField] string m_areaName;
    [SerializeField] Texture m_areaTexture;

    public string GetAreaName() => m_areaName;
    public Texture GetAreaTexture() => m_areaTexture;
}
