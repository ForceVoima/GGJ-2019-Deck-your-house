using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardHolder : MonoBehaviour, IHolder
{
    public List<Card> cards;

    public virtual void Start()
    {
        cards = new List<Card>();

        Card[] cardsArray = GetComponentsInChildren<Card>();

        foreach (Card card in cardsArray)
        {
            cards.Add(card);
            card.TakeOver(Card.CardStatus.PlayerHand, this);
        }
    }

    public virtual void Enter(Card card)
    {
        cards.Add(card);
    }

    public virtual void Exit(Card card)
    {
        cards.Remove(card);
    }

    public virtual Transform GetTransform()
    {
        return transform;
    }
}
