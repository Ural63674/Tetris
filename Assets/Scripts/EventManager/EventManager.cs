using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static IntUnityEvent OnAddScore = new IntUnityEvent();
    public static UnityEvent OnGameOver = new UnityEvent();
    public static UnityEvent OnGamePaused = new UnityEvent();

    public static void SendAddScore(int number)
    {
        OnAddScore.Invoke(number);
    }

    public static void SendGameOver()
    {
        OnGameOver.Invoke();
    }

    public static void SendGamePaused()
    {
        OnGamePaused.Invoke();
    }
}

/// <summary>
/// ������� ���� ����� unityEvent ��� ����������� �������� UnityEvent � ����� ���������� ���� int
/// </summary>
public class IntUnityEvent : UnityEvent<int>
{

}
