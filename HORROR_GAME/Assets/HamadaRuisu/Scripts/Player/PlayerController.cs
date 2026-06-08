using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements.Experimental;
using Unity.Cinemachine;

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
    [SerializeField] Texture2D m_defaultCursorTex;
    [SerializeField] Texture2D m_interactionCursorTex;
    [SerializeField] Animator m_hideAnimator;
    [SerializeField] AreaManager m_AreaManager;
    [SerializeField] EnemyManager m_enemyManager;
    [SerializeField] FullScreenPassRendererFeature m_cameraPostEffectFeature;
    [SerializeField] SkyboxTransition_2 m_transition;

    [Header("カメラ周りの設定")]
    [SerializeField] CinemachineCamera m_camera;
    [SerializeField] float m_rotationDuration;

    [SerializeField] LayerMask m_layerMask;

    [SerializeField] Vector2Int m_startPosition;
    Vector2Int m_currentAreaIndex;

    CAMERA_ON_OFF_ATATE m_cameraOnOffState = CAMERA_ON_OFF_ATATE.OFF;
    PLAYER_STATE m_playerState = PLAYER_STATE.NORMAL;

    bool m_isRotatingCamera = false;
    bool m_interactedKai = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_currentAreaIndex = m_startPosition;
    }

    void Start()
    {
        ChangeSate("NormalState");
        ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.OFF);

        //カメラのポストエフェクトオフ
        SetCameraPostEffectFlag(false);

    }

    // Update is called once per frame
    void Update()
    {
        m_PlayerView.UpdateBatteryGages(m_PlayerModel.GetMaxBatteryValue(), m_PlayerModel.GetCurrentBatteryValue());
    }

    public bool InteractedWithKai() 
    {
        return m_interactedKai;
    }

    public void InteractionHandler() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
        {
            GameObject interactedObject = hit.collider.gameObject;

            IPressable interactableTarget = interactedObject.GetComponent<IPressable>();

            if (interactableTarget == null) 
            {
                return;
            }

            //カーソルの変更
            Cursor.SetCursor(m_interactionCursorTex, Vector2.zero, CursorMode.Auto);

            //インタラクト可能なオブジェクトを押したときの処理
            if (Input.GetMouseButtonDown(0)) 
            {
                interactableTarget.OnPressed();

                if (interactedObject.tag == "Kai")
                {
                    if (!m_interactedKai) m_enemyManager.FarstEncount();
                    m_interactedKai = true;
                }
            }
            //インタラクト可能なオブジェクトを離したときの処理
            else if (Input.GetMouseButtonUp(0)) 
            {
                interactableTarget.OnReleased();
            }
        }
        else
        {
            //カーソルの変更
             Cursor.SetCursor(m_defaultCursorTex, Vector2.zero, CursorMode.Auto);
        }
    }

    public void ChangeSate(string stateName) 
    {
        m_StateMachine.ChangeState(stateName);
    }

    public StateMachine GetStateMachine() 
    {
        return m_StateMachine;
    }

    public Vector2Int GetCurrentIndex()
    {
        return m_currentAreaIndex;
    }

    public Animator GetHideAnimator() 
    {
        return m_hideAnimator;
    }

    public void CameraInputHandler()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //カメラオフ
            if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.ON)
            {
                m_PlayerModel.DrainBattery(1);

                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.OFF);

                //カメラのポストエフェクトオフ
                SetCameraPostEffectFlag(false);
            }

            //カメラオン
            else if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.OFF && m_PlayerModel.GetCurrentBatteryValue() > 0)
            {
                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.ON);

                //カメラのポストエフェクトオン
                SetCameraPostEffectFlag(true);
            }
        }
    }

    private void SetCameraPostEffectFlag(bool flag) 
    {
        m_cameraPostEffectFeature.SetActive(flag);
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

    public void MoveAreaInputHandler()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y - 1)));
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y + 1)));
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x + 1, m_currentAreaIndex.y)));
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x - 1, m_currentAreaIndex.y)));
        //}
    }

    public void MoveUp() 
    {
        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y - 1)));
    }

    public void MoveDown()
    {
        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y + 1)));
    }

    public void MoveRight()
    {
        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x + 1, m_currentAreaIndex.y)));
    }

    public void MoveLeft()
    {
        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x - 1, m_currentAreaIndex.y)));
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
        Texture nextAreaTexture = m_MapData.GetArea(moveIndex).GetAreaSkyBoxTexture();

        m_currentAreaIndex = moveIndex;

        m_transition.SetNextTexture(nextAreaTexture);

        m_enemyManager.AutoMove();
    }

    public void RotateCameraInputHandler()
    {
        if (Input.GetKeyDown(KeyCode.D) && !m_isRotatingCamera)
        {
            StartCoroutine(RotateCameraRoutine(new Vector3(0, 90, 0)));
        }

        if (Input.GetKeyDown(KeyCode.A) && !m_isRotatingCamera)
        {
            StartCoroutine(RotateCameraRoutine(new Vector3(0, -90, 0)));
        }
    }

    IEnumerator RotateCameraRoutine(Vector3 rotationValue)
    {
        m_isRotatingCamera = true;

        float elapsedTime = 0f;

        Quaternion startRotation = m_camera.transform.rotation;

        while (elapsedTime < m_rotationDuration)
        {
            elapsedTime += Time.deltaTime;

            float rate = Easing.InOutCubic(elapsedTime / m_rotationDuration);

            rate = Mathf.Clamp01(rate);

            m_camera.transform.rotation = startRotation * Quaternion.Euler(rotationValue.x * rate, rotationValue.y * rate, rotationValue.z * rate);

            yield return null;
        }

        m_isRotatingCamera = false;
    }
}
