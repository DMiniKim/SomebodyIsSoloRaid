using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class CircleCenterPattern : BaseAttackPattern
{
    [Header("센터 원형 패턴")]
    [SerializeField] private float attackRadius =5f;    // 반경
    [SerializeField] private GameObject CircleDecalFab; // 장판 이미지 프리팹


    private void Awake()
    {
        base.warningTime = 1.5f;
    }
    private List<GameObject> spawnDecals = new();
    public override IEnumerator Execute(BossAi controller)
    {
        Vector3 spawnPos = controller.transform.position;
        float decalScale = attackRadius;

        spawnDecals.Clear();

        GameObject decal1 = DecalManager.Instance.SpawnDecal(CircleDecalFab, spawnPos, Quaternion.Euler(0, 0, 0));
        decal1.transform.localPosition = new Vector3(decalScale, 1, decalScale);
        spawnDecals.Add(decal1);

        GameObject decal2 = DecalManager.Instance.SpawnDecal(CircleDecalFab, spawnPos, Quaternion.Euler(0, 90, 0));
        decal1.transform.localPosition = new Vector3(decalScale, 1, decalScale);
        spawnDecals.Add(decal2);

        GameObject decal3 = DecalManager.Instance.SpawnDecal(CircleDecalFab, spawnPos, Quaternion.Euler(0, 180, 0));
        decal1.transform.localPosition = new Vector3(decalScale, 1, decalScale);
        spawnDecals.Add(decal3);

        GameObject decal4 = DecalManager.Instance.SpawnDecal(CircleDecalFab, spawnPos, Quaternion.Euler(0, 270, 0));
        decal1.transform.localPosition = new Vector3(decalScale, 1, decalScale);
        spawnDecals.Add(decal4);
        
        yield return CoroutineManager.WaitForSecond(warningTime);

        Collider[] hits = Physics.OverlapSphere(spawnPos, attackRadius, LayerMask.GetMask("Player"));
        
        foreach (GameObject decal in spawnDecals)
        {
            DecalManager.Instance.DespawnDecal(decal);
        }
    }
}