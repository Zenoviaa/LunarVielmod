using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{

    public class FireflyMinionProj : ModProjectile
    {
        private float _attackCooldown;
        private static float _orbitingOffset;
        public enum AttackState
        {
            Defense_Mode = 0,
            Attack_Mode = 1
        };

        public AttackState AttackStyle { get; set; }
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            //Only have contact damage in defense mode.
            return AttackStyle == AttackState.Defense_Mode;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (AttackStyle == AttackState.Defense_Mode)
            {
                //Boosted Contact Damage in defense mode
                modifiers.FinalDamage += 3.5f;

            }
        }

        public Color GlowColor;
        public Vector3 HuntrianColorXyz;
        public float HuntrianColorOffset;
        public float Timer;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.Yellow, lightColor, 2);
            return base.PreDraw(ref lightColor);
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<FireflyMinionBuff>(owner, Projectile))
                return;

            HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(85, 45, 15),
                new Vector3(15, 60, 60),
                new Vector3(3, 3, 3), HuntrianColorOffset);

            Timer++;
            if (Timer <= 2)
            {
                HuntrianColorOffset = Main.rand.NextFloat(-1f, 1f);
            }

            //Fierflies orbit faster the more you have.
            Vector2 circlePosition = CalculateCirclePosition(owner);
            Projectile.Center = circlePosition;
            float attackCooldown;
            switch (AttackStyle)
            {
                default:
                case AttackState.Defense_Mode:
                    //Attack Slower in defense mode
                    _orbitingOffset += 0.01f;
                    attackCooldown = 80;
                    break;
                case AttackState.Attack_Mode:
                    _orbitingOffset += 0.33f;
                    attackCooldown = 25;
                    break;
            }

            SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            _attackCooldown--;
            if (_attackCooldown <= 0 && foundTarget)
            {
                //Fire Projectile
                Vector2 velocity = VectorHelper.VelocityDirectTo(Projectile.Center, targetCenter, 30);
                Projectile projectile = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<FireflyBomb>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                projectile.DamageType = DamageClass.Summon;

                //How many ticks between attacks?
                _attackCooldown = attackCooldown;
                //_scaleOffset += 0.1f;
            }

            Visuals();
        }

        private Vector2 CalculateCirclePosition(Player owner)
        {
            //Get the index of this minion
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int fireflyCount = owner.ownedProjectileCounts[Type];
            float degreesBetweenFirefly = 360 / (float)fireflyCount;
            float degrees = degreesBetweenFirefly * minionIndex;
            float circleDistance = AttackStyle == AttackState.Defense_Mode ? 48f : 80f;

            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitingOffset));
            switch (AttackStyle)
            {
                case AttackState.Attack_Mode:

                    break;
                case AttackState.Defense_Mode:
                    //float factor =  / (float)fireflyCount;
                    float t = _orbitingOffset + (minionIndex / (float)fireflyCount) * minionIndex;
                    circlePosition = owner.Center + VectorHelper.PointOnHeart(t, VectorHelper.Osc(5, 10));
                    break;
            }

            return circlePosition;
        }


        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 3);
            if (Main.rand.NextBool(12))
            {
                int count = 3;
                for (int k = 0; k < count; k++)
                {
                    Dust.NewDust(Projectile.position, 8, 8, DustID.CopperCoin);
                }
            }

            if (Main.rand.NextBool(12))
            {
                Vector2 randomOrigin = new Vector2(Main.rand.NextFloat(-8, 8f));
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                ParticleManager.NewParticle(Projectile.Center - randomOrigin, speed * 2, ParticleManager.NewInstance<SparkleTrailParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
