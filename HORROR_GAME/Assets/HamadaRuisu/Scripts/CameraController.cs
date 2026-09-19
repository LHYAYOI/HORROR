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
    [SerializeField] float m_limitBarMoveAmount;
    [SerializeField] RectTransform m_limitBarRightTransform;
    [SerializeField] RectTransform m_limitBarLeftTransform;
    [SerializeField] Vector2 m_minMaxHorizontalAngle;
    [SerializeField] Vector2 m_minMaxVerticalAngle;
    [SerializeField] EffectManager m_effectManager;
    [SerializeField] PlayerController m_playerController;
    [SerializeField] EnemyManager m_enemyManager;
    [SerializeField] Animator m_cameraAnimator;
    [SerializeField] GameObject m_cameraUI;
    [SerializeField] CameraView m_cameraView;

    CAMERA_ON_OFF_STATE m_cameraOnOffState = CAMERA_ON_OFF_STATE.OFF;

    float m_currentDrainTime;
    float m_currentBatteryPercentage;

    float m_rotationX;
    float m_rotationY;

    Vector2 m_startRotation;

    bool m_isRotating = false;
    bool m_isSwitchingCamera = false;

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

        m_cameraView.UpdateBatteryPercentage(m_maxBatteryPercentage, m_currentBatteryPercentage);
    }

    public void CameraInputHandler()
    {
        if (m_playerController.IsDead())
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            CameraONOFFHandler();
        }
    }

    public void CameraONOFFHandler() 
    {
        //カメラオフ
        if (m_cameraOnOffState == CAMERA_ON_OFF_STATE.ON && !m_isSwitchingCamera)
        {
            m_isSwitchingCamera = true;

            ChangeCameraOnOffState(CAMERA_ON_OFF_STATE.OFF);
        }

        //カメラオン
        else if (m_cameraOnOffState == CAMERA_ON_OFF_STATE.OFF && !m_isSwitchingCamera && GetCurrentBatteryValue() > 0)
        {
            m_isSwitchingCamera = true;

            InitFreeCamera();

            m_cameraAnimator.SetTrigger("RollCamera");
        }
    }

    public CAMERA_ON_OFF_STATE GetCameraOnOffState()
    {
        return m_cameraOnOffState;
    }

    public void ChangeCameraOnOffState(CAMERA_ON_OFF_STATE state)
    {
        m_cameraOnOffState = state;

        switch (m_cameraOnOffState)
        {
            case CAMERA_ON_OFF_STATE.ON:

                break;

            case CAMERA_ON_OFF_STATE.OFF:

                AudioManagerHamada.instance.Stop("Capturing");

                AudioManagerHamada.instance.Play("CameraOff");

                SetCurrentDrainTime(0);

                m_cameraUI.SetActive(false);

                m_cameraView.HideBatteryUI();

                m_effectManager.SetCameraPostEffectFlag(false);

                m_cameraAnimator.SetTrigger("ShutDownCamera");

                CursorManager.instance.EnableCursor();

                StartCoroutine(ResetToStartRotation());

                break;
        }

        // 変更
        m_enemyManager.SetView();
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
        ChangeCameraOnOffState(CAMERA_ON_OFF_STATE.ON);

        m_cameraUI.SetActive(true);

        m_cameraView.ShowBatteryUI();
    }

    public void InitFreeCamera()
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

        LimitBarProcess();
    }

    private void LimitBarProcess()
    {
        // 右側の限界へのアプローチ
        float rightLimit = m_minMaxHorizontalAngle.y;
        float rightThreshold = rightLimit - 2;

        // 前のステップで作った「初期位置からの変化量」m_rotationY と直接比べる
        if (m_rotationY >= rightThreshold)
        {
            // 58度〜60度の間でのレート（0.0 〜 1.0）を計算
            float rightRate = Mathf.InverseLerp(rightThreshold, rightLimit, m_rotationY);
            rightRate = Mathf.Clamp01(rightRate); // 念のため0〜1にクランプ

            // anchoredPosition.x を書き換えてスライドさせる
            float targetX = Mathf.Lerp(590, 570, Easing.OutCubic(rightRate));
            m_limitBarRightTransform.anchoredPosition = new Vector2(targetX, m_limitBarRightTransform.anchoredPosition.y);
        }
        else
        {
            // 閾値未満ならデフォルト位置（590）に戻す
            m_limitBarRightTransform.anchoredPosition = new Vector2(590, m_limitBarRightTransform.anchoredPosition.y);
        }

        // 左側の限界へのアプローチ
        float leftLimit = m_minMaxHorizontalAngle.x;
        float leftThreshold = leftLimit + 2;

        // 前のステップで作った「初期位置からの変化量」m_rotationY と直接比べる
        if (m_rotationY <= leftThreshold)
        {
            // 58度〜60度の間でのレート（0.0 〜 1.0）を計算
            float leftRate = Mathf.InverseLerp(leftThreshold, leftLimit, m_rotationY);
            leftRate = Mathf.Clamp01(leftRate); // 念のため0〜1にクランプ

            // anchoredPosition.x を書き換えてスライドさせる
            float targetX = Mathf.Lerp(-590, -570, Easing.OutCubic(leftRate));
            m_limitBarLeftTransform.anchoredPosition = new Vector2(targetX, m_limitBarLeftTransform.anchoredPosition.y);
        }
        else
        {
            // 閾値未満ならデフォルト位置（-590）に戻す
            m_limitBarLeftTransform.anchoredPosition = new Vector2(-590, m_limitBarLeftTransform.anchoredPosition.y);
        }

    }

    public IEnumerator RotateCameraRoutine(Vector3 rotationValue)
    {
        m_isRotating = true;

        AudioManagerHamada.instance.Play("Rotate");

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

    public void SetCameraSwitchingFlag(bool flag) 
    {
        m_isSwitchingCamera = flag;
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
