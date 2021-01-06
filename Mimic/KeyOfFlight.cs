using Terraria.ID;
using Terraria.ModLoader;

namespace BothEvils.Mimic
{
    public class KeyOfFlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Key of Flight");
            Tooltip.SetDefault("Charged with the essence of many souls");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 20;
            item.maxStack = 99;
            item.useAnimation = 20;
            item.useTime = 20;
            item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SoulofFlight, 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}