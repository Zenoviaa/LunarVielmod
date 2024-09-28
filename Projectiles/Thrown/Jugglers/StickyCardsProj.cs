using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class StickyCardsProj : ModProjectile
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
        private Vector2[] BungeeGumPos;
        private Vector2[] BungeeGumAuraPos;
        private PrimDrawer TrailDrawer { get; set; } = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            BungeeGumPos = new Vector2[4];
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
            Projectile.rotation += 0.25f;
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

    
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (IsTouchingPlayer())
            {
                int combatText = CombatText.NewText(Juggler.Player.getRect(), Color.White, $"x{Juggler.CatchCount + 1}", true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 60;

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), Projectile.position);
                Juggler.CatchCount++;
                Juggler.DamageBonus += 1f;
                Projectile.Kill();
            }
       
            if(Vector2.Distance(Owner.Center, Projectile.Center) <= 164 || Vector2.Distance(Owner.Center, Projectile.Center) > 512)
            {
                Vector2 directionToOwner = Projectile.Center.DirectionTo(Owner.Center);
                Vector2 targetVelocity = directionToOwner * 16;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.3f);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }

            BungeeGumPos[0] = Projectile.position;
            BungeeGumPos[1] = Projectile.position;
            BungeeGumPos[2] = Owner.Center;
            BungeeGumPos[3] = Owner.Center;

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

            //Don't take too long or else you lose your combo
            Timer++;
            if (Timer >= 598)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dirt"), Projectile.position);
                Juggler.ResetJuggle();
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.0f * Main.essScale);
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
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, new Vector2(1, 1), ModContent.ProjectileType<BungeeGumSlashProj>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, new Vector2(-1, 1), ModContent.ProjectileType<BungeeGumSlashProj>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.position, 1024, 1);

            Projectile.timeLeft = 600;
            Vector2 bounceVelocity = -Projectile.velocity / 2;
            Projectile.tileCollide = false;
            Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4);
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
                SoundStyle jugglerHitMax = SoundRegistry.JugglerHit;
                pitch = MathHelper.Clamp(catchCount * 0.02f, 0f, 1f);
                jugglerHitMax.Pitch = pitch;
                jugglerHitMax.PitchVariance = 0.1f;
                SoundEngine.PlaySound(jugglerHitMax, Projectile.position);
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


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return baseWidth * VectorHelper.Osc(0.5f, 1f, 3);
        }

        public float WidthFunctionAura(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return baseWidth * 0.2f;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Pink * VectorHelper.Osc(0.5f, 1f, 3) * 0.3f;
        }
        
        public Color ColorFunctionAura(float completionRatio)
        {
            return Color.Pink * VectorHelper.Osc(0.5f, 1f, 3);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>(TextureRegistry.BoreParticleWhite).Value;
            Color drawColor = new Color(Color.Pink.R, Color.Pink.G, Color.Pink.B, 0);
            Color drawColor2 = new Color(Color.LightPink.R, Color.LightPink.G, Color.LightPink.B, 0);
            Color auraColor = Color.Lerp(drawColor, drawColor2, VectorHelper.Osc(0f, 1f, 3));
            auraColor *= 0.3f;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null,
                auraColor, Projectile.rotation,
                new Vector2(256, 256), 0.2f, SpriteEffects.None, 0f);


            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            Vector2 textureSize = new Vector2(16, 22);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.StarTrail);
            TrailDrawer.WidthFunc = WidthFunction;
            TrailDrawer.ColorFunc = ColorFunction;
            TrailDrawer.DrawPrims(BungeeGumPos, textureSize * 0.5f - Main.screenPosition, 155);

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SpikyTrail1);
            TrailDrawer.WidthFunc = WidthFunctionAura;
            TrailDrawer.ColorFunc = ColorFunctionAura;
            TrailDrawer.DrawPrims(BungeeGumAuraPos, textureSize * 0.5f - Main.screenPosition, 155);

            if(Timer == 0)
            {
                DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White, Color.Transparent, ref lightColor);
            }
     
            return base.PreDraw(ref lightColor);
        }
    }
}
