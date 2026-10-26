using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour
{
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
    [SerializeField] float m_captureCoolDownDuration;

    [Header("カメラ周りの設定")]
    [SerializeField] CameraController m_cameraController;

    [SerializeField] LayerMask m_layerMask;
    [SerializeField] LayerMask m_captureLayerMask;

    [SerializeField] Vector2Int m_startPosition;
    Vector2Int m_currentAreaIndex;

    bool m_interactedKai = false;

    bool m_isDead = false;

    bool m_captureCoolDown = false;

    float m_cooldownTime = 0;

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

        StartCoroutine(MoveArea(new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y)));
    }

    // Update is called once per frame
    void Update()
    {
        MoveRayHandler();

        CaptureKaiProcess();

        CoolDownCaptureProcess();

        if (Input.GetKeyDown(KeyCode.P)) 
        {
            KillPlayer();
        }

    }

    public bool InteractedWithKai()
    {
        return m_interactedKai;
    }

    public void CameraInputHandler() 
    {
        if (IsDead())
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            m_cameraController.CameraONOFFHandler();
        }
    }

    public void InteractionHandler()
    {
        if (m_isDead) 
        {
            return;
        }

        if (m_cameraController.GetCameraOnOffState() == CAMERA_ON_OFF_STATE.ON)
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

            CursorManager.instance.ChangeCursorImage(CURSOR_TYPE.INTERACTION_HIDE_SPOT);

            if (Input.GetMouseButtonDown(0))
            {
                interactableTarget.OnPressed();

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
        if (m_cameraController.GetCameraOnOffState() != CAMERA_ON_OFF_STATE.ON || m_captureCoolDown)
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
                        if (!m_interactedKai) 
                        {
                            //m_enemyManager.FarstEncount();
                            m_interactedKai = true;
                        }

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

        //クールダウン開始
        if (rate >= 1) 
        {
            m_captureCoolDown = true;
            m_effectManager.ResetFromCaptureEffect();
            Invoke("PlayCaptureCompletedAud", .5f);
        }
    }

    private void PlayCaptureCompletedAud() 
    {
        AudioManagerHamada.instance.Play("CaptureComplete");
    }

    private void CoolDownCaptureProcess() 
    {
        if (!m_captureCoolDown) 
        {
            return;
        }

        m_cooldownTime += Time.deltaTime;

        float rate = Mathf.Clamp01(m_cooldownTime / m_captureCoolDownDuration);

        m_effectManager.SnowNoiseEffect(Easing.OutCubic(1 - rate));

        if (rate >= 1) 
        {
            m_captureCoolDown = false;
            m_cooldownTime = 0.0f;
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
        TryMove(Vector2Int.up);
    }

    public void MoveDown()
    {
        TryMove(Vector2Int.down);
    }

    public void MoveRight()
    {
        TryMove(Vector2Int.right);
    }

    public void MoveLeft()
    {
        TryMove(Vector2Int.left);
    }

    private void TryMove(Vector2Int direction)
    {
        if (m_isDead) 
        {
            return;
        }

        if (m_cameraController.GetCameraOnOffState() == CAMERA_ON_OFF_STATE.ON)
        {
            return;
        }

        AudioManagerHamada.instance.Play("Move");

        //配列用に方向修正
        Vector2Int adjustedDirection = new Vector2Int(direction.x, -direction.y);

        int moveAmount = 1;
        Vector2Int moveIndex = m_currentAreaIndex + (adjustedDirection * moveAmount);

        MapData m_MapData = m_AreaManager.GetMapData();
        AreaController nextArea = m_MapData.GetArea(moveIndex);

        // スキップタイプじゃないエリアに当たるまで、方向に向かって進み続ける
        while (nextArea != null && nextArea.GetAreaType() == AREA_TYPE.SKIPPABLE)
        {
            moveAmount++;
            moveIndex = m_currentAreaIndex + (adjustedDirection * moveAmount);
            nextArea = m_MapData.GetArea(moveIndex);
        }

        if (nextArea != null)
        {
            StartCoroutine(MoveArea(moveIndex));
           // Debug.Log(direction);

            m_enemyManager.AutoMove();
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

    public void CameraStateProcess() 
    {
        m_cameraController.CameraStateProcess();
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
        Debug.Log("P: " + m_MapData.GetArea(moveIndex).name + " " + moveIndex);

        m_transition.SetNextTexture(nextAreaTexture);

        m_transition.SetSkyboxRotation(nextArea.GetAreaRotation());
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

        m_sceneLoader.LoadGameOverSceneWithFade();
    }
}
