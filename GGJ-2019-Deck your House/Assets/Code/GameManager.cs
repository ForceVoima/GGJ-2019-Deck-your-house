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

    public TurnPhase WhoseTurn
    {
        get { return turnPhase; }
    }
#endregion Enums

    public Card selectedCard = null;
    public Transform focalPoint;

    public TableGrid tableGrid;
    public Deck deck;

    public Hand player1Hand;
    public Hand player2Hand;

    public Hand player1DiscardHand;
    public Hand player2DiscardHand;

    public Discard discardPile;

    [Range(0, 5)]
    public int turnNumber = 0;

    public Card[] cardArray;

    public bool playedCard = false;
    public bool playerDiscard = false;
    public bool doubleDiscard = false;

    public int player1commonRoomCards = 0;
    public int player2commonRoomCards = 0;

    public bool quickStart = false;

    public CameraTransitions cameraTransitions;
    public Camera sceneCamera;

    public Room player1Room;
    public Room player2Room;
    public Room commonRoom;

    public void Start()
    {
        if (focalPoint == null)
            focalPoint = transform.Find("Focal point").transform;

        if (cameraTransitions == null)
            cameraTransitions = GameObject.Find("Camera mount").GetComponent<CameraTransitions>();

        if (sceneCamera == null)
        {
            sceneCamera = GameObject.Find("Camera mount/Main Camera").GetComponent<Camera>();

            if (sceneCamera == null)
            {
                sceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            }
        }

        deck.Initialize();
        deck.Shuffle();
        deck.Organize();

        cardArray = deck.GetWholeDeck();

        if (quickStart)
        {
            phase = Phase.RatingsPlayer;
            turnPhase = TurnPhase.Player2;
            InitNextTurn();
        }
        else
        {
            phase = Phase.RatingsPlayer;
            expecting = Expecting.Card;

            UI.Instance.ShowUI(true);
            UI.Instance.UpdateText("Player 1. Rate the your cards from 10 to -10:", "Continue");
            turnNumber = 0;
        }
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

        bool player1 = (turnPhase == TurnPhase.Player1);
        bool player2 = (turnPhase == TurnPhase.Player2);

        bool player1Room = (player1 && slot.ParentRoom.Type == Room.RoomType.Player1);
        bool player2Room = (player2 && slot.ParentRoom.Type == Room.RoomType.Player2);

        bool commonroom = (slot.ParentRoom.Type == Room.RoomType.CommonRoom) &&
                          ( (player1 && player1commonRoomCards < 3) ||
                            (player2 && player2commonRoomCards < 3) );

        bool roomOK = player1Room || player2Room || commonroom;

        if ( phase == Phase.NormalTurns &&
             turnOK &&
             expectingOK &&
             roomOK &&
             slot.Free &&
             selectedCard != null &&
             !playedCard )
        {
            selectedCard.PutIn(slot, turnPhase);
            selectedCard.ShowAllRatings(turnPhase);
            selectedCard = null;

            if (commonroom && player1)
                player1commonRoomCards++;

            else if (commonroom && player2)
                player2commonRoomCards++;

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

            if (doubleDiscard && player1)
                player1DiscardHand.DiscardWholeHand(discardPile);
            else if (doubleDiscard && player2)
                player2DiscardHand.DiscardWholeHand(discardPile);
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
            selectedCard.ShowAllRatings(turnPhase);
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
                if (turnPhase == TurnPhase.Player1)
                {
                    player1Hand.NotYourTurn();
                    discardPile.DealCards(player1DiscardHand);
                }

                else if (turnPhase == TurnPhase.Player2)
                {
                    player2Hand.NotYourTurn();
                    discardPile.DealCards(player2DiscardHand);
                }

                doubleDiscard = true;
                expecting = Expecting.Card;
            }
        }
    }

    public void ContinueSelected()
    {
        InitNextTurn();
    }

    public void ConfirmSelected()
    {

    }

    public void CancelSelected()
    {

    }

    public void CameraMoveFinished()
    {
        if (phase == Phase.RatingsPlayer)
        {
            tableGrid.transform.position = sceneCamera.transform.position + sceneCamera.transform.forward * 10f;

            if (turnPhase == TurnPhase.Player1)
            {
                tableGrid.transform.rotation = Quaternion.Euler(-75f, 180f, 0f);
                //tableGrid.transform.rotation = Quaternion.LookRotation(sceneCamera.transform.position - focalPoint.position);

                Debug.DrawLine(tableGrid.transform.position, tableGrid.transform.position + 10f * tableGrid.transform.forward, Color.red, 10f);

                cardArray = deck.GetWholeDeck();
                tableGrid.Reset(cardArray);
            }
            else if (turnPhase == TurnPhase.Player2)
            {
                tableGrid.transform.rotation = Quaternion.Euler(-75f, 0f, 0f);
                // tableGrid.transform.rotation = Quaternion.LookRotation(sceneCamera.transform.position - focalPoint.position);

                Debug.DrawLine(tableGrid.transform.position, tableGrid.transform.position + 10f * tableGrid.transform.forward, Color.red, 10f);

                cardArray = deck.GetWholeDeck();
                tableGrid.Reset(cardArray);
            }
        }
    }

#region Turns
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

        else if (phase == Phase.EndScreen)
        {
            StartCoroutine(NewGame());
        }
    }

    IEnumerator NewGame()
    {
        player1commonRoomCards = 0;
        player2commonRoomCards = 0;

        foreach (Card card in cardArray)
        {
            card.Enable();
            card.ClearRatings();
            card.PutIn(deck, true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.2f);

        deck.Shuffle();
        deck.Organize();

        yield return new WaitForSeconds(0.2f);

        phase = Phase.RatingsPlayer;
        turnPhase = TurnPhase.Wait1;
        expecting = Expecting.Continue;

        UI.Instance.ShowUI(true);
        UI.Instance.UpdateText("Player 1. Rate the your cards from 10 to -10:", "Continue");
        turnNumber = 0;

        cameraTransitions.TransitionCamera(turnPhase);
    }

    private void NextRatingTurn()
    {
        if (turnPhase == TurnPhase.Wait1)
        {
            turnPhase = TurnPhase.Player1;
            expecting = Expecting.Card;
            ratingToGive = 10;
            UI.Instance.ShowUI(false);
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

        cameraTransitions.TransitionCamera(turnPhase);
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
                deck.DealCards(6, player1Hand, turnPhase);
            }
            else if (turnNumber >= 2 &&
                     turnNumber <= 5)
            {
                deck.DealCards(1, player1Hand, turnPhase);
            }

            playedCard = false;
            playerDiscard = false;
            doubleDiscard = false;

            player1Hand.YourTurn();
            player2Hand.NotYourTurn();
        }
        else if (turnPhase == TurnPhase.Player1)
        {
            turnPhase = TurnPhase.Wait2;
            expecting = Expecting.Continue;

            player1Hand.NotYourTurn();

            UI.Instance.ShowUI(true);
            UI.Instance.UpdateText("Hand the device to player 2.", "Turn " + turnNumber);
        }
        else if (turnPhase == TurnPhase.Wait2)
        {
            turnPhase = TurnPhase.Player2;
            expecting = Expecting.Card;
            UI.Instance.ShowUI(false);

            if (turnNumber == 1)
            {
                deck.DealCards(6, player2Hand, turnPhase);
            }
            else if (turnNumber >= 2 &&
                     turnNumber <= 5)
            {
                deck.DealCards(1, player2Hand, turnPhase);
            }

            playedCard = false;
            playerDiscard = false;
            doubleDiscard = false;

            player2Hand.YourTurn();
            player1Hand.NotYourTurn();
        }
        else if (turnPhase == TurnPhase.Player2)
        {
            if (turnNumber == 5)
            {
                phase = Phase.EndScreen;
                CountScores();
                return;
            }

            turnPhase = TurnPhase.Wait1;
            expecting = Expecting.Continue;

            player2Hand.NotYourTurn();

            turnNumber++;

            UI.Instance.ShowUI(true);
            UI.Instance.UpdateText("Hand the device to player 1.", "Turn " + turnNumber);
        }

        player1Room.AlignAllCards(turnPhase);
        player2Room.AlignAllCards(turnPhase);
        commonRoom.AlignAllCards(turnPhase);

        player1Room.ShowAllCardRatings(turnPhase);
        player2Room.ShowAllCardRatings(turnPhase);
        commonRoom.ShowAllCardRatings(turnPhase);
        discardPile.UpdateAllRatings(turnPhase);

        cameraTransitions.TransitionCamera(turnPhase);
    }
#endregion Turns

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
        focalPoint.position = sceneCamera.transform.position + sceneCamera.transform.forward * 3f;

        if (turnPhase == TurnPhase.Player1)
        {
            focalPoint.rotation = Quaternion.LookRotation(sceneCamera.transform.position - focalPoint.position, Vector3.forward);
        }
        else if (turnPhase == TurnPhase.Player2)
        {
            focalPoint.rotation = Quaternion.LookRotation(sceneCamera.transform.position - focalPoint.position, Vector3.back);
        }

        card.InitMove(focalPoint.position, focalPoint.rotation, 0.4f);
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

            if (!playedCard && doubleDiscard)
                expecting = Expecting.CardSlot;

            else if (!playedCard && !playerDiscard)
                expecting = Expecting.CardSlotOrDiscardPile;

            else if (playedCard && !playerDiscard)
                expecting = Expecting.DiscardPile;

            else if (!playedCard && playerDiscard)
                expecting = Expecting.CardSlotOrDiscardPile;
        }
        else
        {
            selectedCard.Deselect();
            card.Select();
            selectedCard = card;
        }
    }
#endregion NormalTurn

    private void CountScores()
    {
        int player1Score = player1Room.MyScore();
        int player2Score = player2Room.MyScore();
        int commonScore= commonRoom.MyScore();

        UI.Instance.ShowUI(true);
        UI.Instance.UpdateText("Player 1 score: " + player1Score.ToString() + System.Environment.NewLine +
                               "Player 2 score: " + player2Score.ToString() + System.Environment.NewLine +
                               "Common score: " + commonScore.ToString(), "OK");
    }

    private void UpdateCardRatings()
    {
        foreach (Card card in cardArray)
        {
            card.ShowPlayerRating(turnPhase);
        }
    }
}
