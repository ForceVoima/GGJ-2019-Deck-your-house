using UnityEngine;
using System.Collections;

public class Deck : CardHolder
{
    public float cardThickness = 0.05f;

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

    public Card[] GetWholeDeck()
    {
        return cards.ToArray();
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
            return Quaternion.Euler(270f, 180f + randomAngle, 0f);
        }
    }
}
