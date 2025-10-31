using System.Collections;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] PlayerMove playerMove;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && playerMove.IsMeleeAction == false)
        {
            GetMousePosition();
        }
    }



    // 움직임 상태의 유무의 개의치 않고 실행 가능해야함.
    IEnumerator MeleeAction(Vector3 diretion)
    {        
        GameEvents.OnPlayerMeleeAttackStarted?.Invoke(); // 이동 제어
        animator.SetTrigger("MeleeAttack"); // 애니메이션 실행
        rb.linearVelocity = Vector3.zero;

        transform.forward = diretion;

        yield return new WaitForSeconds(1f);
        GameEvents.OnPlayerMeleeAttackEnded?.Invoke();

    }

    // 마우스의 위치 값을 받아서 그 쪽 방향을 따오는 함수.
    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 attackPoint = hit.point;
            Vector3 attackDirection = (attackPoint - transform.position);
            attackDirection.y = 0;
            if (attackDirection.sqrMagnitude > 0.01f)
            {
                StartCoroutine(MeleeAction(attackDirection.normalized));
            }
        }

    }
}
