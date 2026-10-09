using ReLogic.Utilities;
using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem;
public class SpiritHarpHeld : ModProjectile
{
    SlotId _soundSlot;
    ref float Timer => ref Projectile.ai[0];
    ref float Dir => ref Projectile.ai[1];
    Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.light = 0.6f;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            _soundSlot = SoundEngine.PlaySound(AssetReferences.Assets.Sounds.WitchsHarp.Asset, Owner.Center);
        }


        if (Timer % 16 == 0)
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(64, 64);
            var vel = Main.rand.NextVector2Circular(3, 3);
            vel.Y -= 5;
            Particles.Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.SkyBlue.ToVector4(),
                timeLeft = Main.rand.Next(60, 120),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f))
            });
        }

        if (Main.rand.NextBool(18))
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(64, 64);
            var vel = Main.rand.NextVector2Circular(2, 2);
            var sp = SparkleParticle.Spawn(pos, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
            sp.fast = true;
            sp.gravity = 0;
            sp.dampening = 0.05f;
            sp.innerColor = Color.White;
            sp.outerColor = Color.Blue;
        }

        if (this.OwnedByLocalClient())
        {
            Dir = (Main.MouseWorld.X < Projectile.Center.X) ? -1 : 1;
            Projectile.netUpdate = true;
        }

        if (Owner.HasBuff<BellSummoning>())
            Projectile.timeLeft = 30;
       if(SoundEngine.TryGetActiveSound(_soundSlot, out var reuslt))
        {
            reuslt.Position = Projectile.Center;
            reuslt.Volume = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 30f));
        }

        Projectile.Center = Owner.MountedCenter + new Vector2(Dir * 22, 0);
        Owner.heldProj = Projectile.whoAmI;
        var rot = (Projectile.Center - Owner.Center).ToRotation();
        rot -= MathHelper.PiOver2;
        var rotOffset = ExtraMath.Osc(-0.05f, 0.05f, speed: 1, offset: 0);
        var rotOffset2 = ExtraMath.Osc(-0.05f, 0.05f, speed: 1, offset: 1);
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot + rotOffset);
        Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, rot + rotOffset2);
        Owner.ChangeDir((int)Dir);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (SoundEngine.TryGetActiveSound(_soundSlot, out var reuslt))
        {
            reuslt.Stop();
        }

    }
}
