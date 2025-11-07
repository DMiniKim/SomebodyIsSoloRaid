using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
[CreateAssetMenu(fileName = "CircleCenterPattern", menuName = "Scriptable Objects/CircleCenterPattern")]
public class CircleCenterPattern : BaseAttackPattern
{
    [Header("센터 원형 패턴")]
    [SerializeField] private float attackRadius = 7f;    // 반경
    [SerializeField] private GameObject CircleDecalFab; // 장판 이미지 프리팹
    [SerializeField] private float watingAttack = 3f;
    [SerializeField] private float startAlpha = 0.3f;

    private void Awake()
    {
        base.warningTime = 1.5f;
        patternName = "CircleCenterPattern";
    }

    public override IEnumerator Execute(BossAi controller)
    {
        //초기화
        Vector3 spawnPos = controller.transform.position;
        float decalScale = attackRadius;
        // 변수 선언
        GameObject attackRangeDecal = DecalManager.Instance.SpawnDecal(CircleDecalFab, spawnPos, CircleDecalFab.transform.rotation);
        attackRangeDecal.transform.localPosition = controller.transform.position+new Vector3(0f,0.18f,0f);
        GameObject warningDecal = DecalManager.Instance.SpawnDecal(CircleDecalFab, spawnPos, CircleDecalFab.transform.rotation);
        warningDecal.transform.localPosition = controller.transform.position + new Vector3(0f, 0.185f, 0f);
        Renderer[] childRenderers;

        Vector3 targetScale = new Vector3(attackRadius, attackRadius, attackRadius);
        Vector3 startScale = targetScale * 0.2f;

        //데칼들 초기화
        attackRangeDecal.transform.localScale = new Vector3(attackRadius, 1, attackRadius);
        childRenderers = attackRangeDecal.GetComponentsInChildren<Renderer>();
        
        Color baseColor = Color.white;
        if (childRenderers.Length > 0)
        {
            baseColor = childRenderers[0].sharedMaterial.color;
        }
        Color color = new(baseColor.r, baseColor.g, baseColor.b, startAlpha);
        

        foreach (Renderer renderer in childRenderers) 
        {
            renderer.material.color = color;
        }
        float timer = 0f;
        while (timer < watingAttack)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / watingAttack);
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, progress);
            warningDecal.transform.localScale = currentScale;
            yield return null;
        }
        warningDecal.transform.localScale = targetScale;


        //yield return CoroutineManager.WaitForSecond(warningTime);

        Collider[] hits = Physics.OverlapSphere(spawnPos, attackRadius, LayerMask.GetMask("Player"));

        DecalManager.Instance.DespawnDecal(attackRangeDecal);
        DecalManager.Instance.DespawnDecal(warningDecal);

    }
}