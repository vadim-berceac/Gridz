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
        // Debug.Log(characterInputLayer);
        // Debug.Log(characterInputLayer.Health);
        // Debug.Log(characterInputLayer.Health.NormalizedHealth);
        // var hp = characterInputLayer.Health.NormalizedHealth;
        // Slider.value = hp;
    }

    private void OnDisable()
    {
        CameraSystem.SelectedCharacterChanged -= OnCharacterSelected;
    }
}
