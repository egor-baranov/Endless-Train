using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool Groupable = true, HasPickUpAnimation = false;
    public int Amount = 1;
    public Sprite Icon = null;
    public ItemType Type;
    Collider2D Player;
    void Start(){
        Icon = GetComponent<SpriteRenderer>().sprite;
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player") {
            Player = other;
            Player.GetComponent<Player>().AddToBackPack(new ItemObject(Type, Amount, GetComponent<SpriteRenderer>().sprite)); 
            if(HasPickUpAnimation) GetComponent<Animator>().Play("Pick Up");
        }
    }
    void PickUp() {
        if(Type == ItemType.Coin) Player.GetComponent<Player>().Coins++;
        Destroy(gameObject);
    }
    
}
public class ItemObject{
    public ItemType Type;
    public Sprite Icon = null;
    public int Amount = 1;
    public ItemObject(ItemType type, int amount = 1, Sprite icon = null){
        Type = type; Amount = amount; Icon = icon;
    }
}
public enum ItemType{
    Coin, Artifact, None
}