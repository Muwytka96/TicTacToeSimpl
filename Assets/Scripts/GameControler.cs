using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;
}

[Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

[Serializable]
public class Spot
{
    public Text Text;

    public int Index;
}


public class Turn
{
    public Spot Spot { get; set; }

    public int Score { get; set; }
}

public class GameControler : MonoBehaviour
{
    public List<Spot> Board;
    public bool PlayerMove;

    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject restartButton;
    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;
    public GameObject startInfo;

    private int moveCount;

    private string playerSide;
    private string computerSide;

    public string GetPlayerSide => playerSide;
    public string GetComputerSide => computerSide;

    void Awake()
    {
        gameOverPanel.SetActive(false);
        SetGameControlerReferenceOnButtons();

        moveCount = 0;
        restartButton.SetActive(false);
        PlayerMove = true;
    }

    void Update()
    {
        if (!PlayerMove)
        {
            var turn = MiniMax(Board, computerSide);
            if (turn.Spot != null)
            {
                turn.Spot.Text.text = computerSide;
                turn.Spot.Text.GetComponentInParent<Button>().interactable = false;
            }

            EndTurn();
        }
    }

    public void EndTurn()
    {
        moveCount++;

        if (IsWin(Board.ToArray(), playerSide))
        {
            GameOver(playerSide);
            return;
        }

        if (IsWin(Board.ToArray(), computerSide))
        {
            GameOver(computerSide);
            return;
        }

        if (moveCount >= 9)
        {
            GameOver("draw");
            return;
        }

        ChangeSides();
    }

    public void SetStartingSide(string startingSide)
    {
        playerSide = startingSide;
        if (playerSide == "X")
        {
            computerSide = "O";
            SetPlayerColor(playerX, playerO);
        }
        else
        {
            computerSide = "X";
            SetPlayerColor(playerO, playerX);
        }

        StartGame();
    }

    private void StartGame()
    {
        SetBoardInteractable(true);
        SetPlayerButtons(false);
        startInfo.SetActive(false);
    }

    public void RestartGame()
    {
        moveCount = 0;
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        startInfo.SetActive(true);

        SetPlayerButtons(true);
        SetPlaerColorInactive();

        PlayerMove = true;

        foreach (var spot in Board)
        {
            spot.Text.text = string.Empty;
        }
    }

    private Turn MiniMax(List<Spot> board, string side)
    {
        var emptySpots = GetEmptySpots(board);

        if (IsWin(board.ToArray(), playerSide))
        {
            return new Turn
            {
                Score = -10
            };
        }

        if (IsWin(board.ToArray(), computerSide))
        {
            return new Turn
            {
                Score = 10
            };
        }

        if (!emptySpots.Any())
        {
            return new Turn
            {
                Score = 0
            };
        }


        var turns = new List<Turn>();

        foreach (var emptySpot in emptySpots)
        {
            var turn = new Turn();

            var spot = board.First(x => x.Index == emptySpot.Index);

            turn.Spot = spot;

            //turn
            spot.Text.text = side;

            if (side == playerSide)
            {
                var result = MiniMax(board, computerSide);
                turn.Score = result.Score;
            }
            else
            {
                var result = MiniMax(board, playerSide);
                turn.Score = result.Score;
            }

            spot.Text.text = string.Empty;

            turns.Add(turn);
        }

        Turn bestTurn = null;
        if (side == computerSide)
        {
            var bestScore = -10000;

            foreach (var turn in turns)
            {
                if (turn.Score > bestScore)
                {
                    bestScore = turn.Score;
                    bestTurn = turn;
                }
            }
        }
        else
        {
            var bestScore = 10000;
            foreach (var turn in turns)
            {
                if (turn.Score < bestScore)
                {
                    bestScore = turn.Score;
                    bestTurn = turn;
                }
            }
        }

        return bestTurn;
    }

    private IEnumerable<Spot> GetEmptySpots(List<Spot> board)
    {
        var result = board.Where(x => !x.Text.text.Equals("x", StringComparison.OrdinalIgnoreCase) &&
                                      !x.Text.text.Equals("o", StringComparison.OrdinalIgnoreCase));

        return result;
    }

    private bool IsWin(Spot[] board, string side)
    {
        return
            (board[0].Text.text == side && board[1].Text.text == side && board[2].Text.text == side) ||
            (board[3].Text.text == side && board[4].Text.text == side && board[5].Text.text == side) ||
            (board[6].Text.text == side && board[7].Text.text == side && board[8].Text.text == side) ||
            (board[0].Text.text == side && board[3].Text.text == side && board[6].Text.text == side) ||
            (board[1].Text.text == side && board[4].Text.text == side && board[7].Text.text == side) ||
            (board[2].Text.text == side && board[5].Text.text == side && board[8].Text.text == side) ||
            (board[0].Text.text == side && board[4].Text.text == side && board[8].Text.text == side) ||
            (board[2].Text.text == side && board[4].Text.text == side && board[6].Text.text == side);
    }

    private void SetGameControlerReferenceOnButtons()
    {
        foreach (var spot in Board)
        {
            spot.Text.GetComponentInParent<GridScript>().SetGameControlerReference(this);
        }
    }

    private void SetPlayerColor(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    private void SetGameOverText(string value)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }

    private void SetBoardInteractable(bool toggle)
    {
        foreach (var spot in Board)
        {
            spot.Text.GetComponentInParent<Button>().interactable = toggle;
        }
    }

    private void SetPlayerButtons(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }

    private void SetPlaerColorInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }

    private void GameOver(string winningPlayer)
    {
        SetBoardInteractable(false);

        if (winningPlayer == "draw")
        {
            SetGameOverText(" Ничья! ");
            SetPlaerColorInactive();
        }
        else
        {
            SetGameOverText(winningPlayer + " победили!");
        }

        restartButton.SetActive(true);
    }

    private void ChangeSides()
    {
        PlayerMove = !PlayerMove;

        if (PlayerMove == true)
        {
            SetPlayerColor(playerX, playerO);
        }
        else
        {
            SetPlayerColor(playerO, playerX);
        }
    }
}
