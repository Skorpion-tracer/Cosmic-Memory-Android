using CosmicMemory.Controllers;
using CosmicMemory.View;
using UnityEngine;
using Zenject;

namespace CosmicMemory.Installers
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField] private BackgroundLevel _background;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameContext>().AsSingle();
            Container.InstantiatePrefab(_background);
        }
    }
}
