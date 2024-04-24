using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class LoseManager : MonoBehaviour
{
    int score;
    [HideInInspector] public int highScore;
    [SerializeField] ScoreManager scoreManager;
   
    [SerializeField] GameObject menuUI;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] UIController scoreText;
    [SerializeField] FirebaseManager authManager;
    public UnityEvent updateAuth;

    public void Fabiola()
    {
        
        {
            score = scoreManager.score;
            if (score > highScore)
            {
                highScore = score;
                highScoreText.text = highScore.ToString();
                updateAuth.Invoke();
            }
            score = 0;
            ReloadScene();
        }
    }

    private void ReloadScene()
    {
        scoreManager.DeleteScore();
        scoreText.scoreText.text = "0";
       
        menuUI.SetActive(true);
    }

}

