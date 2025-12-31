using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Riverside.WeaponsRS
{
    public class GoldenFishNecronomicon : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToNecronomicon(hintColor: Color.Goldenrod);

            Item.shoot = ModContent.ProjectileType<GoldenFish>();
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }

    public class GoldenFish : ModProjectile
    {
        private enum AIState
        {
            ReturnToOwner,
            Attack
        }
        private ref float Timer => ref Projectile.ai[0];
        private float _speed;
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private int Sticky
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 StickyOffset;
        private int Size;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 24;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(StickyOffset);
            writer.Write(Size);
            writer.Write(_speed);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            StickyOffset = reader.ReadVector2();
            Size = reader.ReadInt32();
            _speed = reader.ReadSingle();
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {

                _speed = 1;
                Sticky = -1;
            }
            float inScale = EasingFunction.InOutSine(Timer / 60f);
            float outScale = (float)Projectile.timeLeft / 60f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = inScale * outScale;

            if(Timer % 24 == 0)
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(Projectile.Center, -Vector2.UnitY, Scale: Main.rand.NextFloat(0.5f, 1f));
                sp.initialColor = Color.Lerp(Color.Goldenrod, Color.Black, 0.5f);
            }
            if (Timer == 1 && this.OwnedByLocalClient())
            {
                Size = Main.rand.Next(0, 3);
                Projectile.netUpdate = true;
            }
            NPC closest = NPCHelper.FindClosestNPC(Projectile.position, 1024);
            if (closest != null)
            {
                //attack
                State = AIState.Attack;
            }
            else
            {
                //Return to owner
                State = AIState.ReturnToOwner;
            }

            switch (State)
            {
                case AIState.ReturnToOwner:
                    AI_ReturnToOwner();
                    break;
                case AIState.Attack:
                    AI_Attack(closest);
                    break;
            }

            if (Sticky != -1)
            {
           
                NPC target = Main.npc[Sticky];
                if (!target.active)
                    Sticky = -1;
                else
                {
                    Vector2 velToTarget = (target.Center + StickyOffset) - Projectile.Center;
                    Projectile.velocity = velToTarget;

                }


            }
            Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
        }

        private void AI_ReturnToOwner()
        {
            if (_speed > 8)
                _speed -= 0.2f;
            else if (_speed < 8)
                _speed += 0.1f;

            Vector2 targetPosition = Owner.Center;
            targetPosition.Y -= 64;
            Vector2 velToTarget = (targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * _speed;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, velToTarget, 0.01f);
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        private void AI_Attack(NPC target)
        {
            if(_speed < 15f)
                _speed += 0.1f;
            Vector2 velToTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * _speed;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, velToTarget, 0.2f);
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (target.friendly)
                return;


            if (Sticky == -1)
            {
                Sticky = target.whoAmI;
                StickyOffset = (Projectile.Center - target.Center);
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float n = 0; n < 7f; n++)
            {
                SmokeParticle dp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(1f, 4f), Color.White, Scale: Main.rand.NextFloat(0.1f, 0.5f));
                dp.initialColor = Color.Lerp(Color.Goldenrod, Color.Black, 0.5f);
            }
        }

        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            var shader2 = RichLaserShader.Instance;
            shader2.LaserColor = Color.White * 0.8f;
            shader2.InnerColor = Color.Yellow * 0.5f;
            shader2.OuterColor = Color.DarkGoldenrod;
            shader2.BloomTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser2");

            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size / 2f);
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.DarkGoldenrod, completionRatio) * 0.2f;
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 16 * Projectile.scale;
            return MathHelper.SmoothStep(width, 0f, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            Rectangle frame = new Rectangle(0, frameHeight * Size, texture.Width, frameHeight);
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            //Draw after image
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldCenter = oldPos + Projectile.Size / 2f;
                Vector2 drawCenter = oldCenter - Main.screenPosition;
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                Color afterImageColor = Color.Lerp(Color.Goldenrod, Color.Transparent, ratio) * 0.1f;
                spriteBatch.Draw(texture, drawCenter, frame, afterImageColor, Projectile.oldRot[i], frame.Size() / 2f, Projectile.scale, spriteEffects, 0);
            }

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
