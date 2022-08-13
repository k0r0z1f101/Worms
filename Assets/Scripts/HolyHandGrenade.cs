using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HolyHandGrenade : MonoBehaviour
{
    [SerializeField] public GameObject _grenadePrefab;
    private GameObject _grenade;
    [SerializeField] private GameObject WeaponPos;

    private void Awake()
    {
        _grenadePrefab = Resources.Load("Grenade").GameObject();
        _grenade = Instantiate(_grenadePrefab);
        _grenade.transform.position = WeaponPos.transform.position;
        _grenade.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        _grenade.GetComponent<MeshRenderer>().enabled=false;
        _grenade.transform.SetParent(transform,false);
    }

    private void OnEnable()
    {
        
        //_grenade.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        

    }
}
