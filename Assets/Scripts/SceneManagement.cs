using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        if (!SceneManager.GetSceneByName("escena_abru").isLoaded)
        {
            SceneManager.LoadScene("escena_abru", LoadSceneMode.Additive);
        }
    }
}