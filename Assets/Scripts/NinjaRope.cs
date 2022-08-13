using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NinjaRope : MonoBehaviour
{
    [SerializeField] private GameObject _ninjaRopePrefab;
    private GameObject _ninjaRope;
    [SerializeField] private GameObject WeaponPos;
    // Start is called before the first frame update
    private void Awake()
    {
        _ninjaRopePrefab = Resources.Load("Bow").GameObject();
        _ninjaRope = Instantiate(_ninjaRopePrefab);
        _ninjaRope.transform.position = WeaponPos.transform.position;
        _ninjaRope.transform.localScale = new Vector3(0.01f, 0.01f,0.01f);
        _ninjaRope.transform.Rotate(Vector3.forward,90);
        _ninjaRope.GetComponent<MeshRenderer>().enabled=false;
        _ninjaRope.transform.SetParent(transform,false);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
