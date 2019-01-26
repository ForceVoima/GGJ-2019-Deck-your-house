using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Camera sceneCamera;

    public enum ClickedItem
    {
        Card,
        CardSlot,
        DiscardPile,
        None
    }

    public ClickedItem clickedItem;

    public Card selectedCard;
    public CardSlot clickedCardSlot;

    public Vector3 clickedPos;

    void Start()
    {
        if (sceneCamera == null)
        {
            sceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(button: 0))
        {
            clickedItem = WhatWasClicked();

            if (clickedItem == ClickedItem.Card)
            {
                GameManager.Instance.CardSelected(selectedCard);
            }

            if (clickedItem == ClickedItem.CardSlot)
            {
                if (clickedCardSlot.Free &&
                    selectedCard != null)
                {
                    selectedCard.PutIn(clickedCardSlot);
                    selectedCard = null;
                }
            }

            if (clickedItem == ClickedItem.None)
            {
                GameManager.Instance.NoneSelected();
            }
        }
    }

    private ClickedItem WhatWasClicked()
    {
        RaycastHit hit;
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            int layer = objectHit.gameObject.layer;

            if (layer == 8)
            {
                if (CardClicked(objectHit) )
                    return ClickedItem.Card;
                else
                    return ClickedItem.None;
            }

            if (layer == 9)
            {
                if (CardSlotClicked(objectHit))
                    return ClickedItem.CardSlot;
                else
                    return ClickedItem.None;
            }
        }

        return ClickedItem.None;
    }

    private bool CardClicked(Transform objectHit)
    {
        Card card = objectHit.GetComponentInParent<Card>();

        if (card != null && card.Selectable)
        {
            if (selectedCard != null)
                selectedCard.Deselect();

            selectedCard = card;
            return true;
        }
        else
            return false;
    }

    private bool CardSlotClicked(Transform objectHit)
    {
        CardSlot slot = objectHit.GetComponentInParent<CardSlot>();

        if (slot != null)
        {
            clickedCardSlot = slot;
            return true;
        }
        else
            return false;
    }
}
