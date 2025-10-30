using System;

public static class GameEvents
{
    // 플레이어가 회피를 시작했을 때 발생하는 이벤트입니다. 회피의 총 쿨타임을 매개변수로 전달합니다.
    public static Action<float> OnDodgeStarted;

    // 쿨타임이 매 프레임 업데이트될 때마다 남은 시간을 전달하는 이벤트입니다.
    public static Action<float> OnCooldownUpdated;

    // 회피 쿨타임이 모두 종료되었을 때 발생하는 이벤트입니다.
    public static Action OnDodgeCooldownFinished;
}
