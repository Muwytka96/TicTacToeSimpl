using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridScript : MonoBehaviour
{
    public Button button;
    public Text buttonText;

    private GameControler gameControler;

    public void SetSpace()
    {
        if (gameControler.PlayerMove == true)
        {
            buttonText.text = gameControler.GetPlayerSide;
            button.interactable = false;
            gameControler.EndTurn();
        }
    }

    public void SetGameControlerReference(GameControler controler)
    {
        gameControler = controler;
    }

}
