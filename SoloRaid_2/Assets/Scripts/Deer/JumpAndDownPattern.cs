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
    [SerializeField] private float fallStartHeight = 20f;
    [SerializeField] private bool isFalling = false;

    //[SerializeField] private float jumpForce = 100f;


    private void Awake()
    {
        patternName = "JumpAndDownPattern";
    }

    public override IEnumerator Execute(BossAi controller)
    {
        controller.animator.SetTrigger("Jump");

        GameObject spawnDecal = DecalManager.Instance.SpawnDecal(jumpSlamPrefab, controller.playerPosition.position + plusTrackingPosition, jumpSlamPrefab.transform.rotation);
        Renderer[] childRenderers;

        Vector3 targetScale = new Vector3(slamRadius, 1, slamRadius);
        Vector3 startScale = targetScale * 0.2f;

        spawnDecal.transform.localScale = Vector3.zero;
        childRenderers = spawnDecal.GetComponentsInChildren<Renderer>();


        Color baseColor = Color.white;
        if (childRenderers.Length > 0)
        {
            baseColor = childRenderers[0].sharedMaterial.color;
        }
        Color startColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        Color targetColor = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        foreach (Renderer renderer in childRenderers)
        {
            renderer.material.color = startColor;
        }

        // 추적
        float timer = 0f;
        while (timer < trackingTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / trackingTime);

            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, progress);
            spawnDecal.transform.localScale = currentScale;
            Color currentColor = Color.Lerp(startColor, targetColor, progress);
            foreach (Renderer renderer in childRenderers)
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
        spawnDecal.transform.localScale = targetScale;
        //foreach (Renderer renderer in childRenderers) { renderer.material.color = targetColor; }
        Vector3 fallStartPosition = slamTarget + (Vector3.up * fallStartHeight);
        controller.transform.position = fallStartPosition;
        controller.GetComponent<Collider>().enabled = true;

        foreach (Renderer r in controller.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
        
        controller.animator.SetTrigger("Slam");
        isFalling = true;

        float fallToTimer = 0f;
        while(fallToTimer < warningTime)
        {
            fallToTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(fallToTimer / warningTime);

            float easeInProgress = progress * progress;
            controller.transform.position = Vector3.Lerp(fallStartPosition, slamTarget, easeInProgress);

            yield return null;
        }
        
        controller.transform.position = slamTarget;
        controller.rb.useGravity = true;
        

        Collider[] hits = Physics.OverlapSphere(slamTarget, slamRadius, LayerMask.GetMask("Player"));

        DecalManager.Instance.DespawnDecal(spawnDecal);
    }

}
