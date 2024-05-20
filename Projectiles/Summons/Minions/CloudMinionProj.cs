using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class CloudMinionProj : ModProjectile
    {
        private static float _orbitCounter;
        private enum ActionState
        {
            Frost_Attack = 0,
            Lightning_Attack = 1,
            Tornado_Attack = 2
        }

        private ref float Timer => ref Projectile.ai[0];
        private ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private ref float AttackTimer => ref Projectile.ai[2];
        private int TornadoIndex = -1;
        private Vector2 TargetPos;
        private Vector3 HuntrianColorXyz;
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(TargetPos);
            writer.Write(TornadoIndex);
            writer.Write(_orbitCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TargetPos = reader.ReadVector2();
            TornadoIndex = reader.ReadInt32();
            _orbitCounter = reader.ReadSingle();
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<CloudMinionBuff>(owner, Projectile))
                return;

            //minion count
            int minionCount = owner.ownedProjectileCounts[Type];
            if (minionCount <= 3)
            {
                State = ActionState.Frost_Attack;
            }
            else if (minionCount <= 6)
            {
                State = ActionState.Lightning_Attack;
            }
            else
            {
                State = ActionState.Tornado_Attack;
            }
   
            switch (State)
            {
                case ActionState.Frost_Attack:
                    AI_FrostAttack();
                    break;

                case ActionState.Lightning_Attack:
                    AI_LightningAttack();
                    break;

                case ActionState.Tornado_Attack:
                    AI_TornadoAttack();
                    break;
            }

            Visuals();
        }

        private void AI_FrostAttack()
        {
            SummonHelper.CalculateIdleValues(Owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);

            //Frost Nearby Enemies
            if (foundTarget)
            {
                //Shoot
                AttackTimer++;
                if (AttackTimer > 90)
                {
                    Vector2 velocity = VectorHelper.VelocityDirectTo(Projectile.Center, targetCenter, 30);

                    //Auroran Bullet Placeholder, it will be instanteous lightning projectile
                    //Maybe just directly damage the target? idk
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<ClimateIceProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    AttackTimer = 0;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer1"), Projectile.position);
                }
            }
        }

        private void AI_LightningAttack()
        {
            SummonHelper.CalculateIdleValues(Owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            //I want  it to move around erratically
            Timer++;
            if (Timer >= 10)
            {
                int range = 200;
                if (!foundTarget)
                    targetCenter = Owner.Center;

                Timer = 0;
                if (Main.myPlayer == Projectile.owner)
                {
                    //Get a new position
                    TargetPos = targetCenter + Main.rand.NextVector2CircularEdge(range, range);
                    Projectile.netUpdate = true;
                }
            }

            Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.position, TargetPos, 15f);

            //Zap Nearby Enemies
            if (foundTarget)
            {
                AttackTimer++;
                if (AttackTimer > 55)
                {
                    Vector2 velocity = VectorHelper.VelocityDirectTo(Projectile.Center, targetCenter, 30);

                    //Auroran Bullet Placeholder, it will be instanteous lightning projectile
                    //Maybe just directly damage the target? idk
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<ClimateLightningProj>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4) * 0.5f,
                      ModContent.ProjectileType<ClimateLightningProj>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4) * 0.5f,
                      ModContent.ProjectileType<ClimateLightningProj>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    AttackTimer = 0;
                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.position);
                }
            }

            //Merge above the player and do large thunderbolts
        }

        private void AI_TornadoAttack()
        {
            _orbitCounter += 0.05f;
            //OK SO
            //WHAT WE NEED TO DO IS.
            //Have the guys move in a n ellipse, how do we do tahat?
            Projectile.Center = CalculateCirclePosition(Owner);
            int tornadoType = ModContent.ProjectileType<ClimateTornadoProj>();
            if (TornadoIndex == -1
                || !Main.projectile[TornadoIndex].active
                || Main.projectile[TornadoIndex].type != tornadoType)
            {
                TornadoIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                 tornadoType, Projectile.damage * 2, Projectile.knockBack, Projectile.owner, ai0: Projectile.whoAmI);
            }
        }

        private Vector2 CalculateCirclePosition(Player owner)
        {      
            //Get the index of this minion
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int count = owner.ownedProjectileCounts[Type];
            float between = 360 / (float)count;
            float degrees = between * minionIndex;
            float circleDistance = 256 + 16;
            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(
                MathHelper.ToRadians(degrees + _orbitCounter));
            return circlePosition;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.DeepPink.R,
                Color.DeepPink.G,
                Color.DeepPink.B, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            switch (State)
            {
                case ActionState.Frost_Attack:
                    DrawHelper.DrawAdditiveAfterImage(Projectile, Color.LightCyan * 0.3f, Color.White * 0.3f, ref lightColor);
                    break;
                case ActionState.Lightning_Attack:
                    DrawHelper.DrawAdditiveAfterImage(Projectile, Color.MediumPurple * 0.3f, Color.White * 0.3f, ref lightColor);
                    break;
                case ActionState.Tornado_Attack:
                    DrawHelper.DrawAdditiveAfterImage(Projectile, Color.LightGreen * 0.3f, Color.White * 0.3f, ref lightColor);
                    break;
            }

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            switch (State)
            {
                case ActionState.Frost_Attack:
                    DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.LightCyan, lightColor, 2);
                    break;
                case ActionState.Lightning_Attack:
                    DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.MediumPurple, lightColor, 2);
                    break;
                case ActionState.Tornado_Attack:
                    DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.LightGreen, lightColor, 2);
                    break;
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 7);

            switch (State)
            {
                case ActionState.Frost_Attack:
                    HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                        new Vector3(85, 45, 150),
                        new Vector3(15, 60, 60),
                        new Vector3(3, 3, 3), 0);


                    if (Main.rand.NextBool(12))
                    {
                        int count = 2;
                        for (int k = 0; k < count; k++)
                        {
                            Dust.NewDust(Projectile.position, 8, 8, DustID.Frost);
                        }
                    }

                    if (Main.rand.NextBool(12))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                            var particle = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<IceyParticle>(), Color.White, Main.rand.NextFloat(.2f, .4f));
                            particle.timeLeft = 12;
                        }
                    }
                    break;
                case ActionState.Lightning_Attack:
                    HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                        new Vector3(125, 100, 40),
                        new Vector3(15, 60, 60),
                        new Vector3(3, 3, 3), 0);

                    if (Main.rand.NextBool(12))
                    {
                        int count = 2;
                        for (int k = 0; k < count; k++)
                        {
                            Dust.NewDust(Projectile.position, 8, 8, DustID.Electric);
                        }
                    }

                    break;
                case ActionState.Tornado_Attack:
                    HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                        new Vector3(125, 150, 40),
                        new Vector3(15, 60, 60),
                        new Vector3(3, 3, 3), 0);

                    if (Main.rand.NextBool(12))
                    {
                        int count = 2;
                        for (int k = 0; k < count; k++)
                        {
                            Dust.NewDust(Projectile.position, 8, 8, DustID.Vortex);
                        }
                    }

                    break;
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
