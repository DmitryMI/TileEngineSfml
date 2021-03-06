﻿using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.ClientSide.TileObjects
{
    public interface ISpiritRenderer
    {
        void PreRendering();
        void Render(Vector2 iconPosition, Icon icon, TileObjectSpirit spirit);

        void SetViewCenter(Vector2 viewCenter);
        void PostRendering();
    }
}