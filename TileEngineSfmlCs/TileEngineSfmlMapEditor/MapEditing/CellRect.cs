namespace TileEngineSfmlMapEditor.MapEditing
{
    public struct CellRect
    {
        public int XLeft { get; set; }
        public int YBottom { get; set; }
        public int XRight { get; set; }
        public int YTop { get; set; }

        public CellRect(int x0, int y0, int x1, int y1)
        {
            XLeft = x0;
            YBottom = y0;
            XRight = x1;
            YTop = y1;
        }
    }
}