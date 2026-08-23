using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class MoonBlast : ModProjectile
{
    private enum BlastState
    {
        ChargeUp,
        Blast,
        Blast_Out
    }
    private float _glowAlpha;
    private float _blowtorchInterp;
    private ref float Timer => ref Projectile.ai[0];
    private BlastState State
    {
        get => (BlastState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private float ChargeTime => 60f;
    private float BlastTime => 6;
    private float BlastOutTime => 60f;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2048;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.hostile = false;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.ignoreWater = true;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 start = Projectile.Center;
        Vector2 end = start + Projectile.velocity;
        float lineWidth = 64;
        float collisionPoint = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, lineWidth, ref collisionPoint);
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case BlastState.ChargeUp:
                AI_Charge();
                break;
            case BlastState.Blast:
                AI_Blast();
                break;
            case BlastState.Blast_Out:
                AI_BlastOut();
                break;
        }
    }

    private void SwitchState(BlastState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }

    }
    private void AI_Charge()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle chargeSound = AssetRegistry.Sounds.Bishinine.BishinineFastfall;
            SoundEngine.PlaySound(chargeSound, Projectile.position);
        }
        if(Timer % 6 == 0)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.scaleRange *= 0.66f;
            var dp = DustParticle.Spawn(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 15, spawnParams);
            dp.Velocity = dp.Velocity.RotatedByRandom(MathHelper.ToRadians(25));
            dp.Velocity *= Main.rand.NextFloat(0.5f, 1f);
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.dampening = 0.05f;
        }
        _glowAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / ChargeTime));
        if(Timer >= ChargeTime)
        {
            SwitchState(BlastState.Blast);
        }
    }

    private void AI_Blast()
    {
        Projectile.hostile = true;
        Timer++;
        if(Timer == 2)
        {
            foreach(var proj in Main.ActiveProjectiles)
            {
                if(proj.type == ModContent.ProjectileType<VerliaDesperationMoon>())
                {
                    proj.ai[2] = 1;
                }
            }

            LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 2);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 1f;
            float numDust = 8;
            for (float f = 0; f < numDust; f++)
            {
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Blue;
                spawnParams.scaleRange *= 1;
                Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(10, 50);
                var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.05f;
            }
            numDust = 8;
            for (float f = 0; f < numDust; f++)
            {
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Blue;
                spawnParams.scaleRange *= 1;

                Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(10, 25);

                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, Main.rand.NextFloat(0f, 0.6f));
                var dp = SparkleParticle.Spawn(pos, vel);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.25f;
                dp.outerColor = Color.Blue;
            }

            SoundStyle mooNShotBlast = AssetRegistry.Sounds.Verlia.MoonshotBlast;
            mooNShotBlast.PitchVariance = 0.4f;
            SoundEngine.PlaySound(mooNShotBlast, Projectile.position);
        }
        _glowAlpha *= 0.8f;
        _blowtorchInterp = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / BlastTime));
        if(Timer >= BlastTime)
        {
            SwitchState(BlastState.Blast_Out);
        }
    }

    private void AI_BlastOut()
    {
        Projectile.hostile = false;
        Timer++;

        _glowAlpha *= 0.8f;
        _blowtorchInterp *= 0.9f;
        if(Timer >= BlastOutTime)
        {
            Projectile.Kill();
        }
    }

    private void DrawPixelatedBlowtorch(SpriteBatch sb, Vector2 screenPos)
    {
        //Draw the glow
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.MuzzleFlash, Projectile.Center);
        sbDrawer.rotation = Projectile.velocity.ToRotation();
        sbDrawer.color = Color.Lerp(Color.White, Color.Blue, 0.3f);
        sbDrawer.color *= _glowAlpha;
        sbDrawer.scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, _glowAlpha) * 1.75f;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);


        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.rotation = Projectile.velocity.ToRotation();
        //    glowDrawer.color *= _glowAlpha;
        glowDrawer.scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, _glowAlpha);
        glowDrawer.scale.X *= MathHelper.Lerp(0f, 4f, _blowtorchInterp);
        glowDrawer.scale.Y *= 0.8f;
        glowDrawer.scale *= 1.5f;
        glowDrawer.LeftCenterOrigin();
        glowDrawer.drawOrigin.X += 80;

        glowDrawer.worldPosition -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 186;
        glowDrawer.color = Color.White;
        glowDrawer.color *= _blowtorchInterp;
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);

        glowDrawer.color = Color.Blue;
        glowDrawer.color *= _blowtorchInterp;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.05f;
        Main.spriteBatch.Draw(glowDrawer);

        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.rotation = Projectile.velocity.ToRotation();
        //    glowDrawer.color *= _glowAlpha;
        glowDrawer.scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, _glowAlpha);
        glowDrawer.scale.X *= MathHelper.Lerp(0f, 4f, _blowtorchInterp);
        glowDrawer.scale.Y *= 0.8f;
        glowDrawer.LeftCenterOrigin();
        glowDrawer.drawOrigin.X += 80;
        glowDrawer.color *= _blowtorchInterp * 0.5f;
        glowDrawer.color.A = 0;
        glowDrawer.worldPosition -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 96;
        Main.spriteBatch.Draw(glowDrawer);

        glowDrawer.scale *= 0.8f;
        Main.spriteBatch.Draw(glowDrawer);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBlowtorch, DrawLayer.OverPlayers);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
