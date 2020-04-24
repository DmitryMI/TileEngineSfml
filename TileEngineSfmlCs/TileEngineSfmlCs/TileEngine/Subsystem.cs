namespace TileEngineSfmlCs.TileEngine
{
    public abstract class Subsystem
    {
        private Scene _scene;
        public Scene Scene => _scene;

        internal void SetScene(Scene scene)
        {
            _scene = scene;
        }

        public abstract void OnRegister();
        public abstract void OnUpdate();
        public abstract void OnUnregister();
    }
}
