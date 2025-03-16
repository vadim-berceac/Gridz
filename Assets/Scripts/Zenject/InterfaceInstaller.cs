using UnityEngine;
using Zenject;

public class InterfaceInstaller : MonoInstaller
{
    [SerializeField] private GameObject prefab;
    public override void InstallBindings()
    {
        Container.Bind<Inventory>().FromComponentInNewPrefab(prefab).AsSingle().NonLazy();
    }
}