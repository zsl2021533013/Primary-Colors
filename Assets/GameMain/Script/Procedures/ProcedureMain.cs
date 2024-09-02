using System.IO;
using GameMain.Scripts.Game;
using GameMain.Scripts.UI;
using GameMain.Scripts.Utility;
using GameMain.Scripts.Utility.QFramework_Extension;
using QFramework;
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
        Main
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
            FSM.AddState(ProcedureStates.Main, new GameState(FSM, this));

            FSM.StartState(ProcedureStates.Launch);
        }

        private void Update()
        {
            FSM.Update();
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
            
            mFSM.ChangeState(ProcedureStates.ChangeScene);
        }
    }
    
    public class ChangeSceneState : AbstractState<ProcedureStates, ProcedureMain>
    {
        public static ProcedureStates nextState = ProcedureStates.None;
        public static string nextScenePath = "";
        
        private AsyncOperation asyncOperation;
        private SceneChangePanel panel;
        
        public ChangeSceneState(FSM<ProcedureStates> fsm, ProcedureMain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            asyncOperation = null;

            if (panel == null)
            {
                panel = UIKit.OpenPanel<SceneChangePanel>();
            }
            panel.FadeOut(() =>
            {
                asyncOperation = SceneManager.LoadSceneAsync(nextScenePath);
            });
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (asyncOperation is not null && asyncOperation.isDone && !panel.IsFading)
            {
                panel.FadeIn(null);
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
                ChangeSceneState.nextState = ProcedureStates.Main;
                ChangeSceneState.nextScenePath = PathManager.GetLevelAsset("1-1");
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
        
        public GameState(FSM<ProcedureStates> fsm, ProcedureMain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
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
            
            game.Shutdown();
        }
    }
}