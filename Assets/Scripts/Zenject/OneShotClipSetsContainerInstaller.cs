using UnityEngine;
using Zenject;

public class OneShotClipSetsContainerInstaller : MonoInstaller
{
    [SerializeField] private OneShotClipSetsContainer oneShotClipSetsContainer;
    public override void InstallBindings()
    {
        Container.Bind<OneShotClipSetsContainer>().FromScriptableObject(oneShotClipSetsContainer).AsSingle().NonLazy();
    }
}