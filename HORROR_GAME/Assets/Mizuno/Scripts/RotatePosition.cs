using UnityEngine;

public class RotatePosition : MonoBehaviour
{
    public float RotateSpeed = 100.0f;
    public Vector3 centerPosition = new Vector3(0, 0, 1.5f);

    void Update()
    {
        /* Rotate around a center position. */
        this.gameObject.transform.RotateAround(centerPosition, new Vector3(0, 1, 0), Time.deltaTime * RotateSpeed);
    }
}
