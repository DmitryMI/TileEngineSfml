namespace TileEngineSfmlCs.GameManagement
{
    public enum NetworkAction
    {
        // Server -> Client
        TileObjectUpdate = 101,
        DialogFormUpdate = 102,

        // May be the same as TileObjectUpdate
        TileObjectSpawn = 103,

        DialogFormSpawn = 104,
        DialogFormServerClose = 105,
        CameraUpdate = 106,

        TileObjectDestroy = 107,
        PositionUpdate = 108,

        SoundUpdate = 109,

        // Client -> Server
        ControlInput = 201,
        DialogFormInput = 202,
        DialogFormUserClose = 203
    }
}