using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAndDownPattern", menuName = "Scriptable Objects/JumpAndDownPattern")]
public class JumpAndDownPattern : BaseAttackPattern
{
    [SerializeField] private float trackingTime = 2.0f;     //점프 할 동안 서칭 시간
    [SerializeField] private GameObject jumpSlamPrefab;

    private GameObject spawnDecal;

    public override IEnumerator Execute(BossAi controller)
    {
        controller.animator.SetTrigger("Jump");

        spawnDecal = DecalManager.Instance.SpawnDecal(jumpSlamPrefab, controller.playerPosition.position,Quaternion.identity);

        // 추적
        float timer = 0f;
        while(timer < trackingTime)
        {
            if (controller.playerPosition != null)
            {
                spawnDecal.transform.position = controller.playerPosition.position;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        
        // 장판 고정
        Vector3 slamTarget = spawnDecal.transform.position;
        // 대기
        yield return CoroutineManager.WaitForSecond(warningTime);

        // 공격 실행
        controller.transform.position = slamTarget;
        controller.animator.SetTrigger("Slam");

        Collider[] hits = Physics.OverlapSphere(slamTarget, 5f, LayerMask.GetMask("Player"));

        DecalManager.Instance.DespawnDecal(spawnDecal);
    }
}
