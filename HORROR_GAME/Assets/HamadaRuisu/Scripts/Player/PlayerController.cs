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

    [SerializeField] PlayerModel m_PlayerModel;
    [SerializeField] PlayerView m_PlayerView;
    [SerializeField] StateMachine m_StateMachine;
    [SerializeField] Texture2D m_defaultCursorTex;
    [SerializeField] Texture2D m_interactionCursorTex;
    [SerializeField] Animator m_hideAnimator;
    [SerializeField] Animator m_gameOverAnimation;
    [SerializeField] SceneLoader m_sceneLoader;
    [SerializeField] AreaManager m_AreaManager;
    [SerializeField] EnemyManager m_enemyManager;
    [SerializeField] Material m_skyBoxMaterial;
    [SerializeField] SkyboxTransition_2 m_transition;
    [SerializeField] FullScreenPassRendererFeature m_cameraPostEffectFeature;
    [SerializeField] Volume m_volume;
    Tonemapping m_tonemap;
    Bloom m_bloom;
    ColorAdjustments m_colorAdjustments;
    ChromaticAberration m_chromaticAberration;

    [Header("カメラ周りの設定")]
    [SerializeField] CinemachineCamera m_camera;
    [SerializeField] float m_rotationDuration;

    [SerializeField] LayerMask m_layerMask;

    [SerializeField] Vector2Int m_startPosition;
    Vector2Int m_currentAreaIndex;

    CAMERA_ON_OFF_ATATE m_cameraOnOffState = CAMERA_ON_OFF_ATATE.OFF;

    bool m_interactedKai = false;

    bool m_isDead = false;

    float m_currentFindRate = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_currentAreaIndex = m_startPosition;
    }

    void Start()
    {

        m_volume.profile.TryGet(out m_tonemap);
        m_volume.profile.TryGet(out m_bloom);
        m_volume.profile.TryGet(out m_colorAdjustments);
        m_volume.profile.TryGet(out m_chromaticAberration);

        ChangeSate("NormalState");

        ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.OFF);

        //テクスチャの取得
        MapData m_MapData = m_AreaManager.GetMapData();
        Texture currentAreaTexture = m_MapData.GetArea(m_currentAreaIndex).GetAreaSkyBoxTexture();
        m_transition.SetNextTexture(currentAreaTexture);
    }

    // Update is called once per frame
    void Update()
    {
        MoveRayHandler();

        m_PlayerView.UpdateBatteryGages(m_PlayerModel.GetMaxBatteryValue(), m_PlayerModel.GetCurrentBatteryValue());

        if (Input.GetKeyDown(KeyCode.P)) 
        {
            KillPlayer();
        }

    }

    public bool InteractedWithKai()
    {
        return m_interactedKai;
    }

    public void InteractionHandler()
    {
        if (m_isDead) 
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity,m_layerMask))
        {
            GameObject interactedObject = hit.collider.gameObject;

            IPressable interactableTarget = interactedObject.GetComponent<IPressable>();

            if (interactableTarget == null) 
            {
                interactableTarget = interactedObject.transform.parent.GetComponent<IPressable>();
            }

            Vector2 hotSpot = new Vector2(m_interactionCursorTex.width / 2.0f, m_interactionCursorTex.height / 2.0f);

            Cursor.SetCursor(m_interactionCursorTex, hotSpot, CursorMode.Auto);

            if (Input.GetMouseButtonDown(0))
            {
                interactableTarget.OnPressed();

                if (interactedObject.tag == "Kai")
                {
                    if (!m_interactedKai) m_enemyManager.FarstEncount();
                    m_interactedKai = true;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    interactableTarget.OnReleased();
                }
            }
        }
        else
        {
            
            Vector2 hotSpot = new Vector2(m_defaultCursorTex.width / 2.0f, m_defaultCursorTex.height / 2.0f);
            Cursor.SetCursor(m_defaultCursorTex, hotSpot, CursorMode.Auto);
        }
    }

    public void MoveRayHandler() 
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject interactedObject = hit.collider.gameObject;

            if (interactedObject.tag != "MoveCollider") 
            {
                return;
            }

            IPressable interactableTarget = interactedObject.GetComponent<IPressable>();

            if (interactableTarget != null && Input.GetKeyDown(KeyCode.W))
            {
                interactableTarget.OnPressed();
            }
        }
    }

    public void MoveUp()
    {
        if (m_isDead)
        {
            return;
        }

        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y - 1)));
    }


    public void MoveDown()
    {
        if (m_isDead)
        {
            return;
        }

        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y + 1)));
    }

    public void MoveRight()
    {
        if (m_isDead)
        {
            return;
        }

        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x + 1, m_currentAreaIndex.y)));
    }

    public void MoveLeft()
    {
        if (m_isDead)
        {
            return;
        }

        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x - 1, m_currentAreaIndex.y)));
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

    public void SetFindRate(float rate)
    {
        m_currentFindRate = rate;
    }

    public float GetFindRate()
    {
        return m_currentFindRate;
    }

    public bool IsDead()
    {
        return m_isDead;
    }

    public void CameraInputHandler()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //カメラオフ
            if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.ON)
            {
                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.OFF);
            }

            //カメラオン
            else if (m_cameraOnOffState == CAMERA_ON_OFF_ATATE.OFF && m_PlayerModel.GetCurrentBatteryValue() > 0)
            {
                ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.ON);
            }
        }
    }

    public void DraiCameraBattery(int amount) 
    {
        m_PlayerModel.DrainBattery(amount);
    }

    public CAMERA_ON_OFF_ATATE GetCameraOnOffState()
    {
        return m_cameraOnOffState;
    }

    private void SetCameraPostEffectFlag(bool flag)
    {
        m_cameraPostEffectFeature.SetActive(flag);

        m_tonemap.active = flag;
        m_bloom.active = flag;
        m_colorAdjustments.active = flag;
        m_chromaticAberration.active = flag;
    }

    public void ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE state)
    {
        m_cameraOnOffState = state;

        switch (m_cameraOnOffState) 
        {
            case CAMERA_ON_OFF_ATATE.ON:

                SetCameraPostEffectFlag(true);

                break;

            case CAMERA_ON_OFF_ATATE.OFF:

                DraiCameraBattery(1);

                SetCameraPostEffectFlag(false);

                break;
        }
    }

    public void MoveAreaInputHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y)));
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

        m_transition.SetNextTexture(nextAreaTexture);

        m_enemyManager.AutoMove();
    }

    public void RotateCameraInputHandler()
    {
        if (m_isDead) 
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

    public void KillPlayer()
    {
        StartCoroutine(PlayerDeathRoutine());
    }

    public IEnumerator PlayerDeathRoutine()
    {
        m_isDead = true;

        //カメラをオフにする
        ChangeCameraOnOffState(CAMERA_ON_OFF_ATATE.OFF);

        //カーソルをデフォルトに
        Vector2 hotSpot = new Vector2(m_defaultCursorTex.width / 2.0f, m_defaultCursorTex.height / 2.0f);
        Cursor.SetCursor(m_defaultCursorTex, hotSpot, CursorMode.Auto);

        //死亡アニメーション再生
        m_gameOverAnimation.Play("GameOver");

        yield return null;

        //アニメーションが終わるまでシーンを切り替えない
        AnimatorStateInfo stateInfo = m_gameOverAnimation.GetCurrentAnimatorStateInfo(0);

        while(stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = m_gameOverAnimation.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        m_sceneLoader.LoadGameOverScene();
    }
}
