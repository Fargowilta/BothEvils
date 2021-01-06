using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

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