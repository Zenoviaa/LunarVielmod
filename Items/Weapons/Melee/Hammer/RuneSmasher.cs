using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Particles;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee.Hammer
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod


    public class RuneSmasher : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.LunarVeil.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Throwing;
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 126;
            Item.useAnimation = 126;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<RuneHammerSlash>();
            Item.autoReuse = true;

            comboWaitTime = 121;
            maxCombo = 9;



            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 2;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<RuneHammerStaminaSlash>();
        }
    }

    public class RuneHammerSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/RuneSmasher";
    
        public bool Hit;
        public int BounceTimer;
        public int BounceDelay;
        public bool bounced = false;
        public int bounceCount;
        public const int Swing_Speed_Multiplier = 8;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;

        }

        public override void SetDefaults()
        {
            holdOffset = 70;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 38;
            Projectile.width = 38;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BounceTimer);
            writer.Write(BounceDelay);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BounceTimer = reader.ReadInt32();
            BounceDelay = reader.ReadInt32();
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);

            SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
            hammerSlash1.PitchVariance = 0.2f;

            SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
            hammerSlash2.PitchVariance = 0.2f;

            swings.Add(new CircleSwingStyle
            {
                swingTime = 90,
                startSwingRotOffset = -MathHelper.ToRadians(155),
                endSwingRotOffset = MathHelper.ToRadians(155),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash1
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 90,
                startSwingRotOffset = -MathHelper.ToRadians(155),
                endSwingRotOffset = MathHelper.ToRadians(175),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash2
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 90,
                startSwingRotOffset = -MathHelper.ToRadians(175),
                endSwingRotOffset = MathHelper.ToRadians(225),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash1
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 78,
                startSwingRotOffset = -MathHelper.ToRadians(225),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash2
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 78,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash1
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 78,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(435),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash2
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 80,
                startSwingRotOffset = -MathHelper.ToRadians(435),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash1
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 100,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(435),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash2
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 120,
                startSwingRotOffset = -MathHelper.ToRadians(435),
                endSwingRotOffset = MathHelper.ToRadians(235),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash1
            });
        }

        private float _hitCount;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 oldMouseWorld = Main.MouseWorld;

            _hitCount++;
            float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
            SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);
            if (bounceTimer <= 0 && bounceCount < 1)
            {



                if (Main.myPlayer == player.whoAmI)
                {
                    player.velocity = Projectile.DirectionTo(oldMouseWorld) * -3f;
                }

                bounceTimer = 10 * ExtraUpdateMult;
                Projectile.netUpdate = true;

            }


            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 16);
                FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);
                //  Particle.NewParticle<IceStrikeParticle>(target.Center, Vector2.Zero, Color.White);

                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;
            }
            bounceCount++;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (!Hit)
            {
                modifiers.Knockback *= 0.5f;
            }
            else
            {
                modifiers.Knockback *= 2;
            }
        }

        //TRAIL VISUALS
        #region Trail Visuals
        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 92;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 326, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.DarkGoldenrod, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DarkGoldenrod, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.LightningTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.DarkGoldenrod;
            shader.BlendState = BlendState.AlphaBlend;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }

    public class RuneHammerLightning : ModProjectile
    {
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private Vector2[] _lightningPos;
        private float _lightningTrailTimer;
        public override string Texture => TextureRegistry.EmptyTexture;
        public float Duration => 30f;
        public float LightningProgress => _lightningTrailTimer / Duration;
        private float BeamLength;
        private const int NumSamplePoints = 3;
        private float MaxBeamLength => 2400;
        public Vector2 ImpactPos;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            //Lightning pos array, you can also use Projectile.oldPos, this is just for laser projectiles
            _lightningPos = new Vector2[64];

            //This is the actual trail itself
            _lightningTrail = new();

            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 10;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 48;


            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = (int)Duration;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            _lightningTrailTimer++;

            //How often it recalculates the lightning trail
            float randomizeRate = 8;
            if (_lightningTrailTimer % randomizeRate == 0)
            {
                _lightningTrail.RandomPositions(_lightningPos);
            }

            //Hitscan and calculate where the lightning go
            float targetBeamLength = PerformBeamHitscan();
            BeamLength = targetBeamLength;
            ImpactPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
            for (int i = 0; i < _lightningPos.Length; i++)
            {
                float progress = (float)i / (float)_lightningPos.Length;
                _lightningPos[i] = Vector2.Lerp(Projectile.Center, ImpactPos, progress);
            }

            if (_lightningTrailTimer == 1)
            {
                FXUtil.ShakeCamera(ImpactPos, 1024, 16);
                FXUtil.PunchCamera(ImpactPos, Projectile.velocity.SafeNormalize(Vector2.Zero) * 8, 0.5f, 2, 30);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 spawnPoint = ImpactPos + Main.rand.NextVector2Circular(8, 8);
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Particle.NewParticle<GlowParticle>(ImpactPos, velocity, Color.White,
                        Scale: Main.rand.NextFloat(2f, 3f));
                }
            }


            Projectile.rotation = Projectile.velocity.ToRotation();
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


        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.DarkGoldenrod, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DarkGoldenrod, LightningProgress);
            //This will make it fade out near the end
            return fadeColor;
        }

        private float WidthFunction(float completionRatio)
        {
            float easedLightningProgress = Easing.InCirc(1f - LightningProgress);
            float width = MathHelper.Lerp(128, 16, completionRatio);
            float width2 = 0;
            return MathHelper.Lerp(width, width2, LightningProgress);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = _lightningPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 12, ref collisionPoint))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningBolt2Shader lightningShader = LightningBolt2Shader.Instance;
            lightningShader.PrimaryColor = Color.White;
            lightningShader.NoiseColor = Color.DarkGoldenrod;
            lightningShader.Speed = 5;
            _lightningTrail.Draw(spriteBatch, _lightningPos, Projectile.oldRot, ColorFunction, WidthFunction, lightningShader);
            return false;
        }
    }

    public class RuneHammerStaminaSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere()+"/RuneSmasher";
    
        public bool Hit;
        public int BounceTimer;
        public int BounceDelay;
        public bool bounced = false;
        public int bounceCount;
        public const int Swing_Speed_Multiplier = 8;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            holdOffset = 70;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 38;
            Projectile.width = 38;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BounceTimer);
            writer.Write(BounceDelay);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BounceTimer = reader.ReadInt32();
            BounceDelay = reader.ReadInt32();
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);

            SoundStyle hammerSlash1 = SoundRegistry.HammerSmash1;
            hammerSlash1.PitchVariance = 0.2f;

            SoundStyle hammerSlash2 = SoundRegistry.HammerSmash2;
            hammerSlash2.PitchVariance = 0.2f;


            swings.Add(new CircleSwingStyle
            {
                swingTime = 120,
                startSwingRotOffset = -MathHelper.ToRadians(435),
                endSwingRotOffset = MathHelper.ToRadians(235),
                easingFunc = (float lerpValue) => Easing.InOutBack(lerpValue),
                swingSound = hammerSlash2
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
                swingXRadius = 128 / 1.5f,
                swingYRadius = 64 / 1.5f,
                swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = hammerSlash1,
                swingSoundLerpValue = 0.5f
            });

        }

        private float _hitCount;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 oldMouseWorld = Main.MouseWorld;

            _hitCount++;
            float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
            SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {

                FXUtil.ShakeCamera(target.Center, 1024, 16);
                FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);

                if (ComboIndex == 0)
                {
                    SoundEngine.PlaySound(SoundRegistry.HammerSmashLightning1, target.Center);
                    float speed = 512;
                    float spawnOffset = 512;
                    Vector2 spawnPoint;
                    for (float f = 0; f < 4; f++)
                    {

                        spawnPoint = target.Center - Vector2.UnitY * spawnOffset;
                        spawnPoint.X += Main.rand.NextFloat(-24f, 24f);

                        Vector2 velocity = Vector2.UnitY * speed;
                        velocity.X = Main.rand.NextFloat(-speed / 4f, speed / 4f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint, velocity,
                            ModContent.ProjectileType<RuneHammerLightning>(), Projectile.damage, Projectile.knockBack);
                    }

                    spawnPoint = target.Center - Vector2.UnitY * spawnOffset;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint, Vector2.UnitY,
                        ModContent.ProjectileType<RuneHammerLightning>(), Projectile.damage * 2, Projectile.knockBack);
                    if (bounceTimer <= 0 && bounceCount < 1)
                    {
                        if (Main.myPlayer == player.whoAmI)
                        {
                            player.velocity = -Vector2.UnitY * 9;
                        }
                    }
                }

                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;
            }
            bounceCount++;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage *= 2;
            if (ComboIndex == 0)
            {
                modifiers.Knockback *= 1f;
            }
            else
            {
                modifiers.Knockback *= 2;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = (int)(ComboIndex + 1);
            int dir = comboPlayer.ComboDirection;
            if (ComboIndex < 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }

        //TRAIL VISUALS
        #region Trail Visuals
        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 92;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 326, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.DarkGoldenrod, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DarkGoldenrod, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.LightningTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.DarkGoldenrod;
            shader.BlendState = BlendState.AlphaBlend;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }
}