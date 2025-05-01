using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Button pauseButton;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button backFromSettingsButton;

    public CanvasGroup pauseMenuGroup;
    public CanvasGroup settingsGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        backFromSettingsButton.onClick.AddListener(CloseSettings);

        pauseMenuGroup.alpha = 0;
        pauseMenuGroup.interactable = false;
        pauseMenuGroup.blocksRaycasts = false;

        settingsGroup.alpha = 0;
        settingsGroup.interactable = false;
        settingsGroup.blocksRaycasts = false;
    }

    // Called when the Pause button is pressed
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Pause the game
        StartCoroutine(FadeIn(pauseMenuGroup, () =>
        {
            // Any additional actions after fade-in (optional)
        }));
    }

    // Called when the Resume button is pressed
    public void ResumeGame()
    {
        isPaused = false;
        StartCoroutine(FadeOut(pauseMenuGroup, () =>
        {
            Time.timeScale = 1f;  // Resume the game
        }));
    }

    // Called when the Settings button is pressed
    public void OpenSettings()
    {
        // Fade out the pause menu without resuming the game
        StartCoroutine(FadeOut(pauseMenuGroup, () =>
        {
            // Settings panel will be faded in separately
            StartCoroutine(FadeIn(settingsGroup, () =>
            {
                // Any additional actions after settings fade-in (optional)
            }));
        }));
    }

    // Called when the Back button in the settings panel is pressed
    public void CloseSettings()
    {
        // Fade out the settings panel and return to the pause menu
        StartCoroutine(FadeOut(settingsGroup, () =>
        {
            // After settings fade out, fade in the pause menu
            StartCoroutine(FadeIn(pauseMenuGroup, () =>
            {
                // Any additional actions after fade-in (optional)
            }));
        }));
    }

    // Called when the Main Menu button is pressed
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;  // Unpause game before loading the new scene
        StartCoroutine(FadeOut(pauseMenuGroup, () =>
        {
            FindObjectOfType<CardManager>().SaveGame();
            SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
        }));
    }

    // Fade-in method to activate and show the CanvasGroup
    IEnumerator FadeIn(CanvasGroup group, System.Action onComplete = null)
    {
        group.gameObject.SetActive(true);  // Activate the panel before starting fade-in
        group.interactable = true;
        group.blocksRaycasts = true;

        float time = 0f;
        while (time < fadeDuration)
        {
            group.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        group.alpha = 1f;

        onComplete?.Invoke();  // Callback once fade-in is done
    }

    // Fade-out method to deactivate and hide the CanvasGroup
    IEnumerator FadeOut(CanvasGroup group, System.Action onComplete = null)
    {
        group.interactable = false;
        group.blocksRaycasts = false;

        float time = 0f;
        while (time < fadeDuration)
        {
            group.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        group.alpha = 0f;

        group.gameObject.SetActive(false);  // Deactivate the panel after fade-out
        onComplete?.Invoke();  // Callback once fade-out is done
    }
}
