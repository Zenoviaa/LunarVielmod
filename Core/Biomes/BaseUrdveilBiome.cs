using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.TitleSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Biomes
{
   
    public abstract class BaseUrdveilBiome : ModBiome
    {
        private string _lastCard;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (Main.CurrentFrameFlags.AnyActiveBossNPC)
            {
                return;
            }
            if (!string.IsNullOrEmpty(_lastCard) && _lastCard == DisplayName.Value)
                return;

            _lastCard = DisplayName.Value;
            TitleCardUISystem uiSystem = ModContent.GetInstance<TitleCardUISystem>();
            uiSystem.OpenUI(DisplayName.Value, 7);
            uiSystem.titleUIState.titleCardUI.LineTexture = ModContent.Request<Texture2D>(TitleCardUISystem.RootTexturePath + "UnderlineBiome");
        }
    }
}
