using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "LighteningPattern", menuName = "Scriptable Objects/LighteningPattern")]
public class LighteningPattern : BaseAttackPattern
{
    [SerializeField] private int strikeCount = 5;  //낙뢰 개수
    [SerializeField] private float patternRadius = 15f;  // 보스 중심 범위
    [SerializeField] private float intervalTime = 0.3f;  // 낙뢰 떨어지는 간격
    [SerializeField] private GameObject lightningPrefab;

    [SerializeField] private float strikeRadius = 3f;// 1발 반경
  
    public override IEnumerator Execute(BossAi controller)
    {
        Vector3 center = controller.transform.position;
        for (int i = 0; i < strikeCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * patternRadius;
            Vector3 strikePos = center + new Vector3(randomCircle.x, 0, randomCircle.y);

            RaycastHit hit;
            if (Physics.Raycast(strikePos + Vector3.up * 50f, Vector3.down, out hit, LayerMask.GetMask("Ground")))
            {
                strikePos = hit.point;
            }

            controller.StartCoroutine(StrikeCoroutine(strikePos));
        }

        yield return CoroutineManager.WaitForSecond(intervalTime);
    }

    private IEnumerator StrikeCoroutine(Vector3 position)
    {
        float decalScale = strikeRadius;

        GameObject decal = DecalManager.Instance.SpawnDecal(lightningPrefab, position, lightningPrefab.transform.rotation);
        decal.transform.localScale = new Vector3(decalScale, decalScale, decalScale);        



        yield return CoroutineManager.WaitForSecond(warningTime);

        // 데미지 연산

        Collider[] hits = Physics.OverlapSphere(position, strikeRadius, LayerMask.GetMask("Player"));


        DecalManager.Instance.DespawnDecal(decal);

    }
}
