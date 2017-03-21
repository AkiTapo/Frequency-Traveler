using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {


    public Texture2D startGameIcon;



    void Start()
    {

    }

    void OnGUI()
    {
        if (GameManager.instance.isPlaying == false)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - startGameIcon.width / 2, Screen.height / 2, 242, 60), startGameIcon))
            {
                GameManager.instance.StartGame();
            }
        }
    



        if (Input.GetKey(KeyCode.Escape))
        {
            GameManager.instance.EndGame();
        }
    }
}