﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitions : MonoBehaviour
{
    private Quaternion wait1 = Quaternion.LookRotation(Vector3.right, Vector3.up);
    private Quaternion player1 = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    private Quaternion wait2 = Quaternion.LookRotation(Vector3.left, Vector3.up);
    private Quaternion player2 = Quaternion.LookRotation(Vector3.back, Vector3.up);

    private Quaternion startRot;
    private Quaternion endRot;

    public float timer = 0f;
    public float timerEnd = 1.0f;

    public bool moving = false;

    private void Start ()
    {
	}
	
	private void Update ()
    {
        if (moving)
        {
            timer += Time.deltaTime;

            if (timer > timerEnd)
            {
                transform.rotation = endRot;
                moving = false;
            }
            else
            {
                float factor = SmoothLerpTime(timer / timerEnd);
                transform.rotation = Quaternion.Lerp(startRot, endRot, factor);
            }
        }
    }

    public void TransitionCamera(GameManager.TurnPhase turnPhase)
    {
        startRot = transform.rotation;
        timer = 0f;
        timerEnd = 0.5f;

        switch (turnPhase)
        {
            case GameManager.TurnPhase.Player1:
                endRot = player1;
                break;
            case GameManager.TurnPhase.Wait2:
                endRot = wait2;
                break;
            case GameManager.TurnPhase.Player2:
                endRot = player2;
                break;
            case GameManager.TurnPhase.Wait1:
                endRot = wait1;
                break;
        }

        moving = true;
    }

    private float SmoothLerpTime(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}