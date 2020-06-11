using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scollScript : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float relativeSpeed;
    [SerializeField] float oldPositionX;

    void Start()
    {
        if (relativeSpeed < 1)
        {
            relativeSpeed = 1;
        }

        target = GameObject.FindGameObjectWithTag("Player").transform;
        oldPositionX = target.position.x;
    }

    void Update()
    {
        transform.Translate((target.position.x - oldPositionX)/ relativeSpeed, 0, 0);
        oldPositionX = target.position.x;
    }

    void paralaxEffect()
    {

    }
}
