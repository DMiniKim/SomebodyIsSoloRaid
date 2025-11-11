using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private Image image;

    
    [SerializeField] PlayerMove playerMove;
    void Awake()
    {
        if (image != null)
        {
            image.gameObject.SetActive(false);
        }
        
        if (textTime == null)
        {
            textTime = GetComponentInChildren<TextMeshProUGUI>();
        }
        playerMove = FindAnyObjectByType<PlayerMove>();
    }

    
    private void OnEnable()
    {
        GameEvents.OnDodgeStarted += ShowCooldownUI;
        GameEvents.OnCooldownUpdated += UpdateCooldownText; 
        GameEvents.OnDodgeCooldownFinished += HideCooldownUI;
    }

    private void OnDisable()
    {
        GameEvents.OnDodgeStarted -= ShowCooldownUI;
        GameEvents.OnCooldownUpdated -= UpdateCooldownText; 
        GameEvents.OnDodgeCooldownFinished -= HideCooldownUI;
    }
    private void Update()
    {
        playerMove

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