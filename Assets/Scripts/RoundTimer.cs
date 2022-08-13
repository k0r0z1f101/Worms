using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    private float _startTime;
    private float _stopTime;
    private bool _running = false;
    private AudioSource _audioSource;
    private AudioClip _timerTick;
    private int _countdownSoundTimer;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_running)
        {
            int currentTeam = GameObject.Find("GameController").GetComponent<GameController>().GetCurrentTeam();
            GetComponent<TextMeshProUGUI>().color = currentTeam == 0 ? Color.red : Color.blue;
            GetComponent<TextMeshProUGUI>().text = "" + (int)(_stopTime - (Time.time - _startTime));
            if ((int)(_stopTime - (Time.time - _startTime)) == _countdownSoundTimer)
            {
                _audioSource = GetComponent<AudioSource>();
                _timerTick = Resources.Load<AudioClip>("TIMERTICK") as AudioClip;
                _audioSource.PlayOneShot(_timerTick);
                --_countdownSoundTimer;
            }

            if ((int)(_stopTime - (Time.time - _startTime)) == 0)
            {
                GameObject.Find("GameController").GetComponent<GameController>().EndWormTurn();
            }
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
        _countdownSoundTimer = 5;
    }
}
