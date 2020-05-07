using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public static int currentScore = 0;

    public static string currentPlayerName;

    public Text scoreText;

    public static void AddScore(string player, int points)
    {
        if (player == currentPlayerName)
        {
            currentScore += points;
        }
    }

    public static void ResetScore(string player)
    {
        if (player == currentPlayerName)
        {
            currentScore = 0;
        }
    }

    void Update()
    {
        scoreText.text = "Score: " + currentScore;
    }
}
