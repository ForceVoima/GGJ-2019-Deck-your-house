﻿using UnityEngine;
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

    public void DealCards(Hand hand)
    {
        cardsToDeal = cards.Count;
        dealToHand = hand;

        StartCoroutine(DealCards());
    }

    IEnumerator DealCards()
    {
        while (cardsToDeal > 0)
        {
            cards[cards.Count - 1].ShowAllRatings(GameManager.Instance.WhoseTurn);
            cards[cards.Count - 1].PutIn(dealToHand);
            cardsToDeal--;

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        dealToHand.Organize();
    }

    public void UpdateAllRatings(GameManager.TurnPhase turnPhase)
    {
        foreach (Card card in cards)
        {
            card.ShowAllRatings(turnPhase);
        }
    }
}
