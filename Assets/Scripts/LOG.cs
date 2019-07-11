using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOG : MonoBehaviour
{
    public GameObject CoinPrefab, ChestPrefab;
    public bool CoinCanBeGenerated = false;
    public bool EnemyCanBeGenerated = false;
    public bool ChestCanBeGenerated = false;
    public GameObject Generator;
    void Start() {
        Generator = GameObject.Find("Generator");
        GenerateSomething();
    }
    void GenerateSomething() {
        if (CoinCanBeGenerated)
            if(Random.Range(0, 2 + AmountOfTrue(EnemyCanBeGenerated, ChestCanBeGenerated)) == 0) {
                Instantiate(CoinPrefab, transform.position, Quaternion.identity).transform.parent = transform;
                return;
            }
        if(EnemyCanBeGenerated) 
            if(Random.Range(0, 2) == 0){ 
                Instantiate(RandomFrom(Generator.GetComponent<Generator>().CurrentLevel.GetComponent<Level>().Enemies), 
                    transform.position, Quaternion.identity).transform.parent = transform; 
                return;
            }
        if(ChestCanBeGenerated) 
            if(Random.Range(0, 4 - AmountOfTrue(CoinCanBeGenerated, EnemyCanBeGenerated)) == 0){
                Instantiate(ChestPrefab, transform.position, Quaternion.identity).transform.parent = transform;
                return;
            }
    }
    int AmountOfTrue(bool a, bool b) => (a ? 1 : 0) + (b ? 1 : 0);
    GameObject RandomFrom(GameObject[] arr) => arr[Random.Range(0, arr.Length)];
}
