using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scollScript : MonoBehaviour
{
    public Transform target;
    public GameObject Player;
    playerFSM playerScript;
    public float relativeSpeed = 1;
    private float oldPosition;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerScript = Player.GetComponent<playerFSM>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        oldPosition = target.position.x;
    }

    void FixedUpdate()
    {
        if (playerScript.horizontal != 0)
        {
           transform.Translate((target.position.x - oldPosition) / relativeSpeed, 0, 0);
           //Debug.Log("target: " +target.position.x);
           //Debug.Log("target antigo: " + oldPosition);
           oldPosition = target.position.x;
        }
    }
}
