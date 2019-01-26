using UnityEngine;

public interface IHolder
{
    void Enter(Card card);
    void Exit(Card card);
    Transform GetTransform();
}