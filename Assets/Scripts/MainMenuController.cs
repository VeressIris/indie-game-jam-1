using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowOptionsMenu()
    {
        if (optionsMenu.activeInHierarchy == false)
        {
            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
        else
        {
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }
}
