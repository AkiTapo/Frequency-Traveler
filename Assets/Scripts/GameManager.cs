using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject ship, spawner, wave;
    public static bool startGame;
    public static float levelMovingSpeed;
    [Range(0.01f, 10)]
    public float levelSpeed;
    [Range(4, 100)]
    public float obstacleSpawnInterval;
    [Range(0.01f, 10)]
    public float birdSpeed;
    [Range(1, 10)]
    public float birdSpawnInterval;

    bool isPlaying;


    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void LateUpdate()
    {
        //Assigning rock and bird moving speed, and making it randomize a bit.
        levelMovingSpeed = levelSpeed / 70; 
        Spawner.birdFlySpeed = birdSpeed;
        Spawner.rockSpawnInterwal = obstacleSpawnInterval;
        Spawner.birdSpawnInterval = birdSpawnInterval;

        if (startGame && !isPlaying)
        {
            isPlaying = true;
            print("Game Started");
            wave.SetActive(true);
            spawner.SetActive(true);
            ship.SetActive(true);
        }
        if (!startGame && isPlaying)
        {
            isPlaying = false;
            print("Game Ended");
            wave.SetActive(false);
            spawner.SetActive(false);
            ship.SetActive(false);

            //Destroy(wave);
            //Destroy(spawner);
            //Destroy(ship);
        }
    }
}
