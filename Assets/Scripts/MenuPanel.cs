using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour {
    public int Height = 4, Width = 6;
    public GameObject SampleB, Generator;  
    public GameObject[,] Items = new GameObject[0, 0];
    public Sprite[] MenuCellSprite = new Sprite[9];
    public GameObject InventoryCellPrefab, CloseInventory, LastChest; 
    public bool OpenedByChest = false;
    public List<ItemObject> Content = null; 
    void Start(){
        Generator = GameObject.Find("Generator");
    }
    public void Recreate(ref List<ItemObject> content, int height = 4, int width = 6, bool ByChest = false, GameObject chest = null){
        Clear();
        Content = content;
        if(ByChest) { OpenedByChest = true; LastChest = chest; }
        Draw(ref content, height, width);
    }
    private void Draw(ref List<ItemObject> content, int height = 4, int width = 6) {
        Generator.GetComponent<Generator>().MenuOpened = true;
        Height = height; Width = width;
        Items = new GameObject[Height, Width];
        for(int y = 0; y < Height; ++y)
            for(int x = 0; x < Width; ++x) {
                int Xpos = 0, Ypos = 0;
                if(Width % 2 == 1) Xpos = 100 * (x - Width / 2);
                else Xpos = 50 + 100 * (x - Width / 2);
                if(Height % 2 == 1) Ypos = 100 * ((Height - y - 1) - Height / 2);
                else Ypos = 50 + 100 * ((Height - y - 1) - Height / 2);
                Items[y, x] = Instantiate(InventoryCellPrefab, transform.position, Quaternion.identity);
                Items[y, x].transform.SetParent(transform);
                Items[y,x].GetComponent<RectTransform>().anchoredPosition = new Vector2(Xpos, Ypos);
                Items[y,x].GetComponent<RectTransform>().localScale = SampleB.GetComponent<RectTransform>().localScale;
                if (y == 0){
                    if(x == 0) Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[0];
                    else if(x == Width - 1) {
                        Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[2];
                        GameObject CI = Instantiate(CloseInventory, transform.position, Quaternion.identity);
                        CI.transform.SetParent(Items[y, x].transform);
                        CI.GetComponent<RectTransform>().anchoredPosition = new Vector2(32F, 32F);
                        CI.GetComponent<RectTransform>().localScale = SampleB.GetComponent<RectTransform>().localScale;
                        Generator.GetComponent<Generator>().SetCloseButton(CI);
                    }
                    else  Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[1];
                } 
                else if (y == Height - 1){
                    if(x == 0) Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[6];
                    else if(x == Width - 1) Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[8];
                    else  Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[7];
                }
                else {
                    if(x == 0) Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[3];
                    else if(x == Width - 1) Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[5];
                    else  Items[y, x].GetComponent<Image>().sprite = MenuCellSprite[4];
                }
            }
        for(int i = 0; i < Mathf.Min(Width * Height, content.Count); ++i) 
            Items[i / Width, i % Width].GetComponent<InventoryCell>().Content = content[i];
    }
    public void DeleteFromContent(ItemObject item){
        if(item == null) return;
        for(int i = 0; i < Content.Count; ++i) 
            if(Content[i].Type == item.Type) {
                Content[i].Amount -= item.Amount; 
                if(Content[i].Amount == 0) Content.RemoveAt(i);
            }
    }
    public void Clear() {
        Generator.GetComponent<Generator>().MenuOpened = false;
        if(Items != null) foreach(var i in Items) Destroy(i);
        Items = null;
        if(OpenedByChest) LastChest.GetComponent<Chest>().Close();
        OpenedByChest = false; LastChest = null;
        Content = null;
    }
}
