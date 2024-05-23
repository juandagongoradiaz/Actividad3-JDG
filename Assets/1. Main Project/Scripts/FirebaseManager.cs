using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using Firebase.Extensions;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference dbReference;


    [Header("Login")]
    [SerializeField] private TMP_InputField emailLoginField;
    [SerializeField] private TMP_InputField passwordLoginField;
    [SerializeField] private TMP_Text warningLoginText;
    [SerializeField] TMP_Text confirmationPasswordText;

    [Header("Register")]
    [SerializeField] private TMP_InputField usernameRegisterField;
    [SerializeField] private TMP_InputField emailRegisterField;
    [SerializeField] private TMP_InputField passwordRegisterField;
    [SerializeField] private TMP_InputField passwordRegisterVerifyField;
    [SerializeField] private TMP_Text warningRegisterText;

    [Header("Forgot Password")]
    [SerializeField] TMP_InputField forgotPasswordEmail;
    [SerializeField] private TMP_Text warningForgetPasswordText;

    [Header("UserData")]
    [SerializeField] TMP_Text usernameField;

    [Header("Game")]
   
    [SerializeField] GameObject gameUI, menuUI, loginUI, registerUI, passUI;


    [Header("Friend Request")]
    [SerializeField] private TMP_InputField friendUsernameField;
    [SerializeField] private TMP_Text friendRequestStatusText;

    [Header("Friend Request Panel")]
    [SerializeField] private GameObject friendRequestElementPrefab;
    [SerializeField] private Transform friendRequestContent;

    [Header("Friend List Panel")]
    [SerializeField] private GameObject friendElementPrefab;
    [SerializeField] private Transform friendListContent;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
                auth.StateChanged += AuthStateChanged;
            }
            else
            {
                print($"No se pueden resolver todas las dependencias de Firebase: {dependencyStatus}");
            }
        });
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                // El usuario ha cerrado sesión
                dbReference.Child("users").Child(user.UserId).Child("status").SetValueAsync(false);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                // El usuario ha iniciado sesión
                dbReference.Child("users").Child(user.UserId).Child("status").SetValueAsync(true);
            }
        }
    }

    void InitializeFirebase()
    {
        print($"Configurando autorización de Firebase");
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void ForgotPasswordButton()
    {
        if (string.IsNullOrEmpty(forgotPasswordEmail.text))
        {
            warningForgetPasswordText.text = $"Falta ingresar el correo electrónico";
            return;
        }

        FogotPassword(forgotPasswordEmail.text);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("El usuario ha salido"); 
        Logout();
    }



    void FogotPassword(string forgotPasswordEmail)
    {
        warningLoginText.text = ""; 
        warningRegisterText.text = ""; 
        auth.SendPasswordResetEmailAsync(forgotPasswordEmail).ContinueWithOnMainThread(RestoreTask => {

            if (RestoreTask.IsCanceled)
            {
                Debug.LogError($"El cambio de contraseña ha sido cancelado.");
            }

            else if (RestoreTask.IsFaulted)
            {
                foreach (FirebaseException exception in RestoreTask.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                    }

                }
            }

            confirmationPasswordText.text = "El correo para reestablecer la contraseña ha sido enviado.";
            loginUI.SetActive(true);
            passUI.SetActive(false);
        });
    }

    IEnumerator Login(string email, string password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        confirmationPasswordText.text = "";
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo en el inicio con {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Ingreso Fallido";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Falta ingresar el correo electrónico.";
                    break;
                case AuthError.MissingPassword:
                    message = "Falta ingresar la contraseña.";
                    break;
                case AuthError.WrongPassword:
                    message = "Contraseña incorrecta.";
                    break;
                case AuthError.InvalidEmail:
                    message = "Correo electrónico inválido.";
                    break;
                case AuthError.UserNotFound:
                    message = "Usuario no encontrado.";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            confirmationPasswordText.text = "";
            user = LoginTask.Result.User;
            Debug.LogFormat("Usuario iniciado exitosamente: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "";

            // Actualizar estado en Firebase
            dbReference.Child("users").Child(user.UserId).Child("status").SetValueAsync(true);

            yield return new WaitForSeconds(1);

            usernameField.text = user.DisplayName;
            LoadFriendRequests();
            LoadFriendList();
            gameUI.SetActive(true);
            menuUI.SetActive(false);
        }
    }

    IEnumerator Register(string email, string password, string username)
    {
        if (username == "") warningRegisterText.text = "Falta ingresar el usuario.";
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text) warningRegisterText.text = "Las contraseñas no coinciden";
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Fallo en el registro con {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Registro fallido";

                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Falta ingresar el correo electrónico.";
                        break;
                    case AuthError.MissingPassword:
                        message = "Falta ingresar la contraseña";
                        break;
                    case AuthError.WeakPassword:
                        message = "Contraseña débil.";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Correo electrónico ya en uso.";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                user = RegisterTask.Result.User;

                if (user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = username };

                    var ProfileTask = user.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Fallo el registro con {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Configuración de usuario fallida";
                    }
                    else
                    {
                        var DBTask = dbReference.Child("users").Child(user.UserId).Child("username").SetValueAsync(username);
                        DBTask = dbReference.Child("users").Child(user.UserId).Child("score").SetValueAsync(0.ToString());
                        registerUI.SetActive(false);
                        loginUI.SetActive(true);
                        
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }



    public void SendFriendRequest()
    {
        string friendUsername = friendUsernameField.text;
        if (string.IsNullOrEmpty(friendUsername))
        {
            friendRequestStatusText.text = "Por favor, ingresa el nombre del usuario.";
            return;
        }

        StartCoroutine(SendFriendRequestCoroutine(friendUsername));
    }

    IEnumerator SendFriendRequestCoroutine(string friendUsername)
    {
        // Verificar que el usuario está autenticado
        if (user == null)
        {
            friendRequestStatusText.text = "Debes iniciar sesión para enviar solicitudes de amistad.";
            yield break;
        }

        // Buscar el usuario en la base de datos
        var userQuery = dbReference.Child("users").OrderByChild("username").EqualTo(friendUsername).GetValueAsync();
        yield return new WaitUntil(() => userQuery.IsCompleted);

        if (userQuery.Result.Exists && userQuery.Result.Children.Count() > 0)
        {
            DataSnapshot snapshot = userQuery.Result.Children.FirstOrDefault();
            string friendUserId = snapshot.Key;

            // Enviar la solicitud de amistad
            var friendRequestTask = dbReference.Child("friend_requests").Child(friendUserId).Child(user.UserId).SetValueAsync(user.DisplayName);
            yield return new WaitUntil(() => friendRequestTask.IsCompleted);

            if (friendRequestTask.Exception != null)
            {
                friendRequestStatusText.text = "Error al enviar la solicitud de amistad.";
                Debug.LogWarning($"Failed to send friend request: {friendRequestTask.Exception}");
            }
            else
            {
                friendRequestStatusText.text = "Solicitud de amistad enviada exitosamente.";
                Debug.Log("Friend request sent successfully!");
            }
        }
        else
        {
            friendRequestStatusText.text = "Usuario no encontrado.";
            Debug.LogWarning("User not found");
        }
    }

    public void LoadFriendRequests()
    {
        if (user == null)
        {
            Debug.LogWarning("No user is signed in");
            return;
        }

        StartCoroutine(LoadFriendRequestsCoroutine());
    }

    IEnumerator LoadFriendRequestsCoroutine()
    {
        var friendRequestsTask = dbReference.Child("friend_requests").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => friendRequestsTask.IsCompleted);

        if (friendRequestsTask.Exception != null)
        {
            Debug.LogWarning($"Failed to load friend requests: {friendRequestsTask.Exception}");
            yield break;
        }

        DataSnapshot snapshot = friendRequestsTask.Result;
        foreach (Transform child in friendRequestContent)
        {
            Destroy(child.gameObject);
        }

        foreach (DataSnapshot requestSnapshot in snapshot.Children)
        {
            string requestId = requestSnapshot.Key;
            string requestUsername = requestSnapshot.Value.ToString();

            GameObject requestElement = Instantiate(friendRequestElementPrefab, friendRequestContent);
            requestElement.GetComponent<FriendRequestElement>().SetUp(requestId, requestUsername, this);
        }
    }

    public void HandleFriendRequest(string requestId, bool isAccepted)
    {
        StartCoroutine(HandleFriendRequestCoroutine(requestId, isAccepted));
    }

    IEnumerator HandleFriendRequestCoroutine(string requestId, bool isAccepted)
    {
        if (isAccepted)
        {
            // Agregar a la lista de amigos
            var addFriendTask = dbReference.Child("friends").Child(user.UserId).Child(requestId).SetValueAsync(true);
            yield return new WaitUntil(() => addFriendTask.IsCompleted);

            if (addFriendTask.Exception != null)
            {
                Debug.LogWarning($"Failed to accept friend request: {addFriendTask.Exception}");
                yield break;
            }

            // También agrega al amigo la referencia de este usuario
            var addReverseFriendTask = dbReference.Child("friends").Child(requestId).Child(user.UserId).SetValueAsync(true);
            yield return new WaitUntil(() => addReverseFriendTask.IsCompleted);

            if (addReverseFriendTask.Exception != null)
            {
                Debug.LogWarning($"Failed to accept friend request: {addReverseFriendTask.Exception}");
                yield break;
            }

            // Actualizar la lista de amigos después de aceptar la solicitud
            LoadFriendList();
        }

        // Eliminar la solicitud de amistad
        var removeRequestTask = dbReference.Child("friend_requests").Child(user.UserId).Child(requestId).RemoveValueAsync();
        yield return new WaitUntil(() => removeRequestTask.IsCompleted);

        if (removeRequestTask.Exception != null)
        {
            Debug.LogWarning($"Failed to remove friend request: {removeRequestTask.Exception}");
        }
        else
        {
            Debug.Log("Friend request handled successfully");
            LoadFriendRequests(); // Recargar solicitudes de amistad
        }
    }

    public void LoadFriendList()
    {
        if (user == null)
        {
            Debug.LogWarning("No user is signed in");
            return;
        }

        StartCoroutine(LoadFriendListCoroutine());
    }

    IEnumerator LoadFriendListCoroutine()
    {
        var friendListTask = dbReference.Child("friends").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => friendListTask.IsCompleted);

        if (friendListTask.Exception != null)
        {
            Debug.LogWarning($"Failed to load friend list: {friendListTask.Exception}");
            yield break;
        }

        DataSnapshot snapshot = friendListTask.Result;
        foreach (Transform child in friendListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (DataSnapshot friendSnapshot in snapshot.Children)
        {
            string friendId = friendSnapshot.Key;

            // Get friend's username
            var friendUsernameTask = dbReference.Child("users").Child(friendId).Child("username").GetValueAsync();
            yield return new WaitUntil(() => friendUsernameTask.IsCompleted);

            if (friendUsernameTask.Exception != null)
            {
                Debug.LogWarning($"Failed to get friend's username: {friendUsernameTask.Exception}");
                continue;
            }

            string friendUsername = friendUsernameTask.Result.Value.ToString();
            GameObject friendElement = Instantiate(friendElementPrefab, friendListContent);
            FriendElement friendElementScript = friendElement.GetComponent<FriendElement>();
            friendElementScript.SetUp(friendUsername);

            // Listen for online status
            dbReference.Child("users").Child(friendId).Child("status").ValueChanged += (sender, args) =>
            {
                if (args.DatabaseError != null)
                {
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }

                if (args.Snapshot != null && args.Snapshot.Value != null)
                {
                    bool isOnline = (bool)args.Snapshot.Value;
                    friendElementScript.SetStatus(isOnline);
                }
                else
                {
                    Debug.LogWarning("El nodo 'status' para este amigo está vacío o no existe.");
                    // Aquí puedes manejar el caso en el que el nodo 'status' esté vacío o no exista
                    // Por ejemplo, podrías establecer el estado en línea como desconocido o fuera de línea
                }
            };
        }
    }

    public void Logout()
    {
        if (user != null)
        {
            dbReference.Child("users").Child(user.UserId).Child("status").SetValueAsync(false);
            auth.SignOut();
            user = null;
            // Actualiza la UI y otros elementos necesarios al cerrar sesión
        }
    }
}


