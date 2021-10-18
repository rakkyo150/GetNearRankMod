using GetNearRankMod.Managers;
using GetNearRankMod.Utilities;
using Zenject;

namespace GetNearRankMod.Installers
{
    internal class MenuButtonInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<GetUsersData>().AsSingle();
            Container.Bind<PlaylistMaker>().AsSingle();

            Container.BindInterfacesAndSelfTo<MenuButtonManager>().AsSingle();
        }
    }
}
