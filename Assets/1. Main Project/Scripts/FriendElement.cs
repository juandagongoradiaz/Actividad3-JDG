using UnityEngine;
using TMPro;

public class FriendElement : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;

    public void SetUp(string username)
    {
        usernameText.text = username;
    }
}