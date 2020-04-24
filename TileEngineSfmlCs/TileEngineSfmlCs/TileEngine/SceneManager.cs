namespace TileEngineSfmlCs.TileEngine
{
    public class SceneManager
    {
        #region Singleton
        
        private static SceneManager _instance;

        public static SceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SceneManager();
                }

                return _instance;
            }
        }
        #endregion

    }
}
