using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameObject _worms;
    private GameObject _wind;
    private GameObject _terrain;
    private GameObject _gameController;

    private void Awake()
    {
        _worms = GameObject.Find("Worms");
        _wind = GameObject.Find("Wind");
        _terrain = GameObject.Find("Terrain");
        _gameController = GameObject.Find("GameController");
    }

    public void SetWormsText()
    {
        int value = (int)(_worms.transform.GetChild(1).GetComponent<Slider>().value);
        _worms.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + value;
    }

    public void SetWindText()
    {
        int value = (int)(_wind.transform.GetChild(1).GetComponent<Slider>().value);
        string newText = value == 4 ? "~" : "" + value;
        _wind.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = newText;
    }

    public void StartGame()
    {
        GameController script = _gameController.GetComponent<GameController>();
        float terrainValue = _terrain.transform.GetChild(1).GetComponent<Slider>().value;
        int holesNumber = (int)(terrainValue >= 0.5f ? 60 + (terrainValue - 0.5f) * 1200.0f : 20 + terrainValue * 80.0f);
        float radius = 7.5f - terrainValue * 6.0f;
        script.SetGame((int)(_worms.transform.GetChild(1).GetComponent<Slider>().value), (int)(_wind.transform.GetChild(1).GetComponent<Slider>().value), holesNumber,radius);
        script.StartGame();
    }
}
