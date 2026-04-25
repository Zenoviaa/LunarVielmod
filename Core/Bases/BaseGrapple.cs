using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.Content.Areas.Terror.TilesTR;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public class GrapplePlayer : ModPlayer
    {
        public Vector2? ropePosition;
        public bool slowFall;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (ropePosition.HasValue)
            {
                Vector2 positionToRopeTo = ropePosition.Value;
                Vector2 targetVelocity = positionToRopeTo - Player.Center;
                Player.velocity = targetVelocity;
                ropePosition = null;
            }
            if (slowFall)
            {
                Player.velocity.Y *= 0.95f;
                slowFall = false;
            }
        }
    }

    public class GrappleGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public float grappleLineTileDistance;
        public bool isGrapple;

        public override bool AltFunctionUse(Item item, Player player)
        {
            if (isGrapple)
            {
                return true;
            }
            return base.AltFunctionUse(item, player);
        }
        public override bool CanShoot(Item item, Player player)
        {
            return base.CanShoot(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (isGrapple && player.ownedProjectileCounts[item.shoot] == 0)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: grappleLineTileDistance * 16 * 2);
                return false;
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }

    public interface IGrapplable
    {
        Rectangle GetHookRectangle(int tileX, int tileY);
    }

    public class GrappleLineHookSystem : ModSystem
    {
        private static List<Rectangle> _hookPoints;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _hookPoints = new List<Rectangle>();
        }
        public override void Unload()
        {
            base.Unload();
            _hookPoints = null;
        }

        public static bool IsCollidingWithHookTile(Vector2 hookPoint)
        {
            _hookPoints.Clear();
            Vector2 centerSearchPoint = hookPoint;
            Vector2 topLeftWorld = centerSearchPoint - new Vector2(128);
            Vector2 bottomRightWorld = centerSearchPoint + new Vector2(128);

            Point topLeftTile = topLeftWorld.ToTileCoordinates();
            Point bottomRightTile = bottomRightWorld.ToTileCoordinates();
            for(int x = topLeftTile.X; x < bottomRightTile.X; x++)
            {
                for(int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
                {
                    Point tilePoint = new Point(x, y);
                    Tile tile = Main.tile[tilePoint];
                    if (!tile.HasTile)
                        continue;
                    if(tile.TileType == ModContent.TileType<AcaciaTreeTop>())
                    {
                        Rectangle acaciaTreeHookRectangle = new Rectangle(0, 0, 170, 54);
                        Vector2 worldCoordinates = tilePoint.ToWorldCoordinates();
                        acaciaTreeHookRectangle.X = (int)(worldCoordinates.X - acaciaTreeHookRectangle.Width / 2);
                        acaciaTreeHookRectangle.Y = (int)(worldCoordinates.Y - acaciaTreeHookRectangle.Height / 2);
                        acaciaTreeHookRectangle.Y -= 64;
                        _hookPoints.Add(acaciaTreeHookRectangle);
                    } 
                    else if (tile.TileType == ModContent.TileType<MangroveTreeTop>())
                    {
                        Rectangle mangroveTreeHookRectangle = new Rectangle(0, 0, 584, 92);
                        Vector2 worldCoordinates = tilePoint.ToWorldCoordinates();
                        mangroveTreeHookRectangle.X = (int)(worldCoordinates.X - mangroveTreeHookRectangle.Width / 2);
                        mangroveTreeHookRectangle.Y = (int)(worldCoordinates.Y - mangroveTreeHookRectangle.Height / 2);
                        mangroveTreeHookRectangle.Y -= 64;
                        _hookPoints.Add(mangroveTreeHookRectangle);
                    }
                }
            }
            for (int i = 0; i < _hookPoints.Count; i++)
            {
                Rectangle hookRectangle = _hookPoints[i];
                if (hookRectangle.Contains(hookPoint.ToPoint()))
                    return true;
            }
            return false;
        }
            
    }

    public abstract class GrappleLine : ModProjectile
    {
        private enum AIState
        {
            Shoot,
            Hook,
            Retract
        }

        private float _traveledDistance;

        private Vector2[] _grappleLinePoints;
        private Vector2[] GrappleLinePoints
        {
            get
            {
                if (_grappleLinePoints == null || _grappleLinePoints.Length != VerletChain.points.Length)
                {
                    _grappleLinePoints = new Vector2[VerletChain.points.Length];
                }

                VerletChain.FillArr(_grappleLinePoints);
                return _grappleLinePoints;
            }
        }

        private Vector2 TargetHookPoint;
        private Vector2 ResizeShrinkPosition;
        private VerletChain VerletChain;
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private ref float Distance => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(ResizeShrinkPosition);
            writer.WriteVector2(TargetHookPoint);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            ResizeShrinkPosition = reader.ReadVector2();
            TargetHookPoint = reader.ReadVector2();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Shoot:
                    AI_Shoot();
                    break;
                case AIState.Hook:
                    AI_Hook();
                    break;
                case AIState.Retract:
                    AI_Retract();
                    break;
            }

           // Owner.itemAnimation = 2;
           // Owner.itemTime = 2;
//Owner.heldProj = Projectile.whoAmI;
            VerletChain?.Update();
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (State)
            {
                case AIState.Shoot:

                    SwitchState(AIState.Hook);
                    break;
            }


            return false;
        }

        private void AI_Shoot()
        {
            Timer++;
            if (Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    TargetHookPoint = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }
              
                SoundStyle hookSound = AssetRegistry.Sounds.Gun.GrappleShoot;
                hookSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hookSound, Projectile.Center);
            }

            if(Timer >= 2)
            {
                float distance = Vector2.Distance(Projectile.position, Projectile.oldPosition);
                _traveledDistance += distance;
                if(_traveledDistance >= Distance)
                {
                    SwitchState(AIState.Retract);
                }
            }

            if (GrappleLineHookSystem.IsCollidingWithHookTile(Projectile.Center) &&
                Vector2.Distance(Projectile.Center, TargetHookPoint) < 256)
                SwitchState(AIState.Hook);

            GrapplePlayer grapplePlayer = Owner.GetModPlayer<GrapplePlayer>();
            grapplePlayer.slowFall = true;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.extraUpdates = 2;
        }

        public override bool ShouldUpdatePosition()
        {
            if (State == AIState.Hook)
                return false;
            return base.ShouldUpdatePosition();
        }

        private void AI_Retract()
        {
            Timer++;
            Projectile.extraUpdates = 1;
            Vector2 directionToPlayer = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Vector2 velocityToPlayer = directionToPlayer * Projectile.velocity.Length();
            Projectile.velocity = velocityToPlayer;
            Projectile.rotation = -Projectile.velocity.ToRotation();
            float distance = Vector2.Distance(Owner.Center, Projectile.Center);
            if(distance <= 32 || Timer >= 60)
            {
                Projectile.Kill();
            }
        }
        private void AI_Hook()
        {
            Projectile.extraUpdates = 0;
            Timer++;
            if (Timer == 1)
            {
                float segmentLength = 16;
                VerletChain = new VerletChain(Owner.Center, Projectile.Center, segmentLength);
                SoundStyle hookSound = AssetRegistry.Sounds.Gun.GrappleCharge;
                hookSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hookSound, Projectile.Center);

                float num = 4;
                for (float f = 0; f < num; f++)
                {
                    Vector2 velocity = -Projectile.velocity;
                    velocity = velocity.RotatedByRandom(0.5f);
                    velocity *= Main.rand.NextFloat(0.3f, 0.6f);
                    var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightGray;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(12, 25);
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                    particle.VectorScale *= 0.25f;
                }
            }
            if (VerletChain == null)
                return;

            if (VerletChain.points.Length <= 1)
            {
                Projectile.Kill();
                return;
            }
        

            //Shrink if you hold the use down
            if (this.OwnedByLocalClient() && Owner.controlUseItem && Owner.HeldItem.shoot == Type && VerletChain.points.Length > 3)
            {
                ref VerletPoint currentPoint = ref VerletChain.points[0];
                ref VerletPoint nextPoint = ref VerletChain.points[1];
                currentPoint.position = Vector2.Lerp(currentPoint.position, nextPoint.position, 0.5f);
                currentPoint.oldPosition = currentPoint.position;
                currentPoint.pinned = true;
                float distance = Vector2.Distance(currentPoint.position, nextPoint.oldPosition);
                if(distance <= 1)
                {
                    VerletChain.ShrinkByOne();
                    ResizeShrinkPosition = VerletChain.points[1].position;
                    Projectile.netUpdate = true;
                }


            }
            else
            {
                VerletChain.points[0].pinned=false;

            }

            //Kill the projectile if you right click
            if (this.OwnedByLocalClient() && Owner.controlUseItem && Owner.HeldItem.shoot == Type && Owner.altFunctionUse == 2)
            {
                Projectile.Kill();
                return;
            }

            if(ResizeShrinkPosition != Vector2.Zero)
            {
                float segmentLength = 16;


                VerletChain = new VerletChain(ResizeShrinkPosition, Projectile.Center, segmentLength);
                ResizeShrinkPosition = Vector2.Zero;
            }



            if (VerletChain.points.Length <= 1)
                return;

            ref VerletPoint point = ref VerletChain.points[VerletChain.points.Length - 1];
            point.position = Projectile.Center;
            VerletChain.externalForces = Owner.direction * Vector2.UnitX * 0.1f;
            point.pinned = true;

            Vector2 ropePosition = VerletChain.points[0].position;
            GrapplePlayer grapplePlayer = Owner.GetModPlayer<GrapplePlayer>();
            grapplePlayer.ropePosition = ropePosition;
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
            Owner.itemRotation = Projectile.rotation;
            if(Owner.direction == -1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            if (Owner.controlJump)
            {
                Projectile.Kill();
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.LightGray, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return 2;
        }
        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            switch (State)
            {
                case AIState.Shoot:
                    DrawHookingTrail();
                    break;
                case AIState.Hook:
                    DrawGrappleLinePoints();
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHook(ref lightColor);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }
        private void DrawHook(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawCenter, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
        private void DrawHookingTrail()
        {

            var shader = BasicLaserAlphaShader.Instance;
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;

            float segmentLength = 16;
            float numPoints = Vector2.Distance(Owner.Center, Projectile.Center) / segmentLength;
            numPoints += 1;
      
            List<Vector2> hookTrail = new List<Vector2>();
            for (float n = 0; n < numPoints; n++)
            {
                float completionRatio = n / numPoints;
                Vector2 position = Vector2.Lerp(Owner.Center, Projectile.Center, completionRatio);
                hookTrail.Add(position);
            }

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, hookTrail.ToArray(), ColorFunction, WidthFunction, shader);
        }

        private void DrawGrappleLinePoints()
        {
            if (VerletChain == null)
                return;
            if (GrappleLinePoints.Length <= 2)
                return;

            var shader = BasicLaserAlphaShader.Instance;
            shader.LaserTexture = TrailRegistry.LightningTrail2;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, GrappleLinePoints, ColorFunction, WidthFunction, shader);
        }
    }
}
