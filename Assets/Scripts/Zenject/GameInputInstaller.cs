using UnityEngine;
using Zenject;

public class GameInputInstaller : MonoInstaller
{
    [SerializeField] private GameObject prefab;
    public override void InstallBindings()
    {
        Container.Bind<GameInput>().FromComponentInNewPrefab(prefab).AsSingle().NonLazy();
    }
}