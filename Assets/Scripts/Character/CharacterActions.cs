using UnityEngine;

public class CharacterActions : CharacterAnimationParams
{
    [field: SerializeField] public CharacterSkinModule Skin { get; private set; }
    
    [field: Header("Test Weapon")]
    [field: SerializeField] public Weapon Weapon { get; private set; }

    private void Start()
    {
        TestEquip();
    }

    protected override void HandleDrawWeapon(bool isDrawWeapon)
    {
        if (!Weapon)
        {
            return;
        }
        base.HandleDrawWeapon(isDrawWeapon);

        if (!isDrawWeapon)
        {
            Weapon.gameObject.SetActive(true);
            TestEquip();
            return;
        }
        //временно
        Weapon.gameObject.SetActive(false);
    }
    
    
    private void TestEquip()
    {
        if (!Weapon)
        {
            return;
        }
        Weapon.Pickup();
        Weapon.Equip(Skin.BonesCollector);
    }
    
    //текущий OneShotClip
    protected override void HashParams()
    {
        base.HashParams();
        //находим хеш OneShotClip
    }

    protected override void UpdateParams()
    {
        base.UpdateParams();
        //обновляем хеш OneShotClip
    }
}
