using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Tooltip("Card base value"), Range(0, 20)]
    public int baseValue = 10;

    [Tooltip("Player 1 joy rating."), Range(-10, 10)]
    public int player1Rating = 0;

    [Tooltip("Player 2 joy rating."), Range(-10, 10)]
    public int player2Rating = 0;

    public string flavorText = "This is very nice card.";

    public bool selected = false;
    public bool moving = false;
    public bool faceUp = true;

    private Vector3 startPos;
    private Vector3 endPos;

    private Quaternion startRot;
    private Quaternion endRot;

    private float timer = 0f;
    private float timerEnd = 1.0f;

    [SerializeField]
    private GameObject glowEffect;

    public enum CardStatus
    {
        PlayerHand,
        CardSlot,
        Deck,
        Discard,
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
            return (status == CardStatus.PlayerHand || status == CardStatus.Testing);
        }
    }

    public IHolder holder;

    // Use this for initialization
    void Start ()
    {
        if (glowEffect == null)
            glowEffect = transform.Find("Glow effect").gameObject;

        glowEffect.SetActive(false);
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

    private void OnMouseOver()
    {
        if (!selected)
            glowEffect.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (!selected)
            glowEffect.SetActive(false);
    }

    public void PutIn(CardSlot slot)
    {
        Deselect();
        InitMove(slot.Position, true);
        status = CardStatus.CardSlot;
        holder = slot;
        holder.Enter(this);

        transform.SetParent(holder.GetTransform());
    }

    #region MoveStuff
    public void InitMove(Vector3 target, bool faceUpTarget)
    {
        // This is totally for testing only:
        faceUp = !faceUp;

        // Tell the holder you're leaving.
        holder.Exit(this);

        timer = 0f;

        startPos = transform.position;
        startRot = transform.rotation;

        endPos = target;

        if (faceUp)
        {
            endRot = Quaternion.LookRotation(forward: Vector3.down, upwards: Vector3.forward);
        }
        else
        {
            endRot = Quaternion.LookRotation(forward: Vector3.up, upwards: Vector3.forward);
        }

        moving = true;
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