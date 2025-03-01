using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectNextCharacter : MonoBehaviour
{
    private List<CharacterMovement> _chars;
    private int _currentIndex = -1;

    private void Awake()
    {
        _chars = FindObjectsByType<CharacterMovement>(FindObjectsSortMode.None).ToList();
       // Debug.LogWarning(_chars.Count);
    }

    public void Button()
    {
        if (_chars.Count > 0)
        {
            CharacterMovement.OnCharacterSelected(GetNextCharacter());
        }
    }
    
    private CharacterMovement GetNextCharacter()
    {
        if (_chars == null || _chars.Count == 0)
        {
            return null; // Возвращаем null, если список пустой или не инициализирован
        }

        _currentIndex++;

        // Если достигли конца списка, возвращаемся в начало
        if (_currentIndex >= _chars.Count)
        {
            _currentIndex = 0;
        }

        return _chars[_currentIndex];
    }
}
