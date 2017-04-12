using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    //public Vector3 myRotation;
    public int drag;
    public static bool drowning, shipReset;
    public bool drown;
    bool drowningInWater;
    Collision collision;
    private float shipDrownTime, timer;
    public int lives;

    void Awake()
    {
        lives = GameManager.instance.getLives();
    }

    void Update()
    {
        timer = Time.time;

        if (drowning && shipDrownTime + 4 < timer)
        {
            respawnShip();
        }
    }

    void LateUpdate()
    {
        drown = drowning;
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
                GetComponent<Rigidbody>().velocity = gameObject.transform.right * -1;
            }
        }

        //Drown when flip or when collided with rock
        if (!drowning && (transform.localEulerAngles.z < 295 && transform.localEulerAngles.z > 45 || collision != null && collision.gameObject.tag == "Rock"))
        {
            if (collision != null && collision.gameObject.tag == "Rock" && !drowning)
            {
                //Remove 50 points if not game over
                if (!GameManager.instance.gameOver)
                {
                    GameManager.instance.setScore(-50);
                    ContactPoint contact = collision.contacts[0];
                    GameManager.instance.indicateEvent = true;
                    GameManager.instance.eventCollisionPoint = contact.point;
                }
                collision = null;
            }

            drowning = true;
            GameManager.instance.setLives(-1);
            gameObject.GetComponent<Rigidbody>().drag = drag * 4;
            shipDrownTime = timer;
        }
        if (!drowning)
        {
            GetComponent<Rigidbody>().drag = drag;
        }

        //Respawn ship
        if (transform.position.y < -7 || transform.position.x < -11 || transform.position.x > 10 || Input.GetKey(KeyCode.R))
        {
            respawnShip();
        }
        else
        {
            shipReset = false;
        }
    }

    public void respawnShip()
    {
        transform.position = new Vector3(-6.87f, 1f, 0.7f);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        drowning = false;
        shipReset = true;
        collision = null;
        //drown = false;
    }

    void OnCollisionEnter(Collision collision)
    {

        this.collision = collision;


        if (!Input.GetKeyDown(KeyCode.D) || !Input.GetKeyDown(KeyCode.A))
        {
            if (!drowning && collision != null && collision.gameObject.tag == "Wave")
            {

                //transform.forward()
                //print(" Rotation " + transform.localEulerAngles.z);

                if (transform.localEulerAngles.z > 295)
                {
                    // ORG // GetComponent<Rigidbody>().velocity = new Vector3((360 - transform.localEulerAngles.z) / 10, 0, 0);
                    GetComponent<Rigidbody>().velocity = gameObject.transform.right * 2;
                }
                if (transform.localEulerAngles.z < 45)

                {
                    // ORG // GetComponent<Rigidbody>().velocity = new Vector3((transform.localEulerAngles.z - 45) / 20, 0, 0);
                    GetComponent<Rigidbody>().velocity = gameObject.transform.right * -1 * 2;
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

            gameObject.GetComponent<Rigidbody>().drag = drag * 2;
            gameObject.GetComponent<Rigidbody>().angularDrag = 0.4f;
            print("Drownging and colliding with " + collision.gameObject);
        }

    }
}
