using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public enum CAMERA_ON_OFF_STATE
{
    ON,
    OFF
};

public class CameraController : MonoBehaviour
{
    [SerializeField] float m_maxBatteryPercentage;
    [SerializeField] float m_batteryDrainInterval;
    [SerializeField] float m_batteryDrainAmount;
    [SerializeField] float m_batteryRegenerateAmount;
    [SerializeField] float m_rotationDuration;
    [SerializeField] float m_horizontalSensitivity;
    [SerializeField] float m_verticalSensitivity;
    [SerializeField] float m_smoothingSpeed;
    [SerializeField] Vector2 m_minMaxHorizontalAngle;
    [SerializeField] Vector2 m_minMaxVerticalAngle;
    [SerializeField] EffectManager m_effectManager;
    [SerializeField] PlayerController m_playerController;

    CAMERA_ON_OFF_STATE m_cameraOnOffState = CAMERA_ON_OFF_STATE.OFF;

    float m_currentDrainTime;
    float m_currentBatteryPercentage;

    float m_rotationX;
    float m_rotationY;

    Vector2 m_startRotation;

    bool m_isRotating = false;

    private void Awake()
    {
        m_currentBatteryPercentage = m_maxBatteryPercentage;
        m_startRotation = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);
    }

    private void Start()
    {
        ChangeCameraOnOffState(CAMERA_ON_OFF_STATE.OFF);
    }

    private void Update()
    {
        CameraInputHandler();

        CameraStateProcess();
    }

    public void CameraInputHandler()
    {
        if (m_playerController.IsDead())
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            //カメラオフ
            if (m_cameraOnOffState == CAMERA_ON_OFF_STATE.ON)
            {
                ChangeCameraOnOffState(CAMERA_ON_OFF_STATE.OFF);
            }

            //カメラオン
            else if (m_cameraOnOffState == CAMERA_ON_OFF_STATE.OFF && GetCurrentBatteryValue() > 0)
            {
                ChangeCameraOnOffState(CAMERA_ON_OFF_STATE.ON);
            }
        }
    }

    public CAMERA_ON_OFF_STATE GetCameraOnOffState()
    {
        return m_cameraOnOffState;
    }

    public void ChangeCameraOnOffState(CAMERA_ON_OFF_STATE state)
    {
        Debug.Log("ステート変わるよ");

        m_cameraOnOffState = state;

        switch (m_cameraOnOffState)
        {
            case CAMERA_ON_OFF_STATE.ON:

                m_effectManager.SetCameraPostEffectFlag(true);

                EnableFreeCamera();

                CursorManager.instance.EnableCrossHair();

                break;

            case CAMERA_ON_OFF_STATE.OFF:

                SetCurrentDrainTime(0);

                m_effectManager.SetCameraPostEffectFlag(false);

                CursorManager.instance.EnableCursor();

                StartCoroutine(ResetToStartRotation());

                break;
        }
    }

    private void CameraStateProcess()
    {
        if (m_cameraOnOffState == CAMERA_ON_OFF_STATE.ON)
        {
            m_currentDrainTime += Time.deltaTime;

            if (m_currentDrainTime >= m_batteryDrainInterval)
            {
                DrainBattery();
                SetCurrentDrainTime(0);

                if (m_currentBatteryPercentage <= 0)
                {
                    ChangeCameraOnOffState(CAMERA_ON_OFF_STATE.OFF);
                }
            }

            FreeCameraRotationProcess();
        }
        else 
        {
            RotateCameraInputHandler();
        }
    }

    private void RotateCameraInputHandler()
    {
        if (m_playerController.IsDead() || IsRotating())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(RotateCameraRoutine(new Vector3(0, 90, 0)));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(RotateCameraRoutine(new Vector3(0, -90, 0)));
        }
    }

    public void EnableFreeCamera() 
    {
        m_startRotation = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);

        m_rotationY = 0;
        m_rotationX = 0;
    }

    private IEnumerator ResetToStartRotation() 
    {
        float elapsedTime = 0.0f;

        Quaternion rotation = transform.rotation;

        while (elapsedTime < 0.3f) 
        {
            elapsedTime += Time.deltaTime;

            float rate = elapsedTime / 0.3f;

            Quaternion endRotation = Quaternion.Euler(m_startRotation.x, m_startRotation.y, 0);

            transform.rotation = Quaternion.Slerp(rotation, endRotation, Easing.OutCubic(rate));

            yield return null;
        }
    }

    public void FreeCameraRotationProcess() 
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.unscaledDeltaTime * m_horizontalSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.unscaledDeltaTime * m_verticalSensitivity;

        m_rotationY += mouseX;
        m_rotationX -= mouseY;

        m_rotationX = Mathf.Clamp(m_rotationX, m_minMaxVerticalAngle.x, m_minMaxVerticalAngle.y);
        m_rotationY = Mathf.Clamp(m_rotationY, m_minMaxHorizontalAngle.x, m_minMaxHorizontalAngle.y);

        Quaternion endRotation = Quaternion.Euler(m_startRotation.x + m_rotationX, m_startRotation.y + m_rotationY, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, Time.deltaTime * m_smoothingSpeed);
    }

    public IEnumerator RotateCameraRoutine(Vector3 rotationValue)
    {
        m_isRotating = true;

        float elapsedTime = 0f;

        Quaternion startRotation = transform.rotation;

        while (elapsedTime < m_rotationDuration)
        {
            elapsedTime += Time.deltaTime;

            float rate = Easing.InOutCubic(elapsedTime / m_rotationDuration);

            transform.rotation = startRotation * Quaternion.Euler(rotationValue.x * rate, rotationValue.y * rate, rotationValue.z * rate);

            yield return null;
        }

        m_isRotating = false;
    }

    public bool IsRotating() 
    {
        return m_isRotating;
    }

    public float GetDrainInterval()
    {
        return m_batteryDrainInterval;
    }

    public float GetDrainAmount()
    {
        return m_batteryDrainAmount;
    }

    public float GetRegenerateAmount()
    {
        return m_batteryRegenerateAmount;
    }

    public float GetMaxBatteryValue()
    {
        return m_maxBatteryPercentage;
    }

    public float GetCurrentBatteryValue()
    {
        return m_currentBatteryPercentage;
    }

    public float GetCurrentDrainTime()
    {
        return m_currentDrainTime;
    }

    public void SetCurrentDrainTime(float drainTime)
    {
        m_currentDrainTime = drainTime;
    }

    public void AddCurrentDrainTime(float drainTime)
    {
        m_currentDrainTime += drainTime;
    }

    public void AddBattery()
    {
        m_currentBatteryPercentage += m_batteryRegenerateAmount;

        if (m_currentBatteryPercentage > m_maxBatteryPercentage)
        {
            m_currentBatteryPercentage = m_maxBatteryPercentage;
        }
    }

    public void DrainBattery()
    {
        m_currentBatteryPercentage -= m_batteryDrainAmount;

        if (m_currentBatteryPercentage < 0)
        {
            m_currentBatteryPercentage = 0;
        }
    }
}
