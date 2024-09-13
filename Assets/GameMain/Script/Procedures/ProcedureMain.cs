using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameMain.Script.Controller.Scene_System;
using GameMain.Scripts.Game;
using GameMain.Scripts.UI;
using GameMain.Scripts.Utility;
using GameMain.Scripts.Utility.QFramework_Extension;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMain.Scripts.Procedure
{
    public enum ProcedureStates
    {
        None,
        Launch,
        ChangeScene,
        Menu,
        Game
    }
    
    public class ProcedureMain : MonoBehaviour
    {
        public FSM<ProcedureStates> FSM = new FSM<ProcedureStates>();
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            FSM.AddState(ProcedureStates.Launch, new LaunchState(FSM, this));
            FSM.AddState(ProcedureStates.ChangeScene, new ChangeSceneState(FSM, this));
            FSM.AddState(ProcedureStates.Menu, new MenuState(FSM, this));
            FSM.AddState(ProcedureStates.Game, new GameState(FSM, this));

            FSM.StartState(ProcedureStates.Launch);
        }

        private void Update()
        {
            FSM.Update();
        }

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }

    public class LaunchState : AbstractState<ProcedureStates, ProcedureMain>
    {
        public LaunchState(FSM<ProcedureStates> fsm, ProcedureMain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
            UIKit.Config.PanelLoaderPool = new ResourcesPanelLoaderPool();
            UIKit.Root.ScreenSpaceOverlayRenderMode();
            UIKit.Root.SetResolution(1920, 1080, 0.5f);
            
            ChangeSceneState.nextState = ProcedureStates.Menu;
            ChangeSceneState.nextScenePath = PathManager.GetSceneAsset("Menu");

            if (!ES3.KeyExists("GameData"))
            {
                var gameData = new GameData();
                Resources.LoadAll<LevelConfigSO>(PathManager.GetDataAsset("Levels")).ForEach(so =>
                {
                    gameData.levelList.Add(new LevelConfig(so));
                });
                gameData.levelList.Find(l => l.sceneIndex == "1-1").enable = true;
                ES3.Save("GameData", gameData);
                GameState.levelList.AddRange(gameData.levelList);
            }
            else
            {
                GameState.levelList.AddRange(ES3.Load<GameData>("GameData").levelList);
            }

            GameState.levelList.Sort((a, b) => 
            {
                var partsA = a.sceneIndex.Split('-');
                var partsB = b.sceneIndex.Split('-');

                var firstNumberA = int.Parse(partsA[0]);
                var secondNumberA = int.Parse(partsA[1]);

                var firstNumberB = int.Parse(partsB[0]);
                var secondNumberB = int.Parse(partsB[1]);

                var firstComparison = firstNumberA.CompareTo(firstNumberB);

                return firstComparison == 0 ? secondNumberA.CompareTo(secondNumberB) : firstComparison;
            });
            
            mFSM.ChangeState(ProcedureStates.ChangeScene);
        }
    }
    
    public class ChangeSceneState : AbstractState<ProcedureStates, ProcedureMain>
    {
        public static ProcedureStates nextState = ProcedureStates.None;
        public static string nextScenePath = "";
        
        private AsyncOperation asyncOperation;
        private SceneChangePanel panel;
        private bool isFadingIn;
        
        public ChangeSceneState(FSM<ProcedureStates> fsm, ProcedureMain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            asyncOperation = null;
            isFadingIn = false;

            if (panel == null)
            {
                panel = UIKit.OpenPanel<SceneChangePanel>(UILevel.PopUI);
            }
            panel.FadeOut(() =>
            {
                asyncOperation = SceneManager.LoadSceneAsync(nextScenePath);
            });
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (asyncOperation is not null && asyncOperation.isDone && !isFadingIn)
            {
                isFadingIn = true;
                panel.FadeIn(UIKit.ClosePanel<SceneChangePanel>);
                mFSM.ChangeState(nextState);
            }
        }
    } 
    
    public class MenuState : AbstractState<ProcedureStates, ProcedureMain>
    {
        private MenuPanel panel;
        
        public MenuState(FSM<ProcedureStates> fsm, ProcedureMain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            panel = UIKit.OpenPanel<MenuPanel>(new MenuPanelData() { levelList = GameState.levelList });
            panel.levelBtnList.ForEach(lb => lb.onClick.AddListener(() =>
            {
                ChangeSceneState.nextState = ProcedureStates.Game;
                
                ChangeSceneState.nextScenePath = PathManager.GetLevelAsset(lb.LevelNumber);
                GameState.currentLevel = lb.Config;
                
                mFSM.ChangeState(ProcedureStates.ChangeScene);
            }));
        }

        protected override void OnExit()
        {
            base.OnExit();

            UIKit.ClosePanel<MenuPanel>();
        }
    }
    
    public class GameState : AbstractState<ProcedureStates, ProcedureMain>
    {
        private GameBase game = new PrimaryColorsGame();
        
        public static LevelConfig currentLevel;
        public static List<LevelConfig> levelList = new List<LevelConfig>();
        
        public GameState(FSM<ProcedureStates> fsm, ProcedureMain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            PrimaryColors.Interface.RegisterEvent<NextLevelEvent>(NextLevel);
            PrimaryColors.Interface.RegisterEvent<Return2MenuEvent>(Return2Menu);
            
            game.Initialize();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            game.Update(Time.deltaTime);
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            
            game.FixedUpdate(Time.fixedDeltaTime);
        }

        protected override void OnExit()
        {
            base.OnExit();
            
            PrimaryColors.Interface.UnRegisterEvent<NextLevelEvent>(NextLevel);
            PrimaryColors.Interface.UnRegisterEvent<Return2MenuEvent>(Return2Menu);
            
            game.Shutdown();
        }

        private void NextLevel(NextLevelEvent e)
        {
            var currentIndex = levelList.IndexOf(currentLevel);

            if (currentIndex == -1)
            {
                return;
            }

            if (e.getCollectibleObjectInThisLevel && currentLevel.hasCollectibleObject)
            {
                currentLevel.getCollectibleObject = true;
            }
                
            if (currentLevel == levelList.Last())
            {
                ChangeSceneState.nextState = ProcedureStates.Menu;
                ChangeSceneState.nextScenePath = PathManager.GetSceneAsset("Menu");
                
                mFSM.ChangeState(ProcedureStates.ChangeScene);
            }
            else
            {
                var nextScene = levelList[currentIndex + 1];
                    
                ChangeSceneState.nextState = ProcedureStates.Game;
                ChangeSceneState.nextScenePath = PathManager.GetLevelAsset(nextScene.sceneIndex);
                nextScene.enable = true;
                currentLevel = nextScene;
                
                mFSM.ChangeState(ProcedureStates.ChangeScene);
            }

            ES3.Save("GameData", new GameData() { levelList = levelList });
        }

        private void Return2Menu(Return2MenuEvent e)
        {
            ChangeSceneState.nextState = ProcedureStates.Menu;
            ChangeSceneState.nextScenePath = PathManager.GetSceneAsset("Menu");
                
            mFSM.ChangeState(ProcedureStates.ChangeScene);
        }
    }
}