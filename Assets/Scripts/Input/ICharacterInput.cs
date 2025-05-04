using System;
using UnityEngine;

public interface ICharacterInput
{
    public Vector2 GetMoveDirection();
    public Vector2 GetLookDirection();

    public event Action OnAttack;
    public event Action OnInteract;
    public event Action OnJump;
    public event Action OnWeaponSelect0;
    public event Action OnWeaponSelect1;
    public event Action OnWeaponSelect2;
    public event Action OnHoldTarget;
    public event Action OnDrawWeapon;
    public event Action OnSprint; 
    public event Action OnSneak;

    public void ResetHoldTarget();

    public void ForciblyDrawWeapon(bool value);

    public void EnableCharacterInput(bool value);
}
