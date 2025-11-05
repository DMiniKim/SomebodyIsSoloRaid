using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "CircleCenterPattern", menuName = "Scriptable Objects/CircleCenterPattern")]
public class CircleCenterPattern : BaseAttackPattern
{
    [Header("센터 원형 패턴")]
    [SerializeField] private float attackRadius = 5f;    // 반경
    [SerializeField] private GameObject CircleDecalFab; // 장판 이미지 프리팹


    private void Awake()
    {
        base.warningTime = 1.5f;
    }

    public override IEnumerator Execute(BossAi controller)
    {
        Vector3 spawnPos = controller.transform.position;
        float decalScale = attackRadius;

        GameObject decal = DecalManager.Instance.SpawnDecal(CircleDecalFab, spawnPos, CircleDecalFab.transform.rotation);
        decal.transform.localPosition = new Vector3(decalScale, 1, decalScale);

        yield return CoroutineManager.WaitForSecond(warningTime);

        Collider[] hits = Physics.OverlapSphere(spawnPos, attackRadius, LayerMask.GetMask("Player"));

        DecalManager.Instance.DespawnDecal(decal);

    }
}