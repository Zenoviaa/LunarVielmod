using Stellamod.Assets;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;


public class RekFireBreathV2 : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;

    private ref float Timer => ref Projectile.ai[0];
    private ref float RotationDir => ref Projectile.ai[1];
    private ref float DeathTimer => ref Projectile.ai[2];
    private float LifeTime => 60;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.idStaticNPCHitCooldown = 7;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = (int)LifeTime;

    }


    public override bool ShouldUpdatePosition()
    {
        return true;
    }

    public override void AI()
    {

        Timer++;
        if (Timer == 1 && Main.rand.NextBool(8))
        {
            SoundStyle fireballShoot = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1") with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(fireballShoot, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
        }

        if (Main.rand.NextBool(32))
        {
            float time = (float)Projectile.timeLeft / LifeTime;
            FaintSmokeParticle faintSmoke = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center, Projectile.velocity, Scale: Main.rand.NextFloat(0.2f, 0.4f));
            faintSmoke.color = Color.Lerp(Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f) * 0.5f;
            faintSmoke.fadeToColor = Color.DarkGray * 0.5f;
            faintSmoke.Scale = Main.rand.NextFloat(0.5f, 0.9f) * time;
            faintSmoke.behindLayer = true;
        }

        if (Main.rand.NextBool(12))
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.NextFloat(0.5f, 1f));
        }

        if (this.OwnedByLocalClient() && Main.rand.NextBool(128))
        {
            RotationDir = Main.rand.NextFloat(-1f, 1f);
            Projectile.netUpdate = true;
        }
        if (RotationDir != 0)
        {
            Timer++;
            Projectile.velocity = Projectile.velocity.RotatedBy(RotationDir * 0.05f);
        }
        if (DeathTimer == 1)
        {
            Timer++;
            Projectile.velocity *= 0.98f;
        }
        if (Timer >= LifeTime)
            Projectile.Kill();
        Lighting.AddLight(Projectile.Center, TorchID.Torch);
        Projectile.rotation += 0.05f;

    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        DeathTimer = 1;
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //  PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames, DrawLayer.OverNPCsWithOutline);
        return false;
    }

    public void DrawToRenderTargets()
    {
        float time = (float)Timer / LifeTime;
        time = 1f - time;
        float inverseTime = 1f - time;
        float maxRadius = 0.165f;
        float radius1 = MathHelper.Lerp(0f, maxRadius, EasingFunction.OutExpo(inverseTime));
        float radius2 = MathHelper.Lerp(maxRadius, 0f, EasingFunction.InExpo(inverseTime));
        float radius = MathHelper.Lerp(radius1, radius2, inverseTime);
        FlamethrowerRenderer.AddMetaball(Projectile.Center, time, radius);
        PixelationManager.QueueSpritebatchDrawAction(DrawBloom);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.OnFire3, 120);
    }

    private void DrawBloom(SpriteBatch sb, Vector2 screenPos)
    {
        float time = (float)Timer / LifeTime;
        time = 1f - time;
        float inverseTime = 1f - time;
        float maxRadius = 2f;
        float radius1 = MathHelper.Lerp(0f, maxRadius, EasingFunction.OutExpo(inverseTime));
        float radius2 = MathHelper.Lerp(maxRadius, 0f, EasingFunction.InExpo(inverseTime));
        float radius = MathHelper.Lerp(radius1, radius2, inverseTime);

        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomDrawer.color = Color.DarkRed * 0.15f * MathHelper.Lerp(1f, 0f, Timer / LifeTime);
        bloomDrawer.color.A = 0;
        bloomDrawer.scale *= 0.6f * radius;
        sb.Draw(bloomDrawer);
    }
}
