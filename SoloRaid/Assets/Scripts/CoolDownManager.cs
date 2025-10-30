using System.Collections;
using UnityEngine;

public class CoolDownManager : MonoBehaviour
{


    // ������Ʈ Ȱ��ȭ/��Ȱ��ȭ �� �̺�Ʈ ���� �� ������ �����մϴ�.
    private void OnEnable()
    {
        // 'ȸ�� ����' �̺�Ʈ�� �����Ͽ�, �ش� �̺�Ʈ�� �߻��ϸ� HandleDodgeStart �ڷ�ƾ�� �����մϴ�.
        GameEvents.OnDodgeStarted += HandleDodgeStart;
    }

    private void OnDisable()
    {
        GameEvents.OnDodgeStarted -= HandleDodgeStart;
    }

    // --- �߰��� �κ� ---
    // �̺�Ʈ�� �߻����� �� ��Ÿ�� �ڷ�ƾ�� ���۽�Ű�� ������ �մϴ�.
    private void HandleDodgeStart(float coolTimeValue)
    {
        StartCoroutine(CooltimeCoroutine(coolTimeValue));
    }

    
    public IEnumerator CooltimeCoroutine(float coolTimeValue)
    {
        float currentTime = coolTimeValue;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // --- ������ �κ� ---
            // UIManager�� ���� ȣ���ϴ� ���, '��Ÿ���� ������Ʈ �Ǿ���'�� �̺�Ʈ�� �߻���ŵ�ϴ�.
            // UI�� ������ � ��ü�� �� �̺�Ʈ�� �����ϸ� ���� �ð��� ���޹��� �� �ֽ��ϴ�.

            GameEvents.OnCooldownUpdated?.Invoke(currentTime);

            yield return null;
        }

        if (currentTime <= 0)
        {
            // --- ������ �κ� ---
            // Player�� ���¸� ���� �����ϴ� ���, '��Ÿ���� ������'�� �̺�Ʈ�� �߻���ŵ�ϴ�.
            // PlayerMove ��ũ��Ʈ�� �� �̺�Ʈ�� ��� ������ ����(canDodge)�� ������ ���Դϴ�.
            GameEvents.OnDodgeCooldownFinished?.Invoke();
        }
    }

}
