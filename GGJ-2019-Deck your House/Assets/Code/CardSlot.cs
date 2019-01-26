using UnityEngine;
using System.Collections;

public class CardSlot : MonoBehaviour, IHolder
{
    public bool filled = false;

    public bool Free { get { return !filled; } }

    public Card currentCard = null;

    public Vector3 Position
    {
        get { return transform.position; }
    }

    // Room parent;

    public void Enter(Card card)
    {
        filled = true;
        currentCard = card;
    }

    public void Exit(Card card)
    {
        filled = false;
        currentCard = null;
    }
}
