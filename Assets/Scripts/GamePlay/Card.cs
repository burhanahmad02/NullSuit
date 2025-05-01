using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Sprite frontSprite;
    public Sprite backSprite;
    public Image image;
    public bool isFlipped = false;
    public bool isMatched = false;

    private float flipSpeed = 5f;
    private void Start()
    {
        image = GetComponentInChildren<Image>();

        // Only set backSprite if card is not flipped or matched
        if (!isFlipped && !isMatched)
        {
            image.sprite = backSprite;
        }
        else
        {
            image.sprite = frontSprite;
        }
    }

    public void Flip()
    {
        if (isMatched || isFlipped) return; // Don't allow flipping back via user input

        StartCoroutine(FlipAnimation());
    }
    public void Unflip()
    {
        if (isFlipped && !isMatched)
        {
            StartCoroutine(FlipBackAnimation());
        }
    }


    IEnumerator FlipAnimation()
    {
        float duration = 0.25f;
        float time = 0;

        // First half: shrink to zero width
        while (time < duration)
        {
            float scale = Mathf.Lerp(1, 0, time / duration);
            transform.localScale = new Vector3(scale, 1, 1);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(0, 1, 1); // Ensure final scale is exact

        isFlipped = true;
        image.sprite = frontSprite;

        time = 0;

        // Second half: expand back to full width
        while (time < duration)
        {
            float scale = Mathf.Lerp(0, 1, time / duration);
            transform.localScale = new Vector3(scale, 1, 1);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1); // Ensure final scale is exact

        FindObjectOfType<CardManager>().OnCardFlipped(this);
    }


    IEnumerator FlipBackAnimation()
    {
        float duration = 0.25f;
        float time = 0;

        while (time < duration)
        {
            float scale = Mathf.Lerp(1, 0, time / duration);
            transform.localScale = new Vector3(scale, 1, 1);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(0, 1, 1); // Ensure final scale is exact

        isFlipped = false;
        image.sprite = backSprite;

        time = 0;
        while (time < duration)
        {
            float scale = Mathf.Lerp(0, 1, time / duration);
            transform.localScale = new Vector3(scale, 1, 1);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1); // Ensure final scale is exact
    }
    public void FlipImmediate()
    {
        isFlipped = true;
        image.sprite = frontSprite;
        transform.localScale = new Vector3(1, 1, 1);
    }




}
// load and save mechanism
[System.Serializable]
public class GameData
{
    public int rows;
    public int columns;
    public int turnsTaken;
    public int matchesMade;
    public List<CardData> cards;

    [System.Serializable]
    public class CardData
    {
        public string spriteName;
        public bool isMatched;
        public bool isFlipped;
    }
}
