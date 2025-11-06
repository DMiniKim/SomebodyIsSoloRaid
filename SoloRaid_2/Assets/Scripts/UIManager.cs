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
        // textTime 컴포넌트가 Awake에서 찾아지지 않으면 NullReferenceException 발생 가능성 있음.
        // 인스펙터에 할당하거나, 안전하게 GetComponent를 사용하는 것이 좋습니다.
        if (textTime == null)
        {
            textTime = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // --- 추가된 부분 ---
    // 필요한 이벤트들을 구독하고 해지합니다.
    private void OnEnable()
    {
        GameEvents.OnDodgeStarted += ShowCooldownUI;
        GameEvents.OnCooldownUpdated += UpdateCooldownText; // --- 수정된 부분 ---: 쿨타임 업데이트 이벤트를 구독합니다.
        GameEvents.OnDodgeCooldownFinished += HideCooldownUI;
    }

    private void OnDisable()
    {
        GameEvents.OnDodgeStarted -= ShowCooldownUI;
        GameEvents.OnCooldownUpdated -= UpdateCooldownText; // --- 수정된 부분 ---: 쿨타임 업데이트 이벤트 구독을 해지합니다.
        GameEvents.OnDodgeCooldownFinished -= HideCooldownUI;
    }

    // 쿨타임이 시작될 때 UI를 활성화하는 메서드
    private void ShowCooldownUI(float maxCooldown)
    {
        if (image != null) image.gameObject.SetActive(true);
        UpdateCooldownText(maxCooldown); // 초기 값 설정
    }

    // 쿨타임이 업데이트될 때 텍스트를 갱신하는 메서드
    private void UpdateCooldownText(float currentTime)
    {
        
        if (currentTime < 0) currentTime = 0;
        // 소수점 한 자리까지 표시하거나, 올림 처리를 하는 등 다양하게 표현할 수 있습니다.
        textTime.text = Mathf.Ceil(currentTime).ToString("F0"); // --- 수정된 부분 ---: currentTime을 매 프레임 업데이트된 값으로 사용합니다.
    }

    // 쿨타임이 종료될 때 UI를 비활성화하는 메서드
    private void HideCooldownUI()
    {
        if (image != null) image.gameObject.SetActive(false);
    }
}