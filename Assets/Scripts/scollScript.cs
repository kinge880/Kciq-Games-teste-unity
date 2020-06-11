using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scollScript : MonoBehaviour
{
    public Transform target;
    public float relativeSpeed = 1;
    private Vector3 oldPosition;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera").transform;
        oldPosition = target.position;
    }

    void FixedUpdate()
    {
        transform.Translate((target.position.x - oldPosition.x) / relativeSpeed, 0, 0);
        oldPosition = target.position;
    }

}
