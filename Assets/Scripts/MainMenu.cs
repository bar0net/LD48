using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Main");
    }
}
