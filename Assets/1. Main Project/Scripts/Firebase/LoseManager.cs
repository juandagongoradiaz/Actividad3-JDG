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
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] FirebaseManager authManager;
    public GameObject StartGameUI;
    public GameObject Hide; 
    public UnityEvent updateAuth;

    public void Fabiola()
    {
        
        {
            Debug.Log("Score esperado: " + score); 
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
        StartGameUI.SetActive(true);
        Hide.SetActive(false);
    }

}

