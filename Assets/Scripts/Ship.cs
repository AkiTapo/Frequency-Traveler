using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void LateUpdate()
    {
       // OnCollisionEnter too much rotation sink, dissable colider
        /*
        if (transform.localRotation == new Vector4(58,5,5,4))
        {
            GetComponent<Collider>().enabled = false;
        }
        */
    }

    void OnCollisionEnter(Collision collision)
    {
        //print("Collldied");
        foreach (ContactPoint contact in collision.contacts)
        {

            if (collision.gameObject.tag == "Bird")
            {

                
            }
        }

    }
}
