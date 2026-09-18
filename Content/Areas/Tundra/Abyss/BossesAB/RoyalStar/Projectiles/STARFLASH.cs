using Stellamod.Core;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARFLASH : ModProjectile
{
    private float Scale => MathHelper.Lerp(0f, 2f, EasingFunction.OutQuad(Timer / 60f));
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.light = 0.6f;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
    }
    private void DrawStar()
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.FivePointedStar.Asset, Projectile.Center);
        drawer.scale = Vector2.One * Scale;
        drawer.color = Color.Lerp(Color.Gold, Color.Transparent, EasingFunction.OutExpo(Timer / 60f));
        drawer.color.A = 0;
        drawer.rotation = Main.GlobalTimeWrappedHourly * 2;
        Main.spriteBatch.Draw(drawer);


        drawer.color *= ExtraMath.Osc(0.25f, 1f, speed: 64);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
    }

    private void DrawMedallion()
    {
        float _medalAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.OutSine(Timer / 60f));
        SpriteBatch spriteBatch = Main.spriteBatch;
        var starAsset = AssetReferences.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.WarriorStarMedal.Asset;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(starAsset, Projectile.Center);
        drawer.color = Color.DarkOrange * _medalAlpha * 0.4f;
        drawer.color.A = 0;
        drawer.scale = Vector2.One * Scale;
        drawer.rotation = Main.GlobalTimeWrappedHourly * 2;
        spriteBatch.Draw(drawer);
        spriteBatch.Draw(drawer);


        drawer.color *= ExtraMath.Osc(0.25f, 1f, speed: 64);
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawStar();
     //   DrawMedallion();
        return false;
        //return base.PreDraw(ref lightColor);
    }
}
