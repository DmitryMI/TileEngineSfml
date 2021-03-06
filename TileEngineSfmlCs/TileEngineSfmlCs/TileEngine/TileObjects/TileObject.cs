﻿using System.Xml;
using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Networking;
using TileEngineSfmlCs.TileEngine.Containers;
using TileEngineSfmlCs.TileEngine.Interaction;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs;
using TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items;
using TileEngineSfmlCs.TypeManagement;
using TileEngineSfmlCs.TypeManagement.EntityTypes;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.TileObjects
{
    /// <summary>
    /// Base class for all in-game objects
    /// </summary>
    public abstract class TileObject : IFieldSerializer, IPositionProvider
    {
        [FieldEditorReadOnly("Use special replacing tool instead")]
        private Vector2Int _position;

        private Vector2 _offset;

        private Scene _scene;

        private Reliability _updateReliability;

        private float _rotation;

        private int _layerOrder;


        protected int LayerOrderInternal
        {
            get => _layerOrder;
            set
            {
                _layerOrder = value;
            }
        }

        protected float RotationInternal
        {
            get => _rotation;
            set
            {
                _rotation = value;
            }
        }

        protected Reliability UpdateReliability
        {
            get => _updateReliability;
            set => _updateReliability = value;
        }

        [FieldEditorReadOnly("Auto set only")]
        private int _instanceId;

        public int GetInstanceId() => _instanceId;
        
        /// <summary>
        /// Never invoke this method! Internal usage only
        /// </summary>
        /// <param name="instanceId"></param>
        public void SetInstanceId(int instanceId) => _instanceId = instanceId;

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
            set
            {
                _offset = value;
            }
        }

        public int InstanceId => GetInstanceId();

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
            set
            {
                LayerOrderInternal = value;
            }

        }

        public float Rotation => RotationInternal;

        public virtual EntityType GetEntityType()
        {
            return new AssemblyEntityType(GetType());
        }

        public abstract Icon Icon { get; }

        public abstract Icon EditorIcon { get; }

        /// <summary>
        /// May be invoked by mob, that is trying to go through this tile object. For example, a door can check human's ID Card
        /// </summary>
        /// <param name="sender">Object, that is willing to pass</param>
        public abstract void TryPass(TileObject sender);

        public void NetworkUpdate()
        {
            // Simply updates everything. But may be it is good to optimize icon update?
            NetworkManager.Instance.UpdateTileObject(this, UpdateReliability);
        }

        public abstract TileLayer Layer { get; }

        public abstract string VisibleName { get; }
        public abstract string ExamineDescription { get; }

        public abstract bool RequiresUpdates { get; }

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

        /// <summary>
        /// Container that holds this TileObject. If is null, then 'this' is free
        /// </summary>
        public abstract IObjectContainer Container { get; set; }

        /// <summary>
        /// Defines if object is visible, touchable and processable by physics
        /// </summary>
        public abstract bool IsActiveOnScene { get; }

        #region Serialization
        
        public void AppendFields(XmlElement parentElement)
        {
            XmlDocument document = parentElement.OwnerDocument;
            if(document == null)
                return;

            SerializationUtils.Write(_instanceId, nameof(_instanceId), parentElement);
            SerializationUtils.Write(Position, nameof(Position), parentElement);
            SerializationUtils.Write(Offset, nameof(Offset), parentElement);
            AppendUserFields(parentElement);
        }

        public void ReadFields(XmlElement element)
        {
            _instanceId = SerializationUtils.ReadInt(nameof(_instanceId), element, _instanceId);
            Position = SerializationUtils.ReadFieldSerializer<Vector2Int>(nameof(Position), element,  Position);
            Offset = SerializationUtils.ReadFieldSerializer<Vector2>(nameof(Offset), element, Offset);

            ReadUserFields(element);
        }

        protected abstract void AppendUserFields(XmlElement baseElement);
        protected abstract void ReadUserFields(XmlElement baseElement);
        #endregion

        #region Updating

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
        #endregion

        #region Interaction

        /// <summary>
        /// Handles item interaction from a mob
        /// </summary>
        /// <param name="appliedItem">Item, that is active during mob interaction attempt. If item is null, than mob is using hands</param>
        /// <param name="interactionSource">Interacting mob</param>
        /// <param name="descriptor">Interaction descriptor</param>
        /// <returns>If interaction chain must be finished, return FinishChain. Else return ContinueChain</returns>
        public abstract InteractionResult ApplyItem(Item appliedItem, Mob interactionSource,
            InteractionDescriptor descriptor);

        /// <summary>
        /// Handles drag-drop when 'this' is drag-receiver
        /// </summary>
        /// <param name="draggedObject">Object, that is dragged</param>
        /// <param name="interactionSource">Interacting mob</param>
        /// <param name="descriptor">Interaction descriptor</param>
        /// <returns>If interaction chain must be finished, return FinishChain. Else return ContinueChain</returns>
        public abstract InteractionResult DragDrop(TileObject draggedObject, Mob interactionSource,
            InteractionDescriptor descriptor);

        /// <summary>
        /// Handles drag-drop when 'this' is dragged target
        /// </summary>
        /// <param name="interactionSource">Interacting mob</param>
        /// <param name="descriptor">Interaction descriptor</param>
        /// <returns>If interaction chain must be finished, return FinishChain. Else return ContinueChain</returns>
        public abstract InteractionResult DragStart(Mob interactionSource, InteractionDescriptor descriptor);



        #endregion
    }
}
