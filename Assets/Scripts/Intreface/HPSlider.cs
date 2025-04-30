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

    private void OnCharacterSelected(Character character)
    {
        Character.SelectedCharacter.Health.OnDamage -= OnHealthChanged;
        Character.SelectedCharacter.Health.OnDamage += OnHealthChanged;
        UpdateHealth();
    }

    private void OnHealthChanged(AnimationTypes.Type hitType, float value)
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        var hp = Character.SelectedCharacter.Health.GetNormalizedHealth();
        Slider.value = hp;
    }

    private void OnDisable()
    {
        CameraSystem.SelectedCharacterChanged -= OnCharacterSelected;
        if (Character.SelectedCharacter == null)
        {
            return;
        }
        Character.SelectedCharacter.Health.OnDamage -= OnHealthChanged;
    }
}
