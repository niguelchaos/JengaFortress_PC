using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour {

    public async void LoginAnonymously() {
        // using (new Load("Logging you in...")) {
        await AuthService.Login();
        SceneManager.LoadSceneAsync("Lobby");
        // }
    }
}