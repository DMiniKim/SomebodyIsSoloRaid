using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private Animator animator;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isMeleeAction = false;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float damage;
    [SerializeField] AttackTrigger attackTrigger;
    [SerializeField] GameObject triggerCollider;
    private bool isDead = false;
    public bool IsDead { get { return isDead; } }
    public bool IsMeleeAction { get { return isMeleeAction; } set { isMoving = value; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }

    [Header("Dodge Setting")]
    [SerializeField] float dodgeForce;
    [SerializeField] float dodgeDuration;
    [SerializeField] float dodgeCooldown;
    private bool isDodging = false;
    private bool canDodge = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        dodgeForce = 7f;
        dodgeDuration = 0.5f;
        dodgeCooldown = 5f;
        currentHealth = maxHealth;
        
        attackTrigger = GetComponent<AttackTrigger>();
    }

    // 이벤트 구독 및 구독 해지를 위한 메서드입니다.
    // 오브젝트가 활성화될 때 쿨타임 종료 이벤트를 구독하고, 비활성화될 때 해지하여 메모리 누수를 방지합니다.
    private void OnEnable()
    {
        GameEvents.OnDodgeCooldownFinished += EnableDodge;
        GameEvents.OnPlayerMeleeAttackStarted += EnableMove;
        GameEvents.OnPlayerMeleeAttackEnded += EnableMove;
    }

    private void OnDisable()
    {
        GameEvents.OnDodgeCooldownFinished -= EnableDodge;
        GameEvents.OnPlayerMeleeAttackStarted -= EnableMove;
        GameEvents.OnPlayerMeleeAttackEnded -= EnableMove;
    }

    // --- 추가된 부분 ---
    // 쿨타임 종료 이벤트가 발생했을 때 호출될 메서드입니다.
    private void EnableDodge()
    {
        canDodge = true;
    }
    private void EnableMove()
    {
        isMeleeAction = !isMeleeAction;
        isMoving = !isMoving;
        animator.SetBool("IsMove", isMoving);
    }

    void Update()
    {       
        if (Input.GetMouseButtonDown(1) && isMeleeAction == false)                    // 마우스 클릭하면 해당 위치로 이동
        {
            animator.SetBool("IsMove", true);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))              //마우스 우클릭 위치에 레이캐스트 발사한게 맞았다면
            {
                targetPosition = hit.point;
                isMoving = true;
                //Debug.Log("Target Position: " + targetPosition);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging && canDodge)          // 스페이스바 누르면 회피 동작
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))              //마우스 우클릭 위치
            {
                Vector3 dodgeTarget = hit.point;
                Vector3 dodgeDirection = (dodgeTarget - transform.position);
                dodgeDirection.y = 0;                        // 수직 무시

                if (dodgeDirection.sqrMagnitude > 0.01f)
                {
                    StartCoroutine(Dodge(dodgeDirection.normalized));
                }
            }
        }
        if (animator.GetBool("IsMove") && !isMoving)            // 이건 한번 강사님 수정 요청 해보자
        {
            animator.SetBool("IsMove", false);
        }
    }


    void FixedUpdate()
    {
        if(isDead)
        {

            return;
        }

        if (isDodging)               // 회피 중일 때는 이동하지 않음
            return;
        if (isMoving)               // 마우스 우클릭 시 이동 및 거리 계산 로직
        {
            Vector3 direction = (targetPosition - transform.position);      // 목표 위치 - 현재 위치  = 벡터
            direction.y = 0;                                                // 수직 무시(초기화시키기)
            float distance = direction.magnitude;                           // 벡터의 크기 = 거리
            if (distance < 0.1f)                                            // 거리가 0.1 이하이면 멈추게
            {
                isMoving = false;
                rb.linearVelocity = Vector3.zero;
                return;
            }

            Vector3 move = direction.normalized * moveSpeed * Time.fixedDeltaTime;       //흔하디 흔한 이동 공식
            rb.MovePosition(rb.position + move);                                         //리지드바디 이동

            if (direction.sqrMagnitude > 0)                                // 방향이 있을 때만 회전
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);                      // 바라보는 방향 계산
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime); // 부드럽게 회전시키기
            }
        }
    }

    IEnumerator Dodge(Vector3 direction)
    {

        isDodging = true;
        isMoving = false;
        canDodge = false;

        GameEvents.OnDodgeStarted?.Invoke(dodgeCooldown);

        rb.linearVelocity = Vector3.zero;

        animator.SetTrigger("Dodge");
        animator.SetBool("IsMove", false);

        transform.forward = direction;          // 회피 방향으로 캐릭터 회전

        rb.AddForce(direction * dodgeForce, ForceMode.Impulse);    // 회피 힘 가하기

        yield return CoroutineManager.WaitForSecond(dodgeDuration);    // 회피 지속 시간 대기

        isMoving = true;
        isDodging = false;

        rb.linearVelocity = Vector3.zero;          // 회피 후 속도 초기화
    }
    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Dead");
        }
        currentHealth -= damage;

        Debug.Log(currentHealth);
    }
    public void EnableAttackTrigger()
    {
        transform.GetChild(0).gameObject.SetActive(true);        
    }
    public void DisableAttackTrigger()
    {
        transform.GetChild(0).gameObject.SetActive(false);        
    }

}
