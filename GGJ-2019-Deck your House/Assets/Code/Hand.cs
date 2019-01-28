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

    public Card selectedCard;
    public int selectedCardIndex = -1;

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
        // No cards left
		if (cards.Count <= 0)
		{
			return;
		}

        // One card left
		if (cards.Count == 1)
		{
            angles[0] = 0f;

			Vector3 currentPos = radius * transform.forward;

			Quaternion currentRot = Quaternion.LookRotation(transform.up, currentPos);
			currentPos += transform.position;
			currentPos -= radius * transform.forward;

            if (instantAdjust)
			{
                cards[0].transform.position = currentPos;
			    cards[0].transform.rotation = currentRot;
            }
            else
                cards[0].InitMove(currentPos, currentRot, 0.5f);

            return;
		}

        if (selectedCardIndex == -1)
        {
            NormalArrangement();
        }
        else
        {
            FocusedArrangement();
        }
	}

    private void FocusedArrangement()
    {
		float minimumAngle = -maxAngle / 2f;
        float maximumAngle = maxAngle / 2f;
        angles = new float[cards.Count];

		int total = cards.Count - 1;

        bool minGap = maxAngle > cardSeparation * (1f * total);

        if (minGap)
        {
            minimumAngle = -(total) * cardSeparation / 2f;
            maximumAngle = total * cardSeparation / 2f;
        }

        int focused = selectedCardIndex;
        
        if (focused == 0)
            angles[focused] = minimumAngle;
        else if (focused == total)
            angles[focused] = maximumAngle;
        else
            angles[focused] = Mathf.Lerp(minimumAngle, maximumAngle, (focused * 1f) / (total * 1f) );

        AdjustCard(focused, angles[focused], (radius + 0.25f));

        int min = -1;
        int max = -1;

        float minA = 0f;
        float maxA = 0f;

        int previous = focused - 1;
        int previousPrevious = focused - 2;
        int next = focused + 1;
        int nextNext = focused + 2;

        if (previous > 0)
        {
            angles[previous] = angles[focused] - cardSeparation;
            AdjustCard(previous, angles[previous]);

            min = 0;
            max = previous;

            maxA = angles[previous];
            minA = maxA - 2f * previous;

            if (minA > minimumAngle)
            {
                minA = minimumAngle;
            }

            AdjustCardRange(min, max, minA, maxA);
        }
        else if (previous == 0)
        {
            angles[previous] = angles[focused] - cardSeparation;
            AdjustCard(previous, angles[previous]);
        }

        
        if (next < total)
        {
            angles[next] = angles[focused] + cardSeparation;
            AdjustCard(next, angles[next]);

            if (nextNext < total)
            {
                angles[nextNext] = angles[next] + cardSeparation;
                AdjustCard(nextNext, angles[nextNext]);

                min = nextNext;
                max = total;

                minA = angles[nextNext];
                maxA = minA + 2f * (max - min);

                if (maxA < maximumAngle)
                {
                    maxA = maximumAngle;
                }
                AdjustCardRange(min, max, minA, maxA);
            }
            else if (nextNext == total)
            {
                angles[nextNext] = angles[next] + cardSeparation;
                AdjustCard(nextNext, angles[nextNext]);
            }
        }
        else if (next == total)
        {
            angles[next] = angles[focused] + cardSeparation;
            AdjustCard(next, angles[next]);
        }
    }

    private void AdjustCardRange(int min, int max, float minA, float maxA)
    {
        int total = max - min;

        for (int i = min; i <= max; i++)
        {
            AdjustCard(i, Mathf.Lerp(minA, maxA, ((i-min) * 1f) / ((total) * 1f)));
        }
    }

    private void NormalArrangement()
    {
		float minAngle = -maxAngle / 2f;
		float currentAngle = 0f;
		int total = cards.Count - 1;

        angles = new float[cards.Count];

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

            AdjustCard(i, currentAngle);
		}
    }
    private void AdjustCard(int i, float angle)
    {
        AdjustCard(i, angle, radius);
    }

    private void AdjustCard(int i, float angle, float customRadius)
    {
        angles[i] = angle;
        float currentAngle = angle * Mathf.Deg2Rad;

        float halfAngle = currentAngle * cardAnglingFactor;

        Vector3 currentPos = customRadius * Mathf.Cos(currentAngle) * transform.forward +
                             customRadius * Mathf.Sin(currentAngle) * transform.right +
                             cardThickness * i * transform.up;

        Vector3 halfAngleVector = customRadius * Mathf.Cos(halfAngle) * transform.forward +
                                  customRadius * Mathf.Sin(halfAngle) * transform.right;

        Quaternion currentRot = Quaternion.LookRotation(transform.up, halfAngleVector);
        currentPos += transform.position;
        currentPos -= radius * transform.forward;

        if (instantAdjust)
        {
            cards[i].transform.position = currentPos;
            cards[i].transform.rotation = currentRot;
        }
        else
            cards[i].InitMove(currentPos, currentRot, 0.5f);
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

    public void SelectCard(Card card)
    {
        selectedCard = card;
        selectedCardIndex = cards.IndexOf(card);

        UpdateCardPositions();
    }

    public void DeselectCard(Card card)
    {
        if (selectedCard == card)
        {
            selectedCard = null;
            selectedCardIndex = -1;
        }

        UpdateCardPositions();
    }
}
