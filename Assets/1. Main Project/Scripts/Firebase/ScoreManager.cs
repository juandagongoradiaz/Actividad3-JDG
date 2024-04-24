using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public int score { get; private set; }


    void Start()
    {
        score = 0;
      
    }

    public void startcounter()
    {
        StartCoroutine(IncrementScoreCoroutine());
    }

    public void stopcounter()
    {
       StopCoroutine(IncrementScoreCoroutine());
    }


    private IEnumerator IncrementScoreCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log(score); 
            AddScore();
        }
    }

    public void AddScore()
    {
        score += 1;
    }

    public void DeleteScore()
    {
        score = 0;
    }
}
