using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Camera sceneCamera;
    public Card clickedCard;
    public Vector3 clickedPos;
    
    void Start()
    {
        if (sceneCamera == null)
        {
            sceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(button: 0))
        {
            if ( WasCardClicked() )
            {
                Debug.Log(Time.fixedTime +  " Card: " + clickedCard.name + " was clicked!");
            }
            else
            {
                if (clickedCard != null)
                {
                    clickedCard.InitMove(clickedPos, true);
                }
            }
        }
    }

    private bool WasCardClicked()
    {
        RaycastHit hit;
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            Card card = objectHit.GetComponentInParent<Card>();

            if (card != null)
            {
                clickedCard = card;
                return true;
            }
            else
            {
                Vector3 point = hit.point;
                point.y = 0f;
                clickedPos = point;

                return false;
            }
        }
        else
            return false;
    }
}
