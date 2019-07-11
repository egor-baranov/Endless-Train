using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Pausable
{
    public bool Empty = false;
    public bool Opened = false;
    public bool canBeOpened = false;
    public List<ItemObject> Content = new List<ItemObject>();
    public Sprite CoinSprite;
    public GameObject Player, Generator;
    public int ChestSizeX = 4, ChestSizeY = 3;
    public bool CanBeOpened { 
        get => canBeOpened;
        set { 
            canBeOpened = value; 
            if(canBeOpened) Player.GetComponent<Player>().OpenDialogWindow();
            else Player.GetComponent<Player>().HideDialogWindow();
        }
    }
    public override void Start(){
        base.Start();
        Generator = GameObject.Find("Generator");
        Player = Generator.GetComponent<Generator>().Player;
        Fill();
    }
    public override void Update() {   
        base.Update();
        if(Opened) Generator.GetComponent<Generator>().Context = ItemMenuContext.Chest;
        CanBeOpened = DistanceFrom(Player.transform.position) <= 0.6F;
        if(Input.GetKeyDown(KeyCode.E) && CanBeOpened && !Opened && !Generator.GetComponent<Generator>().MenuOpened) AnimatedOpen(); //Generator.GetComponent<Generator>().MenuPanelChangeState();
        if(Input.GetKeyDown(KeyCode.E) && Opened) Generator.GetComponent<Generator>().CloseMenu();
        if(Opened && DistanceFrom(Player.transform.position) > 0.6F) Close();
    }
    void Fill(){
        Content.Add(new ItemObject(ItemType.Coin, Random.Range(3, 12), CoinSprite));
    }
    void AnimatedOpen(){ 
        CheckEmpty();   
        Opened = true;
        if(Empty) GetComponent<Animator>().Play("Open Empty");
        else GetComponent<Animator>().Play("Open Full");
    }
    public void Open(){
        Generator.GetComponent<Generator>().Context = ItemMenuContext.Chest;
        Generator.GetComponent<Generator>().InventoryPanel.GetComponent<MenuPanel>().Recreate(ref Content, ChestSizeY, ChestSizeX, true, gameObject);
    }
    public void OnMouseDown(){
        if(CanBeOpened && !Opened && !Generator.GetComponent<Generator>().MenuOpened) AnimatedOpen();
    }
    public void Close(){
        CheckEmpty();
        Opened = false; 
        Generator.GetComponent<Generator>().Context = ItemMenuContext.Inventory;
        if(Empty) GetComponent<Animator>().Play("Close Empty");
        else GetComponent<Animator>().Play("Close Full");
        // Generator.GetComponent<Generator>().MenuPanelChangeState();
    }
    void CheckEmpty() => Empty = Content.Count == 0; 
    float DistanceFrom(Vector3 obj) => Mathf.Sqrt(Mathf.Pow(transform.position.x - obj.x, 2) + Mathf.Pow(transform.position.y - obj.y, 2));
    
}
