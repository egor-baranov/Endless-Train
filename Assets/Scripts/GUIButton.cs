using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GUIButtonType Type = GUIButtonType.None;
    public delegate void ToDo();
    public float TimePressed = 0;
    public GameObject Icon;
    Vector3 NormalLocalScale; 
    bool ShowButton = true;
    public bool Pressed = false;
    public ToDo TODOClick, TODOPressed, TODOPressedEndAction;
    void Start(){
        NormalLocalScale = transform.GetComponent<RectTransform>().localScale;
        TODOClick += Fun; 
        TODOPressed += Fun;
    }
    void Update(){
        if(Pressed && Type != GUIButtonType.RightArrow && Type != GUIButtonType.LeftArrow) TODOPressed.Invoke();
        if(Pressed) TimePressed += 0.02F;
    }
    public void OnPointerDown(PointerEventData eventData){
        Pressed = true; 
        TODOClick.Invoke();
        // костыли поехали
        if(Type == GUIButtonType.Close) GameObject.Find("Generator").GetComponent<Generator>().CloseMenu();
    }
    public void OnPointerUp(PointerEventData eventData){
        Pressed = false;
        if(Type != GUIButtonType.RightArrow && Type != GUIButtonType.LeftArrow) 
            if(TODOPressedEndAction != null) TODOPressedEndAction.Invoke(); 
        TimePressed = 0;
    }
    void Fun() {
        Debug.Log("Pressed");
    }
    public void Hide() => StartCoroutine(HideCoroutine());
    private IEnumerator HideCoroutine() {
        ShowButton = false;
        Vector3 endPos = transform.localScale;
        while(!ShowButton && transform.GetComponent<RectTransform>().localScale.x - endPos.x > 0.001F){
            yield return null;
            Vector3 Dif = endPos - transform.GetComponent<RectTransform>().localScale;
            transform.GetComponent<RectTransform>().localScale = new Vector3();
        }
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
        yield break;
    }
    public void Show() => StartCoroutine(ShowCoroutine());
    private IEnumerator ShowCoroutine() {
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1);
        ShowButton = true;
        while(ShowButton && NormalLocalScale.x - transform.GetComponent<RectTransform>().localScale.x > 0.001F){  
            yield return null;
            Vector3 Dif = transform.GetComponent<RectTransform>().localScale - NormalLocalScale;
        }
        yield break;
    }
}

public enum GUIButtonType {
    Close, Inventory, UpArrow, DownArrow, LeftArrow, RightArrow, Jump, Reload, Action, InventoryItem, None
}