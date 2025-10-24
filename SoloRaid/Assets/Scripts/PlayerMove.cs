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
            if (Physics.Raycast(ray, out hit))              //���콺 ��Ŭ�� ��ġ�� ����ĳ��Ʈ �߻��Ѱ� �¾Ҵٸ�
            {
                targetPosition = hit.point;
                isMoving = true;
                //Debug.Log("Target Position: " + targetPosition);
            }
        }
        if (animator.GetBool("IsMove") && !isMoving)            // �̰� �ѹ� ����� ���� ��û �غ���
        {
            animator.SetBool("IsMove", false);
        }
    }


    void FixedUpdate()
    {
        if (isMoving)               // ���콺 ��Ŭ�� �� �̵� �� �Ÿ� ��� ����
        {
            Vector3 direction = (targetPosition - transform.position);      // ��ǥ ��ġ - ���� ��ġ  = ����
            direction.y = 0;                                                // ���� ����(�ʱ�ȭ��Ű��)
            float distance = direction.magnitude;                           // ������ ũ�� = �Ÿ�
            if (distance < 0.1f)                                            // �Ÿ��� 0.1 �����̸� ���߰�
            {
                isMoving = false;
                rb.linearVelocity = Vector3.zero;
                return;
            }

            Vector3 move = direction.normalized * moveSpeed * Time.fixedDeltaTime;       //���ϵ� ���� �̵� ����
            rb.MovePosition(rb.position + move);                                         //������ٵ� �̵�

            if ( direction.sqrMagnitude > 0)                                // ������ ���� ���� ȸ��
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);                      // �ٶ󺸴� ���� ���
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime); // �ε巴�� ȸ����Ű��
            }
        }
    }

}
