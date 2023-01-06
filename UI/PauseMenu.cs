using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public InputActionReference pauseAction;
    public Team14.CharacterController characterController;

    [Header("UIElements")]
    public Canvas PauseUI;
    public Button ResumeButton;
    public Button ExitButton;

    private bool paused;

    private void OnEnable()
    {
        pauseAction.action.performed += Handler;
        Cursor.lockState = CursorLockMode.None;
        ResumeButton.onClick.AddListener(() => SetPauseGame());
        ExitButton.onClick.AddListener(() => SceneManager.LoadScene("menuScene"));
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= Handler;
        ResumeButton.onClick.RemoveAllListeners();
        ExitButton.onClick.RemoveAllListeners();
    }

    public void SetPauseGame()
    {
        if(!paused)
        {
            Time.timeScale = 0;
            characterController.gameObject.SetActive(false);
            PauseUI.gameObject.SetActive(true);
            paused = true;
        }
        else
        {
            Time.timeScale = 1;
            PauseUI.gameObject.SetActive(false);
            characterController.gameObject.SetActive(true);
            paused = false;
        }
    }

    public void Handler(InputAction.CallbackContext context)
    {
        SetPauseGame();
    }
}
