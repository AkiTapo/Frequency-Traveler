using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public GameObject[] birds;
    GameObject[] newBird;
    Animation birdFly;
    public int amountOfBirds = 10;
    public static float birdSpawnInterval = 5;
    [Range(1, 10)]
    public static float birdFlySpeed = 1;

    public GameObject[] rocks;
    GameObject[] newRock;
    public int amountOfRocks = 10;
    public static float rockSpawnInterwal = 5;
    int rocksPresent;
    int currentRock;

    int currentBird;
    float timer;
    float lastSpawnedBird, lastSpawnedRock;
    float birdFlyAnimationSpeed = 2;
    Transform wave;

    public static Spawner instance;

    // Use this for initialization
    void Start()
    {
        newBird = new GameObject[amountOfBirds];
        newRock = new GameObject[amountOfRocks];
    }

    void OnAwake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;

        //print(Wave.minWaterLevelLocal + "Wave.minWaterLevelLocal");

        // Spawn birds
        manageBirds();
        //Spawn rocks
        manageRocks();



    }

    void manageBirds()
    {
        if (timer > lastSpawnedBird && newBird[currentBird] == null)
        {
            //Should check for all types of birds and rocks instead of first in array
            if (birds[0] != null)
            {
                lastSpawnedBird = timer + birdSpawnInterval;
                newBird[currentBird] = Instantiate(birds[0], new Vector3(transform.position.x, Random.Range(Wave.minWaterLevelLocal - 7, Wave.maxWaterLevelLocal - 11.4f), 0), Quaternion.LookRotation(Vector3.back)) as GameObject;
                newBird[currentBird].GetComponent<Animation>().Play();
                newBird[currentBird].GetComponent<Birds>().alive = true;
                birdFly = newBird[currentBird].GetComponent<Animation>();
                //print("currentBird id " + currentBird);
            }
            else
            {
                print("Insert bird object to the Spawner");
            }
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
            //print("Bird Destroyed");
        }
    }
    
    void manageRocks()
    {
        //Spawn rocks
        if (timer > lastSpawnedRock && currentRock < amountOfRocks)
        {
            if (rocks[0] != null)
            {
                lastSpawnedRock = timer + Random.Range(rockSpawnInterwal, rockSpawnInterwal + 10);
                newRock[currentRock] = Instantiate(rocks[Random.Range(0,4)], new Vector3(transform.position.x, transform.position.y + rocks[0].transform.localScale.y, transform.position.z + 0.7f), Quaternion.Euler(0, Random.Range(0, 360), 0)) as GameObject;
                newRock[currentRock].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                rocksPresent++;
                currentRock++;
            }
            else
            {
                print("Insert rock object into the Spawner");
            }
        }

        if (rocksPresent > 0)
        {
            for (int i = 0; i < rocksPresent; i++)
            {
                if (newRock[i] != null)
                {
                    newRock[i].transform.position = new Vector3(newRock[i].transform.position.x - GameManager.levelMovingSpeed, newRock[i].transform.position.y, newRock[i].transform.position.z);
                    //Kill Rock
                    if (newRock[i].transform.position.x < -13)
                    {
                        Destroy(newRock[i]);
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        if (birdFly != null)
        {
            birdFly["Fly"].speed = birdFlyAnimationSpeed * birdFlySpeed;
        }
    }

}
