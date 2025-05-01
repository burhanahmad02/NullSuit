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
        GenerateDeck();
        GenerateGrid();
    }

    void GenerateDeck()
    {
        
        int totalCards = rows * columns;
        deck.Clear();

        for (int i = 0; i < totalCards / 2; i++)
        {
            if (i < cardFrontSprites.Length)
            {
                deck.Add(cardFrontSprites[i]);
                deck.Add(cardFrontSprites[i]);
            }
        }

        
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
            card.image = cardObj.GetComponent<Image>();
            Debug.Log("Card Name: " + card.frontSprite.name);
        }
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
            flippedCards[0].Flip();
            flippedCards[1].Flip();
        }

        flippedCards.Clear();
    }
}
