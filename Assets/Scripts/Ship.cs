using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    //public Vector3 myRotation;
    public int drag;
    public bool drowning;
    bool drowningInWater;
    Collision collision;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void LateUpdate()
    {
        //To move the ship down the wave
        if (!drowning && collision != null && collision.gameObject.tag == "Wave" )
        {
            if (transform.localEulerAngles.z > 295)
            {
                //GetComponent<Rigidbody>().velocity = new Vector3((360 - transform.localEulerAngles.z) / 10, 0, 0);
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Lerp(GetComponent<Rigidbody>().velocity.x, 5, 0.2f), 0, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Lerp(GetComponent<Rigidbody>().velocity.x, -5, 0.2f), 0, 0);
            }
        }

        //print(transform.localEulerAngles.z);
        // OnCollisionEnter too much rotation sink, dissable colide5       
        if (transform.localEulerAngles.z < 295 && transform.localEulerAngles.z > 45)
        {
            //
            //  GetComponent<Collider>()[0]
            drowning = true;
            //print("Ship colider disabled");
        }
        else
        {
            //GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().drag = drag;
        }

        if (drowning)
        {
            //to dissable all coliders on ship
            foreach (Collider colider in GetComponents<Collider>())
            {
                colider.enabled = false;
            }


            if (collision != null && collision.gameObject.tag == "Wave")
            {
                //gameObject.GetComponent<Rigidbody>().drag = drag * 4;
                //gameObject.GetComponent<Rigidbody>().angularDrag = 1;
            }
            //to sink by facing front
            //transform.LookAt(Vector3.down);
        }
        else
        {
            foreach (Collider colider in GetComponents<Collider>())
            {
                colider.enabled = true;
            }
        }
        
        

        //Respawn ship
        if(transform.position.y < -7)
        {
            transform.position = new Vector3(1.37f, 0.09f, 0.7f);
            transform.rotation = Quaternion.identity;
            drowning = false;
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {

        this.collision = collision;
        //print("Collldied");
        /*
        foreach (ContactPoint contact in collision.contacts)
        {*/

        if (drowning && collision.gameObject.tag == "Wave")
        {

            if (!Input.GetKeyDown(KeyCode.D) || !Input.GetKeyDown(KeyCode.A))
            {
                if (!drowning && collision != null && collision.gameObject.tag == "Wave")
                {
                    //print(" Rotation " + transform.localEulerAngles.z);
                    if (transform.localEulerAngles.z > 295)
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3((360 - transform.localEulerAngles.z) / 10, 0, 0);
                    }
                    if (transform.localEulerAngles.z < 45)
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3((transform.localEulerAngles.z - 45) / 20, 0, 0);
                    }
                }
            }
        
                //Sink slowly, by setting drag to very high when in water and drowning

                if (collision.collider.bounds.Contains(transform.position))
            {
                print("point is inside collider");
            }

            gameObject.GetComponent<Rigidbody>().drag = drag * 4;
            gameObject.GetComponent<Rigidbody>().angularDrag = 1;
            print("Drownging and colliding with " + collision.gameObject);
        }

        if (collision.gameObject.tag == "Rock")
        {

        }

        /*
        if(drowningInWater)
        for (int i = 0; i < 120; i++) {
                if (collision.collider.bounds.Contains())
                {

                }
        }*/

        //Sink slowly, by setting drag to very high when in water and drowning
        /*
        if (collision.collider.bounds.Contains(transform.position))
        {
            print("point is inside collider");
        }

        if (collision.gameObject.tag == "Bird")
        {



        }
        */
        // }

    }
}
