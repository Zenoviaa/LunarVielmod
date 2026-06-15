using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Buffs;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    public class RekFireCrystalLaserProj : ModProjectile
    {

        public ref float Time => ref Projectile.ai[0];
        public NPC Owner => Main.npc[(int)Projectile.ai[1]];

        public override string Texture => TextureRegistry.EmptyTexture;
        public const float LaserLength = 2400f;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 72;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.alpha = 255;
            Projectile.hide = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            ShakeScreenPosition.Shake = 3f;

            float progress = Time / 90f;
            progress = MathHelper.Clamp(progress, 0, 1);
            float minSpeed = 500f;
            float maxSpeed = 240f;
            float speed = MathHelper.Lerp(minSpeed, maxSpeed, progress);

            Projectile.Center = Owner.Center;
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.TwoPi / speed);
            if (Time % 2 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Projectile.velocity * 8;
                float scale = Main.rand.NextFloat(2.5f, 3.75f);
                Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, 0, Color.OrangeRed, scale).noGravity = true;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<TSmokeDust>(), vel, 0, Color.OrangeRed, scale / 2).noGravity = true;
                }
            }
            if (Time == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekLaser2"), Projectile.position);
            }

            if (!Owner.active)
            {
                Projectile.Kill();
            }

            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);

            if (Projectile.scale < 1f || Time <= 1)
            {
                Projectile.scale = MathF.Sin(Time / 600f * MathHelper.Pi) * 3f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
            }



            // And create bright light.
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.5f);
            Time++;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GothivianFlames>(), 4 * 70);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * (LaserLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Time >= 60f;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
    }
}
