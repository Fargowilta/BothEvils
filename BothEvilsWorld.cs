using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.GameContent.Tile_Entities;
using Terraria.Graphics.Capture;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace BetterBothEvils
{
    public class BetterBothEvilsWorld : ModWorld
    {
        public static int grassSpread;
        public int crimsonSide = 0;
        public static int heartCount = 0;
        public static Vector2[] heartPos = new Vector2[100];

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int resetIndex = tasks.FindIndex(i => i.Name == "Reset");
            int jungleIndex = tasks.FindIndex(i => i.Name == "Jungle");
            if (resetIndex != -1)
            {
                tasks.Insert(resetIndex + 1, new PassLegacy("Set Crimson Side", SetCrimsonSide));
            }

            int CorruptionIndex = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Corruption"));
            if (CorruptionIndex != -1)
            {
                tasks[CorruptionIndex] = new PassLegacy("Corruption", (progress) => Corruption(progress, tasks[resetIndex] as PassLegacy));
            }

            int altarIndex = tasks.FindIndex(i => i.Name == "Altars");
            if (altarIndex != -1)
            {
                tasks[altarIndex] = new PassLegacy("Altars", Altar);
            }

            int tilecleanupIndex = tasks.FindIndex(i => i.Name == "Tile Cleanup");
            if (tilecleanupIndex != -1)
            {
                tasks[tilecleanupIndex] = new PassLegacy("Tile Cleanup", TileCleanup);
            }

            int microbiomesIndex = tasks.FindIndex(i => i.Name == "Micro Biomes");
            if (microbiomesIndex != -1)
            {
                tasks[microbiomesIndex] = new PassLegacy("Micro Biomes", (progress) => MicroBiomes(progress, tasks[resetIndex] as PassLegacy));
            }
        }

        public void Corruption(GenerationProgress progress, PassLegacy resetPass)
        {
            FieldInfo generationMethod = typeof(PassLegacy).GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic);
            WorldGenLegacyMethod method = (WorldGenLegacyMethod)generationMethod.GetValue(resetPass);
            var dungeonfield = method.Method.DeclaringType?.GetFields
            (
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.Static
            )
            .Single(x => x.Name == "dungeonSide");
            int dungeonSide = (int)dungeonfield.GetValue(method.Target);

            FieldInfo jungleField = typeof(WorldGen).GetField("JungleX", BindingFlags.NonPublic | BindingFlags.Static);
            int JungleX = (int)jungleField.GetValue(null);

            int i2;

            {
                progress.Message = Lang.gen[72].Value;
                int num = 0;
                while ((double)num < (double)Main.maxTilesX * 0.00045 * 0.5)
                {
                    float value = (float)((double)num / ((double)Main.maxTilesX * 0.00045 * 0.5));
                    progress.Set(value);
                    bool flag2 = false;
                    int num2 = 0;
                    int num3 = 0;
                    int num4 = 0;
                    while (!flag2)
                    {
                        int num5 = 0;
                        flag2 = true;
                        int num6 = Main.maxTilesX / 2;
                        int num7 = 200;
                        if (dungeonSide < 0)
                        {
                            num2 = WorldGen.genRand.Next(600, Main.maxTilesX - 320);
                            if (Main.maxTilesY < 1000) num2 = WorldGen.genRand.Next(100, Main.maxTilesX - 50);
                        }
                        else
                        {
                            num2 = WorldGen.genRand.Next(320, Main.maxTilesX - 600);
                            if (Main.maxTilesY < 1000) num2 = WorldGen.genRand.Next(50, Main.maxTilesX - 100);
                        }
                        if (crimsonSide == -1) // Crimson on left
                        {
                            if (num2 > Main.maxTilesX / 2)
                            {
                                flag2 = false;
                                continue;
                            }
                        }
                        else if (crimsonSide == 1)
                        {
                            if (num2 < Main.maxTilesX / 2)
                            {
                                flag2 = false;
                                continue;
                            }
                        }
                        if (Main.maxTilesY < 1000)
                        {
                            num3 = num2 - WorldGen.genRand.Next(50) - 50;
                            num4 = num2 + WorldGen.genRand.Next(50) + 50;
                            if (num3 < 50)
                            {
                                num3 = 50;
                            }
                            if (num4 > Main.maxTilesX - 50)
                            {
                                num4 = Main.maxTilesX - 50;
                            }
                            if (dungeonSide < 0 && num3 < 50)
                            {
                                num3 = 50;
                            }
                            else if (dungeonSide > 0 && num3 > Main.maxTilesX - 50)
                            {
                                num3 = Main.maxTilesX - 50;
                            }
                            num7 = 50;
                        }
                        else
                        {
                            num3 = num2 - WorldGen.genRand.Next(200) - 100;
                            num4 = num2 + WorldGen.genRand.Next(200) + 100;
                            if (num3 < 285)
                            {
                                num3 = 285;
                            }
                            if (num4 > Main.maxTilesX - 285)
                            {
                                num4 = Main.maxTilesX - 285;
                            }
                            if (dungeonSide < 0 && num3 < 400)
                            {
                                num3 = 400;
                            }
                            else if (dungeonSide > 0 && num3 > Main.maxTilesX - 400)
                            {
                                num3 = Main.maxTilesX - 400;
                            }
                        }
                        if (num2 > num6 - num7 && num2 < num6 + num7)
                        {
                            flag2 = false;
                        }
                        if (num3 > num6 - num7 && num3 < num6 + num7)
                        {
                            flag2 = false;
                        }
                        if (num4 > num6 - num7 && num4 < num6 + num7)
                        {
                            flag2 = false;
                        }
                        if (num2 > WorldGen.UndergroundDesertLocation.X && num2 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
                        {
                            flag2 = false;
                        }
                        if (num3 > WorldGen.UndergroundDesertLocation.X && num3 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
                        {
                            flag2 = false;
                        }
                        if (num4 > WorldGen.UndergroundDesertLocation.X && num4 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
                        {
                            flag2 = false;
                        }
                        for (int k = num3; k < num4; k++)
                        {
                            for (int l = 0; l < (int)Main.worldSurface; l += 5)
                            {
                                if (Main.tile[k, l].active() && Main.tileDungeon[(int)Main.tile[k, l].type])
                                {
                                    flag2 = false;
                                    break;
                                }
                                if (!flag2)
                                {
                                    break;
                                }
                            }
                        }
                        if (num5 < 200 && JungleX > num3 && JungleX < num4)
                        {
                            num5++;
                            flag2 = false;
                        }
                    }
                    CrimStart(num2, (int)WorldGen.worldSurfaceLow - 10);
                    for (int m = num3; m < num4; m++)
                    {
                        int num8 = (int)WorldGen.worldSurfaceLow;
                        while ((double)num8 < Main.worldSurface - 1.0)
                        {
                            if (Main.tile[m, num8].active())
                            {
                                int num9 = num8 + WorldGen.genRand.Next(10, 14);
                                for (int n = num8; n < num9; n++)
                                {
                                    if ((Main.tile[m, n].type == 59 || Main.tile[m, n].type == 60) && m >= num3 + WorldGen.genRand.Next(5) && m < num4 - WorldGen.genRand.Next(5))
                                    {
                                        Main.tile[m, n].type = 0;
                                    }
                                }
                                break;
                            }
                            num8++;
                        }
                    }
                    double num10 = Main.worldSurface + 40.0;
                    for (int num11 = num3; num11 < num4; num11++)
                    {
                        num10 += (double)WorldGen.genRand.Next(-2, 3);
                        if (num10 < Main.worldSurface + 30.0)
                        {
                            num10 = Main.worldSurface + 30.0;
                        }
                        if (num10 > Main.worldSurface + 50.0)
                        {
                            num10 = Main.worldSurface + 50.0;
                        }
                        i2 = num11;
                        bool flag3 = false;
                        int num12 = (int)WorldGen.worldSurfaceLow;
                        while ((double)num12 < num10)
                        {
                            if (Main.tile[i2, num12].active())
                            {
                                if (Main.tile[i2, num12].type == 53 && i2 >= num3 + WorldGen.genRand.Next(5) && i2 <= num4 - WorldGen.genRand.Next(5))
                                {
                                    Main.tile[i2, num12].type = 234;
                                }
                                if (Main.tile[i2, num12].type == 0 && (double)num12 < Main.worldSurface - 1.0 && !flag3)
                                {
                                    grassSpread = 0;
                                    SpreadGrass(i2, num12, 0, 199, true, 0);
                                }
                                flag3 = true;
                                if (Main.tile[i2, num12].wall == 216)
                                {
                                    Main.tile[i2, num12].wall = 218;
                                }
                                else if (Main.tile[i2, num12].wall == 187)
                                {
                                    Main.tile[i2, num12].wall = 221;
                                }
                                if (Main.tile[i2, num12].type == 1)
                                {
                                    if (i2 >= num3 + WorldGen.genRand.Next(5) && i2 <= num4 - WorldGen.genRand.Next(5))
                                    {
                                        Main.tile[i2, num12].type = 203;
                                    }
                                }
                                else if (Main.tile[i2, num12].type == 2)
                                {
                                    Main.tile[i2, num12].type = 199;
                                }
                                else if (Main.tile[i2, num12].type == 161)
                                {
                                    Main.tile[i2, num12].type = 200;
                                }
                                else if (Main.tile[i2, num12].type == 396)
                                {
                                    Main.tile[i2, num12].type = 401;
                                }
                                else if (Main.tile[i2, num12].type == 397)
                                {
                                    Main.tile[i2, num12].type = 399;
                                }
                            }
                            num12++;
                        }
                    }
                    int num13 = WorldGen.genRand.Next(10, 15);
                    for (int num14 = 0; num14 < num13; num14++)
                    {
                        int num15 = 0;
                        bool flag4 = false;
                        int num16 = 0;
                        while (!flag4)
                        {
                            num15++;
                            int num17 = WorldGen.genRand.Next(num3 - num16, num4 + num16);
                            int num18 = WorldGen.genRand.Next((int)(Main.worldSurface - (double)(num16 / 2)), (int)(Main.worldSurface + 100.0 + (double)num16));
                            if (num15 > 100)
                            {
                                num16++;
                                num15 = 0;
                            }
                            if (!Main.tile[num17, num18].active())
                            {
                                while (!Main.tile[num17, num18].active())
                                {
                                    num18++;
                                }
                                num18--;
                            }
                            else
                            {
                                while (Main.tile[num17, num18].active() && (double)num18 > Main.worldSurface)
                                {
                                    num18--;
                                }
                            }
                            if (num16 > 10 || (Main.tile[num17, num18 + 1].active() && Main.tile[num17, num18 + 1].type == 203))
                            {
                                WorldGen.Place3x2(num17, num18, 26, 1);
                                if (Main.tile[num17, num18].type == 26)
                                {
                                    flag4 = true;
                                }
                            }
                            if (num16 > 100)
                            {
                                flag4 = true;
                            }
                        }
                    }
                    num++;
                }
                //return;

                // Making the world evil -- corruption
                progress.Message = Lang.gen[20].Value;
                int num19 = 0;
                while ((double)num19 < (double)Main.maxTilesX * 0.00045 * 0.5)
                {
                    float value2 = (float)((double)num19 / ((double)Main.maxTilesX * 0.00045 * 0.5));
                    progress.Set(value2);
                    bool flag5 = false;
                    int num20 = 0;
                    int num21 = 0;
                    int num22 = 0;
                    while (!flag5)
                    {
                        int num23 = 0;
                        flag5 = true;
                        int num24 = Main.maxTilesX / 2;
                        int num25 = 200;
                        num20 = WorldGen.genRand.Next(320, Main.maxTilesX - 320);
                        num21 = num20 - WorldGen.genRand.Next(200) - 100;
                        num22 = num20 + WorldGen.genRand.Next(200) + 100;
                        if (Main.maxTilesX <= 640)
                        {
                            num20 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                            num21 = num20 - WorldGen.genRand.Next(50) - 50;
                            num22 = num20 + WorldGen.genRand.Next(50) + 50;
                            num25 = 50;
                        }
                        if (crimsonSide == -1) // Crimson on left
                        {
                            if (num20 < Main.maxTilesX / 2)
                            {
                                flag5 = false;
                                continue;
                            }
                        }
                        else if (crimsonSide == 1)
                        {
                            if (num20 > Main.maxTilesX / 2)
                            {
                                flag5 = false;
                                continue;
                            }
                        }

                        if (num21 < 285)
                        {
                            num21 = 285;
                        }
                        if (num22 > Main.maxTilesX - 285)
                        {
                            num22 = Main.maxTilesX - 285;
                        }
                        if (Main.maxTilesX < 1000)
                        {
                            num21 = Utils.Clamp(num21, 50, Main.maxTilesX - 50);
                        }
                        if (num20 > num24 - num25 && num20 < num24 + num25)
                        {
                            flag5 = false;
                        }
                        if (num21 > num24 - num25 && num21 < num24 + num25)
                        {
                            flag5 = false;
                        }
                        if (num22 > num24 - num25 && num22 < num24 + num25)
                        {
                            flag5 = false;
                        }
                        if (num20 > WorldGen.UndergroundDesertLocation.X && num20 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
                        {
                            flag5 = false;
                        }
                        if (num21 > WorldGen.UndergroundDesertLocation.X && num21 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
                        {
                            flag5 = false;
                        }
                        if (num22 > WorldGen.UndergroundDesertLocation.X && num22 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
                        {
                            flag5 = false;
                        }
                        for (int num26 = num21; num26 < num22; num26++)
                        {
                            for (int num27 = 0; num27 < (int)Main.worldSurface; num27 += 5)
                            {
                                if (Main.tile[num26, num27].active() && Main.tileDungeon[(int)Main.tile[num26, num27].type])
                                {
                                    flag5 = false;
                                    break;
                                }
                                if (!flag5)
                                {
                                    break;
                                }
                            }
                        }
                        if (num23 < 200 && JungleX > num21 && JungleX < num22)
                        {
                            num23++;
                            flag5 = false;
                        }
                    }
                    int num28 = 0;
                    for (int num29 = num21; num29 < num22; num29++)
                    {
                        if (num28 > 0)
                        {
                            num28--;
                        }
                        if (num29 == num20 || num28 == 0)
                        {
                            int num30 = (int)WorldGen.worldSurfaceLow;
                            while ((double)num30 < Main.worldSurface - 1.0)
                            {
                                if (Main.tile[num29, num30].active() || Main.tile[num29, num30].wall > 0)
                                {
                                    if (num29 == num20)
                                    {
                                        num28 = 20;
                                        ChasmRunner(num29, num30, WorldGen.genRand.Next(150) + 150, true);
                                        break;
                                    }
                                    if (WorldGen.genRand.Next(35) == 0 && num28 == 0)
                                    {
                                        num28 = 30;
                                        bool makeOrb = true;
                                        ChasmRunner(num29, num30, WorldGen.genRand.Next(50) + 50, makeOrb);
                                        break;
                                    }
                                    break;
                                }
                                else
                                {
                                    num30++;
                                }
                            }
                        }
                        int num31 = (int)WorldGen.worldSurfaceLow;
                        while ((double)num31 < Main.worldSurface - 1.0)
                        {
                            if (Main.tile[num29, num31].active())
                            {
                                int num32 = num31 + WorldGen.genRand.Next(10, 14);
                                for (int num33 = num31; num33 < num32; num33++)
                                {
                                    if ((Main.tile[num29, num33].type == 59 || Main.tile[num29, num33].type == 60) && num29 >= num21 + WorldGen.genRand.Next(5) && num29 < num22 - WorldGen.genRand.Next(5))
                                    {
                                        Main.tile[num29, num33].type = 0;
                                    }
                                }
                                break;
                            }
                            num31++;
                        }
                    }
                    double num34 = Main.worldSurface + 40.0;
                    for (int num35 = num21; num35 < num22; num35++)
                    {
                        num34 += (double)WorldGen.genRand.Next(-2, 3);
                        if (num34 < Main.worldSurface + 30.0)
                        {
                            num34 = Main.worldSurface + 30.0;
                        }
                        if (num34 > Main.worldSurface + 50.0)
                        {
                            num34 = Main.worldSurface + 50.0;
                        }
                        i2 = num35;
                        bool flag6 = false;
                        int num36 = (int)WorldGen.worldSurfaceLow;
                        while ((double)num36 < num34)
                        {
                            if (Main.tile[i2, num36].active())
                            {
                                if (Main.tile[i2, num36].type == 53 && i2 >= num21 + WorldGen.genRand.Next(5) && i2 <= num22 - WorldGen.genRand.Next(5))
                                {
                                    Main.tile[i2, num36].type = 112;
                                }
                                if (Main.tile[i2, num36].type == 0 && (double)num36 < Main.worldSurface - 1.0 && !flag6)
                                {
                                    grassSpread = 0;
                                    SpreadGrass(i2, num36, 0, 23, true, 0);
                                }
                                flag6 = true;
                                if (Main.tile[i2, num36].type == 1 && i2 >= num21 + WorldGen.genRand.Next(5) && i2 <= num22 - WorldGen.genRand.Next(5))
                                {
                                    Main.tile[i2, num36].type = 25;
                                }
                                if (Main.tile[i2, num36].wall == 216)
                                {
                                    Main.tile[i2, num36].wall = 217;
                                }
                                else if (Main.tile[i2, num36].wall == 187)
                                {
                                    Main.tile[i2, num36].wall = 220;
                                }
                                if (Main.tile[i2, num36].type == 2)
                                {
                                    Main.tile[i2, num36].type = 23;
                                }
                                if (Main.tile[i2, num36].type == 161)
                                {
                                    Main.tile[i2, num36].type = 163;
                                }
                                else if (Main.tile[i2, num36].type == 396)
                                {
                                    Main.tile[i2, num36].type = 400;
                                }
                                else if (Main.tile[i2, num36].type == 397)
                                {
                                    Main.tile[i2, num36].type = 398;
                                }
                            }
                            num36++;
                        }
                    }
                    for (int num37 = num21; num37 < num22; num37++)
                    {
                        for (int num38 = 0; num38 < Main.maxTilesY - 50; num38++)
                        {
                            if (Main.tile[num37, num38].active() && Main.tile[num37, num38].type == 31 && Main.tile[num37, num38].frameX < 36) // Add check to prevent ebonstone around crimson hearts.
                            {
                                int num39 = num37 - 13;
                                int num40 = num37 + 13;
                                int num41 = num38 - 13;
                                int num42 = num38 + 13;
                                for (int num43 = num39; num43 < num40; num43++)
                                {
                                    if (num43 > 10 && num43 < Main.maxTilesX - 10)
                                    {
                                        for (int num44 = num41; num44 < num42; num44++)
                                        {
                                            if (Math.Abs(num43 - num37) + Math.Abs(num44 - num38) < 9 + WorldGen.genRand.Next(11) && WorldGen.genRand.Next(3) != 0 && Main.tile[num43, num44].type != 31)
                                            {
                                                Main.tile[num43, num44].active(true);
                                                Main.tile[num43, num44].type = 25;
                                                if (Math.Abs(num43 - num37) <= 1 && Math.Abs(num44 - num38) <= 1)
                                                {
                                                    Main.tile[num43, num44].active(false);
                                                }
                                            }
                                            if (Main.tile[num43, num44].type != 31 && Math.Abs(num43 - num37) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(num44 - num38) <= 2 + WorldGen.genRand.Next(3))
                                            {
                                                Main.tile[num43, num44].active(false);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    num19++;
                }
            }
        }

        public void Altar(GenerationProgress progress)
        {
            progress.Message = Lang.gen[26].Value;
            int num = (int)((float)(Main.maxTilesX * Main.maxTilesY) * 2E-05f);
            for (int k = 0; k < num; k++)
            {
                progress.Set((float)k / (float)num);
                for (int l = 0; l < 10000; l++)
                {
                    int num2 = WorldGen.genRand.Next(1, Main.maxTilesX - 3);
                    int num3 = (int)(WorldGen.worldSurfaceHigh + 20.0);
                    int style = WorldGen.genRand.Next(2);
                    WorldGen.Place3x2(num2, num3, 26, style);
                    if (Main.tile[num2, num3].type == 26)
                    {
                        break;
                    }
                }
            }
        }

        public void TileCleanup(GenerationProgress progress)
        {
            {
                for (int k = 40; k < Main.maxTilesX - 40; k++)
                {
                    for (int l = 40; l < Main.maxTilesY - 40; l++)
                    {
                        if (!Main.tile[k, l].active() && Main.tile[k, l].liquid == 0 && WorldGen.genRand.Next(3) != 0 && WorldGen.SolidTile(k, l - 1))
                        {
                            int num = WorldGen.genRand.Next(15, 21);
                            for (int m = l - 2; m >= l - num; m--)
                            {
                                if (Main.tile[k, m].liquid >= 128)
                                {
                                    int num2 = 373;
                                    if (Main.tile[k, m].lava())
                                    {
                                        num2 = 374;
                                    }
                                    else if (Main.tile[k, m].honey())
                                    {
                                        num2 = 375;
                                    }
                                    int maxValue = l - m;
                                    if (WorldGen.genRand.Next(maxValue) <= 1)
                                    {
                                        Main.tile[k, l].type = (ushort)num2;
                                        Main.tile[k, l].frameX = 0;
                                        Main.tile[k, l].frameY = 0;
                                        Main.tile[k, l].active(true);
                                        break;
                                    }
                                }
                            }
                            if (!Main.tile[k, l].active())
                            {
                                num = WorldGen.genRand.Next(3, 11);
                                for (int n = l + 1; n <= l + num; n++)
                                {
                                    if (Main.tile[k, n].liquid >= 200)
                                    {
                                        int num3 = 373;
                                        if (Main.tile[k, n].lava())
                                        {
                                            num3 = 374;
                                        }
                                        else if (Main.tile[k, n].honey())
                                        {
                                            num3 = 375;
                                        }
                                        int num4 = n - l;
                                        if (WorldGen.genRand.Next(num4 * 3) <= 1)
                                        {
                                            Main.tile[k, l].type = (ushort)num3;
                                            Main.tile[k, l].frameX = 0;
                                            Main.tile[k, l].frameY = 0;
                                            Main.tile[k, l].active(true);
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!Main.tile[k, l].active() && WorldGen.genRand.Next(3) != 0)
                            {
                                Tile tile = Main.tile[k, l - 1];
                                if (TileID.Sets.Conversion.Sandstone[(int)tile.type] || TileID.Sets.Conversion.HardenedSand[(int)tile.type])
                                {
                                    Main.tile[k, l].type = 461;
                                    Main.tile[k, l].frameX = 0;
                                    Main.tile[k, l].frameY = 0;
                                    Main.tile[k, l].active(true);
                                }
                            }
                        }
                        if (Main.tile[k, l].type == 137)
                        {
                            if (Main.tile[k, l].frameY <= 52)
                            {
                                int num5 = -1;
                                if (Main.tile[k, l].frameX >= 18)
                                {
                                    num5 = 1;
                                }
                                if (Main.tile[k + num5, l].halfBrick() || Main.tile[k + num5, l].slope() != 0)
                                {
                                    Main.tile[k + num5, l].active(false);
                                }
                            }
                        }
                        else if (Main.tile[k, l].type == 162 && Main.tile[k, l + 1].liquid == 0)
                        {
                            Main.tile[k, l].active(false);
                        }
                        if (Main.tile[k, l].wall == 13 || Main.tile[k, l].wall == 14)
                        {
                            Main.tile[k, l].liquid = 0;
                        }
                        if (Main.tile[k, l].type == 31)
                        {
                            int num6 = (int)(Main.tile[k, l].frameX / 18);
                            int num7 = 0;
                            int num8 = k;
                            num7 += num6 / 2;
                            num6 %= 2;
                            num8 -= num6;
                            int num9 = (int)(Main.tile[k, l].frameY / 18);
                            int num10 = 0;
                            int num11 = l;
                            num10 += num9 / 2;
                            num9 %= 2;
                            num11 -= num9;
                            for (int num12 = 0; num12 < 2; num12++)
                            {
                                for (int num13 = 0; num13 < 2; num13++)
                                {
                                    int num14 = num8 + num12;
                                    int num15 = num11 + num13;
                                    Main.tile[num14, num15].active(true);
                                    Main.tile[num14, num15].slope(0);
                                    Main.tile[num14, num15].halfBrick(false);
                                    Main.tile[num14, num15].type = 31;
                                    Main.tile[num14, num15].frameX = (short)(num12 * 18 + 36 * num7);
                                    Main.tile[num14, num15].frameY = (short)(num13 * 18 + 36 * num10);
                                }
                            }
                        }
                        if (Main.tile[k, l].type == 12)
                        {
                            int num16 = (int)(Main.tile[k, l].frameX / 18);
                            int num17 = 0;
                            int num18 = k;
                            num17 += num16 / 2;
                            num16 %= 2;
                            num18 -= num16;
                            int num19 = (int)(Main.tile[k, l].frameY / 18);
                            int num20 = 0;
                            int num21 = l;
                            num20 += num19 / 2;
                            num19 %= 2;
                            num21 -= num19;
                            for (int num22 = 0; num22 < 2; num22++)
                            {
                                for (int num23 = 0; num23 < 2; num23++)
                                {
                                    int num24 = num18 + num22;
                                    int num25 = num21 + num23;
                                    Main.tile[num24, num25].active(true);
                                    Main.tile[num24, num25].slope(0);
                                    Main.tile[num24, num25].halfBrick(false);
                                    Main.tile[num24, num25].type = 12;
                                    Main.tile[num24, num25].frameX = (short)(num22 * 18 + 36 * num17);
                                    Main.tile[num24, num25].frameY = (short)(num23 * 18 + 36 * num20);
                                }
                                if (!Main.tile[num22, l + 2].active())
                                {
                                    Main.tile[num22, l + 2].active(true);
                                    if (!Main.tileSolid[(int)Main.tile[num22, l + 2].type] || Main.tileSolidTop[(int)Main.tile[num22, l + 2].type])
                                    {
                                        Main.tile[num22, l + 2].type = 0;
                                    }
                                }
                                Main.tile[num22, l + 2].slope(0);
                                Main.tile[num22, l + 2].halfBrick(false);
                            }
                        }
                        if (TileID.Sets.BasicChest[(int)Main.tile[k, l].type] && Main.tile[k, l].type < TileID.Count)
                        {
                            int num26 = (int)(Main.tile[k, l].frameX / 18);
                            int num27 = 0;
                            int num28 = k;
                            int num29 = l - (int)(Main.tile[k, l].frameY / 18);
                            while (num26 >= 2)
                            {
                                num27++;
                                num26 -= 2;
                            }
                            num28 -= num26;
                            int num30 = Chest.FindChest(num28, num29);
                            if (num30 != -1)
                            {
                                int type = Main.chest[num30].item[0].type;
                                if (type != 1156)
                                {
                                    if (type != 1260)
                                    {
                                        switch (type)
                                        {
                                            case 1569:
                                                num27 = 25;
                                                break;

                                            case 1571:
                                                num27 = 24;
                                                break;

                                            case 1572:
                                                num27 = 27;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        num27 = 26;
                                    }
                                }
                                else
                                {
                                    num27 = 23;
                                }
                            }
                            for (int num31 = 0; num31 < 2; num31++)
                            {
                                for (int num32 = 0; num32 < 2; num32++)
                                {
                                    int num33 = num28 + num31;
                                    int num34 = num29 + num32;
                                    Main.tile[num33, num34].active(true);
                                    Main.tile[num33, num34].slope(0);
                                    Main.tile[num33, num34].halfBrick(false);
                                    Main.tile[num33, num34].type = 21;
                                    Main.tile[num33, num34].frameX = (short)(num31 * 18 + 36 * num27);
                                    Main.tile[num33, num34].frameY = (short)(num32 * 18);
                                }
                                if (!Main.tile[num31, l + 2].active())
                                {
                                    Main.tile[num31, l + 2].active(true);
                                    if (!Main.tileSolid[(int)Main.tile[num31, l + 2].type] || Main.tileSolidTop[(int)Main.tile[num31, l + 2].type])
                                    {
                                        Main.tile[num31, l + 2].type = 0;
                                    }
                                }
                                Main.tile[num31, l + 2].slope(0);
                                Main.tile[num31, l + 2].halfBrick(false);
                            }
                        }
                        if (Main.tile[k, l].type == 28)
                        {
                            int num35 = (int)(Main.tile[k, l].frameX / 18);
                            int num36 = 0;
                            int num37 = k;
                            while (num35 >= 2)
                            {
                                num36++;
                                num35 -= 2;
                            }
                            num37 -= num35;
                            int num38 = (int)(Main.tile[k, l].frameY / 18);
                            int num39 = 0;
                            int num40 = l;
                            while (num38 >= 2)
                            {
                                num39++;
                                num38 -= 2;
                            }
                            num40 -= num38;
                            for (int num41 = 0; num41 < 2; num41++)
                            {
                                for (int num42 = 0; num42 < 2; num42++)
                                {
                                    int num43 = num37 + num41;
                                    int num44 = num40 + num42;
                                    Main.tile[num43, num44].active(true);
                                    Main.tile[num43, num44].slope(0);
                                    Main.tile[num43, num44].halfBrick(false);
                                    Main.tile[num43, num44].type = 28;
                                    Main.tile[num43, num44].frameX = (short)(num41 * 18 + 36 * num36);
                                    Main.tile[num43, num44].frameY = (short)(num42 * 18 + 36 * num39);
                                }
                                if (!Main.tile[num41, l + 2].active())
                                {
                                    Main.tile[num41, l + 2].active(true);
                                    if (!Main.tileSolid[(int)Main.tile[num41, l + 2].type] || Main.tileSolidTop[(int)Main.tile[num41, l + 2].type])
                                    {
                                        Main.tile[num41, l + 2].type = 0;
                                    }
                                }
                                Main.tile[num41, l + 2].slope(0);
                                Main.tile[num41, l + 2].halfBrick(false);
                            }
                        }
                        if (Main.tile[k, l].type == 26)
                        {
                            int num45 = (int)(Main.tile[k, l].frameX / 18);
                            int num46 = 0;
                            int num47 = k;
                            int num48 = l - (int)(Main.tile[k, l].frameY / 18);
                            while (num45 >= 3)
                            {
                                num46++;
                                num45 -= 3;
                            }
                            num47 -= num45;
                            for (int num49 = 0; num49 < 3; num49++)
                            {
                                for (int num50 = 0; num50 < 2; num50++)
                                {
                                    int num51 = num47 + num49;
                                    int num52 = num48 + num50;
                                    Main.tile[num51, num52].active(true);
                                    Main.tile[num51, num52].slope(0);
                                    Main.tile[num51, num52].halfBrick(false);
                                    Main.tile[num51, num52].type = 26;
                                    Main.tile[num51, num52].frameX = (short)(num49 * 18 + 54 * num46);
                                    Main.tile[num51, num52].frameY = (short)(num50 * 18);
                                }
                                if (!Main.tile[num47 + num49, num48 + 2].active() || !Main.tileSolid[(int)Main.tile[num47 + num49, num48 + 2].type] || Main.tileSolidTop[(int)Main.tile[num47 + num49, num48 + 2].type])
                                {
                                    Main.tile[num47 + num49, num48 + 2].active(true);
                                    if (!TileID.Sets.Platforms[(int)Main.tile[num47 + num49, num48 + 2].type] && (!Main.tileSolid[(int)Main.tile[num47 + num49, num48 + 2].type] || Main.tileSolidTop[(int)Main.tile[num47 + num49, num48 + 2].type]))
                                    {
                                        Main.tile[num47 + num49, num48 + 2].type = 0;
                                    }
                                }
                                Main.tile[num47 + num49, num48 + 2].slope(0);
                                Main.tile[num47 + num49, num48 + 2].halfBrick(false);
                                if (Main.tile[num47 + num49, num48 + 3].type == 28 && Main.tile[num47 + num49, num48 + 3].frameY % 36 >= 18)
                                {
                                    Main.tile[num47 + num49, num48 + 3].type = 0;
                                    Main.tile[num47 + num49, num48 + 3].active(false);
                                }
                            }
                            for (int num53 = 0; num53 < 3; num53++)
                            {
                                if ((Main.tile[num47 - 1, num48 + num53].type == 28 || Main.tile[num47 - 1, num48 + num53].type == 12) && Main.tile[num47 - 1, num48 + num53].frameX % 36 < 18)
                                {
                                    Main.tile[num47 - 1, num48 + num53].type = 0;
                                    Main.tile[num47 - 1, num48 + num53].active(false);
                                }
                                if ((Main.tile[num47 + 3, num48 + num53].type == 28 || Main.tile[num47 + 3, num48 + num53].type == 12) && Main.tile[num47 + 3, num48 + num53].frameX % 36 >= 18)
                                {
                                    Main.tile[num47 + 3, num48 + num53].type = 0;
                                    Main.tile[num47 + 3, num48 + num53].active(false);
                                }
                            }
                        }
                        if (Main.tile[k, l].type == 237 && Main.tile[k, l + 1].type == 232)
                        {
                            Main.tile[k, l + 1].type = 226;
                        }
                    }
                }
            }
        }

        public void MicroBiomes(GenerationProgress progress, PassLegacy resetPass)
        {
            FieldInfo generationMethod = typeof(PassLegacy).GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic);
            WorldGenLegacyMethod method = (WorldGenLegacyMethod)generationMethod.GetValue(resetPass);
            var dungeonfield = method.Method.DeclaringType?.GetFields
            (
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.Static
            )
            .Single(x => x.Name == "dungeonSide");
            int dungeonSide = (int)dungeonfield.GetValue(method.Target);

            progress.Message = Lang.gen[76].Value + "..Thin Ice";
            float num = (float)(Main.maxTilesX * Main.maxTilesY) / 5040000f;
            float num2 = (float)Main.maxTilesX / 4200f;
            int num3 = (int)((float)WorldGen.genRand.Next(3, 6) * num);
            int k = 0;
            while (k < num3)
            {
                if (Biomes<ThinIceBiome>.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface + 0x14, 0x32, 0xC8, 0x32), WorldGen.structures))
                {
                    k++;
                }
            }
            progress.Set(0.1f);
            LocalizedText localizedText = Lang.gen[76];
            progress.Message = ((localizedText != null) ? localizedText.ToString() : null) + "..Enchanted Swords";
            int num4 = (int)Math.Ceiling((double)num);
            int l = 0;
            while (l < num4)
            {
                Point origin;
                origin.Y = (int)WorldGen.worldSurface + WorldGen.genRand.Next(0x32, 0x64);
                if (WorldGen.genRand.Next(2) == 0)
                {
                    origin.X = WorldGen.genRand.Next(0x32, (int)((float)Main.maxTilesX * 0.3f));
                }
                else
                {
                    origin.X = WorldGen.genRand.Next((int)((float)Main.maxTilesX * 0.7f), Main.maxTilesX - 0x32);
                }
                if (Biomes<EnchantedSwordBiome>.Place(origin, WorldGen.structures))
                {
                    l++;
                }
            }
            progress.Set(0.2f);
            LocalizedText localizedText2 = Lang.gen[76];
            progress.Message = ((localizedText2 != null) ? localizedText2.ToString() : null) + "..Campsites";
            int num5 = (int)((float)WorldGen.genRand.Next(6, 0xC) * num);
            int m = 0;
            while (m < num5)
            {
                if (Biomes<CampsiteBiome>.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface, 0x32, 0xC8, 0x32), WorldGen.structures))
                {
                    m++;
                }
            }
            LocalizedText localizedText3 = Lang.gen[76];
            progress.Message = ((localizedText3 != null) ? localizedText3.ToString() : null) + "..Mining Explosives";
            progress.Set(0.25f);
            int num6 = (int)((float)WorldGen.genRand.Next(0xE, 0x1E) * num);
            int n = 0;
            while (n < num6)
            {
                if (Biomes<MiningExplosivesBiome>.Place(WorldGen.RandomWorldPoint((int)WorldGen.rockLayer, 0x32, 0xC8, 0x32), WorldGen.structures))
                {
                    n++;
                }
            }
            LocalizedText localizedText4 = Lang.gen[76];
            progress.Message = ((localizedText4 != null) ? localizedText4.ToString() : null) + "..Mahogany Trees";
            progress.Set(0.3f);
            int num7 = (int)((float)WorldGen.genRand.Next(6, 0xC) * num2);
            int num8 = 0;
            int num9 = 0;
            while (num8 < num7 && num9 < 0x4E20)
            {
                if (Biomes<MahoganyTreeBiome>.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface + 0x32, 0x32, 0x1F4, 0x32), WorldGen.structures))
                {
                    num8++;
                }
                num9++;
            }
            LocalizedText localizedText5 = Lang.gen[76];
            progress.Message = ((localizedText5 != null) ? localizedText5.ToString() : null) + "..Corruption Pits";
            progress.Set(0.4f);
            int num10 = (int)((float)WorldGen.genRand.Next(1, 3) * num);
            int num11 = 0;
            while (num11 < num10)
            {
                if (Biomes<CorruptionPitBiome>.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface, 0x32, 0x1F4, 0x32), WorldGen.structures))
                {
                    num11++;
                }
            }

            LocalizedText localizedText6 = Lang.gen[76];
            progress.Message = ((localizedText6 != null) ? localizedText6.ToString() : null) + "..Minecart Tracks";
            progress.Set(0.5f);
            TrackGenerator.Run((int)(10f * num), (int)(num * 25f) + 0xFA);
            progress.Set(1f);
        }

        public void SetCrimsonSide(GenerationProgress progress)
        {
            crimsonSide = (WorldGen.genRand.Next(2) == 0) ? -1 : 1;
            heartCount = 0;
        }

        public static void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true, byte color = 0)
        {
            try
            {
                if (WorldGen.InWorld(i, j, 1))
                {
                    if ((int)Main.tile[i, j].type == dirt && Main.tile[i, j].active() && ((double)j < Main.worldSurface || dirt != 0))
                    {
                        int num = i - 1;
                        int num2 = i + 2;
                        int num3 = j - 1;
                        int num4 = j + 2;
                        if (num < 0)
                        {
                            num = 0;
                        }
                        if (num2 > Main.maxTilesX)
                        {
                            num2 = Main.maxTilesX;
                        }
                        if (num3 < 0)
                        {
                            num3 = 0;
                        }
                        if (num4 > Main.maxTilesY)
                        {
                            num4 = Main.maxTilesY;
                        }
                        bool flag = true;
                        for (int k = num; k < num2; k++)
                        {
                            for (int l = num3; l < num4; l++)
                            {
                                if (!Main.tile[k, l].active() || !Main.tileSolid[(int)Main.tile[k, l].type])
                                {
                                    flag = false;
                                }
                                if (Main.tile[k, l].lava() && Main.tile[k, l].liquid > 0)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag && TileID.Sets.CanBeClearedDuringGeneration[(int)Main.tile[i, j].type])
                        {
                            if (grass != 23 || Main.tile[i, j - 1].type != 27)
                            {
                                if (grass != 199 || Main.tile[i, j - 1].type != 27)
                                {
                                    Main.tile[i, j].type = (ushort)grass;
                                    Main.tile[i, j].color(color);
                                    for (int m = num; m < num2; m++)
                                    {
                                        for (int n = num3; n < num4; n++)
                                        {
                                            if (Main.tile[m, n].active() && (int)Main.tile[m, n].type == dirt)
                                            {
                                                try
                                                {
                                                    if (repeat && grassSpread < 1000)
                                                    {
                                                        grassSpread++;
                                                        SpreadGrass(m, n, dirt, grass, true, 0);
                                                        grassSpread--;
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void CrimStart(int i, int j)
        {
            int crimDir = 1;
            heartCount = 0;
            // WorldGen.crimson = true;
            int num = j;
            if ((double)num > Main.worldSurface)
            {
                num = (int)Main.worldSurface;
            }
            while (!WorldGen.SolidTile(i, num))
            {
                num++;
            }
            int num2 = num;
            Vector2 vector = new Vector2((float)i, (float)num);
            Vector2 value = new Vector2((float)WorldGen.genRand.Next(-20, 21) * 0.1f, (float)WorldGen.genRand.Next(20, 201) * 0.01f);
            if (value.X < 0f)
            {
                crimDir = -1;
            }
            float num3 = (float)WorldGen.genRand.Next(15, 26);
            bool flag = true;
            int num4 = 0;
            while (flag)
            {
                num3 += (float)WorldGen.genRand.Next(-50, 51) * 0.01f;
                if (num3 < 15f)
                {
                    num3 = 15f;
                }
                if (num3 > 25f)
                {
                    num3 = 25f;
                }
                int num5 = (int)(vector.X - num3 / 2f);
                while ((float)num5 < vector.X + num3 / 2f)
                {
                    int num6 = (int)(vector.Y - num3 / 2f);
                    while ((float)num6 < vector.Y + num3 / 2f)
                    {
                        if (num6 > num2)
                        {
                            if ((double)(Math.Abs((float)num5 - vector.X) + Math.Abs((float)num6 - vector.Y)) < (double)num3 * 0.3)
                            {
                                Main.tile[num5, num6].active(false);
                                Main.tile[num5, num6].wall = 83;
                            }
                            else if ((double)(Math.Abs((float)num5 - vector.X) + Math.Abs((float)num6 - vector.Y)) < (double)num3 * 0.8 && Main.tile[num5, num6].wall != 83)
                            {
                                Main.tile[num5, num6].active(true);
                                Main.tile[num5, num6].type = 203;
                                if ((double)(Math.Abs((float)num5 - vector.X) + Math.Abs((float)num6 - vector.Y)) < (double)num3 * 0.6)
                                {
                                    Main.tile[num5, num6].wall = 83;
                                }
                            }
                        }
                        else if ((double)(Math.Abs((float)num5 - vector.X) + Math.Abs((float)num6 - vector.Y)) < (double)num3 * 0.3 && Main.tile[num5, num6].active())
                        {
                            Main.tile[num5, num6].active(false);
                            Main.tile[num5, num6].wall = 83;
                        }
                        num6++;
                    }
                    num5++;
                }
                if (vector.X > (float)(i + 50))
                {
                    num4 = -100;
                }
                if (vector.X < (float)(i - 50))
                {
                    num4 = 100;
                }
                if (num4 < 0)
                {
                    value.X -= (float)WorldGen.genRand.Next(20, 51) * 0.01f;
                }
                else if (num4 > 0)
                {
                    value.X += (float)WorldGen.genRand.Next(20, 51) * 0.01f;
                }
                else
                {
                    value.X += (float)WorldGen.genRand.Next(-50, 51) * 0.01f;
                }
                value.Y += (float)WorldGen.genRand.Next(-50, 51) * 0.01f;
                if ((double)value.Y < 0.25)
                {
                    value.Y = 0.25f;
                }
                if (value.Y > 2f)
                {
                    value.Y = 2f;
                }
                if (value.X < -2f)
                {
                    value.X = -2f;
                }
                if (value.X > 2f)
                {
                    value.X = 2f;
                }
                vector += value;
                if ((double)vector.Y > Main.worldSurface + 100.0)
                {
                    flag = false;
                }
            }
            num3 = (float)WorldGen.genRand.Next(40, 55);
            for (int k = 0; k < 50; k++)
            {
                int num7 = (int)vector.X + WorldGen.genRand.Next(-20, 21);
                int num8 = (int)vector.Y + WorldGen.genRand.Next(-20, 21);
                int num9 = (int)((float)num7 - num3 / 2f);
                while ((float)num9 < (float)num7 + num3 / 2f)
                {
                    int num10 = (int)((float)num8 - num3 / 2f);
                    while ((float)num10 < (float)num8 + num3 / 2f)
                    {
                        float num11 = (float)Math.Abs(num9 - num7);
                        float num12 = (float)Math.Abs(num10 - num8);
                        float num13 = 1f + (float)WorldGen.genRand.Next(-20, 21) * 0.01f;
                        float num14 = 1f + (float)WorldGen.genRand.Next(-20, 21) * 0.01f;
                        num11 *= num13;
                        num12 *= num14;
                        double num15 = Math.Sqrt((double)(num11 * num11 + num12 * num12));
                        if (num15 < (double)num3 * 0.25)
                        {
                            Main.tile[num9, num10].active(false);
                            Main.tile[num9, num10].wall = 83;
                        }
                        else if (num15 < (double)num3 * 0.4 && Main.tile[num9, num10].wall != 83)
                        {
                            Main.tile[num9, num10].active(true);
                            Main.tile[num9, num10].type = 203;
                            if (num15 < (double)num3 * 0.35)
                            {
                                Main.tile[num9, num10].wall = 83;
                            }
                        }
                        num10++;
                    }
                    num9++;
                }
            }
            int num16 = WorldGen.genRand.Next(5, 9);
            Vector2[] array = new Vector2[num16];
            for (int l = 0; l < num16; l++)
            {
                int num17 = (int)vector.X;
                int num18 = (int)vector.Y;
                int num19 = 0;
                bool flag2 = true;
                Vector2 vector2 = new Vector2((float)WorldGen.genRand.Next(-20, 21) * 0.15f, (float)WorldGen.genRand.Next(0, 21) * 0.15f);
                while (flag2)
                {
                    vector2 = new Vector2((float)WorldGen.genRand.Next(-20, 21) * 0.15f, (float)WorldGen.genRand.Next(0, 21) * 0.15f);
                    while ((double)(Math.Abs(vector2.X) + Math.Abs(vector2.Y)) < 1.5)
                    {
                        vector2 = new Vector2((float)WorldGen.genRand.Next(-20, 21) * 0.15f, (float)WorldGen.genRand.Next(0, 21) * 0.15f);
                    }
                    flag2 = false;
                    for (int m = 0; m < l; m++)
                    {
                        if ((double)value.X > (double)array[m].X - 0.75 && (double)value.X < (double)array[m].X + 0.75 && (double)value.Y > (double)array[m].Y - 0.75 && (double)value.Y < (double)array[m].Y + 0.75)
                        {
                            flag2 = true;
                            num19++;
                            break;
                        }
                    }
                    if (num19 > 10000)
                    {
                        break;
                    }
                }
                array[l] = vector2;
                CrimVein(new Vector2((float)num17, (float)num18), vector2);
            }
            for (int n = 0; n < heartCount; n++)
            {
                num3 = (float)WorldGen.genRand.Next(16, 21);
                int num20 = (int)heartPos[n].X;
                int num21 = (int)heartPos[n].Y;
                int num22 = (int)((float)num20 - num3 / 2f);
                while ((float)num22 < (float)num20 + num3 / 2f)
                {
                    int num23 = (int)((float)num21 - num3 / 2f);
                    while ((float)num23 < (float)num21 + num3 / 2f)
                    {
                        float num24 = (float)Math.Abs(num22 - num20);
                        float num25 = (float)Math.Abs(num23 - num21);
                        double num26 = Math.Sqrt((double)(num24 * num24 + num25 * num25));
                        if (num26 < (double)num3 * 0.4)
                        {
                            Main.tile[num22, num23].active(true);
                            Main.tile[num22, num23].type = 203;
                            Main.tile[num22, num23].wall = 83;
                        }
                        num23++;
                    }
                    num22++;
                }
            }
            for (int num27 = 0; num27 < heartCount; num27++)
            {
                num3 = (float)WorldGen.genRand.Next(10, 14);
                int num28 = (int)heartPos[num27].X;
                int num29 = (int)heartPos[num27].Y;
                int num30 = (int)((float)num28 - num3 / 2f);
                while ((float)num30 < (float)num28 + num3 / 2f)
                {
                    int num31 = (int)((float)num29 - num3 / 2f);
                    while ((float)num31 < (float)num29 + num3 / 2f)
                    {
                        float num32 = (float)Math.Abs(num30 - num28);
                        float num33 = (float)Math.Abs(num31 - num29);
                        double num34 = Math.Sqrt((double)(num32 * num32 + num33 * num33));
                        if (num34 < (double)num3 * 0.3)
                        {
                            Main.tile[num30, num31].active(false);
                            Main.tile[num30, num31].wall = 83;
                        }
                        num31++;
                    }
                    num30++;
                }
            }
            for (int num35 = 0; num35 < heartCount; num35++)
            {
                AddShadowOrb((int)heartPos[num35].X, (int)heartPos[num35].Y, false);
            }
            int num36 = Main.maxTilesX;
            int num37 = 0;
            vector.X = (float)i;
            vector.Y = (float)num2;
            num3 = (float)WorldGen.genRand.Next(25, 35);
            float num38 = (float)WorldGen.genRand.Next(0, 6);
            for (int num39 = 0; num39 < 50; num39++)
            {
                if (num38 > 0f)
                {
                    float num40 = (float)WorldGen.genRand.Next(10, 30) * 0.01f;
                    num38 -= num40;
                    vector.Y -= num40;
                }
                int num41 = (int)vector.X + WorldGen.genRand.Next(-2, 3);
                int num42 = (int)vector.Y + WorldGen.genRand.Next(-2, 3);
                int num43 = (int)((float)num41 - num3 / 2f);
                while ((float)num43 < (float)num41 + num3 / 2f)
                {
                    int num44 = (int)((float)num42 - num3 / 2f);
                    while ((float)num44 < (float)num42 + num3 / 2f)
                    {
                        float num45 = (float)Math.Abs(num43 - num41);
                        float num46 = (float)Math.Abs(num44 - num42);
                        float num47 = 1f + (float)WorldGen.genRand.Next(-20, 21) * 0.005f;
                        float num48 = 1f + (float)WorldGen.genRand.Next(-20, 21) * 0.005f;
                        num45 *= num47;
                        num46 *= num48;
                        double num49 = Math.Sqrt((double)(num45 * num45 + num46 * num46));
                        if (num49 < (double)num3 * 0.2 * ((double)WorldGen.genRand.Next(90, 111) * 0.01))
                        {
                            Main.tile[num43, num44].active(false);
                            Main.tile[num43, num44].wall = 83;
                        }
                        else if (num49 < (double)num3 * 0.45)
                        {
                            if (num43 < num36)
                            {
                                num36 = num43;
                            }
                            if (num43 > num37)
                            {
                                num37 = num43;
                            }
                            if (Main.tile[num43, num44].wall != 83)
                            {
                                Main.tile[num43, num44].active(true);
                                Main.tile[num43, num44].type = 203;
                                if (num49 < (double)num3 * 0.35)
                                {
                                    Main.tile[num43, num44].wall = 83;
                                }
                            }
                        }
                        num44++;
                    }
                    num43++;
                }
            }
            for (int num50 = num36; num50 <= num37; num50++)
            {
                int num51 = num2;
                while ((Main.tile[num50, num51].type == 203 && Main.tile[num50, num51].active()) || Main.tile[num50, num51].wall == 83)
                {
                    num51++;
                }
                int num52 = WorldGen.genRand.Next(15, 20);
                while (!Main.tile[num50, num51].active() && num52 > 0 && Main.tile[num50, num51].wall != 83)
                {
                    num52--;
                    Main.tile[num50, num51].type = 203;
                    Main.tile[num50, num51].active(true);
                    num51++;
                }
            }
            WorldGen.CrimEnt(vector, crimDir);
        }

        public static void CrimVein(Vector2 position, Vector2 velocity)
        {
            float num = (float)WorldGen.genRand.Next(15, 26);
            bool flag = true;
            Vector2 vector = velocity;
            Vector2 vector2 = position;
            int num2 = WorldGen.genRand.Next(100, 150);
            if (velocity.Y < 0f)
            {
                num2 -= 25;
            }
            while (flag)
            {
                num += (float)WorldGen.genRand.Next(-50, 51) * 0.02f;
                if (num < 15f)
                {
                    num = 15f;
                }
                if (num > 25f)
                {
                    num = 25f;
                }
                int num3 = (int)(position.X - num / 2f);
                while ((float)num3 < position.X + num / 2f)
                {
                    int num4 = (int)(position.Y - num / 2f);
                    while ((float)num4 < position.Y + num / 2f)
                    {
                        float num5 = Math.Abs((float)num3 - position.X);
                        float num6 = Math.Abs((float)num4 - position.Y);
                        double num7 = Math.Sqrt((double)(num5 * num5 + num6 * num6));
                        if (num7 < (double)num * 0.2)
                        {
                            Main.tile[num3, num4].active(false);
                            Main.tile[num3, num4].wall = 83;
                        }
                        else if (num7 < (double)num * 0.5 && Main.tile[num3, num4].wall != 83)
                        {
                            Main.tile[num3, num4].active(true);
                            Main.tile[num3, num4].type = 203;
                            if (num7 < (double)num * 0.4)
                            {
                                Main.tile[num3, num4].wall = 83;
                            }
                        }
                        num4++;
                    }
                    num3++;
                }
                velocity.X += (float)WorldGen.genRand.Next(-50, 51) * 0.05f;
                velocity.Y += (float)WorldGen.genRand.Next(-50, 51) * 0.05f;
                if ((double)velocity.Y < (double)vector.Y - 0.75)
                {
                    velocity.Y = vector.Y - 0.75f;
                }
                if ((double)velocity.Y > (double)vector.Y + 0.75)
                {
                    velocity.Y = vector.Y + 0.75f;
                }
                if ((double)velocity.X < (double)vector.X - 0.75)
                {
                    velocity.X = vector.X - 0.75f;
                }
                if ((double)velocity.X > (double)vector.X + 0.75)
                {
                    velocity.X = vector.X + 0.75f;
                }
                position += velocity;
                if (Math.Abs(position.X - vector2.X) + Math.Abs(position.Y - vector2.Y) > (float)num2)
                {
                    flag = false;
                }
            }
            heartPos[heartCount] = position;
            heartCount++;
        }

        public static void ChasmRunner(int i, int j, int steps, bool makeOrb = false)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (!makeOrb)
            {
                flag2 = true;
            }
            float num = (float)steps;
            Vector2 value;
            value.X = (float)i;
            value.Y = (float)j;
            Vector2 value2;
            value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            value2.Y = (float)WorldGen.genRand.Next(11) * 0.2f + 0.5f;
            int num2 = 5;
            double num3 = (double)(WorldGen.genRand.Next(5) + 7);
            while (num3 > 0.0)
            {
                if (num > 0f)
                {
                    num3 += (double)WorldGen.genRand.Next(3);
                    num3 -= (double)WorldGen.genRand.Next(3);
                    if (num3 < 7.0)
                    {
                        num3 = 7.0;
                    }
                    if (num3 > 20.0)
                    {
                        num3 = 20.0;
                    }
                    if (num == 1f && num3 < 10.0)
                    {
                        num3 = 10.0;
                    }
                }
                else if ((double)value.Y > Main.worldSurface + 45.0)
                {
                    num3 -= (double)WorldGen.genRand.Next(4);
                }
                if ((double)value.Y > Main.rockLayer && num > 0f)
                {
                    num = 0f;
                }
                num -= 1f;
                if (!flag && (double)value.Y > Main.worldSurface + 20.0)
                {
                    flag = true;
                    WorldGen.ChasmRunnerSideways((int)value.X, (int)value.Y, -1, WorldGen.genRand.Next(20, 40));
                    WorldGen.ChasmRunnerSideways((int)value.X, (int)value.Y, 1, WorldGen.genRand.Next(20, 40));
                }
                int num4;
                int num5;
                int num6;
                int num7;
                if (num > (float)num2)
                {
                    num4 = (int)((double)value.X - num3 * 0.5);
                    num5 = (int)((double)value.X + num3 * 0.5);
                    num6 = (int)((double)value.Y - num3 * 0.5);
                    num7 = (int)((double)value.Y + num3 * 0.5);
                    if (num4 < 0)
                    {
                        num4 = 0;
                    }
                    if (num5 > Main.maxTilesX - 1)
                    {
                        num5 = Main.maxTilesX - 1;
                    }
                    if (num6 < 0)
                    {
                        num6 = 0;
                    }
                    if (num7 > Main.maxTilesY)
                    {
                        num7 = Main.maxTilesY;
                    }
                    for (int k = num4; k < num5; k++)
                    {
                        for (int l = num6; l < num7; l++)
                        {
                            if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile[k, l].type != 31 && Main.tile[k, l].type != 22)
                            {
                                Main.tile[k, l].active(false);
                            }
                        }
                    }
                }
                if (num <= 2f && (double)value.Y < Main.worldSurface + 45.0)
                {
                    num = 2f;
                }
                if (num <= 0f)
                {
                    if (!flag2)
                    {
                        flag2 = true;
                        AddShadowOrb((int)value.X, (int)value.Y, true);
                    }
                    else if (!flag3)
                    {
                        flag3 = false;
                        bool flag4 = false;
                        int num8 = 0;
                        while (!flag4)
                        {
                            int num9 = WorldGen.genRand.Next((int)value.X - 25, (int)value.X + 25);
                            int num10 = WorldGen.genRand.Next((int)value.Y - 50, (int)value.Y);
                            if (num9 < 5)
                            {
                                num9 = 5;
                            }
                            if (num9 > Main.maxTilesX - 5)
                            {
                                num9 = Main.maxTilesX - 5;
                            }
                            if (num10 < 5)
                            {
                                num10 = 5;
                            }
                            if (num10 > Main.maxTilesY - 5)
                            {
                                num10 = Main.maxTilesY - 5;
                            }
                            if ((double)num10 > Main.worldSurface)
                            {
                                WorldGen.Place3x2(num9, num10, 26, 0);

                                if (Main.tile[num9, num10].type == 26)
                                {
                                    flag4 = true;
                                }
                                else
                                {
                                    num8++;
                                    if (num8 >= 10000)
                                    {
                                        flag4 = true;
                                    }
                                }
                            }
                            else
                            {
                                flag4 = true;
                            }
                        }
                    }
                }
                value += value2;
                value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.01f;
                if ((double)value2.X > 0.3)
                {
                    value2.X = 0.3f;
                }
                if ((double)value2.X < -0.3)
                {
                    value2.X = -0.3f;
                }
                num4 = (int)((double)value.X - num3 * 1.1);
                num5 = (int)((double)value.X + num3 * 1.1);
                num6 = (int)((double)value.Y - num3 * 1.1);
                num7 = (int)((double)value.Y + num3 * 1.1);
                if (num4 < 1)
                {
                    num4 = 1;
                }
                if (num5 > Main.maxTilesX - 1)
                {
                    num5 = Main.maxTilesX - 1;
                }
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesY)
                {
                    num7 = Main.maxTilesY;
                }
                for (int m = num4; m < num5; m++)
                {
                    for (int n = num6; n < num7; n++)
                    {
                        if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (Main.tile[m, n].type != 25 && n > j + WorldGen.genRand.Next(3, 20))
                            {
                                Main.tile[m, n].active(true);
                            }
                            if (steps <= num2)
                            {
                                Main.tile[m, n].active(true);
                            }
                            if (Main.tile[m, n].type != 31)
                            {
                                Main.tile[m, n].type = 25;
                            }
                        }
                    }
                }
                for (int num11 = num4; num11 < num5; num11++)
                {
                    for (int num12 = num6; num12 < num7; num12++)
                    {
                        if ((double)(Math.Abs((float)num11 - value.X) + Math.Abs((float)num12 - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (Main.tile[num11, num12].type != 31)
                            {
                                Main.tile[num11, num12].type = 25;
                            }
                            if (steps <= num2)
                            {
                                Main.tile[num11, num12].active(true);
                            }
                            if (num12 > j + WorldGen.genRand.Next(3, 20))
                            {
                                Main.tile[num11, num12].wall = 3;
                            }
                        }
                    }
                }
            }
        }

        public static void AddShadowOrb(int x, int y, bool shadowOrb)
        {
            if (x < 10 || x > Main.maxTilesX - 10)
            {
                return;
            }
            if (y < 10 || y > Main.maxTilesY - 10)
            {
                return;
            }
            for (int i = x - 1; i < x + 1; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (Main.tile[i, j].active() && Main.tile[i, j].type == 31)
                    {
                        return;
                    }
                }
            }
            short num = 0;
            if (!shadowOrb) // if (WorldGen.crimson)
            {
                num += 36;
            }
            Main.tile[x - 1, y - 1].active(true);
            Main.tile[x - 1, y - 1].type = 31;
            Main.tile[x - 1, y - 1].frameX = num;
            Main.tile[x - 1, y - 1].frameY = 0;
            Main.tile[x, y - 1].active(true);
            Main.tile[x, y - 1].type = 31;
            Main.tile[x, y - 1].frameX = (short)(18 + num);
            Main.tile[x, y - 1].frameY = 0;
            Main.tile[x - 1, y].active(true);
            Main.tile[x - 1, y].type = 31;
            Main.tile[x - 1, y].frameX = num;
            Main.tile[x - 1, y].frameY = 18;
            Main.tile[x, y].active(true);
            Main.tile[x, y].type = 31;
            Main.tile[x, y].frameX = (short)(18 + num);
            Main.tile[x, y].frameY = 18;
        }
    }
}