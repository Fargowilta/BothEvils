using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BothEvils
{
    public class BothEvilsGlobalItem : GlobalItem
    {
        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if (context == "bossBag" && arg == ItemID.EyeOfCthulhuBossBag)
            {
                if (WorldGen.crimson)
                {
                    NPCLoader.blockLoot.Add(880);
                    NPCLoader.blockLoot.Add(2171);
                    if (Main.rand.Next(2) == 0)
                    {
                        player.QuickSpawnItem(56, Main.rand.Next(20) + 10 + (Main.rand.Next(20) + 10) + (Main.rand.Next(20) + 10));
                        player.QuickSpawnItem(59, Main.rand.Next(3) + 1);
                        player.QuickSpawnItem(47, Main.rand.Next(30) + 20);
                    }
                    else
                    {
                        NPCLoader.blockLoot.Remove(880);
                        NPCLoader.blockLoot.Remove(2171);
                    }
                }
                else
                {
                    NPCLoader.blockLoot.Add(47);
                    NPCLoader.blockLoot.Add(56);
                    NPCLoader.blockLoot.Add(59);
                    if (Main.rand.Next(2) == 0)
                    {
                        player.QuickSpawnItem(880, Main.rand.Next(20) + 10 + (Main.rand.Next(20) + 10) + (Main.rand.Next(20) + 10));
                        player.QuickSpawnItem(2171, Main.rand.Next(3) + 1);
                    }
                    else
                    {
                        NPCLoader.blockLoot.Remove(47);
                        NPCLoader.blockLoot.Remove(56);
                        NPCLoader.blockLoot.Remove(59);
                    }
                }
            }
        }
    }
}