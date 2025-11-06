using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "BaseAttackPattern", menuName = "Scriptable Objects/BaseAttackPattern")]
public abstract class BaseAttackPattern : ScriptableObject
{
    [Header("공통 설정")]
    [SerializeField] protected float warningTime = 1.0f; // 공격 전 경고 시간
    [SerializeField] protected float damage;
    [SerializeField] protected string patternName;
    public string PatternName { get { return patternName; } }

    // 추후 이펙트나 사운드 여기다가 넣기

    // 모든 자식 클래스가 구현해야하는 'Execute' 메서드
    public abstract IEnumerator Execute(BossAi controller);

    protected void RotateToPlayer(Transform transform, Transform playerTransform,float rotationSpeed)
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime * 5); // 5배 보정
    }
    
}
