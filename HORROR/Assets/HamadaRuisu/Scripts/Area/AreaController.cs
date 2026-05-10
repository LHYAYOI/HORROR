using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaController : MonoBehaviour
{
    [SerializeField] AreaModel m_model;
 
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public string GetAreaName() => m_model.GetAreaName();

    public Material GetAreaSkyBoxMaterial() => m_model.GetAreaSkyBoxMaterial();
}
