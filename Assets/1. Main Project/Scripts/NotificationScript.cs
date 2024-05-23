using System.Collections;
using UnityEngine;
using TMPro;



public class NotificationScript : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text statusText;

    public void SetUp(string username)
    {
        usernameText.text = username;
        gameObject.SetActive(true); // Asegúrate de activar el objeto cuando se configura
        Destroy(gameObject, 3f);
    }

    public void SetStatus(bool isOnline)
    {
        statusText.text = isOnline ? "Se ha conectado" : "Se ha desconectado";
    }

}
