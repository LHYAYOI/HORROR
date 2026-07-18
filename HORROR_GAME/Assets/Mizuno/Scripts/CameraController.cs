//using UnityEngine;

//public class CameraController : MonoBehaviour
//{
//    [Header("視点操作の設定")]
//    [Tooltip("マウスで視点を動かす感度")]
//    public float lookSensitivity = 2f;

//    // カメラの回転角度を保持する変数
//    private float rotationX = 0f;
//    private float rotationY = 0f;

//    void Start()
//    {
//        // テストをやりやすくするため、ゲーム開始時にマウスカーソルを画面中央にロックして非表示にする
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;

//        // 現在のカメラの角度を初期値として設定
//        Vector3 rot = transform.localRotation.eulerAngles;
//        rotationX = rot.x;
//        rotationY = rot.y;
//    }

//    void Update()
//    {
//        // --------------------------------------------------
//        // マウスによる視点移動（見回す）
//        // --------------------------------------------------
//        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
//        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

//        rotationY += mouseX;
//        rotationX -= mouseY;

//        // 真上や真下を向いた時に首が回転しすぎないように制限（-90度〜90度）
//        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

//        // カメラの角度を更新
//        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);


       
//        // --------------------------------------------------
//        // 3. テスト終了時のカーソルロック解除
//        // --------------------------------------------------
//        // Escキーを押すとカーソルが表示され、エディタの操作に戻れるようになります
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            Cursor.lockState = CursorLockMode.None;
//            Cursor.visible = true;
//        }
//    }
//}