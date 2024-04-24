using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    int score;
    [HideInInspector] public int highScore;
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] FirebaseManager authManager;
    public UnityEvent updateAuth;

    public void UpdateScore()
    {
        
        {
            Debug.Log("Score esperado: " + score); 
            score = gameManager.score;
            if (score > highScore)
            {
                highScore = score;
                highScoreText.text = highScore.ToString();
                updateAuth.Invoke();
            }
            score = 0;
           
        }
    }

 

}

