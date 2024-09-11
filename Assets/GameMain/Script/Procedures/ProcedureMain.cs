using System.Collections.Generic;
using System.IO;
using GameMain.Script.Controller.Scene_System;
using GameMain.Scripts.Game;
using GameMain.Scripts.UI;
using GameMain.Scripts.Utility;
using GameMain.Scripts.Utility.QFramework_Extension;
using QFramework;
using Script.Architecture;
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
            
            var savePath = Application.persistentDataPath + "/Save";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            
            GameState.sceneList.AddRange(Resources.LoadAll<SceneConfig>("Data/Scenes"));
            GameState.sceneList.Sort((a, b) => 
            {
                var partsA = a.sceneNumber.Split('-');
                var partsB = b.sceneNumber.Split('-');

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

            panel = UIKit.OpenPanel<MenuPanel>();
            panel.startGameBtn.onClick.AddListener(() =>
            {
                ChangeSceneState.nextState = ProcedureStates.Game;
                
                ChangeSceneState.nextScenePath = PathManager.GetLevelAsset("2-10");
                GameState.currentScene = GameState.sceneList.Find(config => config.sceneNumber == "2-10");
                
                mFSM.ChangeState(ProcedureStates.ChangeScene);
            });
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
        
        public static SceneConfig currentScene;
        public static List<SceneConfig> sceneList = new List<SceneConfig>();
        
        public GameState(FSM<ProcedureStates> fsm, ProcedureMain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            PrimaryColors.Interface.RegisterEvent<NextLevelEvent>(NextLevel);
            
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
            
            game.Shutdown();
        }

        private void NextLevel(NextLevelEvent e)
        {
            var currentIndex = sceneList.IndexOf(currentScene);

            if (currentIndex == -1)
            {
                return;
            }
                
            if (currentIndex == sceneList.Count)
            {
                ChangeSceneState.nextState = ProcedureStates.Menu;
                ChangeSceneState.nextScenePath = PathManager.GetSceneAsset("Menu");
                
                mFSM.ChangeState(ProcedureStates.ChangeScene);
            }
            else
            {
                var nextScene = sceneList[currentIndex + 1];
                    
                ChangeSceneState.nextState = ProcedureStates.Game;
                ChangeSceneState.nextScenePath = PathManager.GetLevelAsset(nextScene.sceneNumber);
                currentScene = nextScene;
                
                mFSM.ChangeState(ProcedureStates.ChangeScene);
            }
        }
    }
}