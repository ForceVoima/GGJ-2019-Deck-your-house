using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Player1,
        Player2,
        CommonRoom
    }

    [SerializeField]
    private RoomType type;

    public RoomType Type
    {
        get { return type; }
    }

    public int freeSlots = 0;
    public int fullSlots = 0;
    public int totalSlots = 0;

    public List<CardSlot> allSlots;

    public void Start()
    {
        allSlots = new List<CardSlot>();

        CardSlot[] slots = GetComponentsInChildren<CardSlot>();

        totalSlots = slots.Length;
        freeSlots = totalSlots;
        fullSlots = 0;

        foreach (CardSlot slot in slots)
        {
            slot.SetParentRoom(this);
            allSlots.Add(slot);
        }
    }

    public void AlignAllCards(GameManager.TurnPhase turnPhase)
    {
        foreach (CardSlot slot in allSlots)
        {
            slot.AlignCards(turnPhase);
        }
    }
}
