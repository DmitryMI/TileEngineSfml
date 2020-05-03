using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using TileEngineSfmlCs.TileEngine.ResourceManagement;
using TileEngineSfmlCs.TileEngine.ResourceManagement.ResourceTypes;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.SoundManagement
{
    public class ClientSoundManager
    {
        #region Singleton

        private static ClientSoundManager _instance;

        public static ClientSoundManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        private List<Sound> _playingSounds = new List<Sound>();

        public ClientSoundManager()
        {
            TimeManager.Instance.NextFrameEvent += OnNextFrame;
        }

        private void OnNextFrame()
        {
            for (int i = 0; i < _playingSounds.Count; i++)
            {
                if (_playingSounds[i].Status == SoundStatus.Stopped)
                {
                    _playingSounds.RemoveAt(i);
                    i--;
                }
            }
        }

        public void OnSoundUpdate(SoundClipInstance soundClipInstance)
        {
            SoundClip clip = soundClipInstance.Clip;
            ResourceEntry resource = GameResources.Instance.GetEntry(clip.ResourceId);

            if (resource.LoadedValue == null)
            {
                SoundBuffer soundBuffer = new SoundBuffer(resource.ToByteArray());
                resource.LoadedValue = soundBuffer;
            }

            Sound sound = new Sound((SoundBuffer)resource.LoadedValue);
            _playingSounds.Add(sound);

            switch (soundClipInstance.SoundType)
            {
                case SoundType.Positional:
                    // TODO Positional sounds
                    sound.Play();
                    break;
                case SoundType.ObjectSource:
                    // TODO Movable sounds
                    sound.Play();
                    break;
                case SoundType.Global:
                    sound.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}
