using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour, IHolder
{
	[Range(10f, 180f)]
	public float angle = 30f;

	[Range(5f, 30f)]
	public float radius = 15f;

	[Range(0f, 1f)]
	public float cardAnglingFactor = 0.50f;

	public float cardThickness = 0.05f;

	public List<Card> cards;

	// Use this for initialization
	void Start ()
	{
		cards = new List<Card>();

		Card[] cardsArray = GetComponentsInChildren<Card>();

		foreach (Card card in cardsArray)
		{
			cards.Add(card);
			card.TakeOver(Card.CardStatus.PlayerHand, this);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateCardPositions();
	}

	private void UpdateCardPositions()
	{
		Vector3 up = transform.up;				// Green arrow
		Vector3 forward = transform.forward;	// Blue arrow
		Vector3 right = transform.right;		// Red arrow

		float minAngle = -angle / 2f;
		float currentAngle = 0f;
		float halfAngle = 0f;
		int total = cards.Count - 1;

		Vector3 currentPos = new Vector3();
		Vector3 halfAngleVector = new Vector3();
		Quaternion currentRot = new Quaternion();

		if (cards.Count <= 0)
		{
			return;
		}

		if (cards.Count <= 1)
		{
			currentPos = radius * forward;

			halfAngleVector = radius * forward;

			currentRot = Quaternion.LookRotation(up, currentPos);
			currentPos += transform.position;
			currentPos -= radius * forward;

			cards[0].transform.position = currentPos;
			cards[0].transform.rotation = currentRot;

			return;
		}

		for (int i = 0; i <= total; i++)
		{
			currentAngle = minAngle + angle / (1f * total) * (1f * i);
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

			cards[i].transform.position = currentPos;
			cards[i].transform.rotation = currentRot;
		}
	}

	public void Enter(Card card)
	{
		cards.Add(card);
	}

	public void Exit(Card card)
	{
		cards.Remove(card);
	}
}
