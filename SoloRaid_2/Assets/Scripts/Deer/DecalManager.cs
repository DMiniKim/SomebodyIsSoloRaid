using System.Collections.Generic;
//using Mono.Cecil;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class DecalManager : MonoBehaviour
{
    private static DecalManager instance;
    public static DecalManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new("DecalManager");
                instance = obj.AddComponent<DecalManager>();
            }
            return instance;
        }
    }
    // 오브젝트 풀링용 
    // key는 프리팹 종류를 담는 값 ex) 노랑 장판 ,빨간 장판 등등
    // queue는 먼저 비활성화된 장판부터 꺼내쓰는 형식의 list
    private Dictionary<string, Queue<GameObject>> poolDic = new();

    // 비활성화 된 장판들 위치 담아둘 부모 Transform
    private Transform poolParent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            poolParent = new GameObject("DecalPool").transform;
            DontDestroyOnLoad(poolParent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public GameObject SpawnDecal(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // Key 값을 프리팹 이름으로 지정
        string poolKey = prefab.name;
        GameObject decalToSpawn; // 리턴시킬 임시 변수

        // 해당 프리팹의 풀이 있고 , 풀 안에 재고가 있는지?
        if (poolDic.ContainsKey(poolKey) && poolDic[poolKey].Count > 0)
        {   
            // 꺼내기
            decalToSpawn = poolDic[poolKey].Dequeue();
        }
        else
        {
            decalToSpawn = Instantiate(prefab);
            decalToSpawn.name = poolKey;
        }
        decalToSpawn.transform.position = position;
        decalToSpawn.transform.rotation = rotation;
        decalToSpawn.transform.SetParent(null,false);
        decalToSpawn.SetActive(true);

        return decalToSpawn;
    }

    public void DespawnDecal(GameObject decalInstance)
    {
        if (decalInstance == null) return;

        decalInstance.SetActive(false);
        decalInstance.transform.SetParent(poolParent,true);

        string poolKey = decalInstance.name;

        if (!poolDic.ContainsKey(poolKey))
        {
            poolDic[poolKey] = new();
        }
        
        poolDic[poolKey].Enqueue(decalInstance); 
    }
}
