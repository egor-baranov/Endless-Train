using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Pausable
{
    public int hp = 15;
    public int Coins = 0;
    public bool DialogWindowOpened = false;
    public int X, Y;  
    public bool UnderLadder = false, FixedOnLadder = false, OnStairs = false;
    public bool Jumping = false;
    public bool MobileMode = false;
    float DefaultGravityScale;
    
    public GameObject DialogWindow;
    public List<ItemObject> Backpack = new List<ItemObject>();
    public int BackpackSizeY = 3, BackpackSizeX = 5;
    public GameObject Inventory, Generator;
    public Sprite[] HeartSprites = new Sprite[6];
    public List<GameObject> Hearts = new List<GameObject>(3); 
    public int HP { 
        get => hp; 
        set { 
            GetComponent<Animator>().Play(value > hp ? "Health Restoring" : "Recieve Damage", 1);
            hp = value;
            if(value > 5 * Hearts.Count) hp = 5 * Hearts.Count;
            else if (value < 0) hp = 0;
            for(int i = 0; i < Hearts.Count; ++i) {
                if(hp >= (i + 1) * 5) Hearts[i].GetComponent<Image>().sprite = HeartSprites[5];
                else if(hp <= 5 * i)  Hearts[i].GetComponent<Image>().sprite = HeartSprites[0];
                else Hearts[i].GetComponent<Image>().sprite = HeartSprites[hp % 5];
            }
            if(hp == 0) Die();
        } 
    }
    public override void Start() {
        base.Start();
        Generator = GameObject.Find("Generator");
        Generator.GetComponent<Generator>().Player = gameObject;
        Camera.main.GetComponent<CameraScript>().Player = gameObject;
        Camera.main.GetComponent<CameraScript>().YDifference = transform.position.y - Camera.main.transform.position.y;
        Generator.GetComponent<Generator>().JumpButton.GetComponent<GUIButton>().TODOClick += Jump;
        Generator.GetComponent<Generator>().UpArrow.GetComponent<GUIButton>().TODOPressed += UpByLadder;
        Generator.GetComponent<Generator>().LeftArrow.GetComponent<GUIButton>().TODOPressed += MoveLeft;
        Generator.GetComponent<Generator>().RightArrow.GetComponent<GUIButton>().TODOPressed += MoveRight;
        Generator.GetComponent<Generator>().MoveController.GetComponent<MoveController>().TODO += StopMoving;
        DefaultGravityScale = GetComponent<Rigidbody2D>().gravityScale;
    }
    public override void Update() {
        base.Update();
        if(Paused) return;
        if(Input.GetKeyDown(KeyCode.KeypadPlus)) ++HP;
        else if(Input.GetKeyDown(KeyCode.KeypadMinus)) --HP;
        X = (int)(transform.position.x / 0.32F);
        if (transform.position.x / 0.32F - X >= 0.5) X++;
        Y = (int)(transform.position.y / 0.32F);
        if (transform.position.y / 0.32F - Y >= 0.5) Y++;
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKey(KeyCode.D)) { transform.rotation = Quaternion.identity;
            GetComponent<Rigidbody2D>().velocity = new Vector2 (1.6F / (Jumping ? 1.3F : 1), GetComponent<Rigidbody2D>().velocity.y);
        }
        else if (Input.GetKey(KeyCode.A))  { transform.rotation = new Quaternion(0, 180, 0, 1);
            GetComponent<Rigidbody2D>().velocity = new Vector2 (-1.6F / (Jumping ? 1.3F : 1), GetComponent<Rigidbody2D>().velocity.y);
        }
        if(OnStairs){
            if(!MobileMode && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Space))  
                    { GetComponent<Rigidbody2D>().gravityScale = 0; GetComponent<Rigidbody2D>().velocity = Vector2.zero; } 
            else if(MobileMode && !Generator.GetComponent<Generator>().RightArrow.GetComponent<GUIButton>().Pressed && 
                !Generator.GetComponent<Generator>().LeftArrow.GetComponent<GUIButton>().Pressed && 
                !Generator.GetComponent<Generator>().JumpButton.GetComponent<GUIButton>().Pressed)
                    { GetComponent<Rigidbody2D>().gravityScale = 0; GetComponent<Rigidbody2D>().velocity = Vector2.zero; } 
        }
        if (UnderLadder && !MobileMode)  { 
            if(Input.GetKey(KeyCode.W))  { 
                FixedOnLadder = true; 
                GetComponent<Rigidbody2D>().gravityScale = DefaultGravityScale; 
                GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 1.6F); 
            } 
            else if(Input.GetKey(KeyCode.S))  { 
                FixedOnLadder = false;
                GetComponent<Rigidbody2D>().gravityScale = DefaultGravityScale;
            }
            else if(FixedOnLadder && !Input.GetKey(KeyCode.Space)){ 
                GetComponent<Rigidbody2D>().gravityScale = 0; 
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0); 
            }
        } 
        else if(UnderLadder && MobileMode) {
            if(Generator.GetComponent<Generator>().UpArrow.GetComponent<GUIButton>().Pressed)  { 
                FixedOnLadder = true; 
                GetComponent<Rigidbody2D>().gravityScale = DefaultGravityScale; 
                GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 1.6F); 
            } 
            else if(Generator.GetComponent<Generator>().DownArrow.GetComponent<GUIButton>().Pressed)  { 
                FixedOnLadder = false;
                GetComponent<Rigidbody2D>().gravityScale = DefaultGravityScale;
            }
            else if(FixedOnLadder && !Generator.GetComponent<Generator>().JumpButton.GetComponent<GUIButton>().Pressed){ 
                GetComponent<Rigidbody2D>().gravityScale = 0; 
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0); 
            }
        }
        if(!UnderLadder && !OnStairs) { GetComponent<Rigidbody2D>().gravityScale = DefaultGravityScale; }
        if (!Jumping) {
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) GetComponent<Animator>().Play("Move");
            else if(!MobileMode)
            { GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y); GetComponent<Animator>().Play("Idle"); }
        }
        //if ((Input.GetKeyUp(KeyCode.A) && !Input.GetKey(KeyCode.D)) || (Input.GetKeyUp(KeyCode.D) && !Input.GetKey(KeyCode.A))) 
        if(GetComponent<Rigidbody2D>().velocity.x != 0 || GetComponent<Rigidbody2D>().velocity.y != 0) 
            Camera.main.GetComponent<CameraScript>().Move(transform.position.x, transform.position.y);

    }
    public void StopMoving(){
        if (Jumping || !MobileMode || Paused) return;
        GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y); 
        GetComponent<Animator>().Play("Idle"); 
    }
    public void MoveLeft(){
        if (Paused) return;
        if(!Jumping) GetComponent<Animator>().Play("Move"); 
        transform.rotation = new Quaternion(0, 180, 0, 1); 
        GetComponent<Rigidbody2D>().velocity = new Vector2 (-1.6F / (Jumping ? 1.3F : 1), GetComponent<Rigidbody2D>().velocity.y); 
    }
    public void MoveRight(){
        if (Paused) return;
        if(!Jumping) GetComponent<Animator>().Play("Move"); 
        transform.rotation = Quaternion.identity; 
        GetComponent<Rigidbody2D>().velocity = new Vector2 (1.6F / (Jumping ? 1.3F : 1), GetComponent<Rigidbody2D>().velocity.y); 
    }
    public void UpByLadder(){
        // if(!UnderLadder || Paused) return;
        // transform.rotation = new Quaternion(0, 180, 0, 1); 
        // GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 1.6F); 
    }
    public void DownByLadder(){

    }
    public void OpenDialogWindow(){
        // if (DialogWindowOpened) return;
        // DialogWindowOpened = true;
        // DialogWindow.GetComponent<Animator>().Play("Open");
    }
    public void HideDialogWindow(){
        // if (!DialogWindowOpened) return;
        // DialogWindowOpened = false;
        // DialogWindow.GetComponent<Animator>().Play("Close");
    }
    public void ShowContent(ref List<GameObject> content){
        Generator.GetComponent<Generator>().InventoryPanel.GetComponent<MenuPanel>().Recreate(ref Backpack, 5, 7);
    }
    public void OnTriggerStay2D(Collider2D other){
        if(other.tag == "Ladder") UnderLadder = true;
    }
    public void OnTriggerExit2D(Collider2D other){
        if(other.tag == "Ladder") { UnderLadder = false;} 
    }
    public void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.GetComponent<Block>()) Jumping = false;
    }
    public void OnCollisionStay2D(Collision2D other){
        if(other.gameObject.tag == "Stairs") { OnStairs = true; } 
    }
    public void OnCollisionExit2D(Collision2D other){
        if(other.gameObject.tag == "Stairs") OnStairs = false;
    }
    void Jump() => Jump(1.2F);
    void Jump(float height) => StartCoroutine(JumpCoroutine(height));
    private IEnumerator JumpCoroutine(float height) {
        if (Jumping == true) yield break;
        Jumping = true; 
        GetComponent<Animator>().Play("Jump");
        GetComponent<Rigidbody2D>().AddForce(transform.up * height, ForceMode2D.Impulse);
        float time = 0, LastTPY = transform.position.y;
        while(Jumping) {
            yield return null;
            // if(Mathf.Abs(LastTPY - transform.position.y) < 0.001F) time += 0.02F;
            // else { time = 0; LastTPY = transform.position.y; }
        }
        GetComponent<Animator>().Play("Idle");
        Jumping = false;
        yield break;
    }
    public void OpenInventory(){
        Generator.GetComponent<Generator>().InventoryPanel.GetComponent<MenuPanel>().Recreate(ref Backpack, BackpackSizeY, BackpackSizeX);
    }
    public void AddToBackPack(ItemObject item){
        if(item == null) return;
        foreach (var i in Backpack) if(i.Type == item.Type) { i.Amount += item.Amount; return; }
        Backpack.Add(item);
    }
    void Die(){
        Debug.Log("О нет вы умерли");
        HP = 15; 
    }
}

