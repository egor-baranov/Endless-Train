using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartialTemplate : MonoBehaviour {
    public int Height = -1;
    public int Width = -1;
    public Dictionary<BlockType, Sprite> PTData = new Dictionary<BlockType, Sprite>();
    public void SetContent(Sprite bSprites, Sprite wSprites, Sprite lSprites, Sprite sSprites, Sprite bgSprites, Sprite fgSprites){
        PTData[BlockType.Block] = bSprites; PTData[BlockType.Window] = wSprites; PTData[BlockType.Ladder] = lSprites; 
        PTData[BlockType.Stairs] = sSprites; PTData[BlockType.BGWall] = bgSprites; PTData[BlockType.FGWall] = fgSprites; 
    }
}
