using UnityEngine;
using UnityEngine.UI;

public class HPSlider : MonoBehaviour
{
    [field: SerializeField] public Slider Slider { get; private set; }
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        CameraSystem.SelectedCharacterChanged += OnCharacterSelected;
    }

    private void OnCharacterSelected(CharacterInputLayer characterInputLayer)
    {
        CharacterInputLayer.SelectedCharacter.Health.OnDamage -= OnHealthChanged;
        CharacterInputLayer.SelectedCharacter.Health.OnDamage += OnHealthChanged;
        UpdateHealth();
    }

    private void OnHealthChanged(AnimationTypes.Type hitType, float value)
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        var hp = CharacterInputLayer.SelectedCharacter.Health.GetNormalizedHealth();
        Slider.value = hp;
    }

    private void OnDisable()
    {
        CameraSystem.SelectedCharacterChanged -= OnCharacterSelected;
        if (CharacterInputLayer.SelectedCharacter == null)
        {
            return;
        }
        CharacterInputLayer.SelectedCharacter.Health.OnDamage -= OnHealthChanged;
    }
}
