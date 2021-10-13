using GetNearRankMod.Managers;
using GetNearRankMod.Override;
using Zenject;

namespace GetNearRankMod.Installers
{
    internal class MenuButtonInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<ChangeRankRange>().AsSingle();
            Container.Bind<ExecuteBatch>().AsSingle();

            Container.BindInterfacesAndSelfTo<MenuButtonManager>().AsSingle();
        }
    }
}
