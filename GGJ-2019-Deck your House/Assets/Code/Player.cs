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
    public Discard clickedDiscardPile;

    public Vector3 clickedPos;

    void Start()
    {
        if (sceneCamera == null)
        {
            sceneCamera = GameObject.Find("Camera mount/Main Camera").GetComponent<Camera>();

            if (sceneCamera == null)
            {
                sceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            }
        }
    }
    
    void Update()
    {
        #if UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(button: 0))
            HandleInputs();
        /*
        Tested drag'n'drop:

        else if (Input.GetMouseButtonUp(button: 0))
            HandleInputs();
        */

        #endif

        #if UNITY_ANDROID

        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    HandleInputs();
                }
                /*
                Tested drag'n'drop:

                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    HandleInputs();
                }
                */
            }
        }

        #endif
    }

    public void HandleInputs()
    {
        clickedItem = WhatWasClicked();

        if (clickedItem == ClickedItem.Card)
        {
            GameManager.Instance.CardSelected(selectedCard);
        }

        else if (clickedItem == ClickedItem.CardSlot)
        {
            GameManager.Instance.CardSlotSelected(clickedCardSlot);
        }

        else if (clickedItem == ClickedItem.DiscardPile)
        {
            GameManager.Instance.DiscardPileSelected(clickedDiscardPile);
        }

        else if (clickedItem == ClickedItem.None)
        {
            GameManager.Instance.NoneSelected();
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
                if ( CardClicked(objectHit) )
                    return ClickedItem.Card;
                else
                    return ClickedItem.None;
            }

            else if (layer == 9)
            {
                if ( CardSlotClicked(objectHit) )
                    return ClickedItem.CardSlot;
                else
                    return ClickedItem.None;
            }

            else if (layer == 10)
            {
                if ( DiscardPileClicked(objectHit) )
                    return ClickedItem.DiscardPile;
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

    private bool DiscardPileClicked(Transform objectHit)
    {
        Discard discard = objectHit.GetComponentInParent<Discard>();

        if (discard != null)
        {
            clickedDiscardPile = discard;
            return true;
        }
        else
            return false;
    }
}
