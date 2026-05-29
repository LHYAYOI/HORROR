using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] Material m_skyBoxMaterial;
    [SerializeField] SkyboxTransition_2 m_transition;

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

    public Vector2Int GetCurrentIndex() 
    {
        return m_currentAreaIndex;
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
            StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y - 1)));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y + 1)));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x + 1, m_currentAreaIndex.y)));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x - 1, m_currentAreaIndex.y)));
        }
    }

    public IEnumerator MoveArea(Vector2Int moveIndex)
    {
        MapData m_MapData = m_AreaManager.GetMapData();

        AreaController area = m_MapData.GetArea(moveIndex);

        if (area == null)
        {
            yield break;
        }

        //バッテリー回復
        if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.OFF)
        {
            m_PlayerModel.AddBattery(1);
        }

        //テクスチャの取得
        Texture currentAreaTexture = m_MapData.GetArea(m_currentAreaIndex).GetAreaSkyBoxTexture();
        Texture nextAreaTexture = m_MapData.GetArea(moveIndex).GetAreaSkyBoxTexture();

        m_skyBoxMaterial.mainTexture = nextAreaTexture;
        m_currentAreaIndex = moveIndex;

        Debug.Log(currentAreaTexture);
        Debug.Log(nextAreaTexture);

        m_transition.SetNextTexture(nextAreaTexture);

        //yield return StartCoroutine(m_transition.BlendRoutine(currentAreaTexture, nextAreaTexture));

        //m_transition.SetBlendingTextures(currentAreaTexture, nextAreaTexture);

        //float elapsedTime = 0.0f;

        //while (elapsedTime < 0.5f) 
        //{
        //    elapsedTime += Time.deltaTime;

        //    float rate = elapsedTime / 0.5f;

        //    Mathf.Clamp01(rate);

        //    m_transition.SetBlend(rate);

        //    yield return null;
        //}

        //m_currentAreaIndex = moveIndex;

        // m_transition.SetBlend(0);

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
