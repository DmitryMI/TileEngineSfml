namespace TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items
{
    public abstract class Item : Obj
    {
        public override bool IsPassable => true;
        public override bool IsLightTransparent => true;
        public override bool IsGasTransparent => true;
    }
}
