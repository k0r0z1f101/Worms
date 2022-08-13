using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bazooka : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject _bazookaPrefab;
    private GameObject bazooka;
    [SerializeField] private GameObject WeaponPos;

    private void Awake()
    {
        _bazookaPrefab = Resources.Load("Bazooka").GameObject();
        bazooka =Instantiate(_bazookaPrefab);
        bazooka.transform.position = WeaponPos.transform.position;
        //bazooka.transform.Rotate(Vector3.up,90);
        //bazooka.SetActive(false);
        bazooka.GetComponentInChildren<MeshRenderer>().enabled = false;
        bazooka.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        bazooka.transform.SetParent(transform,false);
    }

    private void OnEnable()
    {
        

    }

    private void Update()
    {
        
    }
}
