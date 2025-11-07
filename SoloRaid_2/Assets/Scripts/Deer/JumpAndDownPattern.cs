using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAndDownPattern", menuName = "Scriptable Objects/JumpAndDownPattern")]
public class JumpAndDownPattern : BaseAttackPattern
{
    [SerializeField] private float trackingTime = 2.0f;     //점프 할 동안 서칭 시간
    [SerializeField] private float slamRadius = 2f;     // 공격 반경
    [SerializeField] private GameObject jumpSlamPrefab;
    [SerializeField] private Vector3 plusTrackingPosition = new Vector3(0f, 0.18f, 0f);
    //[SerializeField] private float jumpForce = 100f;
    [SerializeField] private BossAi bossAi;

    private void Awake()
    {
        patternName = "JumpAndDownPattern";
    }

    public override IEnumerator Execute(BossAi controller)
    {
        controller.animator.SetTrigger("Jump");
        bossAi = controller;
        GameObject spawnDecal = DecalManager.Instance.SpawnDecal(jumpSlamPrefab, controller.playerPosition.position + plusTrackingPosition, jumpSlamPrefab.transform.rotation);
        Renderer[] childRenderers;

        Vector3 targetScale = new Vector3(slamRadius, 1, slamRadius);
        Vector3 startScale = targetScale * 0.2f;
        
        spawnDecal.transform.localScale = Vector3.zero;
        childRenderers = spawnDecal.GetComponentsInChildren<Renderer>();


        Color baseColor = Color.white;
        if(childRenderers.Length > 0)
        {
            baseColor = childRenderers[0].sharedMaterial.color;
        }
        Color startColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        Color targetColor = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        foreach(Renderer renderer in childRenderers)
        {
            renderer.material.color = startColor;
        }

        // 추적
        float timer = 0f;
        while(timer < trackingTime)
        {
            if (timer > (trackingTime / 2f)&& controller.rb.useGravity == false)
            {
                controller.rb.useGravity = true;                
            }
            if (timer > (trackingTime / 2f))
            {
                //controller.... 어떻게 해야하지..?
            }

            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / trackingTime);
            
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, progress);
            spawnDecal.transform.localScale = currentScale;
            Color currentColor = Color.Lerp(startColor, targetColor, progress);
            foreach(Renderer renderer in childRenderers)
            {
                renderer.material.color = currentColor;
            }

            if (controller.playerPosition != null)
            {
                spawnDecal.transform.position = controller.playerPosition.position + plusTrackingPosition;
            }
            yield return null;
        }
        
        // 장판 고정
        Vector3 slamTarget = spawnDecal.transform.position;
        // 대기
        yield return CoroutineManager.WaitForSecond(warningTime);

        // 공격 실행
        controller.transform.position = slamTarget;
        controller.animator.SetTrigger("Slam");

        Collider[] hits = Physics.OverlapSphere(slamTarget, 2f, LayerMask.GetMask("Player"));

        DecalManager.Instance.DespawnDecal(spawnDecal);
    }
    
}
