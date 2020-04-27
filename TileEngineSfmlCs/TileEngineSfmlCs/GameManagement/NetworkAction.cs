﻿namespace TileEngineSfmlCs.GameManagement
{
    public enum NetworkAction
    {
        // Server -> Client
        TileObjectUpdate = 101,
        DialogFormUpdate = 102,
        TileObjectSpawn = 103,
        DialogFormSpawn = 104,

        // Client -> Server
        ControlInput = 201,
        DialogFormInput = 202,
    }
}