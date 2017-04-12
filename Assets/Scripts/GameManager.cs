using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject ship;
    public GameObject spawner;
    public GameObject wave;
    GameObject spawnerInstance, shipInstance, waveInstance;
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
    private int score = 0;
    private int lives = 3;
    public bool gameOver;
    public int shipLives = 3;
    public bool gameStartedOnce;
    public int boxSpawnInterval;
    int getPrize;
    public bool indicateEvent;
    public Vector3 eventCollisionPoint;
    float timer, eventCollisionTime;

    //bool isPlaying;



    public static GameManager instance = null;

    void Start()
    {
        spawnerInstance = null;
        waveInstance = wave;
        shipInstance = ship;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //Destroy(this);
        }
        DontDestroyOnLoad(this);
        lives = shipLives;
    }
    void Update()
    {
        timer = Time.time;

        if (indicateEvent && eventCollisionTime + 2 < timer)
        {
            indicateEvent = false;
        }
        if (!indicateEvent)
        {
            eventCollisionTime = timer;
        }
    }

    void LateUpdate()
    {
        //print("Lives " + getLives());
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

        if (lives == 0)
        {
            gameOver = true;
        }
    }

    public void StartGame()
    {

        if (!gameStartedOnce)
        {
            InstantiateObjects();
            gameStartedOnce = true;
        }

        if (!isPlaying)
        {
            isPlaying = true;
            print("Game Started");
            waveInstance.SetActive(true);
            spawnerInstance.SetActive(true);
            shipInstance.SetActive(true);
        }
    }
    public void PauseGame()
    {
        if (isPlaying)
        {
            isPlaying = false;
            print("Game Paused");
            spawnerInstance.SetActive(false);
        }

    }
    public void RestartGame()
    {
        destroyObjects();
        InstantiateObjects();
        gameOver = false;
        setScore(0);
        setLives(3);

    }


    void destroyObjects()
    {
        GameObject[] rocksDestroy = GameObject.FindGameObjectsWithTag("Rock");
        foreach (GameObject rock in rocksDestroy)
        {
            Destroy(rock);
        }
        GameObject[] birdsDestroy = GameObject.FindGameObjectsWithTag("Bird");
        foreach (GameObject bird in birdsDestroy)
        {
            Destroy(bird);
        }
        Destroy(shipInstance);
        Destroy(spawnerInstance);
    }


    void InstantiateObjects()
    {
        shipInstance = Instantiate(ship, new Vector3(12.25f, -5.78f, 0), Quaternion.identity);
        spawnerInstance = Instantiate(spawner, new Vector3(12.25f, -5.78f, 0), Quaternion.identity);
        spawnerInstance.GetComponent<Spawner>().enabled = true;
        shipInstance.GetComponent<Ship>().enabled = true;
        spawnerInstance.SetActive(true);
        shipInstance.SetActive(true);

    }


    public void setScore(int addScore)
    {
        if (addScore == 0)
        {
            score = 0;
        }
        if (score + addScore >= 0)
        {
            score += addScore;
        }

    }

    public int getScore()
    {
        return score;
    }

    public void setLives(int addLives)
    {
        if (lives + addLives >= 0 && lives + addLives < 7)
        {
            lives += addLives;
        }

        //What is this?
        if (addLives == 3)
        {
            lives = 3;
        }
        print("You have " + lives + " Lives");
    }
    public int getLives()
    {
        return lives;
    }


    public void controlGameDificulity(int difficulty)
    {
        switch (difficulty)
        {
            //Increase difficulty
            case 1:
                levelSpeed += 0.1f;
                obstacleSpawnInterval -= 1;
                birdSpeed += 0.1f;
                obstacleSpawnInterval -= 0.5f;

                break;
            //decrease difficulty
            case 2:
                levelSpeed -= 0.1f;
                obstacleSpawnInterval += 1;
                birdSpeed += 0.1f;
                obstacleSpawnInterval += 0.5f;

                break;
        }
    }

    public void setPrize()
    {
        getPrize = Random.Range(1, 3);

        print("Prize number " + getPrize);
        switch (getPrize)
        {
            //Prize is lives
            case 1:
                setLives(1);
                break;
            //Prize is score 300
            case 2:
                setScore(300);
                break;
        }
    }
}
