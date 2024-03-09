using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace test_alt_trouble.Items
{
	public class AltSword : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test_alt_trouble.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 380;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 120;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = 1;
			Item.noMelee = true;
			Item.knockBack = 12;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.BlackBolt;
			Item.shootSpeed = 7;
            Item.crit = 32;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddIngredient(ItemID.LunarBar, 25);
			recipe.AddIngredient(ItemID.BrokenHeroSword, 1);
			recipe.AddIngredient(ItemID.LivingFireBlock, 8);
			recipe.AddIngredient(ItemID.FragmentSolar, 60);
			recipe.AddIngredient(ItemID.FieryGreatsword, 1);
			recipe.AddTile<Tiles.Furniture.AltWorckbench>();
			recipe.Register();
		}
	}
}