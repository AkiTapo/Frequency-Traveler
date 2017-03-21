using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    //public Vector3 myRotation;
    public int drag;
    public static bool drowning, shipReset;
    //public bool drown;
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
        //drowning = drown;
        //To move the ship down the wave
        if (!drowning && collision != null && collision.gameObject.tag == "Wave")
        {

            //To controll ship with buttons, this overrides ship naturaly sliding down the wave
            if (Input.GetKey(KeyCode.D))
            {
                //ORG
                GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Lerp(GetComponent<Rigidbody>().velocity.x, 5, 0.2f), 0, 0);


            }
            if (Input.GetKey(KeyCode.A))
            {

                //GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Lerp(GetComponent<Rigidbody>().velocity.x, -5, 0.2f), 0, 0);
                GetComponent<Rigidbody>().velocity = gameObject.transform.right*-1;
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

        /*
        if (drowning)
        {
            if (collision != null && collision.gameObject.tag == "Wave")
            {
                //gameObject.GetComponent<Rigidbody>().drag = drag * 4;
                //gameObject.GetComponent<Rigidbody>().angularDrag = 1;
            }

        }
        else
        {
            foreach (Collider colider in GetComponents<Collider>())
            {
                colider.enabled = true;
            }
        }

        */

        //Respawn ship
        if (transform.position.y < -7 || transform.position.x < -11 || transform.position.x > 10)
        {
            transform.position = new Vector3(1.37f, 0.09f, 0.7f);
            transform.rotation = Quaternion.identity;
            drowning = false;
            shipReset = true;
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            //drown = false;
        }
        else
        {
            shipReset = false;
        }

    }

    void OnCollisionEnter(Collision collision)
    {

        this.collision = collision;
        //print("Collldied");
        /*
        foreach (ContactPoint contact in collision.contacts)
        {*/


        if (!Input.GetKeyDown(KeyCode.D) || !Input.GetKeyDown(KeyCode.A))
        {
            if (!drowning && collision != null && collision.gameObject.tag == "Wave")
            {

                //transform.forward()
                //print(" Rotation " + transform.localEulerAngles.z);
                
                if (transform.localEulerAngles.z > 295)
                {
                    // ORG // GetComponent<Rigidbody>().velocity = new Vector3((360 - transform.localEulerAngles.z) / 10, 0, 0);
                    GetComponent<Rigidbody>().velocity = gameObject.transform.right;
                }
                if (transform.localEulerAngles.z < 45)

                {
                    // ORG // GetComponent<Rigidbody>().velocity = new Vector3((transform.localEulerAngles.z - 45) / 20, 0, 0);
                    GetComponent<Rigidbody>().velocity = gameObject.transform.right * -1;
                }
            }
        }


        if (drowning && collision.gameObject.tag == "Wave")
        {
            //Sink slowly, by setting drag to very high when in water and drowning

            if (collision.collider.bounds.Contains(transform.position))
            {
                //print("point is inside collider");
            }

            gameObject.GetComponent<Rigidbody>().drag = drag * 4;
            gameObject.GetComponent<Rigidbody>().angularDrag = 1;
            print("Drownging and colliding with " + collision.gameObject);
        }

        if (collision.gameObject.tag == "Rock")
        {
            drowning = true;
            gameObject.GetComponent<Rigidbody>().drag = drag * 4;
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
