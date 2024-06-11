using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal abstract class NiiviCrystal : ModNPC
    {
        public virtual Color AfterImageStartColor => Color.White;
        public virtual Color AfterImageEndColor => Color.Transparent;

        public virtual Color TrailStartColor => Color.White;
        public virtual Color TrailEndColor => Color.Transparent;

        public virtual Color AuraColor => Color.White;

        private PrimitiveTrail TrailDrawer;
        protected ref float Timer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 32;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }


        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 18;
            NPC.lifeMax = 1000;
            NPC.damage = 100;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.defense = 100;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle soundStyle = SoundRegistry.Niivi_CrystalSummon;
                soundStyle.Volume = 0.66f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(TrailStartColor, TrailEndColor, completionRatio) * (1f - completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return (NPC.width * NPC.scale * 1f - completionRatio) * 0.5f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw After-Image
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = TextureAssets.Npc[Type].Value;
            int projFrames = Main.npcFrameCount[Type];
            float drawScale = 1f;
            float drawRotation = NPC.rotation;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;

            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = NPC.GetAlpha(Color.Lerp(AfterImageStartColor, AfterImageEndColor, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(texture, drawPos, NPC.frame, color, NPC.oldRot[k], drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw Trail
            Vector2 drawPosition = NPC.Center - Main.screenPosition;

            TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailDrawer.SpecialShader = TrailRegistry.FireVertexShader;
            TrailDrawer.SpecialShader.UseColor(Color.DarkCyan);
            TrailDrawer.SpecialShader.SetShaderTexture(TrailRegistry.WaterTrail);


            TrailDrawer.Draw(NPC.oldPos, -Main.screenPosition + drawOrigin, 32);



            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
           
            var shader = ShaderRegistry.MiscSilPixelShader;
            float progress = Timer / 60f;
            progress = 1f - progress;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            //The color to lerp to
            shader.UseColor(Color.White);

            //Should be between 0-1
            //1 being fully opaque
            //0 being the original color
            shader.UseSaturation(progress);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            // Retrieve reference to shader
            shader = ShaderRegistry.MiscFireWhitePixelShader;

            //You have to set the opacity/alpha here, alpha in the spritebatch won't do anything
            //Should be between 0-1
            float opacity = 0.75f;
            shader.UseOpacity(opacity);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(0.15f);

            //How fast the extra texture animates
            float speed = 1.0f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(new Color(AuraColor.R, AuraColor.G, AuraColor.B, 0));

            //Texture itself
            shader.UseImage1(TrailRegistry.WaterTrail);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin();
            return false;
        }
    }

    internal class NiiviCrystalFrost : NiiviCrystal
    {
        //Damage Values
        private int Damage_FrostBreath => 102;
        private int Damage_FrostBomb => 64;

        public override Color AfterImageStartColor => Color.LightCyan;

        public override Color TrailStartColor => Color.Cyan;
        public override Color TrailEndColor => Color.BlueViolet;
        public override Color AuraColor => Color.LightCyan;

        private ref float Rotation => ref NPC.ai[1];
        public override void AI()
        {
            base.AI();
            NPC.TargetClosest();
            //Spawn effect
            if (Timer == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    int dustType = ModContent.DustType<GlowDust>();
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(8, 8);
                    Dust.NewDustPerfect(NPC.Center, dustType, velocity, Alpha: 0, newColor: Color.White);
                }
            }

            //Oscillate movement
            float ySpeed = MathF.Sin(Timer * 0.05f);
            NPC.velocity = new Vector2(0, ySpeed);

            //Slowly rotate while shooting projectile
            float length = 720;
            float amountToRotateBy = 3 * MathHelper.TwoPi / length;
            Rotation += amountToRotateBy;

            if (Timer % 4 == 0)
            {
                float particleSpeed = 16;
                Vector2 velocity = Rotation.ToRotationVector2() * particleSpeed;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                Color[] colors = new Color[] { Color.Cyan, Color.LightCyan, Color.DarkCyan, Color.White };
                Color color = colors[Main.rand.Next(0, colors.Length)];

                //Spawn Star and Snowflake Particles
                for (int i = 0; i < 4; i++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        //Snowflake particle
                        ParticleManager.NewParticle<SnowFlakeParticle>(NPC.Center, velocity, color, 1f);
                    }
                    else
                    {
                        //Star particle
                        ParticleManager.NewParticle<StarParticle2>(NPC.Center, velocity, color, 1f);
                    }
                }

            }

            if (Timer % 4 == 0 && StellaMultiplayer.IsHost)
            {
                float speed = 16;
                Vector2 velocity = Rotation.ToRotationVector2() * speed;

                //Add some random offset to the attack
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                int type = ModContent.ProjectileType<NiiviFrostBreathProj>();
                int damage = Damage_FrostBreath;
                float knockback = 1;

                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, type,
                    damage, knockback, Main.myPlayer);
                }
            }

            if (Timer % 32 == 0)
            {

                if (StellaMultiplayer.IsHost)
                {
                    int type = ModContent.ProjectileType<NiiviFrostFlowerProj>();
                    int damage = Damage_FrostBomb;
                    float knockback = 1;
                    float maxDistance = 768;
                    float progress = Main.rand.NextFloat(0.2f, 1f);
                    float distance = progress * maxDistance;
                    Vector2 velocity = NPC.Center.DirectionTo(Main.player[NPC.target].Center) * distance / 32f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                        type, damage, knockback, Main.myPlayer);
                }

            }

            if (Timer >= length)
            {
                //Some teleport effect or something
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, 
                        ModContent.ProjectileType<NiiviCrystalWarpExplosionProj>(), 0, 0, Main.myPlayer);
                }
                NPC.active = false;
            }
        }
    }

    internal class NiiviCrystalLightning : NiiviCrystal
    {
        private int Damage_Lightning_Small => 120;
        private int Damage_Lightning_Big => 150;

        private ref float AttackTimer => ref NPC.ai[1];
        public override Color AfterImageStartColor => Color.White;

        public override Color TrailStartColor => Color.LightGoldenrodYellow;
        public override Color TrailEndColor => Color.DarkGoldenrod;
        public override Color AuraColor => Color.LightYellow;


        public override void AI()
        {
            base.AI();
            AttackTimer--;
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            float length = 450 ;
            //Oscillate movement
            float ySpeed = MathF.Sin(Timer * 0.05f);
            NPC.velocity = new Vector2(0, ySpeed);

            if(AttackTimer <= 0)
            {
                if (Timer > 30 && Timer % 15 == 0 && Timer <= 360 && StellaMultiplayer.IsHost)
                {
                    Vector2 pos = target.Center + target.velocity * 48;
                    pos += new Vector2(Main.rand.NextFloat(-64, 64), 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, Vector2.UnitY,
                        ModContent.ProjectileType<NiiviThundercloudProj>(), Damage_Lightning_Small, 2, Main.myPlayer);
                }

                if(Timer % 100 == 0)
                {
                    AttackTimer = 30;
                }

                if (Timer == 400 && StellaMultiplayer.IsHost)
                {
                    Vector2 pos = target.Center + target.velocity * 48;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, Vector2.UnitY,
                        ModContent.ProjectileType<NiiviLightningRayWarnProj>(), Damage_Lightning_Big, 2, Main.myPlayer);
                }

            }

            if (Timer >= length)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<NiiviCrystalWarpExplosionProj>(), 0, 0, Main.myPlayer);
                }
                NPC.active = false;
            }
        }
    }

    internal class NiiviCrystalStars : NiiviCrystal
    {
        //Damage Values
        private int Damage_Comet => 72;

        public override Color AfterImageStartColor => Color.White;

        public override Color TrailStartColor => Color.Blue;
        public override Color TrailEndColor => Color.DeepSkyBlue;
        public override Color AuraColor => Color.DarkBlue;


        public override void AI()
        {
            base.AI();
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            float length = 360;
            float ySpeed = MathF.Sin(Timer * 0.05f);
            NPC.velocity = new Vector2(0, ySpeed);

            if (Timer % 8 == 0 && StellaMultiplayer.IsHost)
            {
                int type = ModContent.ProjectileType<NiiviCometProj>();
                int damage = Damage_Comet;
                int knockback = 1;

                float height = 768;

                if (Main.rand.NextBool(2))
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 cometOffset = -Vector2.UnitY * height + new Vector2(Main.rand.NextFloat(512, 1750), 0);
                    Vector2 cometPos = targetCenter + cometOffset;

                    float speed = 12;
                    Vector2 velocity = new Vector2(-1, 1) * speed;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), cometPos, velocity,
                        type, damage, knockback, Main.myPlayer);
                }
                else
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 cometOffset = -Vector2.UnitY * height + new Vector2(Main.rand.NextFloat(-1750, -512), 0);
                    Vector2 cometPos = targetCenter + cometOffset;

                    float speed = 12;
                    Vector2 velocity = new Vector2(1, 1) * speed;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), cometPos, velocity,
                        type, damage, knockback, Main.myPlayer);
                }
            }

            if(Timer >= length)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<NiiviCrystalWarpExplosionProj>(), 0, 0, Main.myPlayer);
                }
                NPC.active = false;
            }
        }
    }
}
