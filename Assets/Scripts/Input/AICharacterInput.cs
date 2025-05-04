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
    public event Action OnWeaponSelect0;
    public event Action OnWeaponSelect1;
    public event Action OnWeaponSelect2;
    public event Action OnHoldTarget;
    public event Action OnDrawWeapon;
    public event Action OnSprint;
    public event Action OnSneak;

    // Методы для установки состояния AI
    public void SetMoveDirection(Vector2 direction) => _moveDirection = direction;
    public void SetLookDirection(Vector2 direction) => _lookDirection = direction;
    public void TriggerAttack() => OnAttack?.Invoke();
    public void TriggerInteract() => OnInteract?.Invoke();
    public void TriggerJump() => OnJump?.Invoke();
    public void TriggerHoldTarget() => OnHoldTarget?.Invoke();
    public void TriggerDrawWeapon() => OnDrawWeapon?.Invoke();
    public void SetSprinting() => OnSprint?.Invoke();
    public void SetSneaking() => OnSneak?.Invoke();

    // Реализация интерфейса ICharacterInput
    public Vector2 GetMoveDirection() => _moveDirection;
    public Vector2 GetLookDirection() => _lookDirection;

    public AICharacterInput()
    {
        
    }

    public void ResetHoldTarget()
    {
        
    }
    
    public void EnableCharacterInput(bool value)
    {
        if (value)
        {
            //отклчаем подписку на события управления
        }
    }
    
    // Пример использования в AI
    private void ToUpdate()
    {
        // Здесь AI может обновлять свои намерения, например:
        // SetMoveDirection(someDirection);
        // if (shouldAttack) TriggerAttack();
    }
    
    public void ForciblyDrawWeapon(bool value)
    {
        //_isWeaponDrawn = value;
        OnDrawWeapon?.Invoke();
    }
}
