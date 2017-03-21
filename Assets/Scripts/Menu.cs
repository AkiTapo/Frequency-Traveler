using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{


    public Texture2D startGameIcon;



    void Start()
    {

    }

    void OnGUI()
    {

        //Create my style
        GUIStyle FTStyle = new GUIStyle(GUI.skin.label);
        Font gameFont = (Font)Resources.Load("Fonts/LeelaUIb", typeof(Font));


        FTStyle.font = gameFont;
        FTStyle.fontSize = 30;
        FTStyle.normal.textColor = new Color32(0xff, 0xd2, 0x00, 0xFF);

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
            GUI.Label(new Rect(Screen.width - Screen.width / 6, Screen.height / 50, 200, 50), "SCORE: " + GameManager.instance.score.ToString(), FTStyle);
        }
    }
}