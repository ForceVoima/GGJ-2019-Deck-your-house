using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
#region Statics
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load<GameManager>("GameManager"));
            }

            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    #endregion Statics

    public int ratingToGive = 10;
#region Enums
    public enum Expecting
    {
        Card,
        RatingConfirmation,
        CardSlot,
        Discard
    }

    public Expecting expecting = Expecting.Card;

    public enum Phase
    {
        RatingsPlayer1,
        RatingsPlayer2,
        Deal6,
        Player1Turn,
        Player2Turn,
        EndScreen
    }

    public Phase phase = Phase.RatingsPlayer1;
#endregion Enums

    public Card selectedCard = null;
    public Transform focalPoint;

    public void Start()
    {
        if (focalPoint == null)
            focalPoint = transform.Find("Focal point").transform;
    }

    public void CardSelected(Card card)
    {
        if (!card.Selectable)
            return;

        if (phase == Phase.RatingsPlayer1)
        {
            RatingPlayer1(card);
        }

        if (phase == Phase.RatingsPlayer2)
        {
            RatingPlayer2(card);
        }
    }

    public void NoneSelected()
    {
        if (phase == Phase.RatingsPlayer1 ||
            phase == Phase.RatingsPlayer2)
        {
            if (expecting == Expecting.RatingConfirmation &&
                selectedCard != null)
            {
                DeFocusCard(selectedCard);
            }
        }
    }

#region Ratings

    private void RatingPlayer1(Card card)
    {
        if ( (expecting == Expecting.Card ||
              selectedCard == null) &&
              !card.player1Rated )
        {
            FocusCard(card);
        }
        else if (expecting == Expecting.RatingConfirmation)
        {
            if (selectedCard == card)
            {
                DeFocusCard(card);
                card.Player1Rating(ratingToGive);
                NextRating();
            }
            else
            {
                DeFocusCard(selectedCard);
                FocusCard(card);
            }
        }
    }

    private void RatingPlayer2(Card card)
    {
        if ( (expecting == Expecting.Card ||
              selectedCard == null) &&
              !card.player2Rated)
        {
            FocusCard(card);
        }
        else if (expecting == Expecting.RatingConfirmation)
        {
            if (selectedCard == card)
            {
                DeFocusCard(card);
                card.Player2Rating(ratingToGive);
                NextRating();
            }
            else
            {
                DeFocusCard(selectedCard);
                FocusCard(card);
            }
        }
    }

    private void FocusCard(Card card)
    {
        card.InitMove(focalPoint.position, true);
        card.Select();
        selectedCard = card;
        expecting = Expecting.RatingConfirmation;
    }

    private void DeFocusCard(Card card)
    {
        card.Return(true);
        card.Deselect();
        selectedCard = null;
        expecting = Expecting.Card;
    }

    private void NextRating()
    {
        if (ratingToGive > 1)
        {
            ratingToGive--;
            expecting = Expecting.Card;
        }
        else if (ratingToGive == 1)
        {
            ratingToGive = -1;
            expecting = Expecting.Card;
            // Update UI help text
        }
        else if (ratingToGive > -10)
        {
            ratingToGive--;
            expecting = Expecting.Card;
        }
        else if (ratingToGive == -10)
        {
            if (phase == Phase.RatingsPlayer1)
            {
                // Reset deck for player 2
                phase = Phase.RatingsPlayer2;
                expecting = Expecting.Card;
                ratingToGive = 10;
            }
            else if (phase == Phase.RatingsPlayer2)
            {
                ratingToGive = 0;
                phase = Phase.Player1Turn;
            }
            else
                Debug.LogError("We shouldn't end up here!");
        }
        else
            Debug.LogError("We shouldn't end up here!");
    }

#endregion Ratings
}
