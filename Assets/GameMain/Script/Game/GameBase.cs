namespace GameMain.Scripts.Game
{
    public abstract class GameBase
    {
        public bool GameOver
        {
            get;
            protected set;
        }
        
        public virtual void Initialize() { }

        public virtual void Shutdown() { }

        public virtual void Update(float elapse) { }
        
        public virtual void FixedUpdate(float elapse) { }
    }
}