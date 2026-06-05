using Stellamod.Assets;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls;

public class SimpleSpikeball : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 6;
        Projectile.penetrate = 6;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle throwSound = SoundID.Item1 with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(throwSound, Projectile.position);
        }
        if (Main.rand.NextBool(32))
        {
            var fs = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center, Vector2.Zero, Scale: 0.2f);
            fs.color = Color.DarkGray * 0.1f;
            fs.Scale *= 0.2f;
        }

        Projectile.velocity.X *= 0.98f;
        Projectile.velocity.Y += 0.5f;
        Projectile.rotation += Projectile.velocity.X * 0.1f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for(float f = 0; f < 7; f++)
        {
            var fx = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8));
            fx.outerColor = Color.DarkGray;
            fx.Scale *= 0.4f;
            fx.dampening = 0.05f;
            fx.gravity = 0;
            fx.noTileCollide = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if(Projectile.velocity.X != oldVelocity.X)
        {
            Projectile.velocity.X *= -1;
        }
        if(Projectile.velocity.Y != oldVelocity.Y)
        {
            Projectile.velocity.Y *= -1;
        }
        return false;
        //return base.OnTileCollide(oldVelocity);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float scale = inScale * outScale;
        SpritebatchDrawer ballDrawer2 = SpritebatchDrawer.FromProjectile(Projectile);
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            ballDrawer2.worldPosition = pos;
            ballDrawer2.color = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)Projectile.oldPos.Length) * 0.07f;
            ballDrawer2.color.A = 0;
            ballDrawer2.rotation = Projectile.oldRot[i];
            ballDrawer2.scale = Vector2.One * scale;
            Main.spriteBatch.Draw(ballDrawer2);
        }
        SpritebatchDrawer ballDrawer = SpritebatchDrawer.FromProjectile(Projectile);

        ballDrawer.scale *= scale;
        Main.spriteBatch.Draw(ballDrawer);

        SpritebatchDrawer glintDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarGlint, Projectile.Center);
        glintDrawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 12, Projectile.identity);
        glintDrawer.color.A = 0;
        Main.spriteBatch.Draw(glintDrawer);
        return false;
    }

    public void DrawToRenderTargets()
    {
        //  throw new NotImplementedException();
    }
}
