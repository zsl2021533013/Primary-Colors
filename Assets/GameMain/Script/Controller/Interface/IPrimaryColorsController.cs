namespace Script.View_Controller.Interface
{
    public interface IPrimaryColorsController
    {
        public void OnAwake();

        public void OnUpdate(float elapse);

        public void OnFixedUpdate(float elapse);

        public void OnGameShutdown();
    }
}