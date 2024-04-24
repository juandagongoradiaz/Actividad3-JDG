using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSourceSFX;
    public AudioSource audioSourceMusic01;
    public AudioSource audioSourceMusic02;

    public AudioClip UISFX;
    public AudioClip GameStart;
  

    public void UIButton()
    {
        audioSourceSFX.PlayOneShot(UISFX); 
    }

    public void StartGame()
    {
        audioSourceSFX.PlayOneShot(GameStart);
        audioSourceMusic01.Stop();
        StartCoroutine(Music02Start());
    }

    IEnumerator Music02Start()
    {
        yield return new WaitForSeconds(1.5f);
        audioSourceMusic02.Play();
    }
    public void EndGame()
    {
        Debug.Log("EndGame");
        StartCoroutine(Music01Start());
        audioSourceMusic02.Stop();
    }

    IEnumerator Music01Start()
    {
        yield return new WaitForSeconds(1.5f);
        audioSourceMusic01.Play();
    }


}
