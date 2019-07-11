using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Pausable
{
    float CurrentTransperency = 0;
    public override void Start(){
        base.Start();
        Color Current = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(Current.r, Current.g, Current.b, 0);
        StartCoroutine(ChangeTransperency(1));
    }
    public override void Update() {
        base.Update();
        transform.position = transform.parent.position;
        if(transform.parent.GetComponent<Enemy>().HP <= 0 || 
            DistanceFrom(transform.parent.GetComponent<Enemy>().Player.transform.position) > transform.parent.GetComponent<Enemy>().RequiredDistanceFromPlayer){
            StartCoroutine(ChangeTransperency(0, true));
        }
    }
    float DistanceFrom(Vector3 obj) => Mathf.Sqrt(Mathf.Pow(transform.position.x - obj.x, 2) + Mathf.Pow(transform.position.y - obj.y, 2));
    IEnumerator ChangeTransperency(float EndTransperency, bool destroy = false)
    {
        CurrentTransperency = EndTransperency;
        while(Mathf.Abs(GetComponent<SpriteRenderer>().color.a  - EndTransperency) >= 0.001F)
        {
            yield return new WaitForSeconds(0.02F);
            if (EndTransperency != CurrentTransperency) yield break;
            Color Current = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(Current.r, Current.g, Current.b, Current.a + (EndTransperency - Current.a) / 10);
        }
        if(destroy) Destroy(gameObject);
        yield break;
    }
}
