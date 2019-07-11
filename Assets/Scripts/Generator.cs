using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour { 
    public GameObject Player;
    public GameObject LevelPrefab;
    public GameObject CurrentLevel;
    public GameObject UpArrow, DownArrow, LeftArrow, RightArrow, JumpButton, MoveController, InventoryPanel, ArrowsPanel, ButtonsPanel,
    InventoryButton;
    public GameObject[] Enemies;
    public  Sprite[] RedBG, BlueBG, GreenBG, BrownBG;
    public  Sprite[] RedW, BlueW, GreenW, BrownW;
    public  Sprite[] RedL, BlueL, GreenL, BrownL;
    public  Sprite[] RedS, BlueS, GreenS, BrownS;
    public  Sprite[] RedB, BlueB, GreenB, BrownB;
    bool menuOpened = false;
    // BlockSprites[ColorPalette][BlockType]
    public Dictionary<ColorPalette, Dictionary<BlockType, Sprite[]>> BlockSprites;
    private ItemMenuContext context = ItemMenuContext.Inventory;
    public Sprite InventoryIcon, ChestIcon;
    public ItemMenuContext Context { 
        get => context; 
        set { 
            context = value;
            if(value == ItemMenuContext.Inventory)  InventoryButton.GetComponent<GUIButton>().Icon.GetComponent<Image>().sprite = InventoryIcon;
            else if(value == ItemMenuContext.Chest) InventoryButton.GetComponent<GUIButton>().Icon.GetComponent<Image>().sprite = ChestIcon;
        }
    }
    public bool MenuOpened { get => menuOpened; 
        set { 
            menuOpened = value; 
            ArrowsPanel.SetActive(!value && Player.GetComponent<Player>().MobileMode);
            ButtonsPanel.SetActive(!value && Player.GetComponent<Player>().MobileMode);
            GetComponent<Game>().Paused = value;
            // InventoryPanel.SetActive(value);
        }
    }
    void Start() {
        ArrowsPanel.SetActive(Player.GetComponent<Player>().MobileMode);
        ButtonsPanel.SetActive(Player.GetComponent<Player>().MobileMode);
        BlockSprites= new Dictionary<ColorPalette, Dictionary<BlockType, Sprite[]>>(){
        [ColorPalette.Red] = new Dictionary<BlockType, Sprite[]>(){
            [BlockType.BGWall] = RedBG, [BlockType.Block] = RedB, [BlockType.Ladder] = RedL, [BlockType.Stairs] = RedS, [BlockType.Window] = RedW
        },
        [ColorPalette.Blue] = new Dictionary<BlockType, Sprite[]>(){
            [BlockType.BGWall] = BlueBG, [BlockType.Block] = BlueB, [BlockType.Ladder] = BlueL, [BlockType.Stairs] = BlueS, [BlockType.Window] = BlueW
        },
        [ColorPalette.Green] = new Dictionary<BlockType, Sprite[]>(){
            [BlockType.BGWall] = GreenBG, [BlockType.Block] = GreenB, [BlockType.Ladder] = GreenL, [BlockType.Stairs] = GreenS, [BlockType.Window] = GreenW
        },
        [ColorPalette.Brown] = new Dictionary<BlockType, Sprite[]>(){
            [BlockType.BGWall] = BrownBG, [BlockType.Block] = BrownB, [BlockType.Ladder] = BrownL, [BlockType.Stairs] = BrownS, [BlockType.Window] = BrownW
        }
    };
        CurrentLevel = Instantiate(LevelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        CurrentLevel.GetComponent<Level>().LevelPalette = new Palette();
        CurrentLevel.GetComponent<Level>().Enemies = Enemies;
        CurrentLevel.transform.parent = transform;
        InventoryButton.GetComponent<GUIButton>().TODOClick += MenuPanelChangeState;
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)) MenuPanelChangeState();
    }
    public void MenuPanelChangeState() {
        // if(Context == ItemMenuContext.Chest && MenuOpened) Player.GetComponent<Player>().NearestChest.GetComponent<Chest>().Close(); 
        MenuOpened = !MenuOpened;
        if(MenuOpened) {
            if(Context == ItemMenuContext.Inventory) Player.GetComponent<Player>().OpenInventory();
            // else if(Context == ItemMenuContext.Chest) Player.GetComponent<Player>().NearestChest.GetComponent<Chest>().Open();
        }
        else InventoryPanel.GetComponent<MenuPanel>().Clear();
    }
    public void CloseMenu() { 
        InventoryPanel.GetComponent<MenuPanel>().Clear();
        MenuOpened = false;
    }
    public void SetCloseButton(GameObject closeButton) => closeButton.GetComponent<GUIButton>().TODOClick += CloseMenu;
}
public class Palette {
    public ColorPalette BlockColor, LadderColor, StairsColor, BGColor;
    public Palette() {
        List<ColorPalette> Colors = new List<ColorPalette>(){ ColorPalette.Red, ColorPalette.Blue, ColorPalette.Green, ColorPalette.Brown };
        int a = -1;
        a = Random.Range(0, 4); BlockColor = Colors[a]; Colors.RemoveAt(a);
        a = Random.Range(0, 3); LadderColor = Colors[a]; Colors.RemoveAt(a);
        a = Random.Range(0, 2); StairsColor = Colors[a]; Colors.RemoveAt(a);
                                BGColor = Colors[0]; Colors.RemoveAt(0);
    }

}
public enum ColorPalette {
    Red, Blue, Green, Brown, None
}
public enum ItemMenuContext {
    Inventory, Chest, None
}