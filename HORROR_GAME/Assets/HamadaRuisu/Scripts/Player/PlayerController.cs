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

    [SerializeField] PlayerModel m_PlayerModel;
    [SerializeField] PlayerView m_PlayerView;
    [SerializeField] StateMachine m_StateMachine;
    [SerializeField] Animator m_hideAnimator;
    [SerializeField] Animator m_gameOverAnimation;
    [SerializeField] SceneLoader m_sceneLoader;
    [SerializeField] AreaManager m_AreaManager;
    [SerializeField] EnemyManager m_enemyManager;
    [SerializeField] Material m_skyBoxMaterial;
    [SerializeField] EffectManager m_effectManager;
    [SerializeField] SkyboxTransition_2 m_transition;
    [SerializeField] float m_captureDuration;

    [Header("カメラ周りの設定")]
    [SerializeField] CameraController m_cameraController;

    [SerializeField] LayerMask m_layerMask;
    [SerializeField] LayerMask m_captureLayerMask;

    [SerializeField] Vector2Int m_startPosition;
    Vector2Int m_currentAreaIndex;

    bool m_interactedKai = false;

    bool m_isDead = false;

    float m_currentFindRate = 1.0f;

    float m_captureTimer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_currentAreaIndex = m_startPosition;
    }

    void Start()
    {
        ChangeSate("NormalState");

        //テクスチャの取得
        MapData m_MapData = m_AreaManager.GetMapData();
        Texture currentAreaTexture = m_MapData.GetArea(m_currentAreaIndex).GetAreaSkyBoxTexture();
        m_transition.SetNextTexture(currentAreaTexture);
    }

    // Update is called once per frame
    void Update()
    {
        MoveRayHandler();

        CaptureKaiProcess();

        m_PlayerView.UpdateBatteryPercentage(m_cameraController.GetMaxBatteryValue(), m_cameraController.GetCurrentBatteryValue());

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

            CursorManager.instance.ChangeCursorImage(CURSOR_TYPE.INTERACTION);

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
            CursorManager.instance.ChangeCursorImage(CURSOR_TYPE.DEFAULT);
        }
    }

    // カメラがオンの状態で、カメラに映っているカイを捕獲する処理
    public void CaptureKaiProcess()
    {
        if (m_cameraController.GetCameraOnOffState() != CAMERA_ON_OFF_STATE.ON)
        {
            m_captureTimer = 0;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_captureLayerMask))
        {
            GameObject interactedObject = hit.collider.gameObject;

            if (interactedObject.CompareTag("Kai"))
            {
                KaiControler KaiController = interactedObject.transform.GetComponent<KaiControler>();

                if (!KaiController.WasShot())
                {
                    m_captureTimer += Time.deltaTime;

                    if (m_captureTimer >= m_captureDuration)
                    {
                        KaiController.Captured();
                        m_captureTimer = m_captureDuration;
                    }
                }
            }
        }
        else 
        {
            m_captureTimer -= Time.deltaTime * 4;

            if (m_captureTimer < 0.0f)
            {
                m_captureTimer = 0.0f;
            }
        }

        float rate = Mathf.Clamp01(m_captureTimer / m_captureDuration);

        m_effectManager.SetDolyCameraEffectRate(Easing.OutCubic(rate));
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
        if (m_cameraController.GetCameraOnOffState() == CAMERA_ON_OFF_STATE.OFF)
        {
            m_cameraController.AddBattery();
        }

        //テクスチャの取得
        Texture currentAreaTexture = m_MapData.GetArea(m_currentAreaIndex).GetAreaSkyBoxTexture();
        Texture nextAreaTexture = m_MapData.GetArea(moveIndex).GetAreaSkyBoxTexture();

        m_skyBoxMaterial.mainTexture = nextAreaTexture;
        m_currentAreaIndex = moveIndex;

        m_transition.SetNextTexture(nextAreaTexture);

        m_enemyManager.AutoMove();
    }

    public void KillPlayer()
    {
        StartCoroutine(PlayerDeathRoutine());
    }

    public IEnumerator PlayerDeathRoutine()
    {
        m_isDead = true;

        //カメラをオフにする
        m_cameraController.ChangeCameraOnOffState(CAMERA_ON_OFF_STATE.OFF);

        //カーソルをデフォルトに
        CursorManager.instance.ChangeCursorImage(CURSOR_TYPE.DEFAULT);

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
