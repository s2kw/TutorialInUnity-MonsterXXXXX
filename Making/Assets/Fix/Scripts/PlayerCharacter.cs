using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fix
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerCharacter : MonoBehaviour, IStrikeInputReceiver, IAttacker
    {
        private Rigidbody2D rigidbody;
        private bool alreadyInit = false;
        [SerializeField] private float power=1f;
        private bool isAttacking = false;
        public void Init()
        {
            if (alreadyInit) return;
            
            rigidbody ??= GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            this.Init();
            
            // プレイヤーキャラ登録
            GameManager.Instance?.AddPlayer(this);
        }

        
        void Update()
        {
            if (isAttacking is not true) return;

            // しきい値を下回ったら終了
            if (rigidbody.velocity.magnitude <= 0.5f)
            {
                rigidbody.velocity = new Vector2(0f, 0f);
                isAttacking = false;
                OnFinishMove();
            }
        }
        
        /// <summary>
        /// 攻撃処理を受け取る
        /// </summary>
        /// <param name="rad"></param>
        public void OnTriggerStrike(float rad)
        {
            Debug.Log($"OnTriggerStrike:{rad}",this);
            float x = Mathf.Cos(rad);
            float y = Mathf.Sin(rad);
            var dir = new Vector2(x, y);

            this.rigidbody.AddForce( dir * this.power,ForceMode2D.Impulse);
            this.isAttacking = true;
            
            GameManager.Instance?.OnBeginAttack(this);
            
        }

        void OnFinishMove()
        {
            this.isAttacking = false;
            GameManager.Instance?.OnEndAttack(this);
        }
    }
}
