using UnityEngine;
using System.Collections;

public class Deck : CardHolder
{
    public float cardThickness = 0.05f;
    public int cardsToDeal = 0;

    public Hand dealToHand;

    public bool spawnDeck = false;

    public override void Initialize()
    {
        base.Initialize();

        if (spawnDeck)
            SpawnDeck();
    }

    private void SpawnDeck()
    {
        GameObject[] cardPool = Resources.LoadAll<GameObject>("Cards");
        Card temp;

        int index = 0;
        int spawnedCards = 1;

        while (spawnedCards <= 20)
        {
            temp = SpawnCard(cardPool[index], spawnedCards);
            cards.Add(temp);
            temp.TakeOver(Card.CardStatus.Deck, this);

            spawnedCards++;
            index++;

            if (cardPool.Length == index)
            {
                index = 0;
            }
        }
    }

    private Card SpawnCard(GameObject prefab, int number)
    {
        GameObject go = Instantiate(prefab, transform.position, transform.rotation, transform);
        go.name = number + " " + prefab.name;

        Card card = go.GetComponent<Card>();
        return card;
    }

    public void Organize()
    {
        Vector3 position;

        for (int i = 0; i < cards.Count; i++)
        {
            position = cards[i].transform.position;
            position.y += i * cardThickness;
            cards[i].transform.position = position;
            cards[i].transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.right);
        }

        cards.Reverse();
    }

    public void Shuffle()
    {
        // Fancy Fisher-Yates shuffle:
        Card[] array = cards.ToArray();
        Card temp;
        int j;

        for (int i = 0; i < array.Length; i++)
        {
            j = Random.Range(0, array.Length - 1);

            temp = array[j];
            array[j] = array[i];
            array[i] = temp;
        }

        Vector3 position;

        for (int i = 0; i < array.Length; i++)
        {
            position = cards[i].transform.position;
            position.y += i * cardThickness;
            cards[i].transform.position = position;
            cards[i].transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.right);
        }

        cards.Clear();

        for (int i = 0; i < array.Length; i++)
        {
            cards.Add(array[i]);
        }
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
            return Quaternion.Euler(270f, 270f + randomAngle, 0f);
        }
    }

    public void DealCards(int amount, Hand hand, GameManager.TurnPhase turnPhase)
    {
        cardsToDeal = amount;
        dealToHand = hand;

        StartCoroutine(DealCards(turnPhase));
    }

    IEnumerator DealCards(GameManager.TurnPhase turnPhase)
    {
        // Debug.Log("Deck " + name + " dealing cards when " + turnPhase);

        while (cardsToDeal > 0)
        {
            cards[cards.Count - 1].ShowPlayerRating(turnPhase);
            cards[cards.Count - 1].PutIn(dealToHand);
            cardsToDeal--;

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        dealToHand.Organize();
    }
}
