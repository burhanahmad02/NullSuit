using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    private List<Card> flippedCards = new List<Card>();

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
            // Log the match
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
