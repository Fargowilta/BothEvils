using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BothEvils
{
    public class BothEvilsGlobalNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
			if (npc.type == 4 && !Main.expertMode)
            {
				if (WorldGen.crimson)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 47, Main.rand.Next(30) + 20, false, 0, false, false);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 56, Main.rand.Next(20) + 10, false, 0, false, false);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 56, Main.rand.Next(20) + 10, false, 0, false, false);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 56, Main.rand.Next(20) + 10, false, 0, false, false);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 59, Main.rand.Next(3) + 1, false, 0, false, false);
				}
				else
                {
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 880, Main.rand.Next(20) + 10, false, 0, false, false);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 880, Main.rand.Next(20) + 10, false, 0, false, false);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 880, Main.rand.Next(20) + 10, false, 0, false, false);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 2171, Main.rand.Next(3) + 1, false, 0, false, false);
				}
			}
		}
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
			if (type == NPCID.Dryad)
			{
				if (WorldGen.crimson)
				{ 
					foreach (Item item in shop.item)
					{
						if (item.type == ItemID.CrimsonSeeds)
						{
							shop.item[nextSlot].SetDefaults(ItemID.CorruptSeeds);
						}
						else if (item.type == ItemID.ViciousPowder)
                        {
							shop.item[nextSlot].SetDefaults(ItemID.VilePowder);
                        }
						//1.4
						/*else if (item.type == ItemID.CrimsonGrassWalls)
                        {
							shop.item[nextSlot].SetDefaults(ItemID.CorruptGrassWall)
                        }*/
					}
				}
				else
                {
					foreach (Item item in shop.item)
                    {
						if (item.type == ItemID.CorruptSeeds)
                        {
							shop.item[nextSlot].SetDefaults(ItemID.CrimsonSeeds);
                        }
						else if (item.type == ItemID.VilePowder)
                        {
							shop.item[nextSlot].SetDefaults(ItemID.ViciousPowder);
                        }
						//1.4
						/*else if (item.type == ItemID.CorruptGrassWalls)
                        {
							shop.item[nextSlot].SetDefaults(ItemID.CrimsonGrassWalls)
                        }*/
					}
				}
			}

			if (type == NPCID.Steampunker)
            {
				if (WorldGen.crimson)
                {
					foreach (Item item in shop.item)
                    {
						if (item.type == ItemID.RedSolution)
                        {
							shop.item[nextSlot].SetDefaults(ItemID.PurpleSolution);
                        }
						// 1.4
						/*else if (item.type == ItemID.FleshCloningVaat) 
                        {
							shop.item[nextSlot].SetDefaults(ItemID.DecayChamber)
                        }*/
                    }						
                }
				else
                {
					foreach (Item item in shop.item)
                    {
						if (item.type == ItemID.PurpleSolution)
                        {
							shop.item[nextSlot].SetDefaults(ItemID.RedSolution);
                        }
						// 1.4
						/*else if (item.type == ItemID.DecayChamber) 
                        {
							shop.item[nextSlot].SetDefaults(ItemID.FleshCloningVaat)
                        }*/
					}
				}
            }
		}
    }
}