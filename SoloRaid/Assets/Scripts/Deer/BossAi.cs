using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAi : MonoBehaviour
{
    public Transform playerPosition;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackRange = 15f;       // 이거리 안에 들어오면 패턴 시작
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Animator animator;
    private bool isMoving = false;              // 공격 flag
    private bool isAttacking = false;           // 이동 flag
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    private Rigidbody rb;

    // 패턴 담을 리스트
    [SerializeField] private List<BaseAttackPattern> attackPatterns;
    [SerializeField] private float attackCooldown = 1.5f;         // 공격 쿨타임


    public enum AIState
    {
        Idle,
        Chasing,
        Attacking,
        Cooldown
    }
    public AIState currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어 정보 할당
        currentState = AIState.Idle;
    }
    private void Update()
    {
        switch (currentState)
        {
            case AIState.Idle:
                IdleState();
                break;
            case AIState.Chasing:
                ChasingState();
                break;
            case AIState.Attacking:
                // Coroutine로 공격 패턴 실행
                break;
            case AIState.Cooldown:
                // 이것도
                break;
        }
    }
    public void ChangeState(AIState newState) // bool이나 enum 상태 변경 함수
    {
        currentState = newState;
        switch (newState)
        {
            case AIState.Idle:
                isMoving = false;
                isAttacking = false;
                break;
            case AIState.Chasing:
                isMoving = true;
                break;
            case AIState.Attacking:
                isMoving = false;
                isAttacking = true;
                break;
        }
    }
    private void IdleState()
    {
        if (playerPosition != null)
        {
            ChangeState(AIState.Chasing);
        }
        // TODO  애니메이션 같은거
        animator.SetBool("IsMove", false);
    }
    private void ChasingState()
    {
        if (playerPosition == null)
        {
            ChangeState(AIState.Idle);
        }
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);

        if (distanceToPlayer <= attackRange)       // 위 거리가 사정거리 내라면
        {
            ChangeState(AIState.Attacking); // 공격상태
            //ExecuteRandomAttack();        // 랜덤 공격 실행
        }
        else
        {
            // 이동하기
            Vector3 direction = (playerPosition.position - transform.position).normalized; // 벡터 및 정규화
            direction.y = 0;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime); // 이동

            //회전하기

            Quaternion lookRotation = Quaternion.LookRotation(direction);   // 바라보는 방향 계산
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime); // 자연스레 회전
            animator.SetBool("IsMove", true);
        }
    }
    private void ExecuteRandomAttack()
    {
        // 공격 전 초기화 작업들
        ChangeState(AIState.Attacking);
        animator.SetBool("IsMove", false);
        rb.linearVelocity = Vector3.zero;
        

        // 예외 처리
        if (attackPatterns.Count == 0)
        {
            Debug.LogWarning("미친넘아 패턴이 없는 데 어캐 공격함");
            ChangeState(AIState.Cooldown);
            return;
        }

        int randomIndex = Random.Range(0, attackPatterns.Count);
        BaseAttackPattern select = attackPatterns[randomIndex];

        StartCoroutine(AttackCoroutine(select));
    }

    private IEnumerator AttackCoroutine(BaseAttackPattern attackPattern)
    {
        // 패턴 실행하고 끝날 때 까지 대기하는 코루틴 실행
        yield return StartCoroutine(attackPattern.Execute(this));

        // 끝나고 Cooldown 만들기
        ChangeState(AIState.Cooldown);

        // 대기 
        yield return new WaitForSeconds(attackCooldown);

        // 혹시모를 예외처리 
        if (currentState == AIState.Cooldown)   // 지금 쿨타임 상태라면
        {
             ChangeState(AIState.Chasing);
        }
    }

}

