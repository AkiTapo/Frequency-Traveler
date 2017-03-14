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
    Transform wave;


    // Use this for initialization
    void Start()
    {
        newBird = new GameObject[amountOfBirds];

    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;


        // Spawn birds
        if (timer > lastSpawnedBird && newBird[currentBird] == null)
        {
            lastSpawnedBird = timer + birdSpawnInterval;
            newBird[currentBird] = Instantiate(bird, new Vector3(transform.position.x, Random.Range(Wave.minWaterLevelLocal/3, Wave.maxWaterLevelLocal/3), 0), Quaternion.LookRotation(Vector3.back)) as GameObject;
            newBird[currentBird].GetComponent<Animation>().Play();

            newBird[currentBird].GetComponent<Birds>().alive = true;

            birdFly = newBird[currentBird].GetComponent<Animation>();
            birdFly["Fly"].speed = birdFlySpeed;

            print("currentBird id " + currentBird);

        }

        for (int i = 0; i < amountOfBirds; i++)
        {
            if (newBird[i] != null && newBird[i].GetComponent<Birds>().alive == true)
            {
                newBird[i].GetComponent<Rigidbody>().velocity = new Vector3(-1, 0, 0);

                // when current bird is dead
                if (newBird[i].GetComponent<Birds>().alive == false)
                {
                    print("Next bird comming");
                    currentBird++;
                }
            }
        }

        if(newBird[currentBird] != null && (newBird[currentBird].transform.position.y < - 2 * Wave.minWaterLevelLocal || newBird[currentBird].transform.position.x < - 11) )
        {
            GameObject.Destroy(newBird[currentBird]);
            print("Bird Destroyed");
        }





    }

}
