using Zenject;

public class ContainerInventoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ContainerInventory>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}