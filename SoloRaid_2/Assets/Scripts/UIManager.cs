using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private Image image;
    [SerializeField] private Slider HpSlider;
    
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
        HpSlider = GetComponentInChildren<Slider>();
    }
    private void FixedUpdate()
    {
        HpSlider.value = playerMove.currentHealth / playerMove.maxHealth;
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
        //

    }




    
    private void ShowCooldownUI(float maxCooldown)
    {
        if (image != null) 
            image.gameObject.SetActive(true);

        UpdateCooldownText(maxCooldown); // 초기 값 설정
    }    
    private void UpdateCooldownText(float currentTime)
    {        
        if (currentTime < 0) currentTime = 0;
        
        textTime.text = Mathf.Ceil(currentTime).ToString("F0"); // 남은 시간을 정수로 표시(반올림)
    }    
    private void HideCooldownUI()
    {
        if (image != null) image.gameObject.SetActive(false); // 끄기
    }
}