using System;
using UnityEngine;

public interface ICharacterInput
{
    public Vector2 GetMoveDirection();
    public Vector2 GetLookDirection();

    public event Action OnAttack;
    public event Action OnInteract;
    public event Action OnJump;
    public event Action OnHoldTarget;
    public event Action OnDrawWeapon;
    public event Action<bool> OnSprint; 
    public event Action<bool> OnSneak;
}
