using ReLogic.Content;
using Stellamod.Common.Platforms;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;



public class BigMoltenPlatform : AbstractPlatformNPC
{
    private Asset<Texture2D> _glowMaskTexture;
    private Asset<Texture2D> _decorationTexture;
    public override Point GetPlatformSize()
    {
        return new Point(856, 358);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _glowMaskTexture ??= ModContent.Request<Texture2D>($"{Texture}_Glow");
        _decorationTexture ??= ModContent.Request<Texture2D>($"{Texture}_Decoration");
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_decorationTexture, NPC.Center);
        drawer.texture = _decorationTexture.Value;
        drawer.worldPosition += new Vector2(0, -218);
        drawer.sourceRect = null;
        drawer.color = Color.White;
        drawer.spriteEffects = SpriteEffects.None;
        drawer.scale = Vector2.One;
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.worldPosition = NPC.position;
        drawer.drawOrigin = Vector2.Zero;
        //drawer.texture = _decorationTexture.Value;
        drawer.color = Color.White;
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromNPC(NPC);
        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            drawer.worldPosition = NPC.position;
            drawer.worldPosition += f.ToRotationVector2() * 2;
            drawer.drawOrigin = Vector2.Zero;
            drawer.texture = _glowMaskTexture.Value;
            drawer.color = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 0.5f, speed: 3)) * 0.1f;
            drawer.color.A = 0;
            //drawer.texture = _decorationTexture.Value;
            spriteBatch.Draw(drawer);
        }

        return false;
    }

}

public class SmallMoltenPlatform : AbstractPlatformNPC
{
    private Asset<Texture2D> _glowMaskTexture;
    private Asset<Texture2D> _decorationTexture;

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _glowMaskTexture ??= ModContent.Request<Texture2D>($"{Texture}_Glow");
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.worldPosition = NPC.position;
        drawer.drawOrigin = Vector2.Zero;
        drawer.color = Color.White;
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.color = Color.White;
        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            drawer.worldPosition = NPC.position;
            drawer.worldPosition += f.ToRotationVector2() * 2;
            drawer.drawOrigin = Vector2.Zero;
            drawer.texture = _glowMaskTexture.Value;
            drawer.color = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 0.5f, speed: 3)) * 0.1f;
            drawer.color.A = 0;
            //drawer.texture = _decorationTexture.Value;
            spriteBatch.Draw(drawer);
        }


        return false;
    }

    public override Point GetPlatformSize()
    {
        return new Point(146, 150);
    }
}