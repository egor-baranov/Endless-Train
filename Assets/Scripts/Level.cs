using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Level : MonoBehaviour
{
    public const int Height = 15, Width = 60;
    public GameObject BGWallPrefab, WindowPrefab;
    public Sprite[] BGWallSource, WindowSource;  
    public GameObject[] Templates;
    public int TemplateAmount = 1;
    private RandomList<Sprite> BgSprites, WindowSprites;
    public GameObject[,] CellBG = new GameObject[Height, Width];
    public GameObject[,] CellFG = new GameObject[Height, Width];
    public BlockType [,] CellBlock = new BlockType[Height, Width];
    private Tuple<float> Shift = new Tuple<float>(0, 0); 
    public Palette LevelPalette; 
    public GameObject[] Enemies; 

    void Awake() {
        BgSprites = new RandomList<Sprite>(BGWallSource);
    }
    void Start() {
        Refresh();
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.R)) Refresh(); 
        if(transform.parent.GetComponent<Generator>().Player.transform.position.x > Width * 0.32F)  {
            transform.parent.GetComponent<Generator>().Player.transform.position = new Vector3(-0.2727949F, 0.162345F, 0);
            LevelPalette = new Palette();
            Refresh();
        }
    }
    void Refresh() {
        // BG.Clear();
        Clear();
        Fill();
    }
    void Fill() {
        for (int i = 0; i < Height; ++i) 
            for (int j = 0; j < Width; ++j) { 
                CellBG[i, j] = Instantiate(BGWallPrefab, new Vector2(0.32F * j  + Shift.second, 0.32F * i + Shift.first), Quaternion.identity);
                CellBG[i, j].transform.parent = transform;
                CellBG[i, j].GetComponent<SpriteRenderer>().sprite = 
                    RandomFrom(transform.parent.GetComponent<Generator>().BlockSprites[LevelPalette.BGColor][BlockType.BGWall]);
                CellBG[i, j].GetComponent<Block>().X = j;
                CellBG[i, j].GetComponent<Block>().Y = i;
            }
        int k = 1; 
        while(k < Width - 8) {
            k += AddTemplate(Templates[Random.Range(0, Templates.Length)], 0, k).GetComponent<PartialTemplate>().Width + 1;
        } 
    }
    GameObject AddTemplate(GameObject temp, int y = -1, int x = -1) {
        if(y == -1 || y >= Height - temp.GetComponent<PartialTemplate>().Height) y = Random.Range(0, Height - temp.GetComponent<PartialTemplate>().Height);
        if(x == -1 || x >= Width - temp.GetComponent<PartialTemplate>().Width) x = Random.Range(0, Width - temp.GetComponent<PartialTemplate>().Width);
        y = 0; // FIX
        var newTemp = Instantiate(temp, new Vector2(x * 0.32F + Shift.second, y * 0.32F + Shift.first), Quaternion.identity);
        newTemp.transform.parent = transform;
        for(int i = y; i < y + temp.GetComponent<PartialTemplate>().Height; ++i)
            for(int j = x; j < x + temp.GetComponent<PartialTemplate>().Width; ++j){
                if(CellFG[i, j] != null) Destroy(CellFG[i, j]);
                CellFG[i, j] = newTemp;
            }
        //инициализация палитры
        newTemp.GetComponent<PartialTemplate>().SetContent(
            RandomFrom(transform.parent.GetComponent<Generator>().BlockSprites[LevelPalette.BlockColor][BlockType.Block]),
            RandomFrom(transform.parent.GetComponent<Generator>().BlockSprites[LevelPalette.BlockColor][BlockType.Window]),
            RandomFrom(transform.parent.GetComponent<Generator>().BlockSprites[LevelPalette.LadderColor][BlockType.Ladder]),
            RandomFrom(transform.parent.GetComponent<Generator>().BlockSprites[LevelPalette.StairsColor][BlockType.Stairs]),
            RandomFrom(transform.parent.GetComponent<Generator>().BlockSprites[LevelPalette.BGColor][BlockType.BGWall]),
            RandomFrom(transform.parent.GetComponent<Generator>().BlockSprites[LevelPalette.BlockColor][BlockType.Block])
        );
        return newTemp;
    }
    void Clear() => Clear(new Tuple<int>(-1, -1), new Tuple<int>(-1, -1));
    void Clear(Tuple<int> y, Tuple<int> x) {
        // Debug.Log(string.Format("Clear: x = [{0} : {1}], y = [{2} : {3}]", x.first, x.second, y.first, y.second));
        for (int i = (y.first == -1 ? 0: y.first); i <= (y.second == -1 ? Height - 1 : y.second); ++i) 
            for (int j = (x.first == -1 ? 0 : x.first); j <= (x.second == -1 ? Width- 1 : x.second); ++j) {
                Destroy(CellBG[i, j]); Destroy(CellFG[i, j]);
            }
    }
    Sprite RandomFrom(Sprite[] arr) => arr[Random.Range(0, arr.Length)];
}
struct LevelArray {
    // структура, отвечающая за композицию шаблонов и генерацию уровней, вне зависимости от их физики (для инстанциации берем Values и проходимся циклом)
    // она не должна удалять GameObject-ы, так как просто создает схему, по которой будет сгенерирован уровень 
    public BlockInfo[] Values;
    public Tuple<int> Size;
    public LevelArray (int y, int x, BlockInfo obj) {
        Values = new BlockInfo[y * x];
        Size = new Tuple<int>(y, x);
        for(int i = 0; i < Values.Length; ++i) Values[i] = obj;
    }
    public BlockInfo this[int y, int x] {
        get => Values[y * Size.first  +  x];
        set => Values[y * Size.first + x] = value;
    }
    public void Clear() => Clear(new Tuple<int>(-1, -1), new Tuple<int>(-1, -1));
    public void Clear(Tuple<int> y, Tuple<int> x) {
        for (int i = (y.first == -1 ? 0: y.first); i <= (y.second == -1 ? this.Size.first - 1 : y.second); ++i) 
            for (int j = (x.first == -1 ? 0 : x.first); j <= (x.second == -1 ? this.Size.second - 1 : x.second); ++j) {
                this[i, j] = null; 
            }
    }
    // вставка группы объектов в прямоугольную область массива
    public void Insert(Tuple<int> y, Tuple<int> x, LevelArray content, bool overlap = true) {
        for(int i = y.first; i <= min(y.second, Size.first - 1); ++i) 
            for(int j = x.first; j <= min(x.second, Size.second - 1); j++) 
                if (this[y.first + i, x.first + j] == null || overlap) this[y.first + i, x.first + j] = content[i, j];
    }
    // вставка объекта в область массива 
    public void Insert(int y, int x, BlockInfo obj, bool overlap = true) {
        if (this[y, x] == null ||  overlap) this[y, x] = obj;
    }
    // вставка объекта в прямоугольную область массива с копипастом
    public void Insert(Tuple<int> y, Tuple<int> x, BlockInfo obj, bool overlap = true) {
        for(int i = y.first; i <= min(y.second, Size.second - 1); ++i) 
            for(int j = x.first; j <= min(x.second, Size.second - 1); j++)
                if (this[y.first + i, x.first + j] == null ||  overlap) this[y.first + i, x.first + j] = obj;
    }
    // вставка рандомного объекта в область массива
    public void InsertRandom(int y, int x, RandomList<BlockInfo> RL, bool overlap = true) {
        if (this[y, x] == null ||  overlap) this[y, x] = RL.Next();
    }
     // вставка радномных объектов в прямоугольную область массива с копипастом
    public void Insert(Tuple<int> y, Tuple<int> x, RandomList<BlockInfo> RL, bool overlap = true) {
        for(int i = y.first; i <= min(y.second, Size.second - 1); ++i) 
            for(int j = x.first; j <= min(x.second, Size.second - 1); j++)
                if (this[y.first + i, x.first + j] == null ||  overlap) this[y.first + i, x.first + j] = RL.Next();
    }
    private int min(int a, int b) => (a < b ? a : b); 
}
class BlockInfo {
    BlockType Type;
    Quaternion Rotation;
    public BlockInfo(BlockType T, Quaternion R){
        Type = T;
        Rotation = R;
    }
    public BlockInfo(BlockType T){
        Type = T;
        Rotation = Quaternion.identity;
    }
}
// возвращает случайный объект из содержащихся
struct RandomList<T>{
    private List<T> Values;
    public int Size; 
    public RandomList(List<T> val) {
        Values = val;
        Size = val.Count;
    }
    public RandomList(T[] val) {
        Values = new List<T>();
        foreach(T i in val) Values.Add(i);
        Size = val.Length;
    }
    public T Next()  => Values[Random.Range(0, Size)];
    public void Add(T obj) => Values.Add(obj);
    public void Add(List<T> objs) {
        foreach (T i in objs) Values.Add(i);
    }
}
struct Tuple<T> {
    private T f, s; 
    public T first { get => f; private set => first = value; }
    public T second { get => s; private set => second = value; }
    public Tuple(T _first, T _second) {
        this.f = _first; 
        this.s = _second;
    }
}