using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


namespace Fix
{
    public class ArrowController: MonoBehaviour, IDragInputReceiver
    {
        [SerializeField][Tooltip("ひっぱったやじるしの長さ上限")]
        float arrowScaleLimit = 3f;
        
        [SerializeField][Tooltip("ひっぱったやじるしのサイズ調整係数")]
        float arrowScaleFactor = 300f;

        private UnityEngine.UI.Image arrowImage;

        
        private Vector2 startPos;

        void Start()
        {
            // 通知の依頼
            InputManager.Instance?.TellMeWhenDrag(this);

            UIManager.Instance?.Init();
            
            this.arrowImage = GetComponent<UnityEngine.UI.Image>();
        }

        private void OnDestroy()
        {
            // 通知の解除
            InputManager.Instance?.DontTellMeWhenDrag(this);
        }
        
        #region IDragEventReciever 関連
        public void OnBeginDrag(Vector2 pos)
        {
            this.startPos = pos;
            
            // 表示
            this.arrowImage.enabled = true;
            // スケールの初期化
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        public void OnDrag(Vector2 pos)
        {
            // 矢印の角度を変更する
            Vector2 direction = pos - startPos;
            
            // 距離
            float distance = direction.magnitude / arrowScaleFactor;
            distance = Mathf.Clamp(distance, 0.5f, arrowScaleLimit);
    
            // 距離を矢印の高さに変換
            this.transform.localScale = new Vector2(1f, distance);
    
            // 位置の差を角度に変換
            float angleRadians = Mathf.Atan2(direction.y, direction.x);
            // ラジアンから度に変換
            float angleDegrees = angleRadians * Mathf.Rad2Deg + 90f;
            // 角度を0から360度の範囲に調整
            angleDegrees = (angleDegrees + 360) % 360;
    
            this.transform.localRotation = Quaternion.Euler(0f, 0f, angleDegrees);
        }
        public void OnEndDrag(Vector2 pos)
        {
            this.arrowImage.enabled = false;
        }
        #endregion IDragEventReciever 関連
    }
}