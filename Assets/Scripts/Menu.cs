using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{


    public Texture2D startGameIcon, gameOver;
    public Texture2D livesIconFull, livesIconEmpty;
    private Texture2D[] liveIcons;
    float opacity = 0;



    void Start()
    {
        liveIcons = new Texture2D [2];
    }

    void OnGUI()
    {

        //Create my style
        GUIStyle FTStyle = new GUIStyle(GUI.skin.label);
        Font gameFont = (Font)Resources.Load("Fonts/ostrich-regular", typeof(Font));

        FTStyle.font = gameFont;
        FTStyle.fontSize = 30;
        FTStyle.normal.textColor = new Color32(0x38, 0x66, 0x78, 0xFF); //#386678


        //Main Menu
        if (GameManager.instance.isPlaying == false)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - startGameIcon.width / 2, Screen.height / 2, 242, 60), startGameIcon) || Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.instance.StartGame();
            }
        }

        //In Game
        if (GameManager.instance.isPlaying == true)
        {
            GUI.Label(new Rect(Screen.width / 25, Screen.height / 50, 200, 50), "SCORE: " + GameManager.instance.getScore().ToString(), FTStyle);

            //Lives
            for (int i = 0; i < 3; i++)
            {
                GUI.DrawTexture(new Rect(Screen.width - Screen.width / 6 + (35 * i), Screen.height / 50, 34 , 34), livesIconEmpty);
            }

            for (int i = 0; i < GameManager.instance.getLives(); i++)
            {
                GUI.DrawTexture(new Rect(Screen.width - Screen.width / 6 + (35 * i), Screen.height / 50, 34, 34), livesIconFull);
            }

            if (GameManager.instance.gameOver)
            {
                if(GUI.Button(new Rect(Screen.width / 2 - gameOver.width / 2, Screen.height / 2, 242, 60), gameOver) || Input.GetKeyDown(KeyCode.Return)){
                    //Restart game
                }
                GameObject.Find("Plane").GetComponent<MeshRenderer>().enabled = true;
                if (opacity < 0.8f)
                {
                    GameObject.Find("Plane").GetComponent<Renderer>().material.color = new Vector4(1, 1, 1, opacity += 0.005f);
                }
                GameObject.Find("Wave").GetComponent<Wave>().waveIntensity = 1;


            }
        }
    }
}