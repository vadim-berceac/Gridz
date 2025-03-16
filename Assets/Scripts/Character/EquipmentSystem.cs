using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [field: Header("Test Weapon")]
    [field: SerializeField] public WeaponModelHolder PrimaryWeaponModelHolder { get; private set; }
    
    [field: Header("Test Armor")]
    [field: SerializeField] public CharacterSkinData PrimaryArmor { get; private set; }
    
}
