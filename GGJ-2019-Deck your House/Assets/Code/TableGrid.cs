using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableGrid : CardHolder, IHolder
{
    public int height = 4;
    public int width = 5;

    public float horizontalSpacing = 5f;
    public float verticalSpacing = 7f;

    public void Reset(Card[] deck)
    {
        if (!initialized)
            Initialize();

        for (int i = deck.Length - 1; i >= 0; i--)
        {
            deck[i].ResetRatingTexts();
            deck[i].PutIn(this);
        }

        StartCoroutine( DealGrid(deck) );
    }

    private void SetupLayout()
    {
        float yPos = -(height - 1) * verticalSpacing / 2f;
        Vector3 position = new Vector3(0f, 0f, yPos);
        int cardNumber = 0;

        for (int y = 1; y <= height; y++)
        {
            position.x = -(width - 1) * horizontalSpacing / 2f;

            for (int x = 1; x <= width; x++)
            {
                cards[cardNumber].transform.position = position;

                position.x += horizontalSpacing;
                cardNumber++;
            }

            position.z += verticalSpacing;
        }
    }

    IEnumerator DealGrid(Card[] deck)
    {
        Quaternion correntRotation = transform.rotation;

        Vector3 right = transform.right;
        Vector3 up = transform.up;

        Vector3 position = new Vector3();
        int cardNumber = 0;

        for (int y = 1; y <= height; y++)
        {
            for (int x = 1; x <= width; x++)
            {
                position = transform.position;
                position += ((1f * x - 0.5f) - (width / 2f)) * right * horizontalSpacing;
                position += ((1f * y - 0.5f) - (height / 2f)) * up * verticalSpacing;

                // cards[cardNumber].transform.position = position;
                deck[cardNumber].InitMove(position, correntRotation, 0.8f);

                cardNumber++;

                yield return new WaitForSeconds(0.1f);
            }

            position.z += verticalSpacing;
        }

        yield return null;
    }
}
