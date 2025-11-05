using System.Collections.Generic;
using UnityEngine;


public class CoroutineManager
{
    private static Dictionary<float, WaitForSeconds> dic = new();

    public static WaitForSeconds WaitForSecond(float time)
    {
        WaitForSeconds waitForSeconds = null;

        if (dic.TryGetValue(time,out waitForSeconds) == false)
        {
            dic[time] = new WaitForSeconds(time);

            waitForSeconds = dic[time];
        }  

        return waitForSeconds;
    }
}
