using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Pausable
{
    public float Speed = 1F;
    public int Damage = 1;
    public bool FaceRight = true;
    public override void Start() {
        base.Start();
        GetComponent<Rigidbody2D>().velocity = (FaceRight ? Vector2.right : Vector2.left) * Speed;
        Destroy(gameObject, 12);
    }
    void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Player>()) {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            other.GetComponent<Player>().HP -= Damage;
            GetComponent<Animator>().Play("Destroy");    
        }
    }
    public void Die() {
        Destroy(gameObject);
    }
}
