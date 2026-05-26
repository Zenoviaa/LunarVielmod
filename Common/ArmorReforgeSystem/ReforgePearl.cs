using ReLogic.Content;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Utilities;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Common.ArmorReforgeSystem;

public class ReforgePearl : UIPanel
{
    private readonly Asset<Texture2D> _pearlTextureAsset;
    private readonly Asset<Texture2D> _pearlSlotTextureAsset;
    public ReforgePearl()
    {
        _pearlTextureAsset = ModContent.Request<Texture2D>($"{ReforgeUISystem.RootTexturePath}Pearl", AssetRequestMode.AsyncLoad);
        _pearlSlotTextureAsset = ModContent.Request<Texture2D>($"{ReforgeUISystem.RootTexturePath}GlisteningPearlSlot", AssetRequestMode.AsyncLoad);
        Width.Set(96, 0f);
        Height.Set(96, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {

        this.QuickMouseInteraction();
        Player player = Main.LocalPlayer;
        int count = player.CountItem(ModContent.ItemType<GlisteningPearl>());
        Rectangle rect = GetDimensions().ToRectangle();
        spriteBatch.Draw(_pearlSlotTextureAsset.Value, rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.Draw(_pearlTextureAsset.Value, rect.TopLeft() + new Vector2(48), null, Color.White, 0,
            _pearlTextureAsset.Value.Size() * 0.5f, 1f, SpriteEffects.None, 0);

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, count.ToString(),
            rect.TopLeft() + new Vector2(48), Color.White, 0f, Vector2.Zero, new Vector2(1f), -1f, 1f);

    }
}