using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Boxes : MonoBehaviour
{
    bool parachuteDetach;
    GameObject parachute;
    float timer, detachTimer;
    // Use this for initialization
    void Start()
    {
        parachute = GameObject.FindGameObjectWithTag("Parachute");

    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;
    }
    void LateUpdate()
    {
        if (transform.parent.gameObject != null) {

            if (!parachuteDetach)
            {
                parachute.transform.parent = transform;
                detachTimer = Time.time;
            }
            else
            {
                transform.position = new Vector3(transform.position.x - GameManager.levelMovingSpeed, transform.position.y, transform.position.z);
                parachute.GetComponent<Rigidbody>().useGravity = true;
                parachute.GetComponent<Rigidbody>().isKinematic = false;
                if (parachute.transform.localScale.y > 0) {
                    parachute.transform.localScale = new Vector3(parachute.transform.localScale.x, Mathf.Lerp(parachute.transform.localScale.y, parachute.transform.localScale.y - 0.1f, 0.1f), parachute.transform.localScale.z);
                }
                if (timer > detachTimer + 4)
                {
                    GetComponent<Rigidbody>().drag = 20;
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Border")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
            //GetComponent<Rigidbody>().drag = 10;
        }
        if (collision.gameObject.tag == "Wave")
        {
            parachuteDetach = true;
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
            GetComponent<Rigidbody>().drag = 100;

        }
    }

}
