using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
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

namespace Stellamod.Projectiles.Summons.Sentries
{
    public class PotOfGreedMinionProj : ModProjectile
    {
        private int _particleCounter;
        private int[] _shadowMinionLifeTime;
        private Projectile[] _shadowMinions;
        private int _newShadowMinionSpawnTimer;
        private float _rotation;

        private const int Max_Shadow_Minion_Count = 7;
        private const int Shadow_Minion_Lifetime = 120;
        private const int Time_Between_Shadow_Minions = 10;
        private const float Shadow_Minion_Summon_Radius = 196;

        //Visuals 
        private const float Body_Radius = 64;
        private const int Body_Particle_Count = 12;

        //Lower number = faster
        private const int Body_Particle_Rate = 2;
        public override void SetStaticDefaults()
        {
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;

            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 80;

            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            //Projectile.friendly = true;

            // Only determines the damage type

            //I DON'T KNOW IF I NEED TO SET minion to true for sentries, I'm not going to
            //	Projectile.minion = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0f;

            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;

            _shadowMinions = new Projectile[Max_Shadow_Minion_Count];
            _shadowMinionLifeTime = new int[Max_Shadow_Minion_Count];
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Void Pre-Draw Effects
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            return true;
        }

        private bool CanSpawnShadowMinion()
        {
            Player player = Main.player[Main.myPlayer];
            if (player.Distance(Projectile.position) > Shadow_Minion_Summon_Radius)
                return false;
            for (int i = 0; i < _shadowMinions.Length; i++)
            {
                Projectile shadowMinion = _shadowMinions[i];
                if (shadowMinion == null)
                    return true;

            }
            return false;
        }


        private int GetNextShadowMinionIndex()
        {
            for (int s = 0; s < _shadowMinions.Length; s++)
            {
                if (_shadowMinions[s] == null)
                {
                    return s;
                }
            }
            return -1;
        }

        private bool IsShadowMinion(Projectile minion)
        {
            if (minion == Projectile)
                return true;

            for (int s = 0; s < _shadowMinions.Length; s++)
            {
                if (_shadowMinions[s] == minion)
                {
                    return true;
                }
            }
            return false;
        }

        private void KillShadowMinions()
        {
            for (int i = 0; i < _shadowMinionLifeTime.Length; i++)
            {
                _shadowMinionLifeTime[i]--;
                Projectile shadowMinion = _shadowMinions[i];
                if (shadowMinion == null)
                    continue;

                if (_shadowMinionLifeTime[i] <= 0 && shadowMinion.active)
                {
                    int dustCircleCount = 16;
                    float degreesPer = 360 / (float)dustCircleCount;
                    for (int k = 0; k < dustCircleCount; k++)
                    {
                        float degrees = k * degreesPer;
                        Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                        Vector2 vel = direction * 8;
                        Dust.NewDust(shadowMinion.Center, 0, 0, DustID.GemAmethyst, vel.X * 0.5f, vel.Y * 0.5f);
                    }

                    SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.position);
                    shadowMinion.Kill();
                    _shadowMinions[i] = null;
                }
            }
        }

        private void ForceKillShadowMinions()
        {
            for (int i = 0; i < _shadowMinionLifeTime.Length; i++)
            {
                _shadowMinionLifeTime[i]--;
                Projectile shadowMinion = _shadowMinions[i];
                if (shadowMinion == null)
                    continue;

                if (shadowMinion.active)
                {
                    int dustCircleCount = 16;
                    float degreesPer = 360 / (float)dustCircleCount;
                    for (int k = 0; k < dustCircleCount; k++)
                    {
                        float degrees = k * degreesPer;
                        Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                        Vector2 vel = direction * 8;
                        Dust.NewDust(shadowMinion.Center, 0, 0, DustID.GemAmethyst, vel.X * 0.5f, vel.Y * 0.5f);
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.position);

                    shadowMinion.Kill();
                    _shadowMinions[i] = null;
                }
            }
        }

        private void UpdateShadowMinions()
        {
            _newShadowMinionSpawnTimer--;
            if (_newShadowMinionSpawnTimer <= 0 && CanSpawnShadowMinion())
            {
                //Spawn a new shadow minion;
                //Get the type of a minion that this player owns, if none this won't do anything
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectileToClone = Main.projectile[i];

                    //Only chec stuff that this guy owns
                    if (projectileToClone.owner != Projectile.owner)
                        continue;

                    //Only check minions\\\\
                    if (!projectileToClone.minion || projectileToClone.sentry)
                        continue;

                    if (IsShadowMinion(projectileToClone))
                        continue;

                    //If we're here, then we have the thing that we want	
                    Vector2 shadowMinionSpawnPosition = Projectile.Center + new Vector2(0, -32);
                    Projectile shadowMinion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), shadowMinionSpawnPosition, Vector2.Zero,
                        projectileToClone.type, Projectile.damage, projectileToClone.knockBack, Projectile.owner);

                    shadowMinion.minionSlots = 0;

                    int shadowMinionIndex = GetNextShadowMinionIndex();
                    _shadowMinions[shadowMinionIndex] = shadowMinion;
                    _shadowMinionLifeTime[shadowMinionIndex] = Shadow_Minion_Lifetime;
                    _newShadowMinionSpawnTimer = Time_Between_Shadow_Minions;
                    SoundEngine.PlaySound(SoundID.Item117, Projectile.position);

                    int dustCircleCount = 48;
                    float degreesPer = 360 / (float)dustCircleCount;
                    for (int k = 0; k < dustCircleCount; k++)
                    {
                        float degrees = k * degreesPer;
                        Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                        Vector2 vel = direction * 8;
                        Dust.NewDust(shadowMinionSpawnPosition, 0, 0, DustID.GemAmethyst, vel.X * 0.5f, vel.Y * 0.5f);
                    }

                    //Break out of this loop so we don't spawn more on accident
                    break;
                }
            }

            Player player = Main.player[Main.myPlayer];
            bool isWithin = player.Distance(Projectile.position) < Shadow_Minion_Summon_Radius;
            if (isWithin)
            {
                for (int i = 0; i < _shadowMinionLifeTime.Length; i++)
                {
                    _shadowMinionLifeTime[i] = Shadow_Minion_Lifetime;
                }
            }
        }


        private void ShadowMinionVisuals()
        {
            for (int i = 0; i < _shadowMinions.Length; i++)
            {
                Projectile shadowMinion = _shadowMinions[i];
                if (shadowMinion == null)
                    continue;

                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(shadowMinion.position, shadowMinion.width, shadowMinion.height, DustID.GemAmethyst);
                    Main.dust[dust].scale = 1.5f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<PotOfGreedMinionBuff>(Main.player[Projectile.owner], Projectile))
                return;

            //Actual Effect
            //Is create shadow clones of your minion
            UpdateShadowMinions();
            KillShadowMinions();
            ShadowMinionVisuals();

            //Visuals
            _rotation += 0.05f;
            int count = 4;

            //This is the flame coming out of the pot
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {
                for (int i = 0; i < Body_Particle_Count; i++)
                {
                    Vector2 position = Projectile.position + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
                    position += new Vector2(Projectile.width / 2, -24);
                    Particle p = ParticleManager.NewParticle(position, new Vector2(0, -2f), ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), Main.rand.NextFloat(0.5f, 1f));
                    p.layer = Particle.Layer.BeforeProjectiles;
                }
                _particleCounter = 0;
            }


            //This is the ring that shows where the shadow minions spawn
            for (int i = 0; i < count; i++)
            {
                Vector2 position = Projectile.Center + new Vector2(Shadow_Minion_Summon_Radius, 0).RotatedBy(((i * MathHelper.PiOver2 / count) + _rotation) * 4);
                ParticleManager.NewParticle(position, new Vector2(0, -0.25f), ParticleManager.NewInstance<VoidParticle>(), default(Color), 1 / 3f);
            }

            float hoverSpeed = 5;
            float rotationSpeed = 2.5f;
            float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
            float rotation = VectorHelper.Osc(MathHelper.ToRadians(-5), MathHelper.ToRadians(5), rotationSpeed);
            Projectile.velocity = new Vector2(0, yVelocity);
            Projectile.rotation = rotation;
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            ForceKillShadowMinions();
        }
    }
}
