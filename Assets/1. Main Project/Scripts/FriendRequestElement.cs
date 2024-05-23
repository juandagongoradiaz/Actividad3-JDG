using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendRequestElement : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;

    private string requestId;
    private FirebaseManager firebaseManager;

    public void SetUp(string requestId, string username, FirebaseManager manager)
    {
        this.requestId = requestId;
        this.firebaseManager = manager;
        usernameText.text = username;

        acceptButton.onClick.AddListener(AcceptRequest);
        rejectButton.onClick.AddListener(RejectRequest);
    }

    private void AcceptRequest()
    {
        firebaseManager.HandleFriendRequest(requestId, true);
    }

    private void RejectRequest()
    {
        firebaseManager.HandleFriendRequest(requestId, false);
    }
}