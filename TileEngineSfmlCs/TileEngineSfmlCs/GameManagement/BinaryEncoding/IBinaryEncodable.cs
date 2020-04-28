namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public interface IBinaryEncodable
    {
        int ByteLength { get; }

        /// <summary>
        /// Serializes data into package
        /// </summary>
        /// <param name="package">Target package</param>
        /// <param name="index">Starting index in package</param>
        /// <returns>Count of written bytes</returns>
        int ToByteArray(byte[] package, int index);
        void FromByteArray(byte[] data, int index);
    }
}