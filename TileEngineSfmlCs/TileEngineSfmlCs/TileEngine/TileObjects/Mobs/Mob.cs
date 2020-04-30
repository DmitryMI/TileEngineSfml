using System;
using TileEngineSfmlCs.GameManagement.ClientControlInput;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Networking;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Mobs
{
    /// <summary>
    /// Something that is living
    /// </summary>
    public abstract class Mob : TileObject
    {
        private bool _isMoving;
        private Icon _activeIcon;
        private double _movingTimeRemaining;
        private Vector2 _movingOffset;
        private Vector2Int _movingStart;
        private Vector2Int _movingFinish;

        public override bool RequiresUpdates => true;

        public override bool IsLightTransparent => true;
        public override bool IsGasTransparent => true;


        public override Icon Icon => _activeIcon;
        public bool IsMoving => _isMoving;
        protected abstract Icon UpFacingIcon { get; }
        protected abstract Icon DownFacingIcon { get; }
        protected abstract Icon LeftFacingIcon { get; }
        protected abstract Icon RightFacingIcon { get; }
        protected abstract bool IgnoreObstacles { get; }
        protected abstract double CellsPerSecond { get; }
        protected abstract bool CanMove { get; }
       

        /// <summary>
        /// Is guaranteed to be invoked after base OnUpdate() in the same frame
        /// </summary>
        protected abstract void OnMobUpdate();

        public Mob()
        {
            InitIcon();
        }

        private void InitIcon()
        {
            _activeIcon = DownFacingIcon;
        }
        

        public void Move(MovementKey movement)
        {
            //LogManager.RuntimeLogger.Log("Movement: " + movement);
            if (Scene == null || InstanceId == -1)
            {
                //LogManager.RuntimeLogger.LogError("Trying to move mob that is not instantiated!");
                return;
            }

            if (!CanMove)
            {
                //LogManager.RuntimeLogger.Log("Mob cannot move!");
                return;
            }

            if (_isMoving)
            {
                //LogManager.RuntimeLogger.Log("Mob is already moving!");
                return;
            }

            int deltaX = 0, deltaY = 0;
            switch (movement)
            {
                case MovementKey.None:
                    break;
                case MovementKey.Up:
                    deltaY = -1; // TODO Project Y axis
                    _activeIcon = UpFacingIcon;
                    break;
                case MovementKey.Right:
                    deltaX = 1;
                    _activeIcon = RightFacingIcon;
                    break;
                case MovementKey.Down:
                    _activeIcon = DownFacingIcon;
                    deltaY = 1;
                    break;
                case MovementKey.Left:
                    _activeIcon = LeftFacingIcon;
                    deltaX = -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(movement), movement, null);
            }

            Vector2Int nextCell = Position + new Vector2Int(deltaX, deltaY);
            //LogManager.RuntimeLogger.Log("Next cell: " + nextCell);
            if (!Scene.IsInBounds(nextCell))
            {
                //LogManager.RuntimeLogger.Log("Next cell not in bounds!");
                return;
            }

            if(!IgnoreObstacles)
            {
                if (Scene.IsPassable(nextCell))
                {
                    //LogManager.RuntimeLogger.Log("MoveInternal:" + nextCell);
                    MoveInternal(nextCell);
                }
                else
                {
                    Scene.SendTryPass(nextCell, this);
                }
            }
            else
            {
                MoveInternal(nextCell);
            }
        }

        protected void MoveInternal(Vector2Int nextCell)
        {
            _movingStart = Position;
            _movingFinish = nextCell;
            _movingTimeRemaining = (nextCell - Position).Magnitude / CellsPerSecond;
            _isMoving = true;
        }

        internal override void OnUpdate()
        {
            if(_isMoving)
            {
                _movingTimeRemaining -= TimeManager.Instance.DeltaTime;
                if (_movingTimeRemaining <= 0)
                {
                    _movingOffset = new Vector2(0, 0);
                    Position = _movingFinish;
                    _isMoving = false;
                }
                else
                {
                    double xStep = CellsPerSecond * TimeManager.Instance.DeltaTime * Math.Sign(_movingFinish.X - _movingStart.X);
                    double yStep = CellsPerSecond * TimeManager.Instance.DeltaTime * Math.Sign(_movingFinish.Y - _movingStart.Y);
                    _movingOffset += new Vector2((float) xStep, (float) yStep);
                    if (Position.Equals(_movingStart))
                    {
                        if (_movingOffset.Magnitude > 0.5f)
                        {
                            Position = _movingFinish;
                            _movingOffset = -_movingOffset;
                        }
                    }
                }

                // TODO Offset overriding may be not so good.
                Offset = _movingOffset;
            }

            NetworkManager.Instance.UpdateTileObject(this, Reliability.Unreliable);
            
            OnMobUpdate();
        }
    }
}
