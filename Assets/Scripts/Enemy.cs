using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Pausable
{
    float CurrentTransperency = 1;
    public int X = 0, Y = 0, HP = 1, FeaturesActivated = 0, AutoAttacksActivated = 0;
    public delegate void Delegate(float i);
    bool FaceRight = true;
    public GameObject Previous = null, Player, Generator, CurrentLevel;
    public GameObject Bullet, CoursePrefab, ShieldPrefab, StaticAttackPrefab, ObstructionPrefab;
    public bool HasShootAttack = false, HasActiveMode = false, CanBecomeInvisible = false, HasAttackAnimation = false, HasProtection = false, HasStaticAttack = false,
    HasObstructionAttack = false, HasCourseAttack = false;
    public bool AutoAttackEnabled = false;
    public List<TypeOfAttack> Features, Attacks;  
    Dictionary<TypeOfAttack, bool> AttackEnabled = new Dictionary<TypeOfAttack, bool>(){
        [TypeOfAttack.Course] = false, [TypeOfAttack.Obstruction] = false, [TypeOfAttack.Protection] = false, [TypeOfAttack.Shooting] = false,
        [TypeOfAttack.Static] = false, [TypeOfAttack.Invisibility] = false
    };
    public float RequiredDistanceFromPlayer = 3F, AttackInterval = 5F;
    public override void Start() {
        base.Start();
        if(CanBecomeInvisible) Features.Add(TypeOfAttack.Invisibility);
        if(HasProtection) Features.Add(TypeOfAttack.Protection);
        
        if(HasObstructionAttack) Attacks.Add(TypeOfAttack.Obstruction);
        if(HasShootAttack) Attacks.Add(TypeOfAttack.Shooting);
        if(HasStaticAttack) Attacks.Add(TypeOfAttack.Static);
        if(HasCourseAttack) Attacks.Add(TypeOfAttack.Course);
        Generator = GameObject.Find("Generator");
        Player = Generator.GetComponent<Generator>().Player;
        CurrentLevel = Generator.GetComponent<Generator>().CurrentLevel;
        // Move();
    }
    public override void Update() {
        base.Update();
        if(Paused) return;
        if (DistanceFrom(Player.transform.position) <= RequiredDistanceFromPlayer && HP > 0) {
            if(HasActiveMode) GetComponent<Animator>().Play("Active Mode");
            ActivateFeature();
            StartCoroutine(SetAutoAttack());
            if((Player.transform.position.x > transform.position.x && !FaceRight) || 
                (Player.transform.position.x < transform.position.x && FaceRight)) Flip();
        }
        else if (HP > 0) {
            AutoAttackEnabled = false;
            foreach(var i in Features) AttackEnabled[i] = false;
            GetComponent<Animator>().Play("Idle");
        }
        X = (int)(transform.position.x / 0.32F);
        Y = (int)(transform.position.y / 0.32F);
        if(X == 0 && !FaceRight) Flip();
        if(X ==  Level.Width && FaceRight) Flip();
    }
    
    void ActivateFeature() {
        if(FeaturesActivated > 0) return;
        List<TypeOfAttack> features = new List<TypeOfAttack>();
        foreach(var i in Features) 
            if(!AttackEnabled[i]) features.Add(i);
        if(features.Count == 0) return;
        switch(features[Random.Range(0, features.Count)]){
            case TypeOfAttack.Protection:
                Instantiate(ShieldPrefab, transform.position, Quaternion.identity).transform.parent = transform;
                StartCoroutine(SetPlayerTrackingForFeature(TypeOfAttack.Protection));
                break;
            case TypeOfAttack.Invisibility:
                SetTransperency(0.3F);
                StartCoroutine(SetPlayerTrackingForFeature(TypeOfAttack.Invisibility, SetTransperency));
                break;
            default:
                break;
        }
    }
    // корутина выключит фичу 
    public IEnumerator SetPlayerTrackingForFeature(TypeOfAttack feature, Delegate endAction = null){
        FeaturesActivated++;
        AttackEnabled[feature] = true;
        while(DistanceFrom(Player.transform.position) <= RequiredDistanceFromPlayer){
            yield return null;
        }
        AttackEnabled[feature] = false;
        endAction?.Invoke(1);
        FeaturesActivated--;
        yield break;
    }
    
    public IEnumerator SetAutoAttack() {
        if(AutoAttackEnabled || Attacks.Count == 0 || AutoAttacksActivated > 0) yield break;
        AutoAttacksActivated++;
        AutoAttackEnabled = true;
        while(DistanceFrom(Player.transform.position) <= RequiredDistanceFromPlayer &&  HP > 0 && AutoAttackEnabled){
            while(Paused) yield return null;
            yield return new WaitForSeconds(AttackInterval);
            if(HP <= 0 || !AutoAttackEnabled) break;
            if(HasAttackAnimation) GetComponent<Animator>().Play("Attack");
            switch(Attacks[Random.Range(0, Attacks.Count)]) {
                case TypeOfAttack.Course:
                        Instantiate(CoursePrefab, transform.position, Quaternion.identity);
                        break;
                case TypeOfAttack.Shooting:
                        Instantiate(Bullet, transform.position, new Quaternion(0, FaceRight ? 0 : 180, 0, 0)).GetComponent<Shell>().FaceRight = FaceRight;
                        break;
                case TypeOfAttack.Static:
                        int x = Player.GetComponent<Player>().X, tY = Player.GetComponent<Player>().Y;
                        while(tY > 0) {
                            Debug.Log("tY = "  + tY.ToString() + ", x = " + x.ToString()); 
                            if (CurrentLevel.GetComponent<Level>().CellBlock[tY, x] == BlockType.None && 
                            CurrentLevel.GetComponent<Level>().CellBlock[tY - 1, x] != BlockType.None) break;
                            tY--;
                        }
                        Instantiate(StaticAttackPrefab, new Vector3(Player.GetComponent<Player>().X * 0.32F, (tY < 0 ? 0 : tY) * 0.32F + 0.16F, 0), Quaternion.identity);
                        break;
                case TypeOfAttack.Obstruction:
                        Instantiate(ObstructionPrefab, 
                        new Vector3(Player.GetComponent<Player>().X * 0.32F, (Player.GetComponent<Player>().Y + 2) * 0.32F, 0), Quaternion.identity);
                        break;
                default:
                    break;

            }                                                       
        }
        AutoAttacksActivated--;
        yield break;
    }

    void Move() {
        GetComponent<Rigidbody2D>().velocity = new Vector2(FaceRight ? 0.8F : -0.8F, GetComponent<Rigidbody2D>().velocity.y);
        GetComponent<Animator>().Play("Move");
        // if(X != Level.Width) if(FaceRight && (CurrentLevel.GetComponent<Level>().CellFG[Y, X + 1] != null)) 
        //     if(CurrentLevel.GetComponent<Level>().CellFG[Y, X + 1].tag != "Ladder") Flip();
        // if(X != 0) if(!FaceRight && (CurrentLevel.GetComponent<Level>().CellFG[Y, X - 1] != null)) 
        //     if(CurrentLevel.GetComponent<Level>().CellFG[Y, X - 1].tag != "Ladder") Flip();
    }
    void OnCollisionEnter2D(Collision2D other) {
        // if(other.gameObject.GetComponent<Block>() && other.gameObject.tag != "Ladder" && other.gameObject != Previous) {
        //     Flip(); 
        //     Move();
        //     // Previous = other.gameObject; 
        // }
        // if((other.gameObject.GetComponent<Enemy>()) && other.gameObject != Previous) {
        //     Flip(); 
        //     Move();
        //     // Previous = other.gameObject; 
        // }
        if(other.gameObject.GetComponent<Player>()) { 
            SetTransperency(1);
            HP = 0; 
            GetComponent<CapsuleCollider2D>().enabled = false; 
            GetComponent<Rigidbody2D>().gravityScale = 0; 
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; 
            GetComponent<Animator>().Play("Destroy");
        }
    }
    void Flip() {
        FaceRight = !FaceRight;
        transform.rotation = new Quaternion(0, FaceRight ? 0 : 180, 0, 0);
    }
    void FlipTo(bool right){
        if(right != FaceRight) Flip();
    }
    void Die(){
        Destroy(gameObject);
    }
    float DistanceFrom(Vector3 obj) => Mathf.Sqrt(Mathf.Pow(transform.position.x - obj.x, 2) + Mathf.Pow(transform.position.y - obj.y, 2));
    void SetTransperency(float EndTransperency) => StartCoroutine(ChangeTransperency(EndTransperency));
    IEnumerator ChangeTransperency(float EndTransperency)
    {
        CurrentTransperency = EndTransperency;
        while(Mathf.Abs(GetComponent<SpriteRenderer>().color.a  - EndTransperency) >= 0.001F)
        {
            yield return new WaitForSeconds(0.02F);
            if (EndTransperency != CurrentTransperency) yield break;
            Color Current = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(Current.r, Current.g, Current.b, Current.a + (EndTransperency - Current.a) / 10);
        }
        yield break;
    }
}
public enum TypeOfAttack{
    Shooting, Course, Obstruction, Protection, Static, Invisibility
}