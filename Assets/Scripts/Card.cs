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

    private float flipSpeed = 5f;

    public void Flip()
    {
        if (isMatched || isFlipped) return;
        StartCoroutine(FlipAnimation());
    }

    IEnumerator FlipAnimation()
    {
        float time = 0;
        while (time < 0.5f)
        {
            transform.localScale = new Vector3(Mathf.Lerp(1, 0, time * 2), 1, 1);
            time += Time.deltaTime * flipSpeed;
            yield return null;
        }

        isFlipped = !isFlipped;
        image.sprite = isFlipped ? frontSprite : backSprite;

        time = 0;
        while (time < 0.5f)
        {
            transform.localScale = new Vector3(Mathf.Lerp(0, 1, time * 2), 1, 1);
            time += Time.deltaTime * flipSpeed;
            yield return null;
        }

        FindObjectOfType<CardManager>().OnCardFlipped(this);
    }
}
