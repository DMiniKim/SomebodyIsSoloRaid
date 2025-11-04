using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LighteningPattern", menuName = "Scriptable Objects/LighteningPattern")]
public class LighteningPattern : BaseAttackPattern
{
    [SerializeField] private int strikeCount = 5;  //³«·Ú °³¼ö
    [SerializeField] private float stormRadius = 15f;  // º¸½º Áß½É ¹üÀ§
    [SerializeField] private float intervalTime = 0.3f;  // ³«·Ú ¶³¾îÁö´Â °£°Ý
    [SerializeField] private GameObject lightningPrefab;

    public override IEnumerator Execute(BossAi controller)
    {
        Vector3 center = controller.transform.position;
        for (int i = 0; i < strikeCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * stormRadius;
            Vector3 strikePos = center + new Vector3(randomCircle.x, 0, randomCircle.y);

            RaycastHit hit;
            if (Physics.Raycast(strikePos + Vector3.up * 50f,Vector3.down,out hit , LayerMask.GetMask("Ground")))
            {
                strikePos = hit.point;
            }
            controller.StartCoroutine(StrikeCoroutine(strikePos));
        }

        yield return CoroutineManager.WaitForSecond(intervalTime);
    }

    private IEnumerator StrikeCoroutine(Vector3 position)
    {
        GameObject decal = DecalManager.Instance.SpawnDecal(lightningPrefab, position, Quaternion.identity);

        yield return CoroutineManager.WaitForSecond(warningTime);

        Collider[] hits = Physics.OverlapSphere(position, 2f, LayerMask.GetMask("Player"));

        DecalManager.Instance.DespawnDecal(decal);
    }
}
