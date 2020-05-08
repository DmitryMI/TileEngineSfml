using System.Collections.Generic;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Networking;
using TileEngineSfmlCs.TimeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.SoundManagement
{
    public class SoundManager
    {
        #region Singleton

        private static SoundManager _instance;

        public static SoundManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        private List<SoundClipInstance> _activeClips = new List<SoundClipInstance>();
        

        public SoundManager()
        {
            TimeManager.Instance.NextFrameEvent += OnNextFrame;
        }

        private void OnNextFrame()
        {
            for (int i = 0; i < _activeClips.Count; i++)
            {
                _activeClips[i].Duration -= TimeManager.Instance.DeltaTime;
                if (_activeClips[i].Duration <= 0)
                {
                    _activeClips.RemoveAt(i);
                    i--;
                }
            }
        }

        private int GenerateSoundInstanceId()
        {
            int id = 0;
            while (id < _activeClips.Count)
            {
                if (_activeClips[id] == null)
                {
                    break;
                }

                id++;
            }

            if (id == _activeClips.Count)
            {
                _activeClips.Add(null);
            }

            return id;
        }

        private void SyncSound(SoundClipInstance clipInstance, IEnumerable<Player> players, Reliability reliability)
        {
            if (clipInstance == null)
            {
                return;
            }
            if (players == null)
            {
                players = NetworkManager.Instance.GetPlayers();
            }

            // TODO Clip duration
            clipInstance.Duration = 2;

            NetworkManager.Instance.UpdateSound(clipInstance, players, reliability);
        }

        /// <summary>
        /// Plays global sound for collection of players
        /// </summary>
        /// <param name="players">Listening players. Pass null to select all connected players</param>
        /// <param name="clip">Sound clip</param>
        /// <param name="volume">Sound volume</param>
        public void PlaySound(IEnumerable<Player> players, SoundClip clip, double volume)
        {
            int instanceId = GenerateSoundInstanceId();
            SoundClipInstance clipInstance = new SoundClipInstance(instanceId, volume, clip);
            _activeClips[instanceId] = clipInstance;

            SyncSound(clipInstance, players, Reliability.Reliable);
        }

        /// <summary>
        /// Plays positional sound to collection of players
        /// </summary>
        /// <param name="players">Listening players. Pass null to select all connected players</param>
        /// <param name="clip">Sound clip</param>
        /// <param name="volume">Sound volume</param>
        /// <param name="position">Sound source position</param>
        public void PlaySound(IEnumerable<Player> players, SoundClip clip, double volume, Vector2Int position)
        {
            int instanceId = GenerateSoundInstanceId();
            SoundClipInstance clipInstance = new SoundClipInstance(instanceId, volume, clip, position);
            _activeClips[instanceId] = clipInstance;

            SyncSound(clipInstance, players, Reliability.Reliable);
        }

        /// <summary>
        /// Plays moving sound to collection of players
        /// </summary>
        /// <param name="players">Listening players. Pass null to select all connected players</param>
        /// <param name="clip">Sound clip</param>
        /// <param name="volume">Sound volume</param>
        /// <param name="positionProvider">Movable sound source</param>
        public void PlaySound(IEnumerable<Player> players, SoundClip clip, double volume, IPositionProvider positionProvider)
        {
            int instanceId = GenerateSoundInstanceId();
            SoundClipInstance clipInstance = new SoundClipInstance(instanceId, volume, clip, positionProvider);
            _activeClips[instanceId] = clipInstance;
            SyncSound(clipInstance, players, Reliability.Reliable);
        }


    }
}
