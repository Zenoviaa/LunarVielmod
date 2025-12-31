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
            Item.damage = 12;
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
            Spawn,
            ReturnToOwner,
            Attack,
        }
        private float _dashTimer;
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(StickyOffset);
            writer.Write(Size);
            writer.Write(_speed);
            writer.WriteVector2(_startDashVelocity);
            writer.WriteVector2(_targetDashVelocity);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            StickyOffset = reader.ReadVector2();
            Size = reader.ReadInt32();
            _speed = reader.ReadSingle();
            _startDashVelocity = reader.ReadVector2();
            _targetDashVelocity = reader.ReadVector2();
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
            Projectile.scale = inScale * outScale * ExtraMath.Osc(0.86f, 1f, 4, offset: Projectile.whoAmI);

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
            switch (State)
            {
                case AIState.Spawn:
                    if (this.OwnedByLocalClient())
                    {
                        //Summon two little helper fish
                        for(int i = 0; i < 2; i++) 
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(32, 32), Projectile.velocity * Main.rand.NextFloat(0.25f, 0.75f), Type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
                    }
                    Projectile.velocity *= 0.5f;
                    State = AIState.ReturnToOwner;
                    break;
                case AIState.ReturnToOwner:
                    AI_ReturnToOwner();
                    break;
                case AIState.Attack:
                    if(closest != null)
                        AI_Attack(closest);
                    break;
            }
     
            if (closest != null)
            {
                //attack
                State = AIState.Attack;
            }
            else
            {
                //Return to owner
                State = AIState.ReturnToOwner;
                _dashTimer = 0;
            }
            if (Sticky != -1)
            {
           
                NPC target = Main.npc[Sticky];
                if (!target.active)
                {
                    Sticky = -1;
                    _speed = 1;
                    _dashTimer = 0;
                }           
                else
                {
                    float speed = MathHelper.Lerp(1f, 3f, Size / 3f);
                    StickyOffset = StickyOffset.RotatedBy(0.0125f * speed);
                    Vector2 velToTarget = (target.Center + StickyOffset) - Projectile.Center;
                    Projectile.velocity = velToTarget;
                    Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
                    Projectile.spriteDirection = 1;
                }


            }
            else
            {
                Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
            }

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
            velToTarget *= MathHelper.Lerp(0.8f, 1f, Size / 3f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, velToTarget, 0.01f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private Vector2 _startDashVelocity;
        private Vector2 _targetDashVelocity;
        private void AI_Attack(NPC target)
        {
            _dashTimer++;
            if(_dashTimer == 1)
            {
                _startDashVelocity = Projectile.velocity;

            }

            if(_dashTimer >= 120)
            {
                Projectile.friendly = true;
            }
            else
            {
                Projectile.friendly = false;
            }
                //      _startDashVelocity = _startDashVelocity.RotatedBy(0.04f);
                _targetDashVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 35;
            float completionRatio = _dashTimer / 240f;
            float ease = EasingFunction.InExpo(completionRatio);
            Vector2 dashVelocity = Vector2.Lerp(_startDashVelocity, _targetDashVelocity, ease);

            Projectile.velocity = dashVelocity;
            Projectile.rotation = Projectile.velocity.ToRotation();
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
            float rotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
                rotation -= MathHelper.Pi;
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

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, rotation, frame.Size() / 2f, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
