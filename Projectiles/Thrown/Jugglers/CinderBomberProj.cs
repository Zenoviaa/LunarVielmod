using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Trails;
using Terraria.Graphics.Shaders;
using Stellamod.Dusts;
using Stellamod.Projectiles.IgniterExplosions;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class CinderBomberProj : ModProjectile
    {
        private enum ActionState
        {
            Thrown,
            Fall
        }

        private ActionState State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private Player Owner => Main.player[Projectile.owner];
        private JugglerPlayer Juggler => Owner.GetModPlayer<JugglerPlayer>();
        private PrimitiveTrail PrimTrailDrawer;
        private PrimDrawer TrailDrawer { get; set; } = null;
        private Vector2[] BungeeGumAuraPos;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            BungeeGumAuraPos = new Vector2[24];
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Thrown:
                    AI_Thrown();
                    break;
                case ActionState.Fall:
                    AI_Fall();
                    break;
            }
        }

        private bool IsTouchingPlayer()
        {
            Rectangle myRect = Projectile.getRect();
            Rectangle playerRect = Owner.getRect();
            return myRect.Intersects(playerRect) || myRect.Contains(playerRect);
        }

        private void AI_Thrown()
        {
            Projectile.velocity.Y += 0.05f;
            Projectile.rotation += 0.05f;

            for (int i = 0; i < BungeeGumAuraPos.Length; i++)
            {
                float f = i;
                float length = BungeeGumAuraPos.Length;
                float progress = f / length;
                float offset = progress * MathHelper.TwoPi;
                Vector2 rotatedOffset = Vector2.UnitY.RotatedBy(offset + (Timer / 20f)).RotatedByRandom(MathHelper.PiOver4 / 24f);
                Vector2 rotatedVector = (rotatedOffset * 48 * VectorHelper.Osc(0.9f, 1f, 9));
                if (i % 2 == 0)
                {
                    BungeeGumAuraPos[i] = rotatedVector * 0.5f + Projectile.position;
                }
                else
                {
                    BungeeGumAuraPos[i] = rotatedVector + Projectile.position;
                }
            }
        }

        private void AI_Fall()
        {
            if (Projectile.velocity.Y < 0)
            {
                Projectile.velocity.Y += 0.1f;
            }
            else
            {
                Projectile.velocity.Y += 0.02f;
            }

            Projectile.velocity *= 0.95f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (IsTouchingPlayer())
            {
                int combatText = CombatText.NewText(Juggler.Player.getRect(), Color.White, $"x{Juggler.CatchCount + 1}", true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 60;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/JuggleCatch1");
                soundStyle.PitchVariance = 0.15f;
                switch (Main.rand.Next(2))
                {

                    case 0:
                        SoundEngine.PlaySound(soundStyle, Projectile.position);
                        break;
                    case 1:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/JuggleCatch2");
                        SoundEngine.PlaySound(soundStyle, Projectile.position);
                        break;
                }

                Juggler.CatchCount++;
                Juggler.DamageBonus += 0.25f;
                Projectile.Kill();
            }

            //Don't take too long or else you lose your combo
            Timer++;
            if (Timer >= 598)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dirt"), Projectile.position);
                Juggler.ResetJuggle();
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Juggler.ResetJuggle();

            //Play womp womp sound or something 
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (Projectile.friendly)
            {
                Juggler.ResetJuggle();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.immortal)
                Juggler.ResetJuggle();
            Projectile.timeLeft = 600;
            Vector2 bounceVelocity = -Projectile.velocity / 2;
            Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4 / 4);
            Projectile.velocity += -Vector2.UnitY * 8;
            Projectile.friendly = false;
            State = ActionState.Fall;

            float catchCount = Juggler.CatchCount;
            float pitch = MathHelper.Clamp(catchCount * 0.05f, 0f, 1f);
            SoundStyle jugglerHit = SoundRegistry.JugglerHit;
            jugglerHit.Pitch = pitch;
            jugglerHit.PitchVariance = 0.1f;
            jugglerHit.Volume = 0.5f;
            SoundEngine.PlaySound(jugglerHit, Projectile.position);

            if (Juggler.CatchCount >= 5)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 4);
                SoundStyle fireBomb = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
                SoundEngine.PlaySound(fireBomb, target.Center);

                SoundStyle jugglerHitMax = SoundRegistry.JugglerHitMax;
                pitch = MathHelper.Clamp(catchCount * 0.02f, 0f, 1f);
                jugglerHitMax.Pitch = pitch;
                jugglerHitMax.PitchVariance = 0.1f;
                SoundEngine.PlaySound(jugglerHitMax, Projectile.position);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, 
                    ModContent.ProjectileType<CinderBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                for (int i = 0; i < 32; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(target.Center, DustID.Torch, speed * 4, Scale: 1f);
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2"), Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(target.Center, velocity, Color.DarkGoldenrod, randScale);
            }
        }

        public float WidthFunctionAura(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return baseWidth * 0.2f * Main.rand.NextFloat(0.5f, 1f);
        }

        public Color ColorFunctionAura(float completionRatio)
        {
            return Color.OrangeRed * VectorHelper.Osc(0.5f, 1f, 3) * 0.33f;
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Orange, Color.RoyalBlue, completionRatio) * (1f - completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return (Projectile.width * Projectile.scale * 1f - completionRatio) * 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionAura, ColorFunctionAura, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            if(Juggler.CatchCount >= 5)
            {
                Vector2 textureSize = new Vector2(24, 24);
                if (State == ActionState.Thrown)
                {
                    GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SpikyTrail1);
                    TrailDrawer.WidthFunc = WidthFunctionAura;
                    TrailDrawer.ColorFunc = ColorFunctionAura;
                    TrailDrawer.DrawPrims(BungeeGumAuraPos, textureSize * 0.5f - Main.screenPosition, 155);
                }
          
    
                PrimTrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
                PrimTrailDrawer.SpecialShader = TrailRegistry.FireVertexShader;
                PrimTrailDrawer.SpecialShader.UseColor(Color.DarkGoldenrod);
                PrimTrailDrawer.SpecialShader.SetShaderTexture(TrailRegistry.WaterTrail);
                PrimTrailDrawer.Draw(Projectile.oldPos, textureSize * 0.5f - Main.screenPosition, 32);
            }

            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}