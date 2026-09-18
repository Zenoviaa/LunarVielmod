using Stellamod.Core;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARTELEPORT : ModProjectile
{
    private float Scale => MathHelper.Lerp(0f, 2f, EasingFunction.OutQuad(Timer / 60f));
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            for(float f = 0; f< 16f; f++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(16, 16);
                Vector2 vel = pos - Projectile.Center;
                
                var sp = SparkleParticle.Spawn(pos, vel);
                sp.dampening = 0.05f;
                sp.fast = true;
                sp.Scale *= 0.6f;
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.WarriorStarMedal.Asset, Projectile.Center);
        drawer.scale = Vector2.One * Scale;
        drawer.color = Color.Lerp(Color.Gold, Color.Transparent, EasingFunction.OutExpo(Timer / 60f));
        drawer.color.A = 0;
        drawer.rotation = Main.GlobalTimeWrappedHourly * 2;
        Main.spriteBatch.Draw(drawer);


        drawer.color *= ExtraMath.Osc(0.25f, 1f, speed: 64);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}
