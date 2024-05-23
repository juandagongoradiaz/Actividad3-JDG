using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [Header("Buttons")]
    public Button inicioButton;
    public Button jugarButton;
    public Button socialButton;

    [Header("GameObjects")]
    public GameObject inicioGameObject;
    public GameObject jugarGameObject;
    public GameObject socialGameObject;

    [Header("Texts Mesh Pro")]
    public TMP_Text inicioText;
    public TMP_Text jugarText;
    public TMP_Text socialText; 

    public void jugarUI()
    {
        //GameObjects
       // jugarGameObject.SetActive(true);
        socialGameObject.SetActive(false);
        inicioGameObject.SetActive(false);

        //Buttons
        jugarButton.interactable = false;
        socialButton.interactable = true;
        inicioButton.interactable = true;


        //Texts Mesh pro
        jugarText.color = Color.white;
        socialText.color = new Color(0.5345911f, 0.5345911f, 0.5345911f, 1f);
        inicioText.color = new Color(0.5345911f, 0.5345911f, 0.5345911f, 1f);


    }

    public void inicioUI()
    {
        //GameObjects
        jugarGameObject.SetActive(false);
        socialGameObject.SetActive(false);
        inicioGameObject.SetActive(true);

        //Buttons
        jugarButton.interactable = true;
        socialButton.interactable = true;
        inicioButton.interactable = false;


        //Texts Mesh pro
        inicioText.color = Color.white;
        socialText.color = new Color(0.5345911f, 0.5345911f, 0.5345911f, 1f);
        jugarText.color = new Color(0.5345911f, 0.5345911f, 0.5345911f, 1f);


    }

    public void socialUI()
    {
        //GameObjects
        jugarGameObject.SetActive(false);
        socialGameObject.SetActive(true);
        inicioGameObject.SetActive(false);

        //Buttons
        jugarButton.interactable = true;
        socialButton.interactable = false;
        inicioButton.interactable = true;


        //Texts Mesh pro
        socialText.color = Color.white;
        inicioText.color = new Color(0.5345911f, 0.5345911f, 0.5345911f, 1f);
        jugarText.color = new Color(0.5345911f, 0.5345911f, 0.5345911f, 1f);


    }



}
