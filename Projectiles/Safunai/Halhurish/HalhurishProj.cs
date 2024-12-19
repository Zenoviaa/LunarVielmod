using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Safunai.Halhurish
{
    public class HalhurishProj : BaseSafunaiProjectile
	{
        protected override Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.OrangeRed, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{

			ShakeModSystem.Shake = 4;
			SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), target.position);
			float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
			float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;
            if (Slam) 
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY * 3, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0.4f, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0.5f, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0.25f, speedY * 2, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 5, speedY * 3, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 2, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger2"), target.position);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                }

                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center, 
                    innerColor: Color.White, 
                    glowColor: Color.Yellow, 
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.12f);

                // Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<GlowCircleBoom>(), 0, 0, Projectile.owner);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), target.position);
                ShakeModSystem.Shake = 4;
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 0.5f).noGravity = true;
                }
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 0.5f).noGravity = true;
                }
            }
        }
	}
}
