using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;

namespace test_alt_trouble.Tiles.Furniture
{
    public class AltWorckbench : ModTile
    {
        public override void SetStaticDefaults() {
            // Properties
            Main.tileFrameImportant[Type] = false;
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileNoAttach[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
            
            // DustType = ModContent.DustType<Dusts.Sparkle>();
            // AdjTiles = new int[] { TileID.WorkBenches };
            
            
            // Placement
            TileObjectData.newTile.CoordinateHeights = new[] { 18, 16 };
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            
            // AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            AdjTiles = new int[] { TileID.WorkBenches, TileID.TinkerersWorkbench, TileID.Anvils, TileID.MythrilAnvil, TileID.LunarCraftingStation };    
            // Etc
            
            AddMapEntry(new Color(200, 200, 200), Language.GetText("Alt Worckbench"));
        }
        
        private readonly int animationFrameHight = 33;
        
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
            int uniqueAnimationFrame = Main.tileFrame[Type];
            uniqueAnimationFrame = uniqueAnimationFrame % 14;
            frameYOffset = uniqueAnimationFrame * animationFrameHight;
            
        }
        public override void AnimateTile(ref int frame, ref int frameCounter) {
            // Or, more compactly:
            if (++frameCounter >= 6)
            {
                frameCounter = 0;
                frame = ++frame % 14;
            }
            
            
        }

       
    }
}