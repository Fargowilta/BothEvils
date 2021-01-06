using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BothEvils
{
    public class BothEvilsConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Crimson and Corruption Avoid Eachother")]
        [Tooltip("Corruption and Crimson biomes are randomly assigned half of the world that only they can generate in, forcing them to avoid eachother.")]
        [DefaultValue(true)]
        public bool CrimsonCorruptionAvoidEachother;

        [Label("Second Hardmode Evil")]
        [Tooltip("Generates a column of the second world evil at one random edge of the world when hardmode begins. For those who want more of a challenge.")]
        [DefaultValue(false)]
        public bool SecondHardmodeEvil;
    }
}