using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SectorPattern", menuName = "Scriptable Objects/SectorPattern")]
public class SectorPattern : BaseAttackPattern
{
    [Header("원뿔 공격 설정")]
    [SerializeField] private float coneLength = 10f;
    [SerializeField] private GameObject coneDecalPrefab; // <--- 여기에 '부채꼴' 프리팹 할당

    // 생성된 장판을 저장할 변수
    private GameObject spawnedDecal;

    public override IEnumerator Execute(BossAi controller)
    {
        // 1. 준비 (플레이어 방향으로 회전)
        RotateToPlayer(controller.transform, controller.playerPosition, 1000f); // 즉시 회전
                
        spawnedDecal = DecalManager.Instance.SpawnDecal(coneDecalPrefab, controller.transform.position, controller.transform.rotation);

        // 장판 크기 설정 (Z축(forward)으로 coneLength 만큼, X축은 coneLength 만큼)
        spawnedDecal.transform.localScale = new Vector3(coneLength, 1, coneLength); // (프리팹 형태에 따라 조절 필요)
                       
        yield return CoroutineManager.WaitForSecond(warningTime);
               
        
        // TODO: 데미지 판정 로직

       
        DecalManager.Instance.DespawnDecal(spawnedDecal);
    }
}
