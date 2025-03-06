using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SelectNextCharacter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private List<LocoMotion> _chars;
    private int _currentIndex = -1;

    private void Awake()
    {
        _chars = FindObjectsByType<LocoMotion>(FindObjectsSortMode.None).ToList();
        textMesh.text = "";
       // Debug.LogWarning(_chars.Count);
    }

    public void Button()
    {
        if (_chars.Count > 0)
        {
            LocoMotion.OnCharacterSelected(GetNextCharacter());
        }
    }
    
    private LocoMotion GetNextCharacter()
    {
        if (_chars == null || _chars.Count == 0)
        {
            textMesh.text = "";
            return null; // Возвращаем null, если список пустой или не инициализирован
        }

        _currentIndex++;

        // Если достигли конца списка, возвращаемся в начало
        if (_currentIndex >= _chars.Count)
        {
            _currentIndex = 0;
        }
        
        textMesh.text = _chars[_currentIndex].gameObject.name;

        return _chars[_currentIndex];
    }
}
