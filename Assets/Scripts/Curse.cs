using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse : Pausable
{
    public GameObject Generator, Player;
    public float TimeAlive = 10F, Speed = 1F;
    public int Damage = 1; 
    public override void Start() {
        base.Start();
        Generator = GameObject.Find("Generator");
        Player = Generator.GetComponent<Generator>().Player;
    }
    public override void Update() {
        base.Update();
        if (TimeAlive < 0) { GetComponent<Rigidbody2D>().velocity = Vector2.zero; GetComponent<Animator>().Play("Destroy"); }
        if (!Paused && TimeAlive > 0 && DistanceFrom(Player.transform.position) >= Speed) GetComponent<Rigidbody2D>().velocity = Difference(Player.transform.position).normalized * Speed;
        else if (!Paused && TimeAlive > 0) GetComponent<Rigidbody2D>().velocity = Difference(Player.transform.position);
        TimeAlive -= 0.02F;
    }
    void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Player>()) {
            other.GetComponent<Player>().HP -= Damage;
        }
    }
    public void Die() => Destroy(gameObject);

    Vector3 Difference(Vector3 obj) => new Vector3(obj.x - transform.position.x, obj.y - transform.position.y, 0);
    float DistanceFrom(Vector3 obj) => Mathf.Sqrt(Mathf.Pow(transform.position.x - obj.x, 2) + Mathf.Pow(transform.position.y - obj.y, 2));
}
