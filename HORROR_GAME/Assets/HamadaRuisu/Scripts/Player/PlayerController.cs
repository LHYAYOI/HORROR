using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour
{
    public enum CAMERA_ON_OFF_ATATE
    {
        ON,
        OFF
    };

    public enum PLAYER_STATE
    {
        NORMAL,
        HIDE
    }

    [SerializeField] PlayerModel m_PlayerModel;
    [SerializeField] PlayerView m_PlayerView;
    [SerializeField] StateMachine m_StateMachine;
    [SerializeField] Animator m_hideAnimator;
    [SerializeField] AreaManager m_AreaManager;
    [SerializeField] Material m_skyBoxMaterial;
    [SerializeField] SkyboxTransition_2 m_transition;

    [Header("カメラ周りの設定")]
    [SerializeField] Camera m_camera;
    [SerializeField] float m_rotationDuration;

    [SerializeField] LayerMask m_layerMask;

    [SerializeField] Vector2Int m_startPosition;
    Vector2Int m_currentAreaIndex;

    CAMERA_ON_OFF_ATATE m_cameraOnOffState = CAMERA_ON_OFF_ATATE.OFF;
    PLAYER_STATE m_playerState = PLAYER_STATE.NORMAL;

    bool m_interactedKai = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_currentAreaIndex = m_startPosition;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraInputHandler();

        MoveAreaInputHandler();

        RotateCameraInputHandler();

        if (m_playerState == PLAYER_STATE.HIDE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("oooooooo");
                m_hideAnimator.SetTrigger("StartFade");
                ChangePlayerState(PLAYER_STATE.NORMAL);
            }
        }

        InteractionHandler();

        m_PlayerView.UpdateBatteryGages(m_PlayerModel.GetMaxBatteryValue(), m_PlayerModel.GetCurrentBatteryValue());
    }

    public bool InteractedWithKai() 
    {
        return m_interactedKai;
    }

    private void InteractionHandler() 
    {
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
        {
            GameObject interactedObject = hit.collider.gameObject;

            IIntaractable interactableTarget = interactedObject.GetComponent<IIntaractable>();

            if (interactableTarget != null && Input.GetMouseButtonDown(0)) 
            {
                interactableTarget.OnInteracted();

                if (interactedObject.tag == "Kai")
                {
                    m_interactedKai = true;
                }
            }
        }
    }

    public StateMachine GetStateMachine() 
    {
        return m_StateMachine;
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

                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.OFF);
            }

            //カメラオン
            else if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.OFF && m_PlayerModel.GetCurrentBatteryValue() > 0)
            {
                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.ON);
            }
        }
    }

    public CAMERA_ON_OFF_ATATE GetCameraOnOffState()
    {
        return m_cameraOnOffState;
    }

    public PLAYER_STATE GetPlayerState() 
    {
        return m_playerState;
    }

    public void ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE state)
    {
        m_cameraOnOffState = state;
    }

    public void ChangePlayerState(PLAYER_STATE state) 
    {
        m_playerState = state;
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

        AreaController currentArea = m_MapData.GetArea(m_currentAreaIndex);
        AreaController nextArea = m_MapData.GetArea(moveIndex);

        if (nextArea == null || nextArea.GetAreaType() == AREA_TYPE.WALL)
        {
            yield break;
        }

        currentArea.gameObject.SetActive(false);
        nextArea.gameObject.SetActive(true);

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
