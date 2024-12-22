using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage.Tomes
{
    internal class StormWelder : BaseMagicTomeItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.shoot = ModContent.ProjectileType<StormWelderTome>();

        }
    }

    internal class StormWelderTome : BaseMagicTomeProjectile
    {
        private float _dustTimer;
        public override string Texture => this.PathHere() + "/StormWelder";
        public override void SetDefaults()
        {
            base.SetDefaults();
            //How often it shoots
            AttackRate = 12;

            //How fast it drains mana, better to change the mana use in the item instead of this tho
            ManaConsumptionRate = 4;

            //How far the tome is held from the player
            HoldDistance = 36;

            //The glow effect around it
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.05f;
        }

        public override void AI()
        {
            base.AI();
            _dustTimer++;
            if (_dustTimer % 16 == 0)
            {
                Color color = Color.Lerp(Color.White, Color.LightGoldenrodYellow, Main.rand.NextFloat(0f, 1f));
                color.A = 0;
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color, Scale: 0.33f);
            }
        }
        protected override void Shoot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Shoot(player, source, position, velocity, damage, knockback);
            Projectile.NewProjectile(source, position, velocity,
                ModContent.ProjectileType<StormWelderLightning>(), damage, knockback, Projectile.owner,
                ai1: Projectile.whoAmI);
            SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, position);
        }
    }

    internal class StormWelderBeam
    {
        public StormWelderBeam(Vector2[] pos)
        {
            Trail = new();
            //Making this number big made like the field wide
            Trail.LightningRandomOffsetRange = 5;

            //This number makes it more lightning like, lower this is the straighter it is
            Trail.LightningRandomExpand = 24;
            LightningPos = pos;
            Trail.RandomPositions(pos);
        }

        public Vector2[] LightningPos;
        public Common.Shaders.MagicTrails.LightningTrail Trail;

        public void Update(float timer)
        {
            if (timer % 6 == 0)
            {
                Trail.RandomPositions(LightningPos);
            }
        }
    }

    internal class StormWelderLightning : ModProjectile
    {

        private float _trailWidth;
        private List<StormWelderBeam> _beams;
        private Common.Shaders.MagicTrails.LightningTrail[] _lightningTrail;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Parent => ref Projectile.ai[1];
        private const int NumSamplePoints = 3;
        private float BeamLength;
        private float MaxBeamLength => 800;
        private Vector2 ImpactPos;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            _beams = new List<StormWelderBeam>();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.friendly = true;
            Projectile.timeLeft = 12;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            for (int i = 0; i < _beams.Count; i++)
            {
                StormWelderBeam beam = _beams[i];
                Vector2[] positions = beam.LightningPos;
                float collisionPoint = 0;
                for (int j = 1; j < positions.Length; j++)
                {
                    Vector2 position = positions[j];
                    Vector2 previousPosition = positions[j - 1];
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 12, ref collisionPoint))
                        return true;
                }
            }

            return false;
        }

        private NPC FindNextTarget(HashSet<NPC> exclude)
        {
            float maxSeekingDistance = MaxBeamLength;
            NPC nextTarget = null;
            foreach (var npc in Main.ActiveNPCs)
            {
                float distanceToNpc = Vector2.Distance(ImpactPos, npc.Center);
                if (distanceToNpc <= maxSeekingDistance && !exclude.Contains(npc))
                    return npc;
            }
            return nextTarget;
        }

        private float PerformBeamHitscan()
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = Projectile.Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[NumSamplePoints];


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;
            return averageLengthSample;
        }


        private void InitialRay()
        {
            List<Vector2> lightningPositions = new List<Vector2>();
            float dist = Vector2.Distance(Projectile.Center, ImpactPos);
            float basePointCount = dist / 16f;
            int pointCount = (int)MathHelper.Clamp(basePointCount, 1, float.MaxValue);
            for (int i = 0; i < pointCount; i++)
            {
                float progress = (float)i / pointCount;
                lightningPositions.Add(Vector2.Lerp(Projectile.Center, ImpactPos, progress));
            }
            StormWelderBeam beam = new StormWelderBeam(lightningPositions.ToArray());
            _beams.Add(beam);
        }
        public override void AI()
        {
            base.AI();

            //OK SO HERE'S WHAT WE DO
            Timer++;
            Projectile.Center = Main.projectile[(int)Parent].Center;
            //Starting impact pos
            ImpactPos = Projectile.Center;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = (Main.MouseWorld - Main.player[Projectile.owner].Center);
                //HITSCAN
                float targetBeamLength = PerformBeamHitscan();
                BeamLength = targetBeamLength;
                ImpactPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
                float distanceToMouse = Vector2.Distance(Projectile.Center, Main.MouseWorld);
                if (distanceToMouse <= BeamLength)
                {
                    ImpactPos = Main.MouseWorld;
                }
            }

            //So first thing we do is shoot the beam out
            for (int i = 0; i < _beams.Count; i++)
            {
                StormWelderBeam beam = _beams[i];
                beam.Update(Timer);
            }

            if (Timer % 2 == 0 || Timer == 1)
            {
                HashSet<NPC> npcsThatHaveBeenHit = new HashSet<NPC>();
                _beams.Clear();
                InitialRay();
                NPC nextTarget = FindNextTarget(npcsThatHaveBeenHit);
                while (nextTarget != null)
                {
                    Vector2 startPos = ImpactPos;
                    Vector2 endPos = nextTarget.Center + (nextTarget.Center - startPos).SafeNormalize(Vector2.Zero) * 8;
                    List<Vector2> lightningPositions = new List<Vector2>();
                    float dist = Vector2.Distance(startPos, endPos);
                    float basePointCount = dist / 16f;
                    int pointCount = (int)MathHelper.Clamp(basePointCount, 1, float.MaxValue);
                    for (int i = 0; i < pointCount; i++)
                    {
                        float f = i;
                        float length = pointCount;
                        float progress = f / length;
                        Vector2 pos = Vector2.Lerp(startPos, endPos, progress);
                        lightningPositions.Add(pos);
                    }


                    //Add the beam to the llist
                    StormWelderBeam beam = new StormWelderBeam(lightningPositions.ToArray());
                    _beams.Add(beam);

                    //Update the impact pos so the next one starts from there
                    ImpactPos = nextTarget.Center;

                    //Add the npc
                    npcsThatHaveBeenHit.Add(nextTarget);
                    nextTarget = FindNextTarget(npcsThatHaveBeenHit);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Electrified, 30);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            return base.PreDraw(ref lightColor);
        }

        private void DrawTrail()
        {
            //Trail
            _trailWidth = 12;
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningBolt2Shader lightningShader = LightningBolt2Shader.Instance;
            lightningShader.PrimaryColor = Color.Yellow;
            lightningShader.NoiseColor = new Color(120, 215, 255);
            lightningShader.Speed = 5;

            for (int i = 0; i < _beams.Count; i++)
            {
                StormWelderBeam beam = _beams[i];
                beam.Trail.Draw(spriteBatch, beam.LightningPos, Projectile.oldRot, ColorFunction, WidthFunction, lightningShader, offset: Projectile.Size / 2f);
            }
        }

        private float WidthFunction(float completionRatio)
        {
            float progress = completionRatio / 0.3f;
            float rounded = Easing.SpikeOutCirc(progress);
            float spikeProgress = Easing.SpikeOutExpo(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));

            float midWidth = _trailWidth;
            float w = MathHelper.Lerp(0, midWidth, fireball);
            _trailWidth -= 0.02f;
            if (_trailWidth <= 3)
                _trailWidth = 3;
            return w;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Yellow, p);
            return trailColor;
        }
    }
}
