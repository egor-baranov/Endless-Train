using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausable : MonoBehaviour
{
    public Vector3 v; // velocity
    public float g = 1.298318498F; // gravityScale
    public bool paused = false;
    public bool Paused { 
        get => paused; 
        set { 
            if (value != paused && value == true) {
                v = GetComponent<Rigidbody2D>().velocity; 
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                g = GetComponent<Rigidbody2D>().gravityScale;
                GetComponent<Rigidbody2D>().gravityScale = 0;
                GetComponent<Animator>().speed = 0;
            } 
            if(value != paused && value == false) {
                if(v != null) GetComponent<Rigidbody2D>().velocity = v;
                if(g != 1.298318498F) GetComponent<Rigidbody2D>().gravityScale = g;
                GetComponent<Animator>().speed = 1; 
            }
            paused = value; 
        } 
    }
    public virtual void Start() {
        g = GetComponent<Rigidbody2D>().gravityScale;
        v = GetComponent<Rigidbody2D>().velocity;
    }
    public virtual void Update() {
        Paused = GameObject.Find("Generator").GetComponent<Game>().Paused;
    }
}
