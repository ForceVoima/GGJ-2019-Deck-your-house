using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    [Tooltip("Card base value"), Range(0, 20)]
    public int baseValue = 10;

    [Tooltip("Player 1 joy rating."), Range(-10, 10)]
    public int player1Rating = 0;

    public bool player1Rated = false;

    [Tooltip("Player 2 joy rating."), Range(-10, 10)]
    public int player2Rating = 0;

    public bool player2Rated = false;

    public string cardNameString = "This is the name of the card";

    public bool selected = false;
    public bool moving = false;
    public bool faceUp = true;

    public bool debug = false;

    private Vector3 startPos;
    private Vector3 endPos;

    private Vector3 previousPos;

    private Quaternion startRot;
    private Quaternion endRot;

    private float timer = 0f;
    private float timerEnd = 1.0f;

    public TextMeshProUGUI cardName;
    public TextMeshProUGUI description;

    public TextMeshProUGUI sumValue;
    public TextMeshProUGUI yourValue;
    public TextMeshProUGUI theirValue;

    [SerializeField]
    private GameObject glowEffect;

    public enum CardStatus
    {
        PlayerHand,
        CardSlot,
        Deck,
        Discard,
        Grid,
        Testing
    }

    [SerializeField]
    private CardStatus status = CardStatus.Testing;

    public CardStatus Status
    {
        get { return status; }
    }

    public bool Selectable
    {
        get
        {
            return (status == CardStatus.PlayerHand ||
                    status == CardStatus.Testing ||
                    status == CardStatus.Grid);
        }
    }

    public IHolder holder;

    // Use this for initialization
    void Start ()
    {
        if (cardName == null)
            cardName = transform.Find("Mesh/Canvas/Name").GetComponent<TextMeshProUGUI>();
        if (description == null)
            description = transform.Find("Mesh/Canvas/Description").GetComponent<TextMeshProUGUI>();
        if (sumValue == null)
            sumValue = transform.Find("Mesh/Canvas/Value-SUM").GetComponent<TextMeshProUGUI>();
        if (yourValue == null)
            yourValue = transform.Find("Mesh/Canvas/Value-yours/Number").GetComponent<TextMeshProUGUI>();
        if (theirValue == null)
            theirValue = transform.Find("Mesh/Canvas/Value-theirs/Number").GetComponent<TextMeshProUGUI>();

        if (glowEffect == null)
            glowEffect = transform.Find("Glow effect").gameObject;

        glowEffect.SetActive(false);

        yourValue.text = "";
        theirValue.text = "";
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (moving)
        {
            if (MoveFinished(Time.deltaTime))
                moving = false;
        }
	}

    public void Player1Rating(int value)
    {
        player1Rating = value;
        yourValue.text = player1Rating.ToString();

        player1Rated = true;
    }

    public void Player2Rating(int value)
    {
        player2Rating = value;
        yourValue.text = player2Rating.ToString();

        player2Rated = true;
    }

    public void ResetRatingTexts()
    {
        yourValue.text = "";
        theirValue.text = "";
    }

    #region Selections

    public void TakeOver(CardStatus status, IHolder holder)
    {
        this.status = status;
        this.holder = holder;
    }

    public void Select()
    {
        selected = true;
        glowEffect.SetActive(true);
    }

    public void Deselect()
    {
        selected = false;
        glowEffect.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (!selected && Selectable)
            glowEffect.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (!selected)
            glowEffect.SetActive(false);
    }
#endregion Selections

    public void PutIn(TableGrid grid)
    {
        Deselect();
        status = CardStatus.Grid;

        NewHolder(grid);
    }

    public void PutIn(Deck deck, bool changeOwner)
    {
        Deselect();
        InitMove(deck.Position, deck.Rotation);
        status = CardStatus.Deck;

        if (changeOwner)
            NewHolder(deck);
    }

    public void PutIn(CardSlot slot)
    {
        Deselect();
        InitMove(slot.Position, true);
        status = CardStatus.CardSlot;

        NewHolder(slot);
    }

    public void InitializeHolder(IHolder holder)
    {
        this.holder = holder;
    }

    private void NewHolder(IHolder newHolder)
    {
        // Let previous holder know!
        if (holder != null)
            holder.Exit(this);

        holder = newHolder;
        holder.Enter(this);

        // Switch to new parent
        transform.SetParent(holder.GetTransform());
    }

#region MoveStuff
    public void InitMove(Vector3 target, bool faceUp)
    {
        BasicMove(target);
        FaceUp(faceUp);
    }

    public void InitMove(Vector3 moveTarget, Quaternion rotTarget)
    {
        BasicMove(moveTarget);
        endRot = rotTarget;
    }

    private void BasicMove(Vector3 target)
    {
        previousPos = transform.position;
        timer = 0f;

        startPos = transform.position;
        startRot = transform.rotation;

        endPos = target;

        moving = true;
    }

    private void FaceUp(bool faceUp)
    {
        this.faceUp = faceUp;

        if (!this.faceUp)
        {
            endRot = Quaternion.LookRotation(forward: Vector3.down, upwards: Vector3.forward);
        }
        else
        {
            endRot = Quaternion.LookRotation(forward: Vector3.up, upwards: Vector3.forward);
        }
    }

    public void Return(bool faceUp)
    {
        InitMove(previousPos, faceUp);
    }

    public bool MoveFinished(float deltaTime)
    {
        timer += deltaTime;

        if (timer > timerEnd)
        {
            moving = false;

            transform.position = endPos;
            transform.rotation = endRot;

            return true;
        }
        else
        {
            float factor = SmoothLerpTime(timer / timerEnd);

            transform.position = Vector3.Lerp(startPos, endPos, factor);
            transform.rotation = Quaternion.Lerp(startRot, endRot, factor);

            return false;
        }
    }

    private float SmoothLerpTime(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
#endregion MoveStuff
}