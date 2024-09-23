using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fix 
{
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<UIEventType, Action> callbackDict = new();
        public enum UIEventType
        {
            GAME_START,
            PLAYER_PHASE,
            ENEMY_PHASE,
            GAME_OVER,
            GAME_ClEAR
        }

        // 画面に表示するラベル文字列データ
        private readonly Dictionary<UIEventType, string> UIEventText = new()
        {
            {UIEventType.GAME_START,"Game Start!"},
            {UIEventType.PLAYER_PHASE,"Player Phase"},
            {UIEventType.ENEMY_PHASE,"Enemy Phase"},
            {UIEventType.GAME_OVER,"Game Over"},
            {UIEventType.GAME_ClEAR,"Clear!"},
        };
        
        [FormerlySerializedAs("uipanelHolder")]
        [Tooltip("PlayerPhaseとか表示するやつの置き場所")]
        [SerializeField] private RectTransform uiPanelHolder;

        [SerializeField] private IUIPanel panel;
        public void Init()
        {
            this.panel = UIPanel.CreatePanel(this.uiPanelHolder);

        }
        
        public void StartGameStartFullScreenEvent( Action _callback )
        {
            var t = UIEventType.GAME_START;
            callbackDict[t] = _callback;
            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnStartScreenEvent(t);
            }
        }
        public void FinishGameStartFullScreenEvent()
        {
            var t = UIEventType.GAME_START;
            callbackDict.GetValueOrDefault(t)?.Invoke();
            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnFinishScreenEvent(t);
            }
        }
        public void StartGameOverFullScreenEvent( Action _callback )
        {
            var t = UIEventType.GAME_OVER;
            callbackDict[t] = _callback;
            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnStartScreenEvent(t);
            }
        }
        public void FnishGameOverFullScreenEvent()
        {
            var t = UIEventType.GAME_OVER;
            callbackDict.GetValueOrDefault(t)?.Invoke();
            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnFinishScreenEvent(t);
            }
        }
        public void StartPlayerPhaseFullScreenEvent( Action _callback )
        {
            var t = UIEventType.PLAYER_PHASE;
            callbackDict[t] = _callback;
            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnStartScreenEvent(UIEventType.PLAYER_PHASE);
            }
        }
        public void FinishPlayerPhaseFullScreenEvent()
        {
            var t = UIEventType.PLAYER_PHASE;
            callbackDict.GetValueOrDefault(t)?.Invoke();
            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnFinishScreenEvent(UIEventType.PLAYER_PHASE);
            }
        }
        public void StartEnemyPhaseFullScreenEvent(Action _callback )
        {
            var t = UIEventType.ENEMY_PHASE;
            callbackDict[t] = _callback;

            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnStartScreenEvent(UIEventType.ENEMY_PHASE);
            }
        }
        public void FinishEnemyPhaseFullScreenEvent()
        {
            var t = UIEventType.ENEMY_PHASE;
            callbackDict.GetValueOrDefault(t)?.Invoke();
            foreach (var e in this.fullScreenEventReceiver)
            {
                e.OnFinishScreenEvent(UIEventType.ENEMY_PHASE);
            }
        }
        
        private HashSet<IUIFullScreenEventReceiver> fullScreenEventReceiver = new();

        public void TellMeWhenStartFullScreenEvent(IUIFullScreenEventReceiver e) => fullScreenEventReceiver.Add(e);
        public void DontTellMeWhenStartFullScreenEvent(IUIFullScreenEventReceiver e) => fullScreenEventReceiver.Remove(e);
        
        
    }
}
