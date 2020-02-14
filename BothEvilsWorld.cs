using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace BothEvils
{
    public class BothEvilsWorld : ModWorld
    {
        public bool crimson;

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int CorruptionIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));

            if (CorruptionIndex != -1)
            {
                //add task right before vanillas
                //tasks.Insert(CorruptionIndex, new PassLegacy("Set Crimson", SetCrimson));
                tasks.Insert(CorruptionIndex, new PassLegacy("Set Crimson", SetCrimson));
            }
        }

        private void SetCrimson(GenerationProgress progress)
        {
            crimson = WorldGen.crimson;
            WorldGen.crimson = true;
        }

        private void CreateBothEvils(/*GenerationProgress progress*/)
        {
            //progress.Message = "Creating Evils";

            //make vanilla create crimson in the next step no matter what was chosen
            WorldGen.crimson = false;

            //create corruption manually
            int i2;
            int num19 = 0;
            while ((double)num19 < (double)Main.maxTilesX * 0.00045)
            {
                float value2 = (float)((double)num19 / ((double)Main.maxTilesX * 0.00045));
                //progress.Set(value2);
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
                    if (num21 < 285)
                    {
                        num21 = 285;
                    }
                    if (num22 > Main.maxTilesX - 285)
                    {
                        num22 = Main.maxTilesX - 285;
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
                                    WorldGen.ChasmRunner(num29, num30, WorldGen.genRand.Next(150) + 150, true);
                                    break;
                                }
                                if (WorldGen.genRand.Next(35) == 0 && num28 == 0)
                                {
                                    num28 = 30;
                                    bool makeOrb = true;
                                    WorldGen.ChasmRunner(num29, num30, WorldGen.genRand.Next(50) + 50, makeOrb);
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
                        if (Main.tile[num37, num38].active() && Main.tile[num37, num38].type == 31)
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

        public override void PostWorldGen()
        {
            CreateBothEvils();
            WorldGen.crimson = crimson;
        }
    }
}