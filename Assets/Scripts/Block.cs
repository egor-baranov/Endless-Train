using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public int X = -1, Y = -1;
    public BlockType Type = BlockType.None;
    public GameObject Generator;
    void Start(){ 
        Generator = GameObject.Find("Generator");
        X = (int)(transform.position.x / 0.32F);
        if (transform.position.x / 0.32F - X >= 0.5) X++;
        Y = (int)(transform.position.y / 0.32F);
        if (transform.position.y / 0.32F - Y >= 0.5) Y++;
        if (transform.parent.GetComponent<PartialTemplate>()) 
            GetComponent<SpriteRenderer>().sprite = transform.parent.GetComponent<PartialTemplate>().PTData[Type]; 
        if (Type != BlockType.Ladder && Type != BlockType.Chest && Type != BlockType.BGWall) 
            Generator.GetComponent<Generator>().CurrentLevel.GetComponent<Level>().CellBlock[Y, X] = Type;
        else Generator.GetComponent<Generator>().CurrentLevel.GetComponent<Level>().CellBlock[Y, X] = BlockType.None;
    }
    public void Delete() {
        Destroy(gameObject);
    }
    public void OnDestroy(){
        if(Generator) Generator.GetComponent<Generator>().CurrentLevel.GetComponent<Level>().CellBlock[Y, X] = BlockType.None;
    }
}

public enum BlockType {
    BGWall, FGWall, Ladder, Stairs, Window, Chest, Block, Ground, Doors, None
}