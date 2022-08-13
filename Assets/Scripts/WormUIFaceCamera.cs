using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WormUIFaceCamera : MonoBehaviour
{
    [SerializeField] private Camera _object;

    private void Awake()
    {   
        _object = Camera.main;
        //print(gameObject.GetComponent<Worm>().GetTeam());
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = transform.parent.parent.name == "Team_0" ? Color.red : Color.blue;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = transform.parent.name.Replace("(Clone)","");

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _object.transform.position);
    }
}
