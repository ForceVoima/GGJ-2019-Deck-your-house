using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : CardHolder, IHolder
{
	[Range(10f, 180f), Tooltip("Max angle of the whole hand.")]
	public float maxAngle = 30f;

    [Range(0f, 60f), Tooltip("Card separation between cards when max angle doesn't apply.")]
    public float cardSeparation = 8f;

	[Range(5f, 30f)]
	public float radius = 15f;

	[Range(0f, 1f)]
	public float cardAnglingFactor = 0.50f;

	public float cardThickness = 0.05f;

    public float[] angles;

    [Range(-180f, 180f)]
    public float yourTurnAngle = 80f;
    [Range(-180f, 180f)]
    public float notYourTurnAngle = 0f;

    public bool instantAdjust = false;

    public bool player2 = false;

    /*
    public bool turning = false;
    
    private float timer = 0f;
    private float timerEnd = 0.6f;
    */

    public override void Enter(Card card)
    {
        cards.Add(card);
    }

    public override void Exit(Card card)
    {
        cards.Remove(card);
        UpdateCardPositions();
    }

    public void Organize()
    {
        UpdateCardPositions();
    }

    private void LateUpdate()
    {
        if (instantAdjust)
            UpdateCardPositions();
    }

    public void Organize(GameManager.TurnPhase turnPhase)
    {
        // Debug.Log("Hand " + name + " is organizing and updating cards during: " + turnPhase);

        UpdateCardPositions();

        foreach (Card card in cards)
        {
            card.ShowPlayerRating(turnPhase);
        }
    }

    private void UpdateCardPositions()
	{
		Vector3 up = transform.up;				// Green arrow
		Vector3 forward = transform.forward;	// Blue arrow
		Vector3 right = transform.right;		// Red arrow

		float minAngle = -maxAngle / 2f;
		float currentAngle = 0f;
		float halfAngle = 0f;
		int total = cards.Count - 1;

		Vector3 currentPos = new Vector3();
		Vector3 halfAngleVector = new Vector3();
		Quaternion currentRot = new Quaternion();

        angles = new float[cards.Count];

        // No cards left
		if (cards.Count <= 0)
		{
			return;
		}

        // One card left
		if (cards.Count == 1)
		{
            angles[0] = 0f;

			currentPos = radius * forward;

			halfAngleVector = radius * forward;

			currentRot = Quaternion.LookRotation(up, currentPos);
			currentPos += transform.position;
			currentPos -= radius * forward;

            if (instantAdjust)
			{
                cards[0].transform.position = currentPos;
			    cards[0].transform.rotation = currentRot;
            }
            else
                cards[0].InitMove(currentPos, currentRot, 0.5f);

            return;
		}

        bool minGap = maxAngle > cardSeparation * (1f * total);

        // minGap = false;

        if (minGap)
            minAngle = -(total) * cardSeparation / 2f;

        for (int i = 0; i <= total; i++)
		{
            if (minGap)
            {
                currentAngle = minAngle + cardSeparation * (1f * i);
            }
            else
            {
                currentAngle = minAngle + maxAngle / (1f * total) * (1f * i);
            }
            angles[i] = currentAngle;
            currentAngle *= Mathf.Deg2Rad;

			halfAngle = currentAngle * cardAnglingFactor;

			currentPos = radius * Mathf.Cos(currentAngle) * forward +
						 radius * Mathf.Sin(currentAngle) * right +
						 cardThickness * i * up;

			halfAngleVector = radius * Mathf.Cos(halfAngle) * forward +
						 	  radius * Mathf.Sin(halfAngle) * right;

			currentRot = Quaternion.LookRotation(up, halfAngleVector);
			currentPos += transform.position;
			currentPos -= radius * forward;

            if (instantAdjust)
			{
                cards[i].transform.position = currentPos;
			    cards[i].transform.rotation = currentRot;
            }
            else
                cards[i].InitMove(currentPos, currentRot, 0.5f);
		}
	}

    public void DiscardWholeHand(Discard discardPile)
    {
        StartCoroutine(DiscardHand(discardPile));
    }

    IEnumerator DiscardHand(Discard discardPile)
    {
        yield return new WaitForSeconds(0.2f);

        Card[] cardArray = cards.ToArray();

        for (int i = 0; i < cardArray.Length; i++)
        {
            cardArray[i].PutIn(discardPile);
        }
    }

    public void YourTurn()
    {
        if (!player2)
            transform.rotation = Quaternion.Euler(yourTurnAngle, 0f, 0f);

        else
            transform.rotation = Quaternion.Euler(yourTurnAngle, 180f, 0f);

        foreach (Card card in cards)
        {
            card.Enable();
        }
    }

    public void NotYourTurn()
    {
        if (!player2)
            transform.rotation = Quaternion.Euler(notYourTurnAngle, 0f, 0f);

        else
            transform.rotation = Quaternion.Euler(notYourTurnAngle, 180f, 0f);

        foreach (Card card in cards)
        {
            card.Disable();
        }

        if (!player2)
            Organize(GameManager.TurnPhase.Player1);
        else
            Organize(GameManager.TurnPhase.Player2);
    }

    public void UpdateHandCardRatings(GameManager.TurnPhase turnPhase)
    {
        // Debug.Log("Hand " + name + " is updating card reading during " + turnPhase);

        foreach (Card card in cards)
        {
            card.ShowPlayerRating(turnPhase);
        }
    }
}
