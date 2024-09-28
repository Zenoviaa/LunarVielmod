using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class FlinchMachineProj : ModProjectile
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

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
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
                Juggler.DamageBonus += 0.33f;
                if(Juggler.CatchCount % 3 == 0)
                {
                    Juggler.SpecialAttack = true;
                }
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


            bool extremeHitEffect = false;
            if (Juggler.CatchCount >= 5)
            {
                SoundStyle jugglerHitMax = SoundRegistry.JugglerHitMax;
                pitch = MathHelper.Clamp(catchCount * 0.02f, 0f, 1f);
                jugglerHitMax.Pitch = pitch;
                jugglerHitMax.PitchVariance = 0.1f;
                SoundEngine.PlaySound(jugglerHitMax, Projectile.position);

                if(Juggler.SpecialAttack)
                {
                    Juggler.SpecialAttack = false;
                    extremeHitEffect = true;
                }
            }


            if (extremeHitEffect)
            {
                SoundStyle fanHit2 = SoundRegistry.FanHit2;
                fanHit2.PitchVariance = 0.1f;
                SoundEngine.PlaySound(fanHit2, Projectile.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 2048, 64);
                target.SimpleStrikeNPC(Projectile.damage * 5, hit.HitDirection, damageType: Projectile.DamageType);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, 
                    ModContent.ProjectileType<FlinchMachineExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                for(int i = 0; i < 7; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan1);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan2);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan3);
                }

                for (int i = 0; i < 16; i++)
                {
                    //Get a random velocity
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(16, 16);

                    //Get a random
                    float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                    ParticleManager.NewParticle<StarParticle>(target.Center, velocity, Color.DarkGoldenrod, randScale);
                }
            }
            else
            {
                SoundStyle fanHit = SoundRegistry.FanHit1;
                fanHit.Pitch = pitch;
                fanHit.PitchVariance = 0.1f;
                fanHit.Volume = 0.85f;
                SoundEngine.PlaySound(fanHit, Projectile.position);

                for (int i = 0; i < 1; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan1);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan2);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan3);
                }

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGray, 1f).noGravity = true;
                }

                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(target.Center, velocity, Color.DarkGoldenrod, randScale);
            }
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Juggler.CatchCount >= 5)
            {
                if (State == ActionState.Thrown)
                {
                    Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                    TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
                    GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
                    TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
                }
            }

            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White * 0.2f, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
