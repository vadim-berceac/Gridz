using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [field: Header("Test Weapon")]
    [field: SerializeField] public WeaponData PrimaryWeaponData { get; private set; }
    
    [field: Header("Test Armor")]
    [field: SerializeField] public CharacterSkinData PrimaryArmorData { get; private set; }
    
    public Transform ViewInstance  {get; private set; }
    
    public void CreateItemsInstance()
    {
        ViewInstance = Instantiate(PrimaryWeaponData.View); 
    }
}
