using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITime : MonoBehaviour
{
    public Text time;
    
    void Update()
    {
        time.text = GetTimeString(TimeManager.Instance.GetRemainingTime());
    }
    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    private string GetTimeString(float timeRemaining)
    {
        int minute = Mathf.floorToInt(timeRemaining / 60);
        int second = Mathf.floorToInt(timeRemaining % 60);

        return string.Format("{0} : {1}", minute.ToString(), second.ToString());

    }
}
