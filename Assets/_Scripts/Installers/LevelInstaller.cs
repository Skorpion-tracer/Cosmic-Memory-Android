using CosmicMemory.Controllers;
using CosmicMemory.Models;
using CosmicMemory.View;
using UnityEngine;
using Zenject;

namespace CosmicMemory.Installers
{
    public sealed class LevelInstaller : MonoInstaller
    {
        [SerializeField] private GameFieldDatas _gameFieldDatas;
        [SerializeField] private CardPictures _cardPictures;

        public override void InstallBindings()
        {
            Container.Bind<CardPictures>().FromInstance(_cardPictures).AsSingle().NonLazy();
            Container.Bind<GameFieldDatas>().FromInstance(_gameFieldDatas).AsSingle().NonLazy();
            Container.Bind<GameField>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();

            Container.InstantiatePrefabResourceForComponent<GameWinUI>("Prefabs/GameWinUI");
            Container.InstantiatePrefabResourceForComponent<GameplayUI>("Prefabs/GameplayUI");

            _cardPictures.SetPictureBack();
        }
    }
}