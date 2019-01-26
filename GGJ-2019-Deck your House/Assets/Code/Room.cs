using UnityEngine;
using System.Collections;

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

    public void Start()
    {
        CardSlot[] slots = GetComponentsInChildren<CardSlot>();

        totalSlots = slots.Length;
        freeSlots = totalSlots;
        fullSlots = 0;

        foreach (CardSlot slot in slots)
        {
            slot.SetParentRoom(this);
        }
    }
}
