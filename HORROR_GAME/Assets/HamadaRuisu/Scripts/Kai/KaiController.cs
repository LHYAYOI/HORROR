using UnityEngine;

public class KaiControler : MonoBehaviour,IPressable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressed()
    {
        Debug.Log("‰Ÿ‚³‚ê‚½");
    }

    public void OnReleased()
    {
        Debug.Log("—£‚µ‚½");
    }

}
