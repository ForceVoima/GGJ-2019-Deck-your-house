﻿using System.Collections;
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

    public bool moving = false;
    public bool faceUp = true;

    private Vector3 startPos;
    private Vector3 endPos;

    private Quaternion startRot;
    private Quaternion endRot;

    public float timer = 0f;
    public float timerEnd = 1.0f;

    // Use this for initialization
    void Start ()
    {
		
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

    public void InitMove(Vector3 target, bool faceUpTarget)
    {
        // This is totally for testing only:
        faceUp = !faceUp;

        timer = 0f;

        startPos = transform.position;
        startRot = transform.rotation;

        endPos = target;

        if (faceUp)
        {
            endRot = Quaternion.LookRotation(forward: Vector3.down, upwards: Vector3.back);
        }
        else
        {
            endRot = Quaternion.LookRotation(forward: Vector3.up, upwards: Vector3.back);
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
}
