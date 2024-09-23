using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fix
{
    /// <summary>
    /// 操作における「攻撃」（Strikeと呼ぶ）を行ったことの通知を受ける
    /// </summary>
    public interface IStrikeInputReceiver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rad">角度情報</param>
        void OnTriggerStrike(float rad);
    }

    public interface IDragInputReceiver
    {
        void OnBeginDrag(Vector2 pos);
        void OnDrag(Vector2 pos);
        void OnEndDrag(Vector2 pos);
    }
    
    /// <summary>
    /// 攻撃可能性のあるプレイヤーキャラクタによる通知を受け取る用で攻撃の完了を管理するオブジェクトへの通知を行う
    /// </summary>
    public interface IAttackerEventReceiver
    {
        // void OnBeginAttack( IAttacker a );
        // void OnEndAttack(IAttacker a);
    }
    
    public interface IAttacker
    {
        
    }

    /// <summary>
    /// 敵のインターフェース
    /// </summary>
    public interface IEnemy
    {
        // ターン減算とその演出
        IEnumerator DecrementAndShowEnemyTurn();
        // 攻撃の実行
        IEnumerator Attack();
        // 出現演出
        IEnumerator Spawn();
    }
    
    /// <summary>
    /// UIイベントの開始と終了を待つタイプのインターフェース。
    /// 「ゲームスタート！」表示開始や終了を通知してくれる。
    /// </summary>
    public interface IUIFullScreenEventReceiver
    {
        void OnStartScreenEvent(UIManager.UIEventType eventType);
        void OnFinishScreenEvent(UIManager.UIEventType eventType);
    }

    public interface IUIPanel
    {
        void Reset(string text = "");
        void StartAnimation(System.Action _callback = null);
    }
}