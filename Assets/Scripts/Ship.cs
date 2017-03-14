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
        
        if (transform.localEulerAngles.z > 90 || transform.eulerAngles.z < -90)
        {
            GetComponent<Collider>().enabled = false;
            print("Ship colider disabled");
        }
        else
        {
            GetComponent<Collider>().enabled = true;
        }
        
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
