

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Astar;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Bow;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.BackupIO;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class IceboundStaff : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToBellMinion(ModContent.ProjectileType<IceboundMinionProj>());
            Item.damage = 16;
            Item.knockBack = 3f;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(),
                material: ModContent.ItemType<WinterbornShard>());
        }
    }



    public class IceboundMinionProj : AbstractBellSummon,
        IDrawToRenderTarget
    {
        enum AIState : byte
        {
            Idle,
            GoHome,
            Chase,
            Attack
        }
        Targeter _targeter;
        Pathfinder _pathfinder;
        private ref float Timer => ref Projectile.ai[0];
        private ref float IsLeader => ref Projectile.ai[1];
        private ref float CooldownTimer => ref Projectile.ai[2];
        AIState _state;
        private Projectile Leader
        {
            get
            {
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != Projectile.owner)
                        continue;
                    if (proj.type != Type)
                        continue;
                    if (proj.ai[1] > 0)
                        return proj;
                }
                return Projectile;
            }
        }
        float HomeSqrDist => 200 * 200;
        float HomeRange => 128 * 128;
        float RunSpeed => 8;
        public bool ThereIsNoLeader()
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.type != Type)
                    continue;
                if (proj.ai[1] > 0)
                    return false;
            }
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write((byte)_state);
            _targeter.NetSend(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _state = (AIState)reader.ReadByte();
            _targeter.NetReceive(reader);
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Projectile.SetTrailCacheLength(16);
            Projectile.StaticDefaultToMinionProjectile();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _pathfinder = new();
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.DefaultToMinionProjectile();
            Projectile.LocalPiercingImmunityTime = 30;
            Projectile.minionSlots = 0.5f;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return _state == AIState.Attack;
        }

        public void DrawTrail(Vector2[] oldPos)
        {
            var shader = BasicLaserAlphaShader.Instance;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.SpringGreen, completionRatio) * MathHelper.SmoothStep(1f, 0f, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(12, 0, completionRatio);
        }

        void DrawPixelatedTrail(GraphicsDevice gDevice)
        {

        }
        private void AI_MoveToward(Vector2 targetCenter, float speed = 8, float accel = 16)
        {
            //chase target
            Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(Projectile.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;
            if (Projectile.velocity.X < targetVelocity.X)
            {
                Projectile.velocity.X += accel;
                if (Projectile.velocity.X >= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }
            else if (Projectile.velocity.X > targetVelocity.X)
            {
                Projectile.velocity.X -= accel;
                if (Projectile.velocity.X <= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }

            if (Projectile.velocity.Y < targetVelocity.Y)
            {
                Projectile.velocity.Y += accel;
                if (Projectile.velocity.Y >= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
            else if (Projectile.velocity.Y > targetVelocity.Y)
            {
                Projectile.velocity.Y -= accel;
                if (Projectile.velocity.Y <= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
        }

        void SwitchState(AIState state)
        {
            Timer = 0;
            _state = state;
            Projectile.netUpdate = true;
        }

        void AI_Idle()
        {
            Timer++;
            var targetPoint = MoonUtils.CalculateHoverAbovePoint(Owner.Center, Timer, Projectile.minionPos);
            MoonUtils.AI_FloatAbove(Projectile.Center, ref Projectile.velocity, targetPoint);
            MoonUtils.SearchForNewTargetByLineOfSight(Owner.Center, Projectile.Center, ref _targeter.targetNpc);
            if(_targeter.HasValidTarget && Timer >= 30)
            {
                SwitchState(AIState.Chase);
            }

            var sqrDistHome = Vector2.DistanceSquared(Owner.Center, Projectile.Center);
            if(sqrDistHome > HomeSqrDist)
            {
                SwitchState(AIState.GoHome);
            }
        }
        void AI_Chase()
        {
            Timer++;
            MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, _targeter.Target.Center, RunSpeed, ref _targeter.targetOldPos);
            MoonUtils.SearchForNewTargetByLineOfSight(Owner.Center, Projectile.Center, ref _targeter.targetNpc);
            if (_targeter.targetNpc != -1 &&
                Collision.CanHitLine(Projectile.position, 1, 1, _targeter.Target.position, 1, 1))
            {
                SwitchState(AIState.Attack);
            }
            if (!_targeter.Target.active)
                SwitchState(AIState.GoHome);
        }

        void AI_Attack()
        {
            Timer++;
            bool isLeader = Leader.whoAmI == Projectile.whoAmI;
            if (isLeader)
            {
                if (CooldownTimer <= 0)
                    AI_MoveToward(_targeter.Target.Center, 8, 1);
            }
            else
            {
                Vector2 targetCenter = Leader.Center;
                float distanceToLeader = Vector2.Distance(Projectile.Center, targetCenter);
                if (distanceToLeader > 64)
                {
                    if (CooldownTimer <= 0)
                        AI_MoveToward(targetCenter, 16, 1);
                }
            }

            if(Timer >= 140)
            {
                SwitchState(AIState.GoHome);
            }
        }

        void AI_GoHome()
        {
            Timer++;
            var targetPoint = MoonUtils.CalculateHoverAbovePoint(Owner.Center, Timer, Projectile.minionPos);
            MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, targetPoint, RunSpeed, ref _targeter.targetOldPos);
            var sqrDst = Vector2.DistanceSquared(Projectile.Center, targetPoint);
            if (sqrDst <= HomeRange)
            {
                SwitchState(AIState.Idle);
            }
        }


        public override void AI()
        {
            base.AI();

            switch (_state)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.GoHome:
                    AI_GoHome();
                    break;
                case AIState.Chase:
                    AI_Chase();
                    break;
                case AIState.Attack:
                    AI_Attack();
                    break;
            }
            if (this.OwnedByLocalClient())
            {
                if (Timer == 1 && ThereIsNoLeader())
                {
                    IsLeader = 1;
                }
            }
            Projectile.spriteDirection = Projectile.direction;
            CooldownTimer--;
            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.velocity = Main.rand.NextVector2CircularEdge(16, 16);
            Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
            CooldownTimer = 5;
            Projectile.netUpdate = true;
            if (Main.rand.NextBool(16))
            {
                SoundEngine.PlaySound(AssetReferences.Assets.Sounds.WinterStorm.Asset with { PitchVariance = 0.75f, Volume = 0.4f }, Projectile.position);
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<WinterboundArrowFlake>(), Projectile.damage / 2, 1, Projectile.owner);
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);


            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public void DrawToRenderTargets()
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
        }
    }
}