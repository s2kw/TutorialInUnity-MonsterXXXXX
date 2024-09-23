using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

namespace Fix
{
    public class InputManager : Singleton<InputManager>,  IDragHandler, IBeginDragHandler, IEndDragHandler, IUIFullScreenEventReceiver
    {
        [Tooltip("ゲーム空間全体を覆うD&D可能なスクリーン")]
        [SerializeField] private Button gameFieldScreen;
        
        [SerializeField][ReadOnly]
        private Vector2 startPos, currentPos;
        
        private HashSet<IDragInputReceiver> dragEventReceivers = new();
        public void TellMeWhenDrag(IDragInputReceiver d) => dragEventReceivers.Add(d);
        public void DontTellMeWhenDrag(IDragInputReceiver d) => dragEventReceivers.Remove(d);
        
        private HashSet<IStrikeInputReceiver> strikeInfoReceivers = new();
        public void TellMeWhenStrikeEnd(IStrikeInputReceiver d) => strikeInfoReceivers.Add(d);
        public void DontTellMeWhenStrikeEnd(IStrikeInputReceiver d) => strikeInfoReceivers.Remove(d);

        private bool inputEnable = true;
        public bool SetInputEnable(bool val) => inputEnable;

        void Start()
        {
            UIManager.Instance?.TellMeWhenStartFullScreenEvent(this);
        }

        private void OnDestroy()
        { 
            UIManager.Instance?.DontTellMeWhenStartFullScreenEvent(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!inputEnable) return;
            // Debug.Log($"Drag開始: {eventData.position}");
            
            startPos = currentPos = eventData.position;
            
            foreach (var e in dragEventReceivers) e.OnBeginDrag(eventData.position);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!inputEnable) return;

            currentPos = eventData.position;
            
            foreach (var e in dragEventReceivers) e.OnDrag(eventData.position);
            
        }
    
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!inputEnable) return;
            // Debug.Log($"Drag終了: {eventData.position}");

            foreach (var e in dragEventReceivers) e.OnEndDrag(eventData.position);
            
            // ドラッグ開始位置から現在位置の差を取得
            Vector2 direction = currentPos - startPos;
            
            // 位置の差を角度に変換
            float angleRadians = Mathf.Atan2(direction.y, direction.x);
            // ラジアンから度に変換
            float angleDegrees = angleRadians * Mathf.Rad2Deg - 90;
            // 角度を0から360度の範囲に調整
            angleDegrees = (angleDegrees + 360) % 360;

            // 情報を必要とするオブジェクト群に連絡
            foreach (var e in strikeInfoReceivers)
            {
                e.OnTriggerStrike(angleDegrees);
            }
        }
        
        #region IUIEvent の受信

        public void OnStartScreenEvent( UIManager.UIEventType type )
        {
            switch (type)
            {
                case UIManager.UIEventType.GAME_START:
                case UIManager.UIEventType.GAME_OVER:
                case UIManager.UIEventType.PLAYER_PHASE:
                case UIManager.UIEventType.ENEMY_PHASE:
                default:
                    break;
            }
        }

        public void OnFinishScreenEvent(UIManager.UIEventType type)
        {
            switch (type)
            {
                case UIManager.UIEventType.GAME_START:
                case UIManager.UIEventType.GAME_OVER:
                case UIManager.UIEventType.PLAYER_PHASE:
                case UIManager.UIEventType.ENEMY_PHASE:
                default:
                    break;
            }
        }


        #endregion

        
    }
}

