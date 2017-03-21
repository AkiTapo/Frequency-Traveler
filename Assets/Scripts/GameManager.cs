using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject ship, spawner, wave;
    public bool isPlaying;
    public static float levelMovingSpeed;
    [Range(0.01f, 10)]
    public float levelSpeed;
    [Range(4, 100)]
    public float obstacleSpawnInterval;
    [Range(0.01f, 10)]
    public float birdSpeed;
    [Range(1, 10)]
    public float birdSpawnInterval;
    public int score = 0;

    //bool isPlaying;



    public static GameManager instance = null;


    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    void LateUpdate()
    {

        //print("Score " + score);
        //Assigning rock and bird moving speed, and making it randomize a bit.
        levelMovingSpeed = levelSpeed / 70;
        Spawner.birdFlySpeed = birdSpeed;
        Spawner.rockSpawnInterwal = obstacleSpawnInterval;
        Spawner.birdSpawnInterval = birdSpawnInterval;

        //Controlls
        if (Input.GetKeyDown(KeyCode.Escape) && isPlaying)
        {
            PauseGame();
        }
    }

    public void StartGame()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            print("Game Started");
            wave.SetActive(true);
            spawner.SetActive(true);
            ship.SetActive(true);
        }
    }
    public void PauseGame()
    {
        if (isPlaying)
        {
            isPlaying = false;
            print("Game Paused");
            //wave.SetActive(false);
            spawner.SetActive(false);
            //ship.SetActive(false);

            //Destroy(wave);
            //Destroy(spawner);
            //Destroy(ship);
        }

    }
}
