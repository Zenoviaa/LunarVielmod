using Stellamod.Common.BossBannerSystem;
using Stellamod.Effects.Generic;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Common.RarityRendering;

public class BossRewardRarity : SpecialRarity
{
    public override void DrawName(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
    {
        //Ok lets make the shader now
        Vector2 textPosition = new Vector2(line.X, line.Y);

        Vector2 textSize = line.Font.MeasureString(line.Text);
        // Get the center of the text.
        Vector2 textCenter = textSize * 0.5f;
        // The position to draw the text.
        // Get the position to draw the glow behind the text.
        Vector2 glowPosition = new(line.X + textCenter.X, line.Y + textCenter.Y / 1.5f);

        NoHitRarityShader noHitRarityShader = NoHitRarityShader.Instance;
        noHitRarityShader.Time = Main.GlobalTimeWrappedHourly * 12;
        noHitRarityShader.Strength = 0.01f;
        noHitRarityShader.NoiseTexture = AssetRegistry.Noise.PerlinBlurred;

        //Draw Backglow
        var texture = AssetRegistry.GlowMasks.SimpleGlowCircle;
        var drawer = SpritebatchDrawer.FromTextureAsset(texture.Asset, Main.screenPosition + glowPosition);
        drawer.color = Color.SkyBlue * 0.2f;
        drawer.color.A = 0;
        drawer.scale = new Vector2(1.7f, 0.3f) * 0.25f;
        spriteBatch.Draw(drawer);

        //Draw Color Wiggly text
        using (new SpritebatchContext(spriteBatch, SpritebatchParams.UI with { effect = noHitRarityShader.Effect }))
        {
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPosition, Color.Lerp(Color.SkyBlue, Color.White, ExtraMath.Osc(0f, 1f, speed: 5)), line.Rotation, line.Origin, line.BaseScale);

            //Draw Flaming Text
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPosition, line.Color, line.Rotation, line.Origin, line.BaseScale);
        }
    }
}
public class NoHitRarity : SpecialRarity
{
    public override void DrawName(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
    {
        //Ok lets make the shader now
        Vector2 textPosition = new Vector2(line.X, line.Y);

        Vector2 textSize = line.Font.MeasureString(line.Text);
        // Get the center of the text.
        Vector2 textCenter = textSize * 0.5f;
        // The position to draw the text.
        // Get the position to draw the glow behind the text.
        Vector2 glowPosition = new(line.X + textCenter.X, line.Y + textCenter.Y / 1.5f);

        NoHitRarityShader noHitRarityShader = NoHitRarityShader.Instance;
        noHitRarityShader.Time = Main.GlobalTimeWrappedHourly * 12;
        noHitRarityShader.Strength = 0.01f;
        noHitRarityShader.NoiseTexture = AssetRegistry.Noise.PerlinBlurred;
        
        //Draw Backglow
        var texture = AssetRegistry.GlowMasks.SimpleGlowCircle;
        var drawer = SpritebatchDrawer.FromTextureAsset(texture.Asset, Main.screenPosition + glowPosition);
        drawer.color = Color.Gold * 0.2f;
        drawer.color.A = 0;
        drawer.scale = new Vector2(1.7f, 0.3f) * 0.25f;
        spriteBatch.Draw(drawer);

        //Draw Star
        Vector2 startPosition = textPosition + new Vector2(textSize.X, textSize.Y * 0.5f);
        var starTexture = BossBanner.RequestStarTexture();
        drawer = SpritebatchDrawer.FromTextureAsset(starTexture, Main.screenPosition + startPosition + new Vector2(14, -3));
        drawer.color = Color.Lerp(Color.White, Color.DarkGray, ExtraMath.Osc(0f, 0.6f, speed: 3));
        spriteBatch.Draw(drawer);

        //Draw Color Wiggly text
        using (new SpritebatchContext(spriteBatch, SpritebatchParams.UI with { effect = noHitRarityShader.Effect }))
        {
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPosition, Color.Lerp(Color.Gold, Color.White, ExtraMath.Osc(0f, 1f, speed: 5)), line.Rotation, line.Origin, line.BaseScale );

            //Draw Flaming Text
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPosition, line.Color, line.Rotation, line.Origin, line.BaseScale);
        }
    }
}

public class BossRarityGlobalItem : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        base.SetDefaults(entity);
        int specialRarityType = ItemSets.SpecialRarity[entity.type];
        if (specialRarityType <= 0)
            return;
        switch (specialRarityType)
        {
            case 1:
                entity.rare = ModContent.RarityType<BossRewardRarity>();
                break;
            case 2:
                entity.rare = ModContent.RarityType<NoHitRarity>();
                break;
        }
    }
}

public abstract class SpecialRarity : ModRarity
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        //Register as a special drawing rarity
        SpecialRarityGlobalItem.SpecialRaritiesByID.Add(Type, this);
    }

    /// <summary>
    /// Draws the item name, spritebatch does not need to be begun/ended within this function
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="item"></param>
    /// <param name="line"></param>
    /// <param name="yOffset"></param>
    public abstract void DrawName(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset);
}

public class SpecialRarityGlobalItem : GlobalItem
{
    public static readonly Dictionary<int, SpecialRarity> SpecialRaritiesByID = new Dictionary<int, SpecialRarity>();
    public override void Unload()
    {
        base.Unload();
        SpecialRaritiesByID?.Clear();
    }

    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
    {
        if (line.Mod == "Terraria" && line.Name == "ItemName")
        {
            if (SpecialRaritiesByID.ContainsKey(item.rare))
            {
                SpecialRaritiesByID[item.rare].DrawName(Main.spriteBatch, item, line, ref yOffset);
                return false;
            }
        }
        return base.PreDrawTooltipLine(item, line, ref yOffset);
    }
}
