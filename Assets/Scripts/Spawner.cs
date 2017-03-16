using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{


    public GameObject [] birds;
    GameObject[] newBird;
    Animation birdFly;
    public int amountOfBirds = 10;
    public float birdSpawnInterval = 5;
    [Range (1,10)]
    public int birdFlySpeed = 1;

    public GameObject [] rocks;
    GameObject[] newRock;
    public int amountOfRocks = 10;
    public float rockSpawnInterwal = 5;
    [Range(1, 10)]
    public int rockSpeed = 1;
    int rocksPresent;

    int currentBird;
    float timer;
    float lastSpawnedBird, lastSpawnedRock;
    float birdFlyAnimationSpeed = 2;
    Transform wave;

    [Range(1, 10)]
    public int levelMovingSPeed = 1;


    // Use this for initialization
    void Start()
    {
        newBird = new GameObject[amountOfBirds];
        //rocks = new GameObject [3];
    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;

        print(Wave.minWaterLevelLocal + "Wave.minWaterLevelLocal");
        // Spawn birds
        if (timer > lastSpawnedBird && newBird[currentBird] == null)
        {
            lastSpawnedBird = timer + birdSpawnInterval;
            newBird[currentBird] = Instantiate(birds[0], new Vector3(transform.position.x, Random.Range(Wave.minWaterLevelLocal - 7, Wave.maxWaterLevelLocal - 10), 0), Quaternion.LookRotation(Vector3.back)) as GameObject;
            newBird[currentBird].GetComponent<Animation>().Play();
            newBird[currentBird].GetComponent<Birds>().alive = true;
            birdFly = newBird[currentBird].GetComponent<Animation>();
            //print("currentBird id " + currentBird);
        }

        //Seems too much calcualtions, try changing.
        //Move birds and to increase currentBird
        for (int i = 0; i < amountOfBirds; i++)
        {
            if (newBird[i] != null && newBird[i].GetComponent<Birds>().alive == true)
            {
                newBird[i].GetComponent<Rigidbody>().velocity = new Vector3(-birdFlySpeed, 0, 0);

                // when current bird is dead
                if (newBird[i].GetComponent<Birds>().alive == false)
                {
                    print("Next bird comming");
                    currentBird++;
                }
            }
        }

        if (newBird[currentBird] != null && (newBird[currentBird].transform.position.y < -2 * Wave.minWaterLevelLocal || newBird[currentBird].transform.position.x < -11))
        {
            GameObject.Destroy(newBird[currentBird]);
            print("Bird Destroyed");
        }



        //Spawn rocks
        if (timer > lastSpawnedRock)
        {
            lastSpawnedRock = timer + rockSpawnInterwal;
            newRock[0] = Instantiate(rocks[0], new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.LookRotation(Vector3.back)) as GameObject;
            rocksPresent++;
        }

        if (rocksPresent > 0)
        {
            for (int i = 0; i < rocksPresent; i++)
            {
                newRock[i].transform.position = new Vector3(rocks[i].transform.position.x - levelMovingSPeed, rocks[i].transform.position.y, rocks[i].transform.position.z);

            }


            //Kill Rock


        }






    }

    void LateUpdate()
    {
        if (birdFly != null) {
            birdFly["Fly"].speed = birdFlyAnimationSpeed * birdFlySpeed;
        }
    }

}
