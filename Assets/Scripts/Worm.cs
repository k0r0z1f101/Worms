using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    private string _name = "";
    private int _health = 100;
    private int _teamNumber;

    public void SetHealth(int healthChange)
    {
        _health += healthChange;
    }

    public void SetName(string name)
    {
        _name = name;
    }
    public void SetTeam(int teamNumber)
    {
        _teamNumber = teamNumber;
    }
}
