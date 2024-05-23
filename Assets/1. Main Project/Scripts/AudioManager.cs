using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSourceSFX;


    public AudioClip UISFX;
   
  

    public void UIButton()
    {
        audioSourceSFX.PlayOneShot(UISFX); 
    }



}
