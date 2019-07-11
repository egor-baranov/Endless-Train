using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGWall : MonoBehaviour
{
    float CurrentTransperency = 1F;
    public void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player") SetTransperency(0.05F);
    }
    public void OnTriggerExit2D(Collider2D other){
        if(other.tag == "Player") SetTransperency(1);
    }
    void SetTransperency(float EndTransperency) => StartCoroutine(ChangeTransperency(EndTransperency));
    IEnumerator ChangeTransperency(float EndTransperency)
    {
        CurrentTransperency = EndTransperency;
        while(Mathf.Abs(GetComponent<SpriteRenderer>().color.a  - EndTransperency) >= 0.001F)
        {
            yield return new WaitForSeconds(0.01F);
            if (EndTransperency != CurrentTransperency) yield break;
            Color Current = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(Current.r, Current.g, Current.b, Current.a + (EndTransperency - Current.a) / 10);
        }
        yield break;
    }
}
