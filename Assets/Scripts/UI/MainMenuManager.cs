using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public Button[] levelButtons;
    public CanvasGroup mainMenuCanvas;
    public AudioClip clickSound;

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
        int rows = PlayerPrefs.GetInt("SelectedRows");
        int cols = PlayerPrefs.GetInt("SelectedColumns");

        GameManager.rows = rows;
        GameManager.columns = cols;
        GameManager.loadSavedGame = false;

        StartCoroutine(FadeAndLoadScene("GamePlay"));
    }

    public void LoadSavedGame()
    {
        int rows = PlayerPrefs.GetInt("SelectedRows");
        int cols = PlayerPrefs.GetInt("SelectedColumns");

        GameManager.rows = rows;
        GameManager.columns = cols;
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
