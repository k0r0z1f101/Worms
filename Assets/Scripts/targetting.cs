using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class targetting : MonoBehaviour
{
    private Vector3 _target;
    // Start is called before the first frame update
    void Start()
    {
        //_target = GetChildWithName(GameObject.Find("Worm"), "Target").transform.position;
    }

    GameObject GetChildWithName(GameObject obj, string name) {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null) {
            return childTrans.gameObject;
        } else {
            return null;
        }
    }    // Update is called once per frame
    void Update()
    {
        _target = GetChildWithName(gameObject.transform.parent.gameObject, "Target").transform.position;
        //Debug.DrawLine(transform.position,Vector3.right, Color.red);
        Debug.DrawRay(transform.position,_target,Color.red);  
    }
}