using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{


    public GameObject bird;
    GameObject[] newBird;
    Animation birdFly;
    public int amountOfBirds = 10;
    public float birdSpawnInterval = 5;

    int currentBird;
    float timer;
    float lastSpawnedBird;
    float birdFlySpeed = 3;


    // Use this for initialization
    void Start()
    {
        newBird = new GameObject[amountOfBirds];
    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;

        if (timer > lastSpawnedBird)
        {
            lastSpawnedBird = timer + birdSpawnInterval;
            newBird[currentBird] = Instantiate(bird, new Vector3(transform.position.x, 0, 0), Quaternion.LookRotation(Vector3.back)) as GameObject;
            newBird[currentBird].GetComponent<Animation>().Play();

            newBird[currentBird].GetComponent<Birds>().alive = true;

            birdFly = newBird[currentBird].GetComponent<Animation>();
            birdFly["Fly"].speed = birdFlySpeed;

            print("currentBird id " + currentBird);

        }
        if (newBird[currentBird] != null)
        {
            newBird[currentBird].GetComponent<Rigidbody>().velocity = new Vector3(-1, 0, 0);

            // when current bird is dead
            if (newBird[currentBird].GetComponent<Birds>().alive == false)
            {
                print("Next bird comming");
                currentBird++;
            }
        }



    }

}
