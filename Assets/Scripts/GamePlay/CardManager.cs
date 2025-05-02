using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static GameData;
using static GameManager;
using UnityEngine.SceneManagement;


public class CardManager : MonoBehaviour
{
    public int rows = 2;
    public int columns = 3;

    public TextMeshProUGUI matchText;
    public TextMeshProUGUI turnText;

    private int matchesMade = 0;
    private int turnsTaken = 0;
    public Slider progressBar; // Or use Image if you're using a filled image instead

    public GameObject cardPrefab;
    public Transform cardParent;
    public Sprite[] cardFrontSprites;
    private List<Card> flippedCards = new List<Card>();
    private List<Sprite> deck = new List<Sprite>();


    string saveFilePath => Path.Combine(Application.persistentDataPath, "save.json");
    public Slider healthSlider;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    private Difficulty difficulty;
    public TextMeshProUGUI difficultyText;

    private int maxHealth;
    private int currentHealth;


    //sfx
    // Declare the audio clips
    public AudioClip cardFlipSFX;
    public AudioClip matchSFX1;
    public AudioClip matchSFX2;
    public AudioClip gameOverSFX1;
    public AudioClip gameOverSFX2;
    public AudioClip gameWinSFX;

    // AudioManager reference (singleton)
    private AudioManager audioManager;

    void Start()
    {
        this.rows = GameManager.rows;
        this.columns = GameManager.columns;
        this.difficulty = GameManager.difficulty;

        // Update difficulty text on UI
        UpdateDifficultyText();

        LoadSprites();

        if (GameManager.loadSavedGame && LoadGame())
            return;

        SetHealthByDifficulty();

        GenerateDeck();
        GenerateGrid();
        AdjustGridCellSize();
        InitializeProgressBar();

        StartCoroutine(ShowAllCardsTemporarily());
        audioManager = AudioManager.Instance;
    }

    void UpdateDifficultyText()
    {
        // Check difficulty and update text
        switch (difficulty)
        {
            case Difficulty.Easy:
                difficultyText.text = "Difficulty: Easy";
                break;
            case Difficulty.Medium:
                difficultyText.text = "Difficulty: Medium";
                break;
            case Difficulty.Hard:
                difficultyText.text = "Difficulty: Hard";
                break;
        }
    }

    void SetHealthByDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                maxHealth = 30;
                break;
            case Difficulty.Medium:
                maxHealth = 15;
                break;
            case Difficulty.Hard:
                maxHealth = 7;
                break;
        }

        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }


    IEnumerator ShowAllCardsTemporarily()
    {
        // Flip all cards forward immediately (show front image)
        foreach (Transform cardTransform in cardParent)
        {
            Card card = cardTransform.GetComponent<Card>();
            if (!card.isMatched)
            {
                card.FlipImmediate(); // Shows front instantly
            }
        }

        yield return new WaitForSeconds(2f);

        // Flip them back with animation
        foreach (Transform cardTransform in cardParent)
        {
            Card card = cardTransform.GetComponent<Card>();
            if (!card.isMatched)
            {
                card.Unflip(); // Uses animated flip
            }
        }
    }



    void GenerateDeck()
    {
        int totalCards = rows * columns;
        int totalPairs = totalCards / 2;

        deck.Clear();

        if (cardFrontSprites.Length < totalPairs)
        {
            Debug.LogError("Not enough unique sprites to generate the required number of pairs.");
            return;
        }

        // Add each pair twice
        for (int i = 0; i < totalPairs; i++)
        {
            deck.Add(cardFrontSprites[i]);
            deck.Add(cardFrontSprites[i]);
        }

        // Shuffle the deck
        for (int i = 0; i < deck.Count; i++)
        {
            Sprite temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void GenerateGrid()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            Card card = cardObj.GetComponent<Card>();
            card.frontSprite = deck[i];
            card.image = cardObj.GetComponentInChildren<Image>();
            card.image.preserveAspect = true;
            card.image.SetNativeSize();
            Debug.Log("Card Name: " + card.frontSprite.name);
            // Assign Flip to Button onClick
            Button button = cardObj.GetComponent<Button>();
            button.onClick.AddListener(card.Flip);
        }
    }
    void AdjustGridCellSize()
    {
        GridLayoutGroup grid = cardParent.GetComponent<GridLayoutGroup>();
        RectTransform rect = cardParent.GetComponent<RectTransform>();

        float parentWidth = rect.rect.width;
        float parentHeight = rect.rect.height;

        // Define the maximum number of rows and columns
        int maxRows = 10;
        int maxColumns = 11;

        // Clamp the number of rows and columns to the maximum values
        rows = Mathf.Clamp(rows, 1, maxRows);
        columns = Mathf.Clamp(columns, 1, maxColumns);

        // Calculate total spacing
        float totalSpacingX = 10f * (columns - 1);  // Adjust horizontal spacing as needed
        float totalSpacingY = 10f * (rows - 1);     // Adjust vertical spacing as needed

        // Calculate available width and height for cells after accounting for spacing
        float availableWidth = parentWidth - totalSpacingX;
        float availableHeight = parentHeight - totalSpacingY;

        // Calculate cell size based on available width and height
        float cellWidth = availableWidth / columns;
        float cellHeight = availableHeight / rows;

        // Assign calculated values to grid
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.spacing = new Vector2(10f, 10f);  // Adjust spacing if necessary
    }

    void LoadSprites()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites/Cards");

        // Filter out "cardBack" sprite
        List<Sprite> frontSprites = new List<Sprite>();
        foreach (Sprite sprite in allSprites)
        {
            if (sprite.name.ToLower() != "cardBack")  // case-insensitive
            {
                frontSprites.Add(sprite);
            }
        }

        cardFrontSprites = frontSprites.ToArray();
    }



    public void OnCardFlipped(Card card)
    {
        if (flippedCards.Contains(card)) return;
        flippedCards.Add(card);

        // Play flip sound
        audioManager.PlaySFX(cardFlipSFX);

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }


    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        turnsTaken++;

        if (flippedCards[0].frontSprite == flippedCards[1].frontSprite)
        {
            flippedCards[0].isMatched = true;
            flippedCards[1].isMatched = true;
            matchesMade++;
            progressBar.value = matchesMade;

            // Randomly choose one of the two match sounds
            AudioClip selectedMatchSound = Random.Range(0, 2) == 0 ? matchSFX1 : matchSFX2;

            // Play the selected match sound
            audioManager.PlaySFX(selectedMatchSound);

            if (matchesMade == (rows * columns) / 2)
            {
                GameWin();
                yield break;
            }
        }
        else
        {
            currentHealth--;
            healthSlider.value = currentHealth;
            if (currentHealth <= 0)
            {
                GameOver();
                yield break;
            }

            flippedCards[0].Unflip();
            flippedCards[1].Unflip();
        }

        flippedCards.Clear();
        UpdateUI();
    }
    void GameOver()
    {
        // Randomly choose one of the two game over sounds
        AudioClip selectedGameOverSound = Random.Range(0, 2) == 0 ? gameOverSFX1 : gameOverSFX2;

        // Play the selected game over sound
        audioManager.PlaySFX(selectedGameOverSound);

        // Blur background music by reducing the volume
        StartCoroutine(audioManager.FadeOutMusic(2f));

        Debug.Log("Game Over!");
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    void GameWin()
    {
        // Play win sound
        audioManager.PlaySFX(gameWinSFX);

        // Blur background music by reducing the volume
        StartCoroutine(audioManager.FadeOutMusic(2f));

        Debug.Log("You Win!");
        gameWinPanel.SetActive(true);
        Time.timeScale = 0;
    }

    //UI code 
    void UpdateUI()
    {
        matchText.text = "Matches: " + matchesMade;
        turnText.text = "Turns: " + turnsTaken;
    }
    void InitializeProgressBar()
    {
        int totalMatches = (rows * columns) / 2;
        progressBar.maxValue = totalMatches;
        progressBar.value = 0;
    }
    // load and save mechanism
    string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"Save_{rows}x{columns}.json");
    }

    public void SaveGame()
    {
        GameData data = new GameData
        {
            rows = this.rows,
            columns = this.columns,
            turnsTaken = this.turnsTaken,
            matchesMade = this.matchesMade,
            cards = new List<GameData.CardData>()
        };

        foreach (Transform cardTransform in cardParent)
        {
            Card card = cardTransform.GetComponent<Card>();
            data.cards.Add(new GameData.CardData
            {
                spriteName = card.frontSprite.name,
                isMatched = card.isMatched,
                isFlipped = card.isFlipped
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("Game Saved");
    }

    public bool LoadGame()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            Debug.LogWarning("No saved game found at: " + path);
            return false;
        }

        string json = File.ReadAllText(path);
        GameData data = JsonUtility.FromJson<GameData>(json);

        this.rows = data.rows;
        this.columns = data.columns;
        this.turnsTaken = data.turnsTaken;
        this.matchesMade = data.matchesMade;

        LoadSprites(); // Ensure sprites are loaded before assignment
        deck.Clear();

        foreach (var cardData in data.cards)
        {
            Sprite sprite = System.Array.Find(cardFrontSprites, s => s.name == cardData.spriteName);
            if (sprite != null)
            {
                deck.Add(sprite);
            }
            else
            {
                Debug.LogWarning("Sprite not found: " + cardData.spriteName);
            }
        }

        GenerateGrid();
        AdjustGridCellSize();
        InitializeProgressBar();

        for (int i = 0; i < data.cards.Count; i++)
        {
            CardData cardData = data.cards[i];
            Debug.Log($"Card {i}: spriteName={cardData.spriteName}, isFlipped={cardData.isFlipped}, isMatched={cardData.isMatched}");

            Card card = cardParent.GetChild(i).GetComponent<Card>();

            if (cardData.isMatched)
            {
                card.frontSprite = System.Array.Find(cardFrontSprites, s => s.name == cardData.spriteName);
                card.FlipImmediate(); // <-- This shows the front immediately
                Debug.Log($"Assigned sprite '{cardData.spriteName}' to matched card at index {i}.");
            }

            card.isMatched = cardData.isMatched;

            // Reset flip state
            if (cardData.isFlipped)
            {
                cardData.isFlipped = false;
            }
        }

        UpdateUI();
        progressBar.value = matchesMade;
        return true;
    }


    public void ResetGame()
    {
        SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}