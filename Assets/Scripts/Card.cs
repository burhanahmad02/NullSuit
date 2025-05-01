using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Sprite frontSprite;
    public Sprite backSprite;
    public Image image;

    public bool isFlipped = false;
    public bool isMatched = false;


    public void Flip()
    {

        // Toggle the flipped state
        isFlipped = !isFlipped;

        image.sprite = isFlipped ? frontSprite : backSprite;

        if (isFlipped)
        {
            Debug.Log("Card is on the front.");
        }
        else
        {
            Debug.Log("Card is on the back.");
        }
    }
}
