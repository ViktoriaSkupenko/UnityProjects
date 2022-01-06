﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Factorys;
 using Gameplay.Builds;
 using Gameplay.Builds.Data;
 using Gameplay.Builds.Data.Marks;
 using Gameplay.HubObject.Data;
using Gameplay.Map;
using Gameplay.UI.Game.Canvas;
 using Gameplay.UI.Menu;
 using Gameplay.Units;
using Infrastructure.SceneStates;
using Interface;
using Plugins.DIContainer;
using UnityEngine;

namespace Gameplay.GameSceneScript
{
    public class AvaibleHabToCreate : MonoBehaviour
    {
        [DI] private GameSceneData _sceneData;
        [DI] private GameSceneData _gameSceneData;

        private List<Build> _main = new List<Build>();
        private List<Build> _extra = new List<Build>();

        private void Awake()
        {
            var allbuild = _sceneData.Build;
            _main = allbuild.Where(x => x.MainDates.GetOrNull<TypeBuild>().Categor == TypeBuild.Category.Main && CheckAtEmptyNeksus(x)).ToList();
            _extra = allbuild.Where(x => x.MainDates.GetOrNull<TypeBuild>().Categor == TypeBuild.Category.Extra &&  CheckAtEmptyNeksus(x)).ToList();
        }

        private static bool CheckAtEmptyNeksus(Build x) => x.MainDates.GetOrNull<NeksusMark>()==null;

        public List<Unit> GetAvaibleUnit() => _sceneData.PlayerUnits;

        public Build GetNeksus => _gameSceneData.Level.Neksus;

        public List<Build> MainBuilds => _main;
        
        public List<Build> ExtraBuild => _extra;
    }
}