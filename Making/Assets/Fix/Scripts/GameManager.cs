using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Fix
{
    public class GameManager : Singleton<GameManager>, IStrikeInputReceiver, IAttackerEventReceiver
    {
        [Tooltip("UIを表示するためのCanvasを参照しておく")]
        [SerializeField] private Canvas UICanvas;
    
        [SerializeField] private Transform PlayerCharacterHolder, EnemyHolder, BGRoot;
        [SerializeField] private GameState state = GameState.Initialize;
        private HashSet<IEnemy> enemies = new();

        private float life;
        public float Life => life;
        
        enum GameState
        {
            Initialize,
            StartScreen,
            BeginPlayerTurn,
            WaitingPlayerInput, // 入力待ち
            PlayerInputApply, // プレイヤーの操作反映待ち
            EndPlayerTurn, // プレイヤーターンが完了した
            BeginEnemyTurn,
            WaitingEnemyMovement,
            EndEnemyTurn,
            GameOverScreen,
        }

        private bool finishGame = false;
        IEnumerator Start()
        {
            finishGame = false;
            // 初期化関連
            // インプットマネージャ出現まで待つ
            while ( InputManager.Instance == null )
            {
                yield return null;
            }
            
            // 通知の依頼
            InputManager.Instance.TellMeWhenStrikeEnd(this);

            // UIManager出現まで待つ
            while ( UIManager.Instance == null )
            {
                yield return null;
            }
            UIManager.Instance.Init();
            
            // プレイヤーキャラの生成 TODO:データに基づいて生成する
            
            
            // 敵の生成
            foreach (var e in enemies)
            {
                yield return e.Spawn();
            }
            
            // ゲーム開始
            StartCoroutine(UpdateGameCycle());
        }
    
        private void OnDestroy()
        {
            StopAllCoroutines();
            
            InputManager.Instance?.DontTellMeWhenStrikeEnd(this);
        }
    
        /// <summary>
        /// ゲームサイクルの実行
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateGameCycle()
        {
            // UIのフルスクリーンイベントを呼び出す
            UIManager.Instance.StartGameStartFullScreenEvent(() => { this.state = GameState.StartScreen;});

            // フルスクリーンイベントが終わるまで待つ
            while (this.state is not GameState.StartScreen) yield return null;
            
            // ゲームをループさせる
            while (finishGame)
            {
    
                yield return StartCoroutine(this.BeginPlayerTurn());
    
                yield return StartCoroutine(this.BeginEnemyTurn());
            }
            
            // GameOverFullScreenを表示し、終わったらステートを切り替える
            UIManager.Instance.StartGameOverFullScreenEvent(() => { this.state = GameState.GameOverScreen;});
            
            // フルスクリーンイベントが終わるまで待つ
            while (this.state is not GameState.GameOverScreen) yield return null;
            
            // TODO:シーン遷移

        }
    
        /// <summary>
        /// プレイヤーターン開始の演出
        /// </summary>
        /// <returns></returns>
        IEnumerator BeginPlayerTurn()
        {
            //　プレイヤーフェーズの表示
            UIManager.Instance?.StartPlayerPhaseFullScreenEvent(() => { this.state = GameState.BeginPlayerTurn;} );
            
            // プレイヤーフェーズの表示終了待ちまで待つ
            while (this.state is not GameState.BeginPlayerTurn) yield return null;

            // プレイヤーの操作を可能に
            InputManager.Instance?.SetInputEnable(true);
            
            // OnTriggerStrikeによる操作完了待ちまで待つ
            while (this.state != GameState.WaitingPlayerInput) yield return null;
            
            // プレイヤー操作の反映待ち
            InputManager.Instance?.SetInputEnable(false);
            while (this.state != GameState.PlayerInputApply) yield return null;
            
            // TODO:ゲームクリア判定
            finishGame = false;

            // 終了
            
        }
    
        // 敵のターンの開始
        IEnumerator BeginEnemyTurn()
        {
            //　敵フェーズの表示
            UIManager.Instance?.StartPlayerPhaseFullScreenEvent(() => { this.state = GameState.BeginEnemyTurn;} );
            
            // 敵の表示終了待ちまで待つ
            while (this.state is not GameState.BeginEnemyTurn) yield return null;

            // ターンの減算
            foreach ( var e in enemies )
            {
                yield return StartCoroutine(e.DecrementAndShowEnemyTurn());
            }
            
            // 敵の処理終了まで待つ
            foreach ( var e in enemies )
            {
                yield return StartCoroutine(e.Attack());
            }
            
            while (this.state is not GameState.EndEnemyTurn) yield return null;
            
            // TODO:ゲームオーバー判定
            finishGame = false;

            // 終了

        }

        // プレイヤーキャラへのキャッシュ
        private HashSet<IStrikeInputReceiver> strikeListener = new();

        // プレイヤーキャラが生成されるとここに入ってくる
        public void AddPlayer(PlayerCharacter p)
        {
            this.strikeListener.Add(p);
        }
        
        public void OnTriggerStrike(float rad)
        {
            // 操作完了の通知を受けった
            this.state = GameState.PlayerInputApply;

            foreach (var strikeInputReceiver in strikeListener)
            {
                strikeInputReceiver.OnTriggerStrike(rad);
            }
        }
        
        #region プレイヤーキャラが攻撃中かどうかを管理する

        private HashSet<IAttackerEventReceiver> playerSideAttackers = new();
        public void TellMeWhenFinishPlayerAttacks(IAttackerEventReceiver e) => playerSideAttackers.Add(e);
        public void DontTellMeWhenFinishPlayerAttacks(IAttackerEventReceiver e) => playerSideAttackers.Remove(e);
        
        #endregion レイヤーキャラが攻撃中かどうかを管理する

        private HashSet<IAttacker> attackers = new();
        public void OnBeginAttack(IAttacker a)
        {
            attackers.Add(a);
        }

        public void OnEndAttack(IAttacker a)
        {
            attackers.Remove(a);
            
            // 全員の操作が完了した？
            if (attackers.Count <= 0)
            {
                if (this.state is GameState.PlayerInputApply)
                {
                    this.state = GameState.EndPlayerTurn;
                    
                }else if (this.state is GameState.WaitingEnemyMovement)
                {
                    this.state = GameState.EndEnemyTurn;
                }
            }
        }

        bool IsClear()
        {
            return enemies.Count <= 0;
        }

        bool IsLose()
        {
            return this.life <= 0;
        }
    }
    
}
