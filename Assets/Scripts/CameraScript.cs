using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Vector2 EndPos = new Vector2(0, 0); 
    public float YDifference;
    public GameObject Player; 
    void FixedUpdate() {
        /* transform.position = new Vector3(Player.transform.position.x, transform.position.y, -10); */
    }
    public void Move(float x, float y) {
        StartCoroutine(MoveCoroutineX(x)); 
        StartCoroutine(MoveCoroutineY(y + YDifference)); 
    }
    private IEnumerator MoveCoroutineX(float currentPos) {
        if (Mathf.Abs(currentPos - transform.position.x) < 1.6F) yield break;
        EndPos.x = currentPos; 
        float Difference = currentPos - transform.position.x;
        while (Mathf.Abs(Difference) >= 0.01F) {
            if(EndPos.x != currentPos) yield break; 
            yield return null; 
            //GetComponent<Rigidbody2D>().velocity = new Vector2(Player.GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y); 
            transform.position = new Vector3(transform.position.x  + Difference / 8, transform.position.y, -10); 
            Difference = currentPos - transform.position.x;
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; 
        yield break;
    }
    private IEnumerator MoveCoroutineY(float currentPos) {
        if (Mathf.Abs(currentPos - transform.position.y) < 0.8F) yield break;
        if(currentPos < 1.085F) currentPos = 1.085F;
        EndPos.y = currentPos; 
        float Difference = currentPos - transform.position.y;
        while (Mathf.Abs(Difference) >= 0.01F) {
            if(EndPos.y != currentPos) yield break; 
            yield return null; 
            //GetComponent<Rigidbody2D>().velocity = new Vector2(Player.GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y); 
            transform.position = new Vector3(transform.position.x, transform.position.y + Difference / 8, -10); 
            Difference = currentPos - transform.position.y;
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; 
        yield break;
    }
}
