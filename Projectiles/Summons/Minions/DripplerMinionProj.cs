using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Buffs.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;

namespace Stellamod.Projectiles.Summons.Minions
{
    /*
             * This minion shows a few mandatory things that make it behave properly. 
             * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
             * If the player targets a certain NPC with right-click, it will fly through tiles to it
             * If it isn't attacking, it will float near the player with minimal movement
             */
    public class DripplerMinionProj : ModProjectile
    {
        private Vector2 _targetOffset;
        private int _targetNpc = -1;

        Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drippler");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int StickToNPC
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private ref float StickTimer => ref Projectile.ai[1];

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player
            Vector2 dirToNpc = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
            if(Projectile.velocity.Length() < moveSpeed)
            {
                Projectile.velocity += dirToNpc * accel;
            }
       
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, targetCenter, degreesToRotate: 0.5f);
        }

        private void AI_IdleAroundOwner()
        {
            float index = SummonHelper.GetProjectileIndex(Projectile);

            float swingRange = MathHelper.TwoPi;
            float swingXRadius = 128;
            float swingYRadius = 16;
            float swingProgress = Main.GlobalTimeWrappedHourly * 0.25f;
            swingProgress += index * MathHelper.TwoPi;
            float xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange);
            float yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange);
            Vector2 offset = new Vector2(xOffset, yOffset);
            Vector2 targetCenter = Owner.Center + offset + new Vector2(0, -64);
            Projectile.velocity = (targetCenter - Projectile.Center) * 0.1f;
           // AI_Movement(targetCenter, moveSpeed: 20);
        }


        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            if (!SummonHelper.CheckMinionActive<DripplerMinionBuff>(Owner, Projectile))
                return;

            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if(Main.rand.NextBool(12))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
            }

            if (StickTimer > 0)
            {
                AI_Sticking();
            }

            if (foundTarget)
            {
                if(_targetNpc == -1)
                {
                    Projectile.velocity = (targetCenter - Projectile.Center) * 0.05f;
                }
            }
            else
            {
                AI_IdleAroundOwner();
            }

            Visuals();
        }

        private void AI_Sticking()
        {
            StickTimer--;
            if(StickTimer <= 0)
            {
                _targetNpc = -1;
            }

            if (_targetNpc == -1)
                return;

            NPC targetNpc = Main.npc[_targetNpc];
            if (!targetNpc.active)
            {
                _targetNpc = -1;
                return;
            }

            if (!targetNpc.CanBeChasedBy())
            {
                _targetNpc = -1;
                return;
            }

            Vector2 targetPos = targetNpc.position - _targetOffset;
            Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
            float dist = Vector2.Distance(Projectile.position, targetPos);
            Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            Timer++;
            if (Timer >= 90)
            {
                float speedX = Main.rand.Next(-15, 15);
                float speedY = Main.rand.Next(-15, -15);
                Vector2 speed = new Vector2(speedX, speedY);

                SoundEngine.PlaySound(SoundID.NPCHit18, targetNpc.Center);
                SoundEngine.PlaySound(SoundID.Item171, targetNpc.Center);

                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), (int)targetNpc.Center.X, (int)targetNpc.Center.Y, speed.X, speed.Y,
                                      ModContent.ProjectileType<BloodWaterProj>(), Projectile.damage / 2, 1f, Projectile.owner);
                }
              
                Timer = 0;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            StickTimer = 30;
            _targetNpc = target.whoAmI;
            _targetOffset = (target.position - Projectile.position) + new Vector2(0.001f, 0.001f);
        }

        private void Visuals()
        {
            Projectile.rotation = Projectile.velocity.X * 0.05f;


            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 0.278f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawGlow(ref lightColor);
            DrawTentacles(ref lightColor);
            DrawBody(ref lightColor);
            DrawEyes(ref lightColor);
            return false;
        }

        private void DrawGlow(ref Color lightColor)
        {
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            float drawScale = 1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 3; i++)
            {
                Color glowColor = Color.Red;
                glowColor.A = 0;
                spriteBatch.Draw(dimLightTexture, Projectile.Center - Main.screenPosition, null, glowColor,
                    Projectile.rotation, dimLightTexture.Size() / 2f, drawScale * VectorHelper.Osc(0.85f, 1f, speed: 8, offset: Projectile.whoAmI), SpriteEffects.None, 0f);
            }

        }
        private void DrawBody(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = Vector2.One;
            drawScale.X = VectorHelper.Osc(0.85f, 1f, speed: 8f, offset: 2f);
            drawScale.Y = VectorHelper.Osc(0.85f, 1f, speed: 8f, offset: 4f);
            drawOrigin -= Projectile.velocity * 0.5f;
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, drawOrigin,
      drawScale, SpriteEffects.None, 0);
        }

        private void DrawEyes(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Eyes").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f - Projectile.velocity.SafeNormalize(Vector2.Zero) * 8;
            Vector2 drawScale = Vector2.One;
            drawScale.X = VectorHelper.Osc(0.75f, 1f, speed: 2f, offset: 2f);
            drawScale.Y = VectorHelper.Osc(0.75f, 1f, speed: 2f, offset: 4f);
            drawPos += Main.rand.NextVector2Circular(1, 1);
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, drawOrigin,
               drawScale, SpriteEffects.None, 0);
        }

        private void DrawTentacles(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Tentacles").Value;
            MiscShaderData shaderData = GameShaders.Misc["LunarVeil:DaedusRobe"];
            shaderData.Shader.Parameters["windNoiseTexture"].SetValue(TextureRegistry.CloudNoise.Value);

            float speed = 1;
            shaderData.Shader.Parameters["uImageSize0"].SetValue(texture.Size());
            shaderData.Shader.Parameters["startPixel"].SetValue(31);
            shaderData.Shader.Parameters["endPixel"].SetValue(58);
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * speed);
            shaderData.Shader.Parameters["distortionStrength"].SetValue(0.075f);


            Vector2 vel = -Projectile.velocity * 0.05f;
            vel.Y *= 0.25f;
            shaderData.Shader.Parameters["movementVelocity"].SetValue(vel);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, shaderData.Shader, Main.GameViewMatrix.TransformationMatrix);


            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos.Y += 32;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), 0f, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
