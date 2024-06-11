using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Stellamod.UI.Systems;
using Stellamod.Dusts;
using Terraria.ID;
using System.Threading;
using Terraria.Audio;
using Stellamod.Buffs;


namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    internal class RekFireEyeLaserProj : ModProjectile
    {

        internal PrimitiveTrail BeamDrawer;
        public ref float Time => ref Projectile.ai[0];
        public NPC Owner => Main.npc[(int)Projectile.ai[1]];
        public ref float Direction => ref Projectile.ai[2];

        public override string Texture => TextureRegistry.EmptyTexture;
        public const float LaserLength = 2400f;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }

        public override void AI()
        {
            if(Time == 1 && Main.myPlayer == Projectile.owner)
            {
                if (Main.rand.NextBool(2))
                {
                    Direction = -1;
                }
                else
                {
                    Direction = 1;
                }
                Projectile.netUpdate = true;
            }
            if(Time == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekLaser2"), Projectile.position);
            }

            Projectile.Center = Owner.Center;
            Projectile.velocity = Projectile.velocity.RotatedBy((MathHelper.PiOver2 * Direction) / 60f);

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
            target.AddBuff(ModContent.BuffType<GothivianFlames>(), 4 * 50);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * (LaserLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            float mult = 1;
            if (Projectile.timeLeft < 60)
            {
                mult = (float)Projectile.timeLeft / (float)60;
            }
            return Projectile.width * Projectile.scale * 1.3f * mult;
        }

        public override bool ShouldUpdatePosition() => false;
        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Orange, Color.Red, 0.2f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Lerp(Color.White, Color.OrangeRed, 0.3f));
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.WaterTrail);

            List<float> originalRotations = new();
            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, i / 8f));
                originalRotations.Add(MathHelper.PiOver2);
            }

            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
    }
}
