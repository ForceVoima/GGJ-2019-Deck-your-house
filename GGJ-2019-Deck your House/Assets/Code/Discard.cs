using UnityEngine;
using System.Collections;

public class Discard : CardHolder
{
    public float cardThickness = 0.05f;

    public int cardsToDeal = 0;
    public Hand dealToHand;

    public void Organize()
    {
        Vector3 position;

        for (int i = 0; i < cards.Count; i++)
        {
            position = cards[i].transform.position;
            position.y += i * cardThickness;
            cards[i].transform.position = position;
            cards[i].transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        }

        cards.Reverse();
    }

    public Vector3 Position
    {
        get
        {
            Vector3 pos = transform.position;
            pos.y += (cards.Count + 1) * cardThickness;
            return pos;
        }
    }

    public Quaternion Rotation
    {
        get
        {
            float randomAngle = Random.Range(-30f, 30f);
            return Quaternion.Euler(270f, 90f + randomAngle, 0f);
        }
    }

    public void DealCards(int amount, Hand hand)
    {
        cardsToDeal = amount;
        dealToHand = hand;

        StartCoroutine(DealCards());
    }

    IEnumerator DealCards()
    {
        while (cardsToDeal > 0)
        {
            cards[cards.Count - 1].PutIn(dealToHand);
            cardsToDeal--;

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        dealToHand.Organize();
    }
}
