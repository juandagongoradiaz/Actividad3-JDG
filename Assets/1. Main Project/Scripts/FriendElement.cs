using UnityEngine;
using TMPro;

public class FriendElement : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text statusText;

    public void SetUp(string username)
    {
        usernameText.text = username;
        statusText.text = "Desconectado"; // Estado inicial
    }

    public void SetStatus(bool isOnline)
    {
        statusText.text = isOnline ? "En línea" : "Desconectado";
    }
}