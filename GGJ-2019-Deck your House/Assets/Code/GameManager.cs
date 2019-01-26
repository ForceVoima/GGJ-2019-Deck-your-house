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
        CardSlotOrDiscardPile,
        DiscardPile,
        Continue
    }

    public Expecting expecting = Expecting.Card;

    public enum Phase
    {
        RatingsPlayer,
        NormalTurns,
        EndScreen
    }

    public Phase phase = Phase.RatingsPlayer;

    public enum TurnPhase
    {
        Wait1,
        Player1,
        Wait2,
        Player2
    }

    public TurnPhase turnPhase = TurnPhase.Wait1;
#endregion Enums

    public Card selectedCard = null;
    public Transform focalPoint;

    public TableGrid tableGrid;
    public Deck deck;

    public Hand player1Hand;
    public Hand player2Hand;

    [Range(0, 5)]
    public int turnNumber = 0;

    public Card[] cardArray;

    public bool playedCard = false;
    public bool playerDiscard = false;

    public int player1commonRoomCards = 0;
    public int player2commonRoomCards = 0;

    public void Start()
    {
        if (focalPoint == null)
            focalPoint = transform.Find("Focal point").transform;

        /*
        phase = Phase.RatingsPlayer;
        expecting = Expecting.Card;

        deck.Initialize();
        deck.Shuffle();
        deck.Organize();

        UI.Instance.ShowUI(true);
        UI.Instance.UpdateText("Player 1. Rate the your cards from 10 to -10:", "Continue");
        turnNumber = 0;
        */

        phase = Phase.RatingsPlayer;
        turnPhase = TurnPhase.Player2;
        InitNextTurn();
    }

    public void CardSelected(Card card)
    {
        if (!card.Selectable)
            return;

        if (phase == Phase.RatingsPlayer)
        {
            if (turnPhase == TurnPhase.Player1 ||
                turnPhase == TurnPhase.Player2)
            {
                RatingPlayer(card);
            }
        }
        else if (phase == Phase.NormalTurns)
        {
            if (turnPhase == TurnPhase.Player1 ||
                turnPhase == TurnPhase.Player2)
            {
                NormalTurnCardSelected(card);
            }
        }
    }

    public void NoneSelected()
    {
        if (phase == Phase.RatingsPlayer)
        {
            if (expecting == Expecting.RatingConfirmation &&
                selectedCard != null)
            {
                DeFocusCard(selectedCard);
            }
        }

        else if (phase == Phase.NormalTurns)
        {
            if ( (expecting == Expecting.CardSlot ||
                  expecting == Expecting.DiscardPile ||
                  expecting == Expecting.CardSlotOrDiscardPile) &&
                 selectedCard != null )
            {
                selectedCard.Deselect();
                selectedCard = null;

                expecting = Expecting.Card;
            }
        }
    }

    public void CardSlotSelected(CardSlot slot)
    {
        bool turnOK = (turnPhase == TurnPhase.Player1 ||
                       turnPhase == TurnPhase.Player2);

        bool expectingOK = (expecting == Expecting.CardSlot ||
                            expecting == Expecting.CardSlotOrDiscardPile);

        if ( phase == Phase.NormalTurns &&
             turnOK &&
             expectingOK &&
             slot.Free &&
             selectedCard != null &&
             !playedCard )
        {
            selectedCard.PutIn(slot);
            selectedCard = null;

            if (!playedCard && !playerDiscard)
            {
                playedCard = true;
                expecting = Expecting.Card;
            }
            else if (!playedCard && playerDiscard)
            {
                playedCard = true;
                InitNextTurn();
            }
        }
    }

    public void DiscardPileSelected(Discard discard)
    {
        bool turnOK = (turnPhase == TurnPhase.Player1 ||
                       turnPhase == TurnPhase.Player2);

        bool expectingOK = (expecting == Expecting.DiscardPile ||
                            expecting == Expecting.CardSlotOrDiscardPile);

        if (phase == Phase.NormalTurns &&
             turnOK &&
             expectingOK &&
             selectedCard != null)
        {
            selectedCard.PutIn(discard);
            selectedCard = null;

            if (!playedCard && !playerDiscard)
            {
                playerDiscard = true;
                expecting = Expecting.Card;
            }

            else if (playedCard && !playerDiscard)
            {
                playerDiscard = true;
                InitNextTurn();
            }

            else if (!playedCard && playerDiscard)
            {
                Debug.Log("Select from Discard pile!");
            }
        }
    }

    public void ContinueSelected()
    {
        InitNextTurn();
    }

    private void InitNextTurn()
    {
        if (phase == Phase.RatingsPlayer)
        {
            NextRatingTurn();
        }

        else if (phase == Phase.NormalTurns)
        {
            NextNormalTurn();
        }
    }

    private void NextRatingTurn()
    {
        if (turnPhase == TurnPhase.Wait1)
        {
            turnPhase = TurnPhase.Player1;
            expecting = Expecting.Card;
            ratingToGive = 10;
            UI.Instance.ShowUI(false);

            cardArray = deck.GetWholeDeck();
            tableGrid.Reset(cardArray);
        }
        else if (turnPhase == TurnPhase.Player1)
        {
            turnPhase = TurnPhase.Wait2;
            expecting = Expecting.Continue;
            
            UI.Instance.ShowUI(true);
            UI.Instance.UpdateText("Player 2: Rate your cards from 10 to -10:", "Continue");
        }
        else if (turnPhase == TurnPhase.Wait2)
        {
            turnPhase = TurnPhase.Player2;
            expecting = Expecting.Card;
            ratingToGive = 10;
            UI.Instance.ShowUI(false);

            deck.Shuffle();
            deck.Organize();

            cardArray = deck.GetWholeDeck();
            tableGrid.Reset(cardArray);
        }
        else if (turnPhase == TurnPhase.Player2)
        {
            // Load instructions for Player 1 turn 1
            phase = Phase.NormalTurns;
            turnPhase = TurnPhase.Wait1;
            expecting = Expecting.Continue;

            turnNumber = 1;

            UI.Instance.ShowUI(true);
            UI.Instance.UpdateText("Hand the device to player 1.", "Turn " + turnNumber);
        }
    }

    private void NextNormalTurn()
    {
        if (turnPhase == TurnPhase.Wait1)
        {
            turnPhase = TurnPhase.Player1;
            expecting = Expecting.Card;
            UI.Instance.ShowUI(false);

            if (turnNumber == 1)
            {
                deck.Shuffle();
                deck.Organize();
                deck.DealCards(6, player1Hand);
            }
            else if (turnNumber >= 2 &&
                     turnNumber <= 3)
            {
                deck.DealCards(2, player1Hand);
            }

            playedCard = false;
            playerDiscard = false;
        }
        else if (turnPhase == TurnPhase.Player1)
        {
            turnPhase = TurnPhase.Wait2;
            expecting = Expecting.Continue;

            UI.Instance.ShowUI(true);
            UI.Instance.UpdateText("Hand the device to player 2.", "Turn " + turnNumber);
        }
        else if (turnPhase == TurnPhase.Wait2)
        {
            turnPhase = TurnPhase.Player2;
            expecting = Expecting.Card;
            ratingToGive = 10;
            UI.Instance.ShowUI(false);

            if (turnNumber == 1)
            {
                deck.DealCards(6, player2Hand);
            }
            else if (turnNumber >= 2 &&
                     turnNumber <= 3)
            {
                deck.DealCards(2, player2Hand);
            }
        }
        else if (turnPhase == TurnPhase.Player2)
        {
            if (turnNumber == 5)
            {
                phase = Phase.EndScreen;
                return;
            }

            turnPhase = TurnPhase.Wait1;
            expecting = Expecting.Continue;

            turnNumber++;

            UI.Instance.ShowUI(true);
            UI.Instance.UpdateText("Hand the device to player 1.", "Turn " + turnNumber);
        }
    }

#region Ratings

    private void RatingPlayer(Card card)
    {
        if ( (expecting == Expecting.Card ||
              selectedCard == null) &&
              ( !card.player1Rated && turnPhase == TurnPhase.Player1 ||
                !card.player2Rated && turnPhase == TurnPhase.Player2 ) )
        {
            FocusCard(card);
        }
        else if (expecting == Expecting.RatingConfirmation)
        {
            if (selectedCard == card)
            {
                card.PutIn(deck: deck, changeOwner: true);

                if (turnPhase == TurnPhase.Player1)
                    card.Player1Rating(ratingToGive);
                else if (turnPhase == TurnPhase.Player2)
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
        card.InitMove(focalPoint.position, true, 0.4f);
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
            InitNextTurn();
        }
        else
            Debug.LogError("We shouldn't end up here!");
    }

    public void PickRandomRatings()
    {
        StartCoroutine(PickRandomRatingsRoutine());
    }

    IEnumerator PickRandomRatingsRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        
        for (int i = 0; i < cardArray.Length; i++)
        {
            RatingPlayer(cardArray[i]);
            yield return new WaitForSeconds(0.1f);
            RatingPlayer(cardArray[i]);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

#endregion Ratings

#region NormalTurn
    private void NormalTurnCardSelected(Card card)
    {
        if (expecting == Expecting.Card)
        {
            card.Select();
            selectedCard = card;

            if (!playedCard && !playerDiscard)
                expecting = Expecting.CardSlotOrDiscardPile;

            else if (playedCard && !playerDiscard)
                expecting = Expecting.DiscardPile;

            else if (!playedCard && playerDiscard)
                expecting = Expecting.CardSlot;
        }
    }
#endregion NormalTurn
}
