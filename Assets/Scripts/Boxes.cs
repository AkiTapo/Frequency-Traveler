using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Boxes : MonoBehaviour
{
    bool parachuteDetach;
    Transform parachute;
    float timer, detachTimer;
    bool prizeSet;
    // Use this for initialization
    void Start()
    {
        parachute = transform.FindChild("Parashute");
    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;
    }
    void LateUpdate()
    {


        if (transform.parent.gameObject != null) {

            //If game is not paused
            if (GameManager.instance.isPlaying)
            {
                //If not landed on water
                if (!parachuteDetach)
                {
                    GetComponent<Rigidbody>().useGravity = true;
                    detachTimer = Time.time;
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x - 0.5f, 0.01f), transform.position.y, transform.position.z);
                }
                //If landed on water
                if (parachuteDetach)
                {
                    parachute.transform.parent = null;
                    transform.position = new Vector3(transform.position.x - GameManager.levelMovingSpeed, transform.position.y, transform.position.z);
                    parachute.GetComponent<Rigidbody>().useGravity = true;
                    parachute.GetComponent<Rigidbody>().isKinematic = false;

                    //Colapse parashute
                    if (parachute.transform.localScale.y > 0.2f)
                    {
                        parachute.transform.localScale = new Vector3(parachute.transform.localScale.x, Mathf.Lerp(parachute.transform.localScale.y, parachute.transform.localScale.y - 0.1f, 0.1f), parachute.transform.localScale.z);
                    }
                    // to make it drown after 4 seconds
                    if (timer > detachTimer + 4)
                    {
                        GetComponent<Rigidbody>().drag = 20;
                    }
                }
            }
            //if game is paused
            else
            {
                GetComponent<Rigidbody>().useGravity = false;
            }
        }
        if (transform.localPosition.y < -13)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Border")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
            //GetComponent<Rigidbody>().drag = 10;
        }
        if (collision.gameObject.tag == "Wave" || collision.gameObject.tag == "Rock" || collision.gameObject.tag == "Ship")
        {

            if(!prizeSet && collision.gameObject.tag == "Ship")
            {
                ContactPoint contact = collision.contacts[0];
                GameManager.instance.setPrize();
                GameManager.instance.indicateEvent = true;
                GameManager.instance.eventCollisionPoint = contact.point;

                print("Collided in point " + contact);
                prizeSet = true;
            }
            //If wave is hit st drag to very big, to make it "flote" above the water
            if (collision.gameObject.tag == "Wave")
            {
                GetComponent<Rigidbody>().drag = 100;
                GetComponent<Rigidbody>().angularDrag = 1;
            }
            //other objects as rocks, bird or ship, then drag is low, for it to drop like in the air
            else
            {
                GetComponent<Rigidbody>().drag = 1;
            }

            parachuteDetach = true;
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);

            

        }
    }

}
