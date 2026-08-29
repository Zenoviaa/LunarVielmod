using Stellamod.Common.Particles;
using Stellamod.Core.Particles;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class DeadEyeProj : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 24;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Projectile.Center = Main.player[Projectile.owner].Center;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Missile_Land") with { PitchVariance = 0.6f };
        SoundEngine.PlaySound(soundStyle, Projectile.position);
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Red, 45, 64);

        for (int i = 0; i < 16; i++)
        {
            Color color = (Main.rand.NextBool(2) ? Color.Red : Color.Green);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = target.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(8, 8),
                innerColor = (Color.Lerp(Color.White, color, 0.5f)).ToVector4(),
                outerColor = color.ToVector4()
            });
        }

        var fx = FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.Red,
            outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
        fx.Scale *= 1f;
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        FXUtil.PunchCamera(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 4, 4, 4);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class DeadEye : AbstractDashItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.damage = 150;
        Item.DamageType = DamageClass.Generic;
    }

    private float _timer;
    public override void BeginDash(Player player)
    {
        base.BeginDash(player);
        _timer = 0;
    }

    public override void UpdateDash(Player player)
    {
        base.UpdateDash(player);
        _timer++;
        if (_timer == 1)
        {
            player.velocity *= 4;
            var dashSound = AssetRegistry.Sounds.SteamPunking.DescendingDash2 with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(dashSound, player.position);
            if (Main.myPlayer == player.whoAmI)
            {
                ProjFirer proj = ProjFirer.From<DeadEyeProj>(player);
                var bonus = player.GetTotalDamage(DamageClass.Generic);
                float damage = player.HeldItem.damage;
                float newDamage = bonus.ApplyTo(damage);
                proj.damage = (int)newDamage;
                proj.New();
            }
        }

        Vector2 stretchPos = player.Center + Main.rand.NextVector2Circular(32, 32);
        var fx = FXUtil.GlowStretch(stretchPos, player.velocity);
        fx.OuterGlowColor = Main.rand.NextBool(2) ? Color.Red : Color.Green;

        for (int i = 0; i < 3; i++)
        {
            Color color = (Main.rand.NextBool(2) ? Color.Red : Color.Green);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = player.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(8, 8),
                innerColor = (Color.Lerp(Color.White, color, 0.5f)).ToVector4(),
                outerColor = color.ToVector4()
            });

        }

        var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(player.Center, -player.velocity.SafeNormalize(Vector2.Zero), newColor: Color.White);
        p2.fadeToColor = Color.DarkRed;
        p2.Scale *= 0.45f;

        if (_timer >= player.GetModPlayer<DashPlayer>().DashDuration / 2)
        {
            player.velocity *= 0.86f;
        }
    }


    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
        dashPlayer.DashDuration /= 4;
    }
}
