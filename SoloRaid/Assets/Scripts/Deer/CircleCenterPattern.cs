using UnityEngine;
using System.Collections;
[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class CircleCenterPattern : BaseAttackPattern
{
    [Header("센터 원형 패턴")]
    [SerializeField] private float attackRadius;    // 반경
    [SerializeField] private GameObject CircleDecalFab; // 장판 이미지 프리팹
    public override IEnumerator Execute(BossAi controller)
    {
        Vector3 spawnPos =controller.transform.position;
        GameObject decal = Instantiate(CircleDecalFab,spawnPos,Quaternion.identity);

        decal.transform.localPosition = new Vector3(attackRadius * 2, 1, attackRadius);

        yield return CoroutineManager.WaitForSecond(attackRadius);

        Collider[] hits = Physics.OverlapSphere(spawnPos, attackRadius, LayerMask.GetMask("Player"));
    }
}