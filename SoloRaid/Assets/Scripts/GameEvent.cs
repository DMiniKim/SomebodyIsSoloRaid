using System;

public static class GameEvents
{
    // �÷��̾ ȸ�Ǹ� �������� �� �߻��ϴ� �̺�Ʈ�Դϴ�. ȸ���� �� ��Ÿ���� �Ű������� �����մϴ�.
    public static Action<float> OnDodgeStarted;

    // ��Ÿ���� �� ������ ������Ʈ�� ������ ���� �ð��� �����ϴ� �̺�Ʈ�Դϴ�.
    public static Action<float> OnCooldownUpdated;

    // ȸ�� ��Ÿ���� ��� ����Ǿ��� �� �߻��ϴ� �̺�Ʈ�Դϴ�.
    public static Action OnDodgeCooldownFinished;
}
