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

    private Room parent;

    public Room ParentRoom
    {
        get { return parent; }
    }

    public void SetParentRoom(Room room)
    {
        parent = room;
    }

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

    public Transform GetTransform()
    {
        return transform;
    }

    public void AlignCards(GameManager.TurnPhase turnPhase)
    {
        if (currentCard != null)
        {
            if (turnPhase == GameManager.TurnPhase.Player1)
                currentCard.InitMove(Position, Quaternion.LookRotation(Vector3.up, Vector3.forward));
            else if (turnPhase == GameManager.TurnPhase.Player2)
                currentCard.InitMove(Position, Quaternion.LookRotation(Vector3.up, Vector3.back));
        }
    }

    public void AllRatings(GameManager.TurnPhase turnPhase)
    {
        if (currentCard != null)
        {
            currentCard.ShowAllRatings(turnPhase);
        }
    }
}
