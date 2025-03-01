using UnityEngine;
using Zenject;

public class CameraSystemInstaller : MonoInstaller
{
    [SerializeField] private GameObject prefab;
    public override void InstallBindings()
    {
        Container.Bind<CameraSystem>().FromComponentInNewPrefab(prefab).AsSingle().NonLazy();
    }
}