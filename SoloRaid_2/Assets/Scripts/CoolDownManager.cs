using System.Collections;
using UnityEngine;

public class CoolDownManager : MonoBehaviour
{


    // 오브젝트 활성화/비활성화 시 이벤트 구독 및 해지를 관리합니다.
    private void OnEnable()
    {
        // '회피 시작' 이벤트를 구독하여, 해당 이벤트가 발생하면 HandleDodgeStart 코루틴을 실행합니다.
        GameEvents.OnDodgeStarted += HandleDodgeStart;
    }

    private void OnDisable()
    {
        GameEvents.OnDodgeStarted -= HandleDodgeStart;
    }

    // --- 추가된 부분 ---
    // 이벤트가 발생했을 때 쿨타임 코루틴을 시작시키는 역할을 합니다.
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

            // --- 수정된 부분 ---
            // UIManager를 직접 호출하는 대신, '쿨타임이 업데이트 되었다'는 이벤트를 발생시킵니다.
            // UI를 포함한 어떤 객체든 이 이벤트를 구독하면 남은 시간을 전달받을 수 있습니다.

            GameEvents.OnCooldownUpdated?.Invoke(currentTime);

            yield return null;
        }

        if (currentTime <= 0)
        {
            // --- 수정된 부분 ---
            // Player의 상태를 직접 변경하는 대신, '쿨타임이 끝났다'는 이벤트를 발생시킵니다.
            // PlayerMove 스크립트가 이 이벤트를 듣고 스스로 상태(canDodge)를 변경할 것입니다.
            GameEvents.OnDodgeCooldownFinished?.Invoke();
        }
    }

}
