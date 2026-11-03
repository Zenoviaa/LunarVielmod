using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace Stellamod.Common.WitchySystem
{
    public class PotionHealthDisplaySet : ModResourceDisplaySet
    {
        public override void PreDrawResources(PlayerStatsSnapshot snapshot)
        {
            base.PreDrawResources(snapshot);

        }
        
        public override void DrawLife(SpriteBatch spriteBatch)
        {
            base.DrawLife(spriteBatch);
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Witchy.PotionGuard.Asset, Main.screenPosition);
            drawer.color = Color.White;
            drawer.worldPosition.X += Main.screenWidth - 128;
            drawer.worldPosition.Y += 111;
            spriteBatch.Draw(drawer);
        }
        public override void DrawMana(SpriteBatch spriteBatch)
        {
            base.DrawMana(spriteBatch);
        }

    }
}
