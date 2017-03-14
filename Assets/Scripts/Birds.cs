using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birds : MonoBehaviour {

    public bool alive;
	// Use this for initialization
	void Start () {
        alive = true;
    }
	
	// Update is called once per frame
	void Update () {
        print("this bird is alive " + alive);
	}

    void OnCollisionEnter(Collision collision)
    {

       // foreach (ContactPoint contact in collision.contacts)
       // {
            if (collision.gameObject.tag == "Border")
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
            }


        if (collision.gameObject.tag != "Border" && collision.gameObject || collision.gameObject.tag == "Ship")
        {
            print("Bird died");
            alive = false;
            GetComponent<Rigidbody>().useGravity = true;



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
