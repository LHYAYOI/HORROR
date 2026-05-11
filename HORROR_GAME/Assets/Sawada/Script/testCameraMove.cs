using UnityEngine;

public class testCameraMove : MonoBehaviour
{
    [SerializeField] private float rotateDuration = 0.4f;

    private bool isRotating = false;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float timer = 0f;

    void Update()
    {
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartRotate(-90f);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartRotate(90f);
            }
        }

        if (isRotating)
        {
            timer += Time.deltaTime;
            float t = timer / rotateDuration;

            // 0〜1に制限
            t = Mathf.Clamp01(t);

            // 適当イージング
            float easedT = EaseInOutCubic(t);

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, easedT);

            if (t >= 1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }

    private void StartRotate(float yAngle)
    {
        isRotating = true;
        timer = 0f;

        startRotation = transform.rotation;

        // 現在の角度からY軸に90度加算
        targetRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + yAngle,
            transform.eulerAngles.z
        );
    }

    private float EaseInOutCubic(float t)
    {
        if (t < 0.5f)
        {
            return 4f * t * t * t;
        }
        else
        {
            return 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }
    }
}
