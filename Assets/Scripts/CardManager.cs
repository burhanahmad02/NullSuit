using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public int rows = 2;
    public int columns = 3;
    public GameObject cardPrefab;
    public Transform cardParent; 
    public Sprite[] cardFrontSprites;
    private List<Card> flippedCards = new List<Card>();
    private List<Sprite> deck = new List<Sprite>();

    void Start()
    {
        LoadSprites();

        GenerateDeck();
        GenerateGrid();
        AdjustGridCellSize();
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

        // Define fixed spacing values (or dynamically compute later)
        float spacingX = -10f;
        float spacingY = 10f;

        // Calculate total spacing
        float totalSpacingX = spacingX * (columns - 1);
        float totalSpacingY = spacingY * (rows - 1);

        // Calculate available size for cells
        float cellWidth = 150f;
        float cellHeight = 185f;

        // Assign to grid
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.spacing = new Vector2(spacingX, spacingY);
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

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (flippedCards[0].frontSprite == flippedCards[1].frontSprite)
        {
            Debug.Log("Match Found: " + flippedCards[0].frontSprite.name);
            flippedCards[0].isMatched = true;
            flippedCards[1].isMatched = true;
        }
        else
        {
            flippedCards[0].Unflip();
            flippedCards[1].Unflip();

        }

        flippedCards.Clear();
    }
}
