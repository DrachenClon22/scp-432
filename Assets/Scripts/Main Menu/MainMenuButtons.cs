using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {

    public CameraController _camera;

    public GameObject startButton;

    public Color activatedColor = Color.white;
    public Color deactivatedColor = new Color(0.2264151f, 0.2264151f, 0.2264151f, 1f);

    private void Start()
    {
        if (Time.timeScale < 1)
            Time.timeScale = 1f;
    }

    public void Button_StartGame(string scene)
    {
        StartCoroutine(LoadScene(scene));
    }

    public void Button_Quit_Game(string scene)
    {
        StartCoroutine(LoadScene2(scene));
    }

    public void Button_Quit()
    {
        Application.Quit();
    }

    public void Button_Unpause(GameObject pause)
    {
        _camera.DoUnpause();

        pause.SetActive(false);
    }

    private IEnumerator LoadScene2(string scene)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!(async.progress > 0.8))
        {
            yield return new WaitForEndOfFrame();
        }

        async.allowSceneActivation = true;
    }

    private IEnumerator LoadScene(string scene)
    {
        startButton.GetComponent<Button>().enabled = false;
        startButton.GetComponent<Image>().color = deactivatedColor;

        AsyncOperation async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!(async.progress > 0.8))
        {
            yield return new WaitForEndOfFrame();
        }

        async.allowSceneActivation = true;
    }
}
