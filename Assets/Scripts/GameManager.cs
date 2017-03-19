using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static bool startGame;
    bool isPlaying;
    public GameObject ship, spawner, wave;

    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void LateUpdate()
    {
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
