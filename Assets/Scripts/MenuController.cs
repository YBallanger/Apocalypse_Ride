using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnPlayButton ()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainScene");
    }

    public void OnQuitButton ()
    {
        Application.Quit();
    }

}
