using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Spears
{
    internal class SiriusProj : ModProjectile
    {
        const float Exploding_Time = 90;
        enum ActionState
        {
            Throwing,
            Lodged_In_Tile,
            Lodged_In_NPC
        }

        private int _targetNpc = -1;
        private Vector2 _targetOffset;

        ActionState State
        {
            get
            {
                return (ActionState)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = (float)value;
            }
        }

        float ExplodingTimer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 45;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Throwing:
                    Projectile.velocity.Y += 0.1f;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                    break;
                case ActionState.Lodged_In_NPC:
                    StickToTarget();
                    Exploding();
                    break;
                case ActionState.Lodged_In_Tile:
                    Projectile.velocity = Vector2.Zero;
                    Exploding();
                    break;
            }
   
        }

        private void StickToTarget()
        {
            NPC target = Main.npc[_targetNpc];
            if (!target.active)
            {
                Projectile.Kill();
            }

            Vector2 targetPos = target.position - _targetOffset;
            Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
            float dist = Vector2.Distance(Projectile.position, targetPos);
            Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
        }

        private void ChargeVisuals(float timer, float maxTimer)
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 24;
            float maxParticleSpawnSpeed = 12;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 5;
                    var d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, newColor: Color.White, Scale: 0.5f);
                    d.noGravity = true;
                }
            }
        }

        private void Exploding()
        {
            ChargeVisuals(ExplodingTimer, Exploding_Time);

            if(ExplodingTimer == 1)
            {
                //Play charging up sound
                SoundStyle summonSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/RisingSummon");
                SoundEngine.PlaySound(summonSoundStyle, Projectile.position);
            }

            ExplodingTimer++;
            if (ExplodingTimer >= Exploding_Time)
            {
                SoundStyle summonSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/RisingSummon");
                SoundEngine.FindActiveSound(summonSoundStyle)?.Stop();

                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
                        break;
                }

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<SiriusBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                //KABOOM
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (_targetNpc == -1)
            {
                _targetNpc = target.whoAmI;
                _targetOffset = (target.position - Projectile.position) + new Vector2(0.001f, 0.001f);
                State = ActionState.Lodged_In_NPC;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            State = ActionState.Lodged_In_Tile;
            Projectile.position += oldVelocity;
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.Niivin, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (_targetNpc == -1)
            {
                DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CausticTrail);
                DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.Niivin, Color.Transparent, ref lightColor);
            }

            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new Vector2(70, 74);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = ExplodingTimer / Exploding_Time;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
