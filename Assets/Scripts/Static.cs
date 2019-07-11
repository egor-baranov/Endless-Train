using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static : Pausable
{   

    public float Lifespan = 5F;
    public int Damage = 1;
    public delegate void Delegate(float i);
    public override void Start(){
        base.Start();
        StartCoroutine(DestroyTracking());
    }
    void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Player>()) {
            other.GetComponent<Player>().HP -= Damage;
        }
    }
    void Die() {
        Destroy(gameObject);
    }
    IEnumerator DestroyTracking(Delegate endAction = null) {
        yield return new WaitForSeconds(Lifespan);
        GetComponent<Animator>().Play("Destroy");
    }
}
