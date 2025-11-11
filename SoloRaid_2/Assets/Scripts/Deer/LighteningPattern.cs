using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LighteningPattern", menuName = "Scriptable Objects/LighteningPattern")]
public class LighteningPattern : BaseAttackPattern
{
    [SerializeField] private int strikeCount = 5;  //낙뢰 개수
    [SerializeField] private float patternRadius;  // 보스 중심 범위
    [SerializeField] private float intervalTime = 0.3f;  // 낙뢰 떨어지는 간격
    [SerializeField] private float attackTerm = 0.5f;
    [SerializeField] private GameObject lightningPrefab;

    [SerializeField] private float strikeRadius;// 1발 반경


    [Header("연출 설정 (Warning Animation)")]
    [Tooltip("장판이 0% -> 100%까지 커지는 시작 스케일 (0이면 0에서 시작)")]
    [SerializeField] private float startScalePercent = 0.3f;
    [Tooltip("장판이 0% -> 100%까지 밝아지는 시작 투명도 (0이면 투명에서 시작)")]
    [SerializeField, UnityEngine.Range(0f, 1f)] private float startAlpha = 0.3f;
    private void Awake()
    {
        patternName = "LighteningPattern";
        patternRadius = 10f;
        strikeRadius = 1f;
        attackTerm = 0.5f;
    }
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
        controller.animator.SetTrigger("LightningAttack");
        yield return CoroutineManager.WaitForSecond(intervalTime);
    }

    private IEnumerator StrikeCoroutine(Vector3 position)
    {
        //float decalScale = strikeRadius;
        GameObject decal = DecalManager.Instance.SpawnDecal(lightningPrefab, position + new Vector3(0f, 0.3f, 0f), lightningPrefab.transform.rotation);
        Renderer[] childRenderers;
        Collider[] childColliders;
        Vector3 targetScale = new Vector3(strikeRadius, 1, strikeRadius);
        Vector3 startScale = targetScale * startScalePercent;

        decal.transform.localScale = startScale;


        childRenderers = decal.GetComponentsInChildren<Renderer>();
        childColliders = decal.GetComponentsInChildren<MeshCollider>();

        Color baseColor = Color.white;
        if(childRenderers.Length >0 )
        {
            baseColor = childRenderers[0].sharedMaterial.color;
        }
        Color startColor = new(baseColor.r, baseColor.g, baseColor.b, startAlpha);
        Color targetColor = new(baseColor.r, baseColor.g, baseColor.b, 1f);

        foreach(Renderer renderer in childRenderers)
        {
            renderer.material.color = startColor;
        }

        
        float time = 0f; 
        while (time < warningTime)
        {
            time += Time.deltaTime;
            float progress = Mathf.Clamp01(time/warningTime);

            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, progress);
            decal.transform.localScale = currentScale;

            Color currentColor = Color.Lerp(startColor, targetColor, progress);
            foreach(Renderer renderer in childRenderers)
            {
                renderer.material.color = currentColor;
            }
            yield return null;
        }

        decal.transform.localScale = targetScale;
        foreach(Renderer rend in childRenderers)
        {
            rend.material.color = targetColor;
        }       

        yield return CoroutineManager.WaitForSecond(attackTerm);

        foreach (Collider col in childColliders)
        {
            col.enabled = true;
        }

        yield return CoroutineManager.WaitForSecond(0.1f);
        foreach (Collider col in childColliders)
        {
            col.enabled = false;
        }

        


        DecalManager.Instance.DespawnDecal(decal);

    }
}
