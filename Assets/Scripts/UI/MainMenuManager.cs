using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip clickSound;
    void PlayClickSound()
    {
        AudioManager.Instance.PlaySFX(clickSound);
    }
    // Modular level selection
    public void LoadLevel(int levelNumber)
    {
        int rows = 0;
        int columns = 0;

        switch (levelNumber)
        {
            case 1:
                rows = 2;
                columns = 3;
                break;
            case 2:
                rows = 4;
                columns = 5;
                break;
            case 3:
                rows = 10;
                columns = 11;
                break;
            default:
                Debug.LogWarning("Invalid level number selected.");
                return;
        }

        SelectLevel(rows, columns);

        // Optionally show buttons like Start New Game or Load Game here
        Debug.Log($"Level {levelNumber} selected with {rows}x{columns}");
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

        SceneManager.LoadScene("GamePlay");
    }

    public void LoadSavedGame()
    {
        int rows = PlayerPrefs.GetInt("SelectedRows");
        int cols = PlayerPrefs.GetInt("SelectedColumns");

        GameManager.rows = rows;
        GameManager.columns = cols;
        GameManager.loadSavedGame = true;

        SceneManager.LoadScene("GamePlay");
    }
}
