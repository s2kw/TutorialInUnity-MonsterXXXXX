using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private bool x,y;
    [SerializeField] private float bounceForce = 0.99f;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    { 
        // Debug.Log($"OnCollisionEnter2D: {other.gameObject.name}",this);
        // 衝突した面の法線ベクトルを使ってボールを弾く

        return;
        
        var r = other.contacts[0];

        var v = other.rigidbody.velocity;

        if (this.x)
        {
            v.x = v.x * -1;
        }

        if (this.y)
        {
            v.y = v.y * -1;
        }

        other.rigidbody.velocity = v;
    }
    
    
    public void Bounce(Vector2 normal, float force)
    {
        // 現在の速度を取得
        Vector2 currentVelocity = rb.velocity;

        // 反射ベクトルを計算
        Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, normal);

        // 反射ベクトルに力を加える
        rb.AddForce(reflectedVelocity.normalized * bounceForce, ForceMode2D.Impulse);
    }
}
