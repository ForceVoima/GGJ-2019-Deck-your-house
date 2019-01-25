using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Tooltip("Player 1 joy rating."), Range(-10, 10)]
    public int player1Rating = 0;

    [Tooltip("Player 2 joy rating."), Range(-10, 10)]
    public int player2Rating = 0;



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
