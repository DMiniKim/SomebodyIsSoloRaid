using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private Image image;

    void Awake()
    {
        if (image != null)
        {
            image.gameObject.SetActive(false);
        }
        // textTime ������Ʈ�� Awake���� ã������ ������ NullReferenceException �߻� ���ɼ� ����.
        // �ν����Ϳ� �Ҵ��ϰų�, �����ϰ� GetComponent�� ����ϴ� ���� �����ϴ�.
        if (textTime == null)
        {
            textTime = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // --- �߰��� �κ� ---
    // �ʿ��� �̺�Ʈ���� �����ϰ� �����մϴ�.
    private void OnEnable()
    {
        GameEvents.OnDodgeStarted += ShowCooldownUI;
        GameEvents.OnCooldownUpdated += UpdateCooldownText; // --- ������ �κ� ---: ��Ÿ�� ������Ʈ �̺�Ʈ�� �����մϴ�.
        GameEvents.OnDodgeCooldownFinished += HideCooldownUI;
    }

    private void OnDisable()
    {
        GameEvents.OnDodgeStarted -= ShowCooldownUI;
        GameEvents.OnCooldownUpdated -= UpdateCooldownText; // --- ������ �κ� ---: ��Ÿ�� ������Ʈ �̺�Ʈ ������ �����մϴ�.
        GameEvents.OnDodgeCooldownFinished -= HideCooldownUI;
    }

    // ��Ÿ���� ���۵� �� UI�� Ȱ��ȭ�ϴ� �޼���
    private void ShowCooldownUI(float maxCooldown)
    {
        if (image != null) image.gameObject.SetActive(true);
        UpdateCooldownText(maxCooldown); // �ʱ� �� ����
    }

    // ��Ÿ���� ������Ʈ�� �� �ؽ�Ʈ�� �����ϴ� �޼���
    private void UpdateCooldownText(float currentTime)
    {
        Debug.Log(currentTime);       

        if (currentTime < 0) currentTime = 0;
        // �Ҽ��� �� �ڸ����� ǥ���ϰų�, �ø� ó���� �ϴ� �� �پ��ϰ� ǥ���� �� �ֽ��ϴ�.
        textTime.text = Mathf.Ceil(currentTime).ToString("F0"); // --- ������ �κ� ---: currentTime�� �� ������ ������Ʈ�� ������ ����մϴ�.
    }

    // ��Ÿ���� ����� �� UI�� ��Ȱ��ȭ�ϴ� �޼���
    private void HideCooldownUI()
    {
        if (image != null) image.gameObject.SetActive(false);
    }
}