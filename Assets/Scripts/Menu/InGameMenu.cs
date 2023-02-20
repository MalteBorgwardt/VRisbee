using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject menuCanvas;
    public InputActionReference toggleReference;

    public GameObject leftHand;
    public GameObject rightHand;

    public Slider handSelectionSlider;

    private void Awake()
    {
        toggleReference.action.started += ToggleMenu;

        menuCanvas.SetActive(false);

        // init hand selection based on initial GameSettings
        handSelectionSlider.value = GameSettings.isLeftHanded ? 0f : 1f;
        ToggleHandSelection(handSelectionSlider.value);
        handSelectionSlider.onValueChanged.AddListener(ToggleHandSelection);

        // deactivate hand visualization if feature a is chosen
        if (GameSettings.studyFeature == Feature.A)
        {
            leftHand.SetActive(false);
            rightHand.SetActive(false);
        }

        ToggleTimeScale(false);
    }

    private void OnDestroy()
    {
        toggleReference.action.started -= ToggleMenu;
    } 

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        bool isActive = !menuCanvas.activeSelf;
        menuCanvas.SetActive(isActive);

        ToggleTimeScale(isActive);
    }

    public static void ToggleTimeScale(bool isStopping)
    {
        if (isStopping)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }


    public void ToggleHandSelection(float value)
    {
        GameSettings.isLeftHanded = value == 0;
    }

    public static void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void SwitchToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
