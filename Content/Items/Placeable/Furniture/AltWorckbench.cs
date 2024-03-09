using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace test_alt_trouble.Items.Placeable.Furniture
{
    public class AltWorckbench : ModItem
    {   
        
        public override void SetDefaults() {
            // ModContent.TileType<Tiles.Furniture.ExampleWorkbench>() retrieves the id of the tile that this item should place when used.
            // DefaultToPlaceableTile handles setting various Item values that placeable items use
            // Hover over DefaultToPlaceableTile in Visual Studio to read the documentation!
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AltWorckbench>());
            Item.width = 37; // The item texture's width
            Item.height = 37; // The item texture's height
            Item.value = 150;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.WorkBench)
                .AddIngredient(ItemID.MythrilAnvil)
                .AddIngredient(ItemID.LunarBar, 20)
                .AddIngredient(ItemID.Ectoplasm, 13)
                .AddIngredient(ItemID.Obsidian, 40)
                .AddIngredient(ItemID.LunarCraftingStation)
                .AddIngredient(ItemID.TinkerersWorkshop)
                .Register();
        }
    }
}