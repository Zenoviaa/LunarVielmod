using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.Areas.Dock.WeaponsDK;
using Stellamod.Core.GunSystem;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT
{
    public class TetheredBuff : ModBuff
    {

    }

    public class ShockLineGlobalNPC : GlobalNPC
    {
        public Vector2? ropePosition;
        public override bool InstancePerEntity => true;
        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
            if (ropePosition.HasValue)
            {
                Vector2 positionToRopeTo = ropePosition.Value;
                Vector2 targetVelocity = positionToRopeTo - npc.Center;
                npc.velocity = targetVelocity;
                ropePosition = null;
            }
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (npc.HasBuff<TetheredBuff>())
                return false;
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
    }

    public class ShockLine : ModProjectile
    {
        private float _shockTimer;
        private float _traveledDistance;
        private enum AIState
        {
            Shoot,
            Tether,
            Retract
        }
        private ref float Timer => ref Projectile.ai[0];
     
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private NPC TetheredNPC
        {
            get => Main.npc[(int)Projectile.ai[2]];
            set => Projectile.ai[2] = value.whoAmI;
        }
        private VerletChain VerletChain;
        private Vector2[] _grappleLinePoints;
        private Vector2[] GrappleLinePoints
        {
            get
            {
                if (_grappleLinePoints == null)
                {
                    _grappleLinePoints = new Vector2[VerletChain.points.Length];
                }

                VerletChain.FillArr(_grappleLinePoints);
                return _grappleLinePoints;
            }
        }


        public const float Max_Distance = 16 * 2 * 16;
        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_shockTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _shockTimer = reader.ReadSingle();
        }
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Shoot:
                    AI_Shoot();
                    break;
                case AIState.Retract:
                    AI_Retract();
                    break;
                case AIState.Tether:
                    AI_Tethered();
                    break;
            }
        }

        private void AI_Shoot()
        {
            Projectile.extraUpdates = 2;
            Timer++;
            if(Timer >= 2)
            {
                float traveledDistance = Vector2.Distance(Projectile.position, Projectile.oldPosition);
                _traveledDistance += traveledDistance;
                if(_traveledDistance >= Max_Distance)
                {
                    SwitchState(AIState.Retract);
                }
            }
        }

        private void AI_Retract()
        {
            Projectile.extraUpdates = 1;
            Timer++;
            Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.velocity.Length();
            float distanceToOwner = Vector2.Distance(Projectile.Center, Owner.Center);
            if(distanceToOwner <= 32)
            {
                Projectile.Kill();
            }
        }

        private void AI_Tethered()
        {
            Projectile.extraUpdates = 0;
            Timer++;
            if(Timer == 1)
            {
                float pointLength = 8;
                VerletChain = new VerletChain(Owner.Center, TetheredNPC.Center, pointLength);
            }


            if(_shockTimer <= 0)
            {
                if (this.OwnedByLocalClient())
                {
                    TetheredNPC.SimpleStrikeNPC(Projectile.damage, 1);
                }

                SoundStyle zap = SoundID.DD2_LightningBugZap;
                zap.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zap, Projectile.position);

                for (float f = 0; f < 2; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<ZapParticle>(TetheredNPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.Scale *= 0.25f;
                    spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
                }
                for (float f = 0; f < 2; f++)
                {
                    Vector2 pVelocity = Main.rand.NextVector2Circular(2, 2);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<SparkParticle>(TetheredNPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }

                SoundStyle shockLineSound = AssetRegistry.Sounds.Gun.ShockLineShock;
                shockLineSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shockLineSound, TetheredNPC.position);
                _shockTimer = 60;
            }

            if (_shockTimer > 0)
                _shockTimer--;
            VerletChain.gravity = -0.2f;
            ref VerletPoint start = ref VerletChain.points[0];
            start.pinned = true;
            start.position = Owner.Center;
            ref VerletPoint end = ref VerletChain.points[VerletChain.points.Length - 1];
   
            VerletChain?.Update();


            TetheredNPC.AddBuff(ModContent.BuffType<TetheredBuff>(), 2);
            ShockLineGlobalNPC shockLineGlobalNPC = TetheredNPC.GetGlobalNPC<ShockLineGlobalNPC>();
            shockLineGlobalNPC.ropePosition = end.position;
            if (!TetheredNPC.active)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if(State == AIState.Shoot)
            {
                TetheredNPC = target;
                SwitchState(AIState.Tether);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }
        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            switch (State)
            {
                case AIState.Shoot:
                case AIState.Retract:
                    DrawHookingTrail();
                    break;
                case AIState.Tether:
                    DrawGrappleLinePoints();
                    break;
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            Color baseColor = Color.Lerp(Color.White, Color.LightGray, completionRatio);
            float shockRatio = _shockTimer / 60f;
            Color finalColor = Color.Lerp(baseColor, Color.Yellow, shockRatio);
            return finalColor;
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 2f;
            float shockRatio = _shockTimer / 60f;
            float finalWidth = MathHelper.SmoothStep(width, width * 4, shockRatio);
            return finalWidth;
        }

        private void DrawHookingTrail()
        {
            var shader = BasicLaserAlphaShader.Instance;
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.InnerColor = Color.Lerp(Color.White, Color.Yellow, _shockTimer / 60f);
            shader.OuterColor = Color.Lerp(Color.White, Color.Yellow, _shockTimer / 60f);
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

            var shader = BasicLaserAlphaShader.Instance;
            shader.LaserTexture = TrailRegistry.LightningTrail2;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, GrappleLinePoints, ColorFunction, WidthFunction, shader);
            if(_shockTimer > 0)
            {
                shader.BlendState = BlendState.Additive;
                TrailDrawer.Draw(Main.spriteBatch, GrappleLinePoints, ColorFunction, WidthFunction, shader, offset: Main.rand.NextVector2Circular(8, 8));
            }
        }
    }

    public class ShockingTethergun : BaseGun
    {
        public override void SetDefaults()
        {
            remainingAmmo = 4;
            maxAmmo = 4;
            reloadWindow = 60;
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShockLine>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            SoundStyle shootSound = AssetRegistry.Sounds.Gun.ShockLineShoot;
            shootSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(shootSound, position);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankGun>(),
                material: ModContent.ItemType<GintzlMetal>());
        }
    }
}
