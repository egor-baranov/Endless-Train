using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsScript : Pausable
{
    public bool Opened = false, CanBeOpened;
    public GameObject Player, Generator; 
    public ColorPalette DoorsColor;
    // public AnimatorController BlAnim, GAnim, RAnim, BrAnim;
    // public Dictionary<ColorPalette, AnimatorController> DoorsAnimator; 
    void Start(){
        base.Start();
        // DoorsAnimator = new Dictionary<ColorPalette, AnimatorController>(){
        //     [ColorPalette.Blue] = BlAnim, [ColorPalette.Green] = GAnim, [ColorPalette.Red] = RAnim, [ColorPalette.Brown] = BrAnim
        // }; 
        Generator = GameObject.Find("Generator");
        Player = Generator.GetComponent<Generator>().Player;
        DoorsColor = Generator.GetComponent<Generator>().CurrentLevel.GetComponent<Level>().LevelPalette.StairsColor; 
        DoorsColor = ColorPalette.Brown; 
        // GetComponent<Animator>().Controller = DoorsAnimator[DoorsColor]; 
    }
    void Update(){
        base.Update();
        if (Generator == null) Generator = GameObject.Find("Generator");
        if (Player == null) Player = Generator.GetComponent<Generator>().Player;
        CanBeOpened = DistanceFrom(Player.transform.position) <= 0.6F;
        if(Input.GetKeyDown(KeyCode.E) && Opened && CanBeOpened) Close();
        else if(Input.GetKeyDown(KeyCode.E) && !Opened && CanBeOpened) Open();
        if(Opened && DistanceFrom(Player.transform.position) > 0.6F) Close();
    }
    void Open(){
        Opened = true;
        GetComponent<Animator>().Play("Open");
    }
    void Close(){
        Opened = false;
        GetComponent<Animator>().Play("Close");
    }
    void OnMouseDown(){
        if(!Opened && CanBeOpened) Open();
        else if (CanBeOpened) Close();
    }
    float DistanceFrom(Vector3 obj) => Mathf.Sqrt(Mathf.Pow(transform.position.x - obj.x, 2) + Mathf.Pow(transform.position.y - obj.y, 2));
}
