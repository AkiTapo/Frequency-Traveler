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
    void OnCollisionEnter(Collision collision)
    {
        //print("Collldied");
        foreach (ContactPoint contact in collision.contacts)
        {

            if (collision.gameObject.tag == "Bird")
            {

                collision.gameObject.GetComponent<Animation>().Play("Die");
            }
        }

    }
}
