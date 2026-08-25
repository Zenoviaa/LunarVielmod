using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
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

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomanceAscendedDash : ModProjectile 
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private float _traveledDistance;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 120;
        Projectile.extraUpdates = 16;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + Vector2.UnitY * -900, Vector2.UnitY, 
            ModContent.ProjectileType<DeadRomanceAscendedCrashBlade>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner, ai0: target.whoAmI);
    }
    public override void AI()
    {
        base.AI();
        DeadRomancePlayer romancePlayer = Owner.GetModPlayer<DeadRomancePlayer>();
        romancePlayer.dashVelocity = (Projectile.Center - Owner.Center);
        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 5;

        Timer++;
        if(Timer == 1)
        {
            SoundStyle dash = AssetRegistry.Sounds.Melee.ExcaliburAscendDash;
            SoundEngine.PlaySound(dash, Projectile.position);
        }
        if (Timer % 16 == 0)
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
            SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.25f;
            sp.fast = true;
            sp.outerColor = Color.Yellow;
        }
        if (Timer % 8 == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
                SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
                sp.gravity = 0;
                sp.noTileCollide = true;
                sp.Scale *= 0.1f;
                sp.fast = true;
                sp.outerColor = Color.Yellow;
            }
        }

        if (this.OwnedByLocalClient())
        {
            if(Timer % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero,
                    ModContent.ProjectileType<DeadRomanceDelayedBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        _traveledDistance += Vector2.Distance(Projectile.position, Projectile.oldPosition);
     
        if (_traveledDistance > 8f)
        {
            _traveledDistance = 0f;
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
            SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.7f;
            sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);

            Vector2 spawnPos2 = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;

            if (Main.rand.NextBool(2))
            {
                Color color = new Color(41, 43, 66);
                var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f);
                sp2.gravity = 0;
                sp2.noTileCollide = true;
                sp2.Scale *= 0.35f;
                sp2.stretchScale2 = new Vector2(1f, 0.5f);
                sp2.offsetRot = 0;
                sp2.noRot = true;
            }


            int denom = 12;
            if (Main.rand.NextBool(denom))
            {
                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 stretchPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                var fx = FXUtil.GlowStretch(stretchPos, Projectile.velocity);
                fx.OuterGlowColor = Color.Gold;
            }
         
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        AI_OrientPlayer();
    }

    private void AI_OrientPlayer()
    {
        float rotation = Projectile.rotation;
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        Owner.itemRotation = rotation * Owner.direction;
        Owner.itemTime = 20;
        Owner.itemAnimation = 20;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(135));
    }

    private float GetTrailWidth(float completionRatio)
    {
        float outEase = (float)Projectile.timeLeft / 120f;
        return MathHelper.SmoothStep(96, 64, completionRatio) * 3 * outEase * EasingFunction.QuadraticBump(completionRatio);
    }
    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Black, completionRatio);
    }

    private void RenderTrail(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.InnerColor = Color.Goldenrod;
        laserShader.OuterColor = Color.DarkGoldenrod;
        laserShader.BloomTexture = TrailRegistry.BeamTrail;
        laserShader.LaserTexture = TrailRegistry.Beamlight;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderTrail);
        return false;
    }
}
