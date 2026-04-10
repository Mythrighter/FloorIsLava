using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    //public void Settings()
    //{
    //    SceneManager.LoadScene("Settings");
    //}

    //public void Controls()
    //{
    //    SceneManager.LoadScene("Controls");
    //}



}
