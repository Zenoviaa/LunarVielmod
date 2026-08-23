

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Bow;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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



    public class IceboundMinionProj : AbstractBellSummon
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float IsLeader => ref Projectile.ai[1];
        private ref float CooldownTimer => ref Projectile.ai[2];
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

        public override void SetStaticDefaults()
        {

            // DisplayName.SetDefault("Jelly Minion");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
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

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 26;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0.5f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
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

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail(Projectile.oldPos);
            return base.PreDraw(ref lightColor);
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

        public override void AI()
        {
            base.AI();
            Timer++;
            if (this.OwnedByLocalClient())
            {
                if (Timer == 1 && ThereIsNoLeader())
                {
                    IsLeader = 1;

                }

            }
            Player player = Main.player[Projectile.owner];
            Projectile.spriteDirection = Projectile.direction;
            CooldownTimer--;
            bool isLeader = Leader.whoAmI == Projectile.whoAmI;
            if (isLeader)
            {
                SummonHelper.SearchForTargets(player, Projectile,
                    out bool foundTarget,
                    out float distanceFromTarget,
                    out Vector2 targetCenter);
                if (foundTarget)
                {
                    if(CooldownTimer <= 0)
                        AI_MoveToward(targetCenter, 12, 1);
                }
                else
                {
                    Vector2 idlePosition = player.Center + new Vector2(0, -48);
                    SummonHelper.CalculateIdleValuesWithOverlap(player, Projectile,
                        out Vector2 vectorToIdlePosition,
                        out float distanceToIdlePosition);
                    SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                }
            }
            else
            {
                SummonHelper.SearchForTargets(player, Leader,
                    out bool foundTarget,
                    out float distanceFromTarget,
                    out Vector2 foundTargetCenter);
                if (!foundTarget)
                {
                    SummonHelper.CalculateIdleValues(player, Projectile,
                        Leader.Center,

                           out Vector2 vectorToIdlePosition,
                           out float distanceToIdlePosition);
                    SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
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
            }

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
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm"), Projectile.position);
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


    }
}