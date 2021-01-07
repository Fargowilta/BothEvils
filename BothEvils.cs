using Terraria.ModLoader;

namespace BothEvils
{
    public class BothEvils : Mod
    {
        public BothEvils()
        {
        }

        public override void Load()
        {
            Libvaxy.Reflection.InitializeCaches();
        }

        public override void Unload()
        {
            Libvaxy.Reflection.UnloadCaches();
        }
    }
}