using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    private float _startTime;
    private float _stopTime;
    private bool _running = false;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_running)
        {
            GetComponent<TextMeshProUGUI>().text = "" + (int)(_stopTime - (Time.time - _startTime));
        }
    }

    public void SetTimer(int seconds)
    {
        _stopTime = seconds;
    }

    public void StartTimer()
    {
        _startTime = Time.time;
        _running = true;
    }
}
