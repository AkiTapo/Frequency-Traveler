﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birds : MonoBehaviour {

    public bool alive;
    bool died;
    Vector3 eventCollisionPoint;
    // Use this for initialization
    void Start () {
        alive = true;
    }
	void LateUpdate()
    {
        if (!alive)
        {
            playDeath();
        }
        
        if(GameManager.instance.spawner == null)
        {
           // Destroy(this);
        }
    }

    void playDeath()
    {
        if (!died) {
            print("Bird died");
            alive = false;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Animation>().Play("Die");
            died = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //oncolisionexit
        //on colission stay
       // foreach (ContactPoint contact in collision.contacts)
       // {
            if (collision.gameObject.tag == "Border")
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
            }


        if (collision.gameObject.tag != "Border" && collision.gameObject)
        {
            //Assign score
            if (collision.gameObject.tag == "Ship" && alive)
            {
                print("Colided with the ship");
                if (!GameManager.instance.gameOver)
                {
                    ContactPoint contact = collision.contacts[0];// Get the colision point

                    //Bad way to detect which bird it is, but it works
                    if (GetComponent<BoxCollider>().center.y == 0.35f)
                    {
                        GameManager.instance.setScore(450);
                    }
                    else
                    {
                        GameManager.instance.setScore(300);
                    }
                    GameManager.instance.setScore(300);
                    GameManager.instance.indicateEvent = true; // event happened
                    GameManager.instance.eventCollisionPoint = contact.point; // Set the colision point
                    GameManager.instance.controlGameDifficulity(1); // increase difficulty
                }
            }

            playDeath();

            if (collision.gameObject.tag == "Wave")
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
                GetComponent<Rigidbody>().drag = 10;
            }
            else
            {
                GetComponent<Rigidbody>().drag = 2;
            }
        }
    }
}
