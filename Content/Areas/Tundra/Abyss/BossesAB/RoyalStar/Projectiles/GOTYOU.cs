using Stellamod.Common.Particles;
using Stellamod.Content.Buffs;
using Stellamod.Content.Rendering.GenericEffects;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class GotYouPlayer : ModPlayer
{
    public Vector2? grabPosition;
    public float grabRotation;
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if (grabPosition.HasValue)
        {
            Player.velocity = grabPosition.Value - Player.Center;
            Player.AddBuff(ModContent.BuffType<Stunlocked>(), 60);
            grabPosition = null;
        }
        float diff = grabRotation - 0.01f;
        if (diff > 0.01f)
        {
            Player.fullRotation = grabRotation;
            Player.fullRotationOrigin = Player.Size * 0.5f;
        }
    }
}

public class GOTYOU : ModProjectile,
    IDrawToRenderTarget
{
    private Player GrabbedPlayer => Main.player[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2; ;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        GotYouPlayer gotYouPlayer = GrabbedPlayer.GetModPlayer<GotYouPlayer>();
        gotYouPlayer.grabPosition = Projectile.Center;
        if (Timer % 4 == 0)
        {
            Vector2 spawnPosition = Projectile.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkOrange, Scale: spawnScale);
        }

        Projectile.velocity.Y += 1f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.09f;
    }

    private void ImpactFX()
    {
        GotYouPlayer gotYouPlayer = GrabbedPlayer.GetModPlayer<GotYouPlayer>();
        gotYouPlayer.grabRotation = 0;
        if (Main.netMode == NetmodeID.Server)
            return;
        ProjFirer firer = ProjFirer.From<STARPUNCH>(Projectile);
        firer.New();

        //Lemme grab the steamroller particles
        for (int i = 0; i < 4; i++)
        {
            FXUtil.MakeSoilParticle(Projectile.Center + Main.rand.NextVector2Circular(48, 32), -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1f));
        }

        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, timeLeft = 200, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, scale = 2f, timeLeft = 120, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, scale = 3f, timeLeft = 45, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, scale = 5f, timeLeft = 25, color = Color.DarkOrange });
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        ImpactFX();
        return base.OnTileCollide(oldVelocity);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    #region Trailing
    private float GetSpiralDashTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 96, completionRatio) * 0.16f;
    }

    private float GetSpiralDashTrailWidth2(float completionRatio)
    {
        return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
    }

    private Color GetSpiralDashTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
    }

    public void DrawToRenderTargets()
    {
        //Cool wind trail thing
        var verts1 = DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, Projectile.Size * 0.5f);
        var verts2 = DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, Projectile.Size * 0.5f);
        ModContent.GetInstance<SpiralingWindTrailRenderer>().PrepareForBigRendering(verts2);
        ModContent.GetInstance<SpiralingWindTrailRenderer>().PrepareForRendering(verts1);
    }
    #endregion
}
