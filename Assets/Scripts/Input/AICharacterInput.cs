using System;
using UnityEngine;

public class AICharacterInput: ICharacterInput 
{
    // Переменные для хранения текущего состояния ввода от AI
    private Vector2 _moveDirection = Vector2.zero;
    private Vector2 _lookDirection = Vector2.zero;

    // События из интерфейса
    public event Action OnAttack;
    public event Action OnInteract;
    public event Action OnJump;
    public event Action<bool> OnHoldTarget;
    public event Action<bool> OnDrawWeapon;
    public event Action<bool> OnSprint;
    public event Action<bool> OnSneak;

    // Методы для установки состояния AI
    public void SetMoveDirection(Vector2 direction) => _moveDirection = direction;
    public void SetLookDirection(Vector2 direction) => _lookDirection = direction;
    public void TriggerAttack() => OnAttack?.Invoke();
    public void TriggerInteract() => OnInteract?.Invoke();
    public void TriggerJump() => OnJump?.Invoke();
    public void TriggerHoldTarget(bool value) => OnHoldTarget?.Invoke(value);
    public void TriggerDrawWeapon(bool value) => OnDrawWeapon?.Invoke(value);
    public void SetSprinting(bool value) => OnSprint?.Invoke(value);
    public void SetSneaking(bool value) => OnSneak?.Invoke(value);

    // Реализация интерфейса ICharacterInput
    public Vector2 GetMoveDirection() => _moveDirection;
    public Vector2 GetLookDirection() => _lookDirection;

    public AICharacterInput()
    {
        
    }

    public void ResetHoldTarget()
    {
        
    }
    
    // Пример использования в AI
    private void ToUpdate()
    {
        // Здесь AI может обновлять свои намерения, например:
        // SetMoveDirection(someDirection);
        // if (shouldAttack) TriggerAttack();
    }
}
