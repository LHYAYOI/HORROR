using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PageChenge : MonoBehaviour
{
    [SerializeField] PageList m_pageList;
    [SerializeField] Image m_left;
    [SerializeField] Image m_right;
    [SerializeField] Image m_leftTurn;
    [SerializeField] Image m_rightTurn;
    [Header("ページめくり速度")]
    [SerializeField] float m_turnTime = 0.2f;

    // 左側のページ番号
    int m_leftPageIndex = 2;
    // うわさ番号
    int m_uwasaNumber = 1;
    // 連打防止
    bool m_turningFlag = false;
    
    float m_widh = 5.89f;
    float m_height = 7.52f;

    public int GetUwasaNumber() { return m_uwasaNumber; }
    public bool GetTurningFlag() { return m_turningFlag; }

    private void Start()
    {
        UpdatePage();
    }

    void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            NextPage();
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            PreviousPage();
        }
    }

    // ページを右にめくる
    private void NextPage()
    {
        // 切換え中
        if (m_turningFlag) return;
        // 次の左ページが存在しない
        if (m_pageList.GetUwasaPageCount(m_uwasaNumber) <= m_leftPageIndex + 2) return;

        StartCoroutine(NextPageAnimation());
    }
    private IEnumerator NextPageAnimation()
    {
        m_turningFlag = true;

        // 次の見開き
        Sprite nextLeft = m_pageList.GetUwasaPage(m_uwasaNumber, m_leftPageIndex + 2);
        Sprite nextRight = m_pageList.GetUwasaPage(m_uwasaNumber, m_leftPageIndex + 3);

        // -------------------------
        // 右ページを縮める
        // -------------------------
        m_rightTurn.sprite = m_right.sprite;
        m_rightTurn.gameObject.SetActive(true);

        // 右ページの奥に次の右ページを先に置く
        m_right.sprite = nextRight;

        yield return ScalePage(m_rightTurn.rectTransform, m_widh, 0.0f, m_turnTime * 0.5f);

        m_rightTurn.gameObject.SetActive(false);

        // -------------------------
        // 左ページに広げる
        // -------------------------
        m_leftTurn.sprite = nextLeft;
        m_leftTurn.gameObject.SetActive(true);

        Vector3 scale = m_leftTurn.rectTransform.localScale;
        scale.x = 0.0f;
        m_leftTurn.rectTransform.localScale = scale;

        yield return ScalePage(m_leftTurn.rectTransform, 0.0f, m_widh, m_turnTime * 0.5f);

        // 新しい見開きに更新
        m_leftPageIndex += 2;

        UpdatePage();

        m_leftTurn.gameObject.SetActive(false);

        m_turningFlag = false;
    }

    // ページを左にめくる
    private void PreviousPage()
    {
        // 切換え中
        if (m_turningFlag) return;
        // 0ページ目に行かない
        if (m_leftPageIndex == 0) return;

        StartCoroutine(PreviousPageAnimation());
    }
    private IEnumerator PreviousPageAnimation()
    {
        m_turningFlag = true;

        // 前の見開き
        Sprite previousLeft = m_pageList.GetUwasaPage(m_uwasaNumber, m_leftPageIndex - 2);
        Sprite previousRight = m_pageList.GetUwasaPage(m_uwasaNumber, m_leftPageIndex - 1);

        // -------------------------
        // 左ページを縮める
        // -------------------------
        m_leftTurn.sprite = m_left.sprite;
        m_leftTurn.gameObject.SetActive(true);

        // 奥に前の左ページを先に置く
        m_left.sprite = previousLeft;

        yield return ScalePage(m_leftTurn.rectTransform, m_widh, 0.0f, m_turnTime * 0.5f);

        m_leftTurn.gameObject.SetActive(false);

        // -------------------------
        // 右ページを広げる
        // -------------------------
        m_rightTurn.sprite = previousRight;
        m_rightTurn.gameObject.SetActive(true);

        Vector3 scale = m_rightTurn.rectTransform.localScale;
        scale.x = 0.0f;
        m_rightTurn.rectTransform.localScale = scale;

        yield return ScalePage(m_rightTurn.rectTransform, 0.0f, m_widh, m_turnTime * 0.5f);

        m_leftPageIndex -= 2;

        UpdatePage();

        m_rightTurn.gameObject.SetActive(false);

        m_turningFlag = false;
    }


    // =========================
    // ページ表示 初期化
    // =========================
    private void UpdatePage()
    {
        m_left.sprite = m_pageList.GetUwasaPage(m_uwasaNumber, m_leftPageIndex);
        m_right.sprite = m_pageList.GetUwasaPage(m_uwasaNumber, m_leftPageIndex + 1);

        m_leftTurn.rectTransform.localScale = new Vector3(m_widh, m_height, 1.0f);
        m_rightTurn.rectTransform.localScale = new Vector3(m_widh, m_height, 1.0f);
        m_leftTurn.gameObject.SetActive(false);
        m_rightTurn.gameObject.SetActive(false);
    }

    // =========================
    // 横方向に縮小/拡大
    // =========================
    private IEnumerator ScalePage(RectTransform rect, float start, float end, float time)
    {
        float timer = 0.0f;

        while (timer < time)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / time);

            // 少し滑らかに
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            float x = Mathf.Lerp(start, end, t);

            Vector3 scale = rect.localScale;
            scale.x = x;

            rect.localScale = scale;

            yield return null;
        }

        Vector3 finalScale = rect.localScale;
        finalScale.x = end;
        rect.localScale = finalScale;
    }

    // =========================
    // 噂切り替え
    // =========================
    public void OnClickHusen(int uwasaNumber)
    {
        if (uwasaNumber == m_uwasaNumber) return;

        // めくる方向 右
        if (m_uwasaNumber < uwasaNumber)
        {
            StartCoroutine(NextUwasaAnimation(uwasaNumber));
        }
        // 左
        else
        {
            StartCoroutine(PreviousUwasaAnimation(uwasaNumber));
        }

        m_uwasaNumber = uwasaNumber;
        m_leftPageIndex = 0;
    }
    private IEnumerator NextUwasaAnimation(int uwasaNumber)
    {
        m_turningFlag = true;

        // 次の見開き
        Sprite nextLeft = m_pageList.GetUwasaPage(uwasaNumber, 0);
        Sprite nextRight = m_pageList.GetUwasaPage(uwasaNumber, 1);

        // -------------------------
        // 右ページを縮める
        // -------------------------
        m_rightTurn.sprite = m_right.sprite;
        m_rightTurn.gameObject.SetActive(true);

        // 右ページの奥に次の右ページを先に置く
        m_right.sprite = nextRight;

        yield return ScalePage(m_rightTurn.rectTransform, m_widh, 0.0f, m_turnTime * 0.5f);

        m_rightTurn.gameObject.SetActive(false);

        // -------------------------
        // 左ページに広げる
        // -------------------------
        m_leftTurn.sprite = nextLeft;
        m_leftTurn.gameObject.SetActive(true);

        Vector3 scale = m_leftTurn.rectTransform.localScale;
        scale.x = 0.0f;
        m_leftTurn.rectTransform.localScale = scale;

        yield return ScalePage(m_leftTurn.rectTransform, 0.0f, m_widh, m_turnTime * 0.5f);

        // 新しい見開きに更新
        m_leftPageIndex = 0;

        UpdatePage();

        m_leftTurn.gameObject.SetActive(false);

        m_turningFlag = false;
    }
    private IEnumerator PreviousUwasaAnimation(int uwasaNumber)
    {
        m_turningFlag = true;

        // 前の見開き
        Sprite previousLeft = m_pageList.GetUwasaPage(uwasaNumber, 0);
        Sprite previousRight = m_pageList.GetUwasaPage(uwasaNumber, 1);

        // -------------------------
        // 左ページを縮める
        // -------------------------
        m_leftTurn.sprite = m_left.sprite;
        m_leftTurn.gameObject.SetActive(true);

        // 奥に前の左ページを先に置く
        m_left.sprite = previousLeft;

        yield return ScalePage(m_leftTurn.rectTransform, m_widh, 0.0f, m_turnTime * 0.5f);

        m_leftTurn.gameObject.SetActive(false);

        // -------------------------
        // 右ページを広げる
        // -------------------------
        m_rightTurn.sprite = previousRight;
        m_rightTurn.gameObject.SetActive(true);

        Vector3 scale = m_rightTurn.rectTransform.localScale;
        scale.x = 0.0f;
        m_rightTurn.rectTransform.localScale = scale;

        yield return ScalePage(m_rightTurn.rectTransform, 0.0f, m_widh, m_turnTime * 0.5f);

        m_leftPageIndex = 0;

        UpdatePage();

        m_rightTurn.gameObject.SetActive(false);

        m_turningFlag = false;
    }
}
