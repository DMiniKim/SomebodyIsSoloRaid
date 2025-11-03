using UnityEngine;

public class DecalManager :MonoBehaviour
{
    private static DecalManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public static DecalManager Instance { get { return instance; } }


    public void SpawnDecalCircle(GameObject prefab,Transform transform)
    {

    }
    public void SpawnDecalSector(GameObject prefab, Transform transform)
    {

    }
    public void DespawnDecal(GameObject prefab)
    {

    }
}
