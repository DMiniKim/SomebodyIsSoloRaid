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
    private bool isMoving = false;   


    [Header("Dodge Setting")]
    [SerializeField] float dodgeForce;
    [SerializeField] float dodgeDuration = 0.5f;
    [SerializeField] float dodgeCooldown = 5f;
    private bool isDodging = false;
    private bool canDodge = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    // �̺�Ʈ ���� �� ���� ������ ���� �޼����Դϴ�.
    // ������Ʈ�� Ȱ��ȭ�� �� ��Ÿ�� ���� �̺�Ʈ�� �����ϰ�, ��Ȱ��ȭ�� �� �����Ͽ� �޸� ������ �����մϴ�.
    private void OnEnable()
    {
        GameEvents.OnDodgeCooldownFinished += EnableDodge;
    }

    private void OnDisable()
    {
        GameEvents.OnDodgeCooldownFinished -= EnableDodge;
    }

    // --- �߰��� �κ� ---
    // ��Ÿ�� ���� �̺�Ʈ�� �߻����� �� ȣ��� �޼����Դϴ�.
    private void EnableDodge()
    {
        canDodge = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))                    // ���콺 Ŭ���ϸ� �ش� ��ġ�� �̵�
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
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging && canDodge)          // �����̽��� ������ ȸ�� ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))              //���콺 ��Ŭ�� ��ġ
            {
                Vector3 dodgeTarget = hit.point;
                Vector3 dodgeDirection = (dodgeTarget - transform.position);
                dodgeDirection.y = 0;                        // ���� ����

                if (dodgeDirection.sqrMagnitude > 0.01f)
                {
                    StartCoroutine(Dodge(dodgeDirection.normalized));
                }
            }
        }
        if (animator.GetBool("IsMove") && !isMoving)            // �̰� �ѹ� ����� ���� ��û �غ���
        {
            animator.SetBool("IsMove", false);
        }
    }


    void FixedUpdate()
    {
        if (isDodging)               // ȸ�� ���� ���� �̵����� ����
            return;
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

            if (direction.sqrMagnitude > 0)                                // ������ ���� ���� ȸ��
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);                      // �ٶ󺸴� ���� ���
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime); // �ε巴�� ȸ����Ű��
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

        transform.forward = direction;          // ȸ�� �������� ĳ���� ȸ��

        rb.AddForce(direction * dodgeForce, ForceMode.Impulse);    // ȸ�� �� ���ϱ�

        yield return new WaitForSeconds(dodgeDuration);    // ȸ�� ���� �ð� ���

        isMoving = true;
        isDodging = false;

        rb.linearVelocity = Vector3.zero;          // ȸ�� �� �ӵ� �ʱ�ȭ
    }

}
