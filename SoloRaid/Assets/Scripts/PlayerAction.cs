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



    // ������ ������ ������ ����ġ �ʰ� ���� �����ؾ���.
    IEnumerator MeleeAction(Vector3 diretion)
    {        
        GameEvents.OnPlayerMeleeAttackStarted?.Invoke(); // �̵� ����
        animator.SetTrigger("MeleeAttack"); // �ִϸ��̼� ����
        rb.linearVelocity = Vector3.zero;

        transform.forward = diretion;

        yield return new WaitForSeconds(1f);
        GameEvents.OnPlayerMeleeAttackEnded?.Invoke();

    }

    // ���콺�� ��ġ ���� �޾Ƽ� �� �� ������ ������ �Լ�.
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
