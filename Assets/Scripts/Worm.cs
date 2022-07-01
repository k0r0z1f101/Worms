using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    private bool IsGrounded;
    
    void OnCollisionStay(Collision other)
    {
        if (other.transform.tag == "Ground")
        {
            IsGrounded = true;
            Debug.Log("Grounded");
        }
        else
        {
            IsGrounded = false;
            Debug.Log("Not Grounded!");
        }
    }
}
