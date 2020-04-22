using System.Xml;
using TileEngineSfmlCs.TileEngine.SceneSerialization;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects
{
    /// <summary>
    /// Base class for all in-game objects
    /// </summary>
    public abstract class TileObject : IFieldSerializer
    {
        private Vector2Int _position;
        private Vector2 _offset;
        private Scene _scene;


        protected int LayerOrderInternal { get; set; }
        protected float RotationInternal { get; set; }


        internal int InstanceId;

        public int GetInstanceId() => InstanceId;

        internal void SetScene(Scene scene)
        {
            _scene = scene;
        }

        public Scene Scene => _scene;

        /// <summary>
        /// Defines visual offset of object. Does not affect physics
        /// </summary>
        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        /// <summary>
        /// Defines coordinate in cell map
        /// </summary>
        public Vector2Int Position
        {
            get => _position;
            set
            {
                Vector2Int oldPosition = _position;
                _position = value;
                if (oldPosition != _position)
                {
                    _scene?.ChangePosition(this, oldPosition);
                }
            }
        }

        public int LayerOrder
        {
            get => LayerOrderInternal;
            set => LayerOrderInternal = value;
        }

        public float Rotation => RotationInternal;

        public abstract Icon Icon { get; }

        public abstract Icon EditorIcon { get; }

        public abstract TileLayer Layer { get; }

        public abstract string VisibleName { get; }
        public abstract string ExamineDescription { get; }

        /// <summary>
        /// Defines if mob can walk through the object
        /// </summary>
        public abstract bool IsPassable { get;  }

        /// <summary>
        /// Defines if light is blocked or not
        /// </summary>
        public abstract bool IsLightTransparent { get;  }

        /// <summary>
        /// Defines if gas is blocked or not
        /// </summary>
        public abstract bool IsGasTransparent { get;  }

        #region Serialization
        
        public void AppendFields(XmlElement parentElement)
        {
            XmlDocument document = parentElement.OwnerDocument;
            if(document == null)
                return;

            SerializationUtils.Write(InstanceId, nameof(InstanceId), parentElement);
            SerializationUtils.Write(Position, nameof(Position), parentElement);
            SerializationUtils.Write(Offset, nameof(Offset), parentElement);
            AppendUserFields(parentElement);
        }

        public void ReadFields(XmlElement element)
        {
            InstanceId = SerializationUtils.ReadInt(nameof(InstanceId), element);
            Position = SerializationUtils.ReadFieldSerializer<Vector2Int>(nameof(Position), element);
            Offset = SerializationUtils.ReadFieldSerializer<Vector2>(nameof(Offset), element);

            ReadUserFields(element);
        }

        protected abstract void AppendUserFields(XmlElement baseElement);
        protected abstract void ReadUserFields(XmlElement baseElement);
        #endregion

        /// <summary>
        /// Invoked just after creation in editor
        /// </summary>
        internal virtual void OnEditorCreate()
        {

        }

        /// <summary>
        /// Invoked during in the same frame after creation
        /// </summary>
        internal virtual void OnCreate()
        {

        }

        /// <summary>
        /// Invoked each frame during objects lifetime
        /// </summary>
        internal virtual void OnUpdate()
        {

        }


        /// <summary>
        /// Invoked when object is being destroyed
        /// </summary>
        internal virtual void OnDestroy()
        {

        }
    }
}
