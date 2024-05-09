using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Gores;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class RibbonStaffTieProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float TargetNPC => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 14;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 48;
            Projectile.timeLeft = int.MaxValue;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.alpha -= 10;
            int npcIndex = (int)TargetNPC;
            Timer++;
            if(Timer == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(90, 90);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.GoreType<RibbonRed>());
                }

                for(int i = 0; i < 3; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                        ModContent.DustType<Dusts.GunFlash>(), newColor: Color.White);
                }
            }

            NPC target = Main.npc[npcIndex];
            if(!target.active || !target.HasBuff(ModContent.BuffType<RibbonWrapped>()))
            {
                Projectile.Kill();
            }
            else
            {
                Vector2 targetPos = target.Center + new Vector2(0.001f, 0.001f) + new Vector2(0, Projectile.height / 3);
                Vector2 directionToTarget = Projectile.Center.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.Center, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }

            //Animate
            int frameSpeed = 5;
            int projFrames = Main.projFrames[Projectile.type];
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= projFrames)
                {
                    Projectile.frame = projFrames - 1;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RibbonStaffBoom1");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(90, 90);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.GoreType<RibbonRed>());
            }
        }
    }
}
