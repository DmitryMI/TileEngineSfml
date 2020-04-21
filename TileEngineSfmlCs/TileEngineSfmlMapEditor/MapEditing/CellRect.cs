namespace TileEngineSfmlMapEditor.MapEditing
{
    public struct CellRect
    {
        public int XLeft { get; set; }
        public int YBottom { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public CellRect(int x, int y, int w, int h)
        {
            XLeft = x;
            YBottom = y;
            Width = w;
            Height = h;
        }
    }
}