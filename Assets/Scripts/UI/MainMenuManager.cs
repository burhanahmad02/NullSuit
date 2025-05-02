using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using static GameManager;

public class MainMenuManager : MonoBehaviour
{
    public Button[] levelButtons;
    public CanvasGroup mainMenuCanvas;
    public AudioClip clickSound;

    private Difficulty selectedDifficulty = Difficulty.None;
    public Button easyButton, mediumButton, hardButton;
    public TextMeshProUGUI difficultyWarningText; // or TMP_Text if using TextMeshPro

    void Start()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true); // include inactive
        foreach (Button btn in allButtons)
        {
            if (btn.GetComponent<ButtonSFX>() == null)
            {
                var sfx = btn.gameObject.AddComponent<ButtonSFX>();
                sfx.clickSound = clickSound; // assign shared sound
            }
        }
    }

    void PlayClickSound()
    {
        AudioManager.Instance.PlaySFX(clickSound);
    }
    public void SelectDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy": selectedDifficulty = Difficulty.Easy; break;
            case "medium": selectedDifficulty = Difficulty.Medium; break;
            case "hard": selectedDifficulty = Difficulty.Hard; break;
            default: selectedDifficulty = Difficulty.None; break;
        }

        PlayerPrefs.SetString("SelectedDifficulty", selectedDifficulty.ToString());
        Debug.Log("Selected difficulty: " + selectedDifficulty);
    }
    public void ShowDifficultyWarning(string message)
    {
        StopAllCoroutines(); // prevent overlapping fades
        StartCoroutine(FadeWarning(message));
    }

    IEnumerator FadeWarning(string message)
    {
        difficultyWarningText.text = message;
        difficultyWarningText.DOFade(1f, 0.5f); // fade in
        yield return new WaitForSeconds(2f);
        difficultyWarningText.DOFade(0f, 0.5f); // fade out
    }

    public void UpdateDifficultyButtons()
    {
        Color selectedColor = Color.green;
        Color normalColor = easyButton.GetComponent<Image>().color;

        easyButton.GetComponent<Image>().color = selectedDifficulty == Difficulty.Easy ? selectedColor : normalColor;
        mediumButton.GetComponent<Image>().color = selectedDifficulty == Difficulty.Medium ? selectedColor : normalColor;
        hardButton.GetComponent<Image>().color = selectedDifficulty == Difficulty.Hard ? selectedColor : normalColor;
    }

    public void LoadLevel(int levelNumber)
    {
        int rows = 0;
        int columns = 0;

        switch (levelNumber)
        {
            case 1: rows = 2; columns = 3; break;
            case 2: rows = 3; columns = 4; break;
            case 3: rows = 4; columns = 5; break;
            case 4: rows = 4; columns = 5; break;
            case 5: rows = 5; columns = 6; break;
            case 6: rows = 6; columns = 7; break;
            case 7: rows = 7; columns = 8; break;
            case 8: rows = 9; columns = 10; break;
            case 9: rows = 10; columns = 11; break;
            default:
                Debug.LogWarning("Invalid level number selected.");
                return;
        }

        SelectLevel(rows, columns);
        Debug.Log($"Level {levelNumber} selected with {rows}x{columns}");

        foreach (Button button in levelButtons)
        {
            button.transform.DOScale(1.1f, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
        }
    }

    public void SelectLevel(int rows, int columns)
    {
        PlayerPrefs.SetInt("SelectedRows", rows);
        PlayerPrefs.SetInt("SelectedColumns", columns);
    }

    public void StartNewGame()
    {
        if (selectedDifficulty == Difficulty.None)
        {
            ShowDifficultyWarning("Please select a difficulty before starting a new game.");
            Debug.LogWarning("Please select a difficulty before starting a new game.");
            return;
        }

        int rows = PlayerPrefs.GetInt("SelectedRows");
        int cols = PlayerPrefs.GetInt("SelectedColumns");

        GameManager.rows = rows;
        GameManager.columns = cols;
        GameManager.difficulty = selectedDifficulty;
        GameManager.loadSavedGame = false;

        // Save an empty state for this difficulty to allow loading later
        PlayerPrefs.SetInt($"HasSave_{selectedDifficulty}", 1);

        StartCoroutine(FadeAndLoadScene("GamePlay"));
    }


    public void LoadSavedGame()
    {
        if (selectedDifficulty == Difficulty.None)
        {
            ShowDifficultyWarning("Please select a difficulty before loading a game.");
            Debug.LogWarning("Please select a difficulty before loading a game.");
            return;
        }

        if (!PlayerPrefs.HasKey($"HasSave_{selectedDifficulty}"))
        {
            ShowDifficultyWarning("No saved game found for selected difficulty.");

            Debug.LogWarning("No saved game found for selected difficulty.");
            return;
        }

        int rows = PlayerPrefs.GetInt("SelectedRows");
        int cols = PlayerPrefs.GetInt("SelectedColumns");

        GameManager.rows = rows;
        GameManager.columns = cols;
        GameManager.difficulty = selectedDifficulty;
        GameManager.loadSavedGame = true;

        StartCoroutine(FadeAndLoadScene("GamePlay"));
    }


    IEnumerator FadeAndLoadScene(string sceneName)
    {
        mainMenuCanvas.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    public void OnButtonHover(Button btn)
    {
        btn.transform.DOScale(1.05f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnButtonExit(Button btn)
    {
        btn.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack);
    }
}
