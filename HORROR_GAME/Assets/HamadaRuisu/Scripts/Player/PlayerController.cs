using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour
{
    public enum CAMERA_ON_OFF_ATATE
    {
        ON,
        OFF
    };

    [SerializeField] PlayerModel m_PlayerModel;
    [SerializeField] PlayerView m_PlayerView;
    [SerializeField] AreaManager m_AreaManager;

    [Header("カメラ周りの設定")]
    [SerializeField] Camera m_camera;
    [SerializeField] float m_rotationDuration;

    Vector2Int m_currentAreaIndex;

    CAMERA_ON_OFF_ATATE m_cameraOnOffState = CAMERA_ON_OFF_ATATE.OFF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraInputHandler();

        MoveAreaInputHandler();

        RotateCameraInputHandler();

        m_PlayerView.UpdateBatteryGages(m_PlayerModel.GetMaxBatteryValue(), m_PlayerModel.GetCurrentBatteryValue());
    }

    private void CameraInputHandler() 
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            //カメラオフ
            if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.ON) 
            {
                m_PlayerModel.DrainBattery(1);

                //m_PlayerView.HideBatteryUI();

                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.OFF);
            }

            //カメラオン
            else if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.OFF && m_PlayerModel.GetCurrentBatteryValue() > 0) 
            {
                //m_PlayerView.ShowBatteryUI();

                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.ON);
            }
        }
    }

    public CAMERA_ON_OFF_ATATE GetCameraOnOffState() 
    {
        return m_cameraOnOffState;
    }

    public void ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE state)
    {
        m_cameraOnOffState = state;
    }

    private void MoveAreaInputHandler()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y - 1));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y + 1));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveArea(new Vector2Int(m_currentAreaIndex.x + 1, m_currentAreaIndex.y));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveArea(new Vector2Int(m_currentAreaIndex.x - 1, m_currentAreaIndex.y));
        }
    }

    public void MoveArea(Vector2Int moveIndex)
    {
        MapData m_MapData = m_AreaManager.GetMapData();

        AreaController area = m_MapData.GetArea(moveIndex);

        if (area != null)
        {
            //バッテリー回復
            if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.OFF)
            {
                m_PlayerModel.AddBattery(1);
            }

            RenderSettings.skybox = area.GetAreaSkyBoxMaterial();
            m_currentAreaIndex = moveIndex;
        }
    }

    private void RotateCameraInputHandler()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StartCoroutine(RotateCameraRoutine(new Vector3(0, 90, 0)));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StartCoroutine(RotateCameraRoutine(new Vector3(0, -90, 0)));
        }
    }

    IEnumerator RotateCameraRoutine(Vector3 rotationValue)
    {
        float elapsedTime = 0f;

        Quaternion startRotation = m_camera.transform.rotation;

        while (elapsedTime < m_rotationDuration)
        {
            elapsedTime += Time.deltaTime;

            float rate = Easing.InOutCubic(elapsedTime / m_rotationDuration);

            m_camera.transform.rotation = startRotation * Quaternion.Euler(rotationValue.x * rate, rotationValue.y * rate, rotationValue.z * rate);

            yield return null;
        }
    }
}
