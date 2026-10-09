
using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core;
using Stellamod.Core.Astar;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Summons.Minions;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class VehementRhapsody : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToBellMinion(ModContent.ProjectileType<VehementMinionProj>());
            Item.damage = 13;
            Item.knockBack = 3;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankStaff>(), 
                material: ModContent.ItemType<Cinderscrap>());
        }
    }

    public class VehementMinionProj : AbstractBellSummon
    {
        enum AIState
        {
            Idle,
            GoHome,
            ChaseTarget,
            AttackTarget
        }

        float _globalTimer;
        Targeter _targeter;
        Pathfinder _pathfinder;
        float alphaCounter = 0;
        private ref float Timer => ref Projectile.ai[0];
        private ref float SpeedTimer => ref Projectile.ai[1];
        private ref float HitCount => ref Projectile.ai[2];
        AIState _state;
        float RunSpeed => 15;
        float HomeRange => 164 * 164;
        float SqrDistanceHome => 1024 * 1024;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write((byte)_state);
            _targeter.NetSend(writer);
            writer.Write(_globalTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _state = (AIState)reader.ReadByte();
            _targeter.NetReceive(reader);
            _globalTimer = reader.ReadSingle();
        }
        public override void SetStaticDefaults()
        {
            Projectile.SetTrailCacheLength(30);
            Projectile.StaticDefaultToMinionProjectile();
        }

        public sealed override void SetDefaults()
        {
            _pathfinder = new();
            Projectile.DefaultToMinionProjectile();
            Projectile.tileCollide = false;
            Projectile.WidthAndHeight = 24;
            Projectile.LocalPiercingImmunityTime = 20;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.light = 0.67f;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return _state == AIState.AttackTarget;
        }


        public override int GetAggro()
        {
            return -90;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (SpeedTimer > 0)
            {
                SpeedTimer--;
                Projectile.extraUpdates = 3;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }

            if (Main.rand.NextBool(12))
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Yellow,
                    outerColor = Color.Red,
                    scaleRange = new Vector2(0.3f, 0.7f),
                    gravity = 0
                };
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(1, 1), spawnParams);
                dp.dampening = 0.1f;
            }
            var dstHome = Vector2.DistanceSquared(Owner.Center, Projectile.Center);
            if(dstHome > SqrDistanceHome)
            {
                var posToGoTo = Owner.Center + new Vector2(0, -64);
                var targetVelocity = posToGoTo - Projectile.Center;
                Projectile.velocity = targetVelocity * 0.1f;
                return;
            }

            switch (_state)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.GoHome:
                    AI_GoHome();
                    break;
                case AIState.ChaseTarget:
                    AI_ChaseTarget();
                    break;
                case AIState.AttackTarget:
                    AI_AttackTarget();
                    break;
            }
  
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        void SwitchState(AIState state)
        {
            _globalTimer = 0;
            Timer = 0;
            _state = state;
        }

    
        void AI_Idle()
        {
            _globalTimer++;
            Timer++;

            if (Timer >= 30)
                MoonUtils.SearchForNewTarget(Owner.Center, Projectile.Center, ref _targeter.targetNpc);

            var xOfffset = MathHelper.Lerp(-64, 64, ExtraMath.Osc(0f, 1f, speed: 0, Projectile.minionPos * 2));
            xOfffset += MathHelper.Lerp(-32f, 32f, MathF.Sin(_globalTimer * 0.025f) * 0.5f + 0.5f);
            var targetPos = Owner.Center + new Vector2(0, -48) + new Vector2(xOfffset, 0);
            MoonUtils.AI_FloatAbove(Projectile.Center, ref Projectile.velocity, targetPos);
   
            var sqrDist = Vector2.DistanceSquared(Projectile.Center, Owner.Center);
            if (sqrDist > HomeRange)
            {
                SwitchState(AIState.GoHome);
            }
            if (_targeter.targetNpc != -1
                && sqrDist < 64 * 64 && _globalTimer >= 30)
            {
                SwitchState(AIState.ChaseTarget);
            }
        }

        void AI_GoHome()
        {
            _globalTimer++;
            MoonUtils.SearchForNewTarget(Owner.Center, Projectile.Center, ref _targeter.targetNpc);
            MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, Owner.Center, RunSpeed, ref _targeter.targetOldPos);

            var sqrDist = Vector2.DistanceSquared(Projectile.Center, Owner.Center);

            if (_targeter.targetNpc != -1 
                && sqrDist < 64 * 64 && _globalTimer >= 30)
            {
                SwitchState(AIState.ChaseTarget);
            }
            if (sqrDist < 64 * 64)
            {
                SwitchState(AIState.Idle);
            }
        }

        void AI_ChaseTarget()
        {
            MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, _targeter.Target.Center, RunSpeed, ref _targeter.targetOldPos);
            MoonUtils.SearchForNewTarget(Owner.Center, Projectile.Center, ref _targeter.targetNpc);
            if (_targeter.targetNpc != -1 && 
                Collision.CanHitLine(Projectile.position, 1,1 , _targeter.Target.position, 1, 1))
            {
                SwitchState(AIState.AttackTarget);
            }
        }

        void AI_AttackTarget()
        {

            _globalTimer++;
            if(_globalTimer < 35)
            {
                Projectile.velocity *= 0.92f;
                return;
            }
            float progress = MathHelper.Clamp(Timer / 35f, 0f, 1f);
            float d = MathHelper.Lerp(3f, 45, progress);
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, _targeter.Target.Center, d);
            if (Projectile.velocity.Length() < 15)
            {
                Projectile.velocity *= 1.5f;
            }

            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.Y -= 1;
            }
            if(!_targeter.Target.active || _globalTimer >= 120)
                SwitchState(AIState.GoHome);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
            if (SpeedTimer <= 0)
            {
                HitCount++;
                if (HitCount >= 15)
                {
                    HitCount = 0;
                    SpeedTimer = 240;
                    _globalTimer = 0;
                }
            }

            var EntitySource = Projectile.GetSource_Death();
            Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0,
                ModContent.ProjectileType<VehementBoom>(), Projectile.damage, 1, Projectile.owner, 0, 0);
            Projectile.velocity = -Projectile.velocity;
            int Sound = Main.rand.Next(1, 6);
            SoundStyle mySound = AssetReferences.Assets.Sounds.Rhap.Asset with { PitchVariance = 1f };
            Timer = 1;
            mySound.Volume = 0.15f;
            mySound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(mySound, Projectile.position);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.5f;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkOrange, Color.LightGoldenrodYellow, completionRatio) * 0.7f;
        }

        private void DrawVehementTrail(GraphicsDevice graphicsDevice)
        {
            RichLaserShader richLaserShader = RichLaserShader.Instance;
            richLaserShader.LaserColor = Color.Yellow * 0.6f;
            richLaserShader.InnerColor = Color.OrangeRed * 0.6f;
            richLaserShader.OuterColor = Color.Red * 0.6f;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, richLaserShader, Projectile.Size * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawVehementTrail, DrawLayer.BehindNPCsWithOutline);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
            return false;
        }
    }

    public class VehementBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 100;
            Projectile.timeLeft = 15;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (float f = 0; f < 2; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MusicDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float i = 0; i < 2; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Orange,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(12, 25),
                        baseSize: Main.rand.NextFloat(0.01f, 0.15f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }
    }
}