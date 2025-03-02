using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectNextCharacter : MonoBehaviour
{
    private List<CharacterActions> _chars;
    private int _currentIndex = -1;

    private void Awake()
    {
        _chars = FindObjectsByType<CharacterActions>(FindObjectsSortMode.None).ToList();
       // Debug.LogWarning(_chars.Count);
    }

    public void Button()
    {
        if (_chars.Count > 0)
        {
            CharacterActions.OnCharacterSelected(GetNextCharacter());
        }
    }
    
    private CharacterActions GetNextCharacter()
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
