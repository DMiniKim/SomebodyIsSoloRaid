using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;


public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Animator animator;
    private bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1))
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
        if (animator.GetBool("IsMove") && !isMoving)            // 이건 한번 강사님 수정 요청 해보자
        {
            animator.SetBool("IsMove", false);
        }
    }


    void FixedUpdate()
    {
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

            if ( direction.sqrMagnitude > 0)                                // 방향이 있을 때만 회전
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);                      // 바라보는 방향 계산
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime); // 부드럽게 회전시키기
            }
        }
    }

}
