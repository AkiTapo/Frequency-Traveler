using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birds : MonoBehaviour {

    public bool alive;
    bool died;
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
                print("Colided with ship");
                GameManager.instance.score += 100;
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
