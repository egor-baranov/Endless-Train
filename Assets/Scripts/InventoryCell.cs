using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemObject content = null;
    public GameObject Icon, AmountText, Frame;
    public int IndexY = 0, IndexX = 0; 
    public ItemObject Content {  
        get => content;   
        set { 
            content = value;           
            if(content != null) {
                Icon.GetComponent<Image>().sprite = Content.Icon;
                Color c = Icon.GetComponent<Image>().color;
                Icon.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1); 
                c = AmountText.GetComponent<Text>().color;
                AmountText.GetComponent<Text>().color = new Color(c.r, c.g, c.b, 1);
                if(value.Amount > 1) AmountText.GetComponent<Text>().text = value.Amount.ToString();
            }
            else {
                Icon.GetComponent<Image>().sprite = null;
                Color c = Icon.GetComponent<Image>().color;
                Icon.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
                c = AmountText.GetComponent<Text>().color;
                AmountText.GetComponent<Text>().color = new Color(c.r, c.g, c.b, 0);
            }
        }
    }
    void Start(){
        GetComponent<GUIButton>().TODOClick += GetItem; 
    }
    public void GetItem(){
        if(transform.parent.GetComponent<MenuPanel>().Generator.GetComponent<Generator>().Context == ItemMenuContext.Inventory) return;
        transform.parent.GetComponent<MenuPanel>().Generator.GetComponent<Generator>().Player.GetComponent<Player>().AddToBackPack(Content);
        transform.parent.GetComponent<MenuPanel>().DeleteFromContent(Content);
        Content = null;
    }
    public void OnPointerEnter(PointerEventData eventData){
        Color c = Frame.GetComponent<Image>().color;
        if(!transform.parent.GetComponent<MenuPanel>().Generator.GetComponent<Generator>().Player.GetComponent<Player>().MobileMode)
            Frame.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0.4F);
    }
    public void OnPointerExit(PointerEventData eventData){
        Color c = Frame.GetComponent<Image>().color;
        Frame.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
    }
}


