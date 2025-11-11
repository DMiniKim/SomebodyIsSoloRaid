using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SectorPattern", menuName = "Scriptable Objects/SectorPattern")]
public class SectorPattern : BaseAttackPattern
{
    [Header("원뿔 공격 설정")]
    //[SerializeField] private float coneLength = 10f;
    [SerializeField] private GameObject coneDecalPrefab; // <--- 여기에 '부채꼴' 프리팹 할당
    [SerializeField] private Quaternion rotation;
    [SerializeField] private float patternRadius = 5f;



    // 생성된 장판을 저장할 변수
    //private GameObject spawnedDecal;
    private void Awake()
    {
        patternName = "SectorPattern";
    }
    public override IEnumerator Execute(BossAi controller)
    {
        warningTime = 2f;
        // 1. 준비 (플레이어 방향으로 회전)
        RotateToPlayer(controller.transform, controller.playerPosition, 1000f); // 즉시 회전
        Vector3 direction = controller.playerPosition.position - controller.transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject spawnedDecal1 = DecalManager.Instance.SpawnDecal(coneDecalPrefab, controller.transform.position + new Vector3(0f, 0.20f, 0f), rotation);
        GameObject spawnedDecal2 = DecalManager.Instance.SpawnDecal(coneDecalPrefab, controller.transform.position + new Vector3(0f, 0.21f, 0f), rotation);

        // 스케일만 변화시킬거임
        MeshRenderer meshRenderer1 = spawnedDecal1.GetComponentInChildren<MeshRenderer>();
        
        // 변화될 2번 데칼
        Vector3 targetScale = new Vector3(patternRadius, patternRadius, patternRadius) * patternRadius;
        Vector3 startScale = targetScale * 0.2f;

        // 1번 데칼에 쓸거임
        Color baseColor = Color.white;
        Color startColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
        spawnedDecal1.transform.localScale = targetScale;
        spawnedDecal2.transform.localScale = startScale;
        meshRenderer1.material.color = startColor;
        float timer = 0f;
        while(timer < warningTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer/ warningTime);
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, progress);
            spawnedDecal2.transform.localScale = currentScale;
            yield return null;
        }
        //float radius = patternRadius * coneLength;
        // 장판 크기 설정 (Z축(forward)으로 coneLength 만큼, X축은 coneLength 만큼)

        spawnedDecal2.transform.localScale = targetScale; // (프리팹 형태에 따라 조절 필요)

        spawnedDecal2.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshCollider>().enabled = true;
        yield return CoroutineManager.WaitForSecond(0.1f);
        spawnedDecal2.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshCollider>().enabled = false;


        // yield return CoroutineManager.WaitForSecond(warningTime);
        //Collider[] hits = Physics.OverlapSphere(spawnedDecal2.transform.position, patternRadius, LayerMask.GetMask("Player"));

        //Debug.Log(hits.Length);

        //foreach (Collider hit in hits)
        //{
        //    if (hit.gameObject.CompareTag("Player"))
        //    {
        //        hit.gameObject.GetComponent<PlayerMove>().TakeDamage(20f);
        //    }
        //}

        // TODO: 데미지 판정 로직


        DecalManager.Instance.DespawnDecal(spawnedDecal1);
        DecalManager.Instance.DespawnDecal(spawnedDecal2);
        controller.animator.SetTrigger("SectorAttack");
    }
    
}
