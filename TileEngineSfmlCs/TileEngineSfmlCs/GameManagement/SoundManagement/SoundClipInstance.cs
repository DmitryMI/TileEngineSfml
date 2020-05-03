using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils;

namespace TileEngineSfmlCs.GameManagement.SoundManagement
{
    public class SoundClipInstance : IBinaryEncodable
    {
        private int _soundInstanceId;
        private SoundClip _clip;
        private SoundType _soundType;
        private Vector2Int _sourcePosition;
        private int _sourceObjectId;
        private double _volume;
        private double _duration;

        public SoundClip Clip => _clip;

        public SoundType SoundType => _soundType;

        public Vector2Int SourcePosition => _sourcePosition;

        public int SourceObjectId => _sourceObjectId;

        public int SoundInstanceId => _soundInstanceId;

        public double Volume => _volume;

        public double Duration
        {
            get => _duration;
            set => _duration = value;
        }

        /// <summary>
        /// For Network usage only
        /// </summary>
        public SoundClipInstance()
        {

        }

        /// <summary>
        /// Creates global sound
        /// </summary>
        /// <param name="soundInstanceId">Sound instance Id derived from SoundManager</param>
        /// <param name="volume">Sound volume multiplicator</param>
        /// <param name="clip">Sound clip</param>
        public SoundClipInstance(int soundInstanceId, double volume, SoundClip clip)
        {
            _soundInstanceId = soundInstanceId;
            _volume = volume;
            _clip = clip;
            _soundType = SoundType.Global;
            _sourceObjectId = -1;
            _sourcePosition = Vector2Int.NegativeOne;
        }

        /// <summary>
        /// Creates sound on specified position
        /// </summary>
        /// <param name="soundInstanceId">Sound instance Id derived from SoundManager</param>
        /// /// <param name="volume">Sound volume multiplicator</param>
        /// <param name="clip">Sound clip</param>
        /// <param name="position">Location of sound source</param>
        public SoundClipInstance(int soundInstanceId, double volume, SoundClip clip, Vector2Int position)
        {
            _soundInstanceId = soundInstanceId;
            _volume = volume;
            _clip = clip;
            _soundType = SoundType.Positional;
            _sourceObjectId = -1;
            _sourcePosition = position;
        }

        /// <summary>
        /// Creates sound with movable source
        /// </summary>
        /// <param name="soundInstanceId">Sound instance Id derived from SoundManager</param>
        /// <param name="volume">Sound volume multiplicator</param>
        /// <param name="clip">Sound clip</param>
        /// <param name="sourceObject">Moveable sound source</param>
        public SoundClipInstance(int soundInstanceId, double volume, SoundClip clip, IPositionProvider sourceObject)
        {
            _soundInstanceId = soundInstanceId;
            _volume = volume;
            _clip = clip;
            _soundType = SoundType.ObjectSource;
            _sourceObjectId = sourceObject.InstanceId;
            _sourcePosition = Vector2Int.NegativeOne;
        }

        public int ByteLength => sizeof(int) +                  // SoundInstanceId
                                 _clip.ByteLength +             // SoundClip
                                 sizeof(byte) +                 // SoundType
                                 _sourcePosition.ByteLength +   // SourcePosition
                                 sizeof(int) +                  // SourceObjectId
                                 sizeof(double) +               // Volume
                                 sizeof(double);                // Duration
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            pos += BinaryUtils.Encode(_soundInstanceId, package, pos);
            pos += _clip.ToByteArray(package, pos);
            pos += BinaryUtils.Encode((byte) _soundType, package, pos);
            pos += _sourcePosition.ToByteArray(package, pos);
            pos += BinaryUtils.Encode(_sourceObjectId, package, pos);
            pos += BinaryUtils.Encode(_volume, package, pos);
            pos += BinaryUtils.Encode(_duration, package, pos);

            //LogManager.RuntimeLogger.Log($"Spawning sound. Id: {_soundInstanceId}, clip resource: {_clip.ResourceId}. Package: {BinaryUtils.PrintBinaryArray(package, index, ByteLength)}");

            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;

            _soundInstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            _clip.FromByteArray(data, pos);
            pos += _clip.ByteLength;
            _soundType = (SoundType) data[pos];
            pos += 1;
            _sourcePosition.FromByteArray(data, pos);
            pos += _sourcePosition.ByteLength;
            _soundInstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            _volume = BitConverter.ToDouble(data, pos);
            pos += sizeof(double);
            _duration = BitConverter.ToDouble(data, pos);
            pos += sizeof(double);

        }
    }
}
