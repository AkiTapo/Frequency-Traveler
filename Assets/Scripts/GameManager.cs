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
    public  bool gameStartedOnce;


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

    void LateUpdate()
    {
        print("Lives " + getLives());
        //print("Score " + score);
        //Assigning rock and bird moving speed, and making it randomize a bit.
        levelMovingSpeed = levelSpeed / 70;
       // Spawner.birdFlySpeed = birdSpeed;
       // Spawner.rockSpawnInterwal = obstacleSpawnInterval;
       // Spawner.birdSpawnInterval = birdSpawnInterval;

        //Controlls
        if (Input.GetKeyDown(KeyCode.Escape) && isPlaying)
        {
            PauseGame();
        }

        if(lives == 0)
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
        if(addScore == 0)
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
        if (lives + addLives >= 0)
        {
            lives += addLives;
        }
    }
    public int getLives()
    {
        return lives;
    }
}
