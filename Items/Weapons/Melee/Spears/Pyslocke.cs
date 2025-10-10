using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Players;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;




namespace Stellamod.Items.Weapons.Melee.Spears
{
    public class Pyslocke : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Urdveil.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Ranged;
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<PyslockeStab>();
            Item.autoReuse = true;

            comboWaitTime = 120;
            maxCombo = 12;

            //Set stamina to use

            //set staminacombo
            maxStaminaCombo = 1;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<PyslockeStaminaStab>();
        }
    }

    public class PyslockeStab : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Pyslocke";

        public bool Hit;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {

            holdOffset = 40;
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

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);
            float ovalRotOffset = 0;
            if (ComboDirection == 1)
            {
                ovalRotOffset = 0;
            }
            else
            {
                ovalRotOffset = MathHelper.Pi + MathHelper.PiOver2;
            }

            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;
            swings.Add(new OvalSwingStyle
            {
                swingTime = 28,
                swingXRadius = 100,
                swingYRadius = 50,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 28,
                swingXRadius = 100,
                swingYRadius = 50,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 12,
                stabRange = 90,
                thrustSpeed = 5,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 12,
                stabRange = 90,
                thrustSpeed = 5,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
                swingXRadius = 100,
                swingYRadius = 50,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
                swingXRadius = 100,
                swingYRadius = 50,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            float circleRange = MathHelper.TwoPi * 4;
            swings.Add(new CircleSwingStyle
            {
                swingTime = 60,
                spinCenter = true,
                spinCenterOffset = 12,
                startSwingRotOffset = -circleRange,
                endSwingRotOffset = circleRange,
                easingFunc = (float lerpValue) => lerpValue,
                swingSound = nSpin
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 30,
                stabRange = 128,
                thrustSpeed = 5,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 60,
                stabRange = 200,
                thrustSpeed = 5,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            float circleRange2 = MathHelper.TwoPi * 6;
            swings.Add(new CircleSwingStyle
            {
                swingTime = 120,
                spinCenter = true,
                spinCenterOffset = 24,
                startSwingRotOffset = -circleRange2,
                endSwingRotOffset = circleRange2,
                easingFunc = (float lerpValue) => lerpValue,
                swingSound = nSpin
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 28,
                swingXRadius = 100,
                swingYRadius = 50,
                swingRange = MathHelper.Pi,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 28,
                swingXRadius = 100,
                swingYRadius = 50,
                swingRange = MathHelper.Pi,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

        }

        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            if (ComboIndex == 6)
            {
                //This npc local hit cooldown time makes it hit multiple times
                Projectile.localNPCHitCooldown = 3 * ExtraUpdateMult;
            }

            if (ComboIndex == 9)
            {
                //This npc local hit cooldown time makes it hit multiple times
                Projectile.localNPCHitCooldown = 4 * ExtraUpdateMult;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Particle.NewParticle<GlowParticle>(target.Center, Vector2.Zero, Color.LightPink);

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<PyslockeExplosion>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);

                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;


            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.1f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            if (ComboIndex == 7)
            {
                modifiers.FinalDamage *= 2;
            }

            if (ComboIndex == 8)
            {
                modifiers.FinalDamage *= 3;
            }
        }

        //TRAIL VISUALS
        public override Vector2 GetFramingSize()
        {
            //Set this to the width and height of the sword sprite
            return new Vector2(68, 72);
        }

        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 80;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 304, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.HotPink, Color.HotPink, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DeepSkyBlue, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);

            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.STARTRAIL;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.HotPink;
            shader.SecondaryColor = Color.DeepSkyBlue;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 50;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
    }

    public class PyslockeStaminaStab : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Pyslocke";

        public bool Hit;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {

            holdOffset = 40;
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

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);

            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;
            swings.Add(new CircleSwingStyle
            {
                swingTime = 48,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 7),
                swingSound = spearSlash1
            });
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = (int)(ComboIndex + 1);
            int dir = comboPlayer.ComboDirection;


            if (ComboIndex < 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }

        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            if (ComboIndex == 0)
            {
                //This npc local hit cooldown time makes it hit multiple times
                Projectile.localNPCHitCooldown = 3 * ExtraUpdateMult;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Particle.NewParticle<IceStrikeParticle>(target.Center, Vector2.Zero, Color.White);

                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;
            }
        }
        private bool _thrust;
        private bool _spawnedExplosion;
        public float thrustSpeed = 3;
        public override void AI()
        {
            base.AI();

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);



            Vector2 swingDirection2 = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (_smoothedLerpValue > 0.5f)
            {
                if (!_spawnedExplosion)
                {
                    Owner.velocity += swingDirection2 * thrustSpeed;

                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld + new Vector2(0, -300), Vector2.Zero,
                        ModContent.ProjectileType<PyslockeSlam>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);

                    _spawnedExplosion = true;

                }
            }



        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            if (ComboIndex == 0)
            {
                SoundStyle spearHit = SoundRegistry.SpearHit1;
                spearHit.PitchVariance = 0.1f;
                SoundEngine.PlaySound(spearHit, Projectile.position);
                modifiers.FinalDamage *= 3;
            }

        }

        //TRAIL VISUALS
        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 80;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 464, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.HotPink, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DeepSkyBlue, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);

            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.STARTRAIL;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.DeepSkyBlue;
            shader.SecondaryColor = Color.HotPink;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 50;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
    }

    public class PyslockeExplosion : BaseExplosionProjectile
    {
        private Core.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private float _timer;
        int rStart = 4;
        int rEnd = 40;
        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningTrail = new Core.Shaders.MagicTrails.LightningTrail();
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 32;
            rStart = Main.rand.Next(4, 8);
            rEnd = Main.rand.Next(40, 60);
            Projectile.hide = true;
        }

        public override void AI()
        {
            base.AI();
            _timer++;
            if (_timer % 6 == 0)
            {
                _lightningTrail.RandomPositions(_circlePos);
            }
        }

        protected override float BeamWidthFunction(float p)
        {
            //How wide the trail is going to be
            float trailWidth = MathHelper.Lerp(96, 0, Easing.OutCubic(p));
            float fadeWidth = MathHelper.Lerp(0, trailWidth, Easing.SpikeOutExpo(p));// * Main.rand.NextFloat(0.75f, 1.0f);
            return fadeWidth;
        }

        protected override Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.Pink, Color.CadetBlue, p);
            Color fadeColor = Color.Lerp(trailColor, Color.Pink, UneasedProgress);
            return trailColor;
        }



        protected override float RadiusFunction(float p)
        {
            //How large the circle is going to be
            return MathHelper.Lerp(rStart, rEnd, Easing.OutExpo(p));
        }

        public void DrawTrail()
        {
            //Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = MagicVaellusShader.Instance;

            //Resets to the default settings for this shader
            shader.SetDefaults();
            shader.NoiseColor = Color.Lerp(Color.Lerp(Color.Pink, Color.CadetBlue, VectorHelper.Osc(0, 1)), Color.Black, 0.4f);

            _lightningTrail ??= new();

            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 16;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 1;
            _lightningTrail.Draw(spriteBatch, _circlePos, Projectile.oldRot, ColorFunctionReal, WidthFunction, shader, offset: Projectile.Size / 2f);

            shader.BlendState = BlendState.Additive;
            _lightningTrail.Draw(spriteBatch, _circlePos, Projectile.oldRot, ColorFunctionReal, WidthFunction, shader, offset: Projectile.Size / 2f);


        }


        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overPlayers.Add(index);
        }
    }



    public class PysBoomer : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.timeLeft = 15;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                for (float f = 0; f < 60; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                SoundStyle morrowExp = new SoundStyle($"Urdveil/Assets/Sounds/MorrowExp");
                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Purple, duration: 25, baseSize: 0.24f);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 velocity = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(25f, 45f);
                    var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.HotPink;
                    particle.OuterGlowColor = Color.CadetBlue;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                    particle.VectorScale *= 0.5f;
                }

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Pink,
                        outerGlowColor: Color.CadetBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
                FXUtil.ShakeCamera(Projectile.position, 1024, 8);
            }

            if (Timer == 6)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 128, Projectile.velocity,
                        ModContent.ProjectileType<MalachoBoom>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
                }
                for (float f = 0; f < 20; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 10)).RotatedByRandom(19.0), 0, Color.Lerp(Color.Pink, Color.CadetBlue, 0.5f), Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                FXUtil.ShakeCamera(Projectile.position, 1024, 32);
                ShakeModSystem.Shake = 3;
            }
        }
    }






    public class PyslockeSlam : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++; ProjectileID.Sets.TrailCacheLength[Type] = 4;
            if (Timer % 16 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, Projectile.velocity * 0.1f, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.penetrate < 3)
            {
                Projectile.velocity.Y += 1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.penetrate--;
            Projectile.velocity *= 0.6f;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            return false;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }


        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Pink, Color.CadetBlue, completionRatio);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            SpriteBatch spriteBatch = Main.spriteBatch;
            //     spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Green.MultiplyRGB(lightColor), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.GlowTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.8f - Main.screenPosition, 155);

            return true;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 4; f++)
            {
                Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity, ModContent.DustType<GlyphDust>(), -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.25f, 1f), Scale: Main.rand.NextFloat(0.25f, 0.75f), newColor: Color.Pink);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.oldVelocity,
        ModContent.ProjectileType<PysBoomer>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
        }
    }
}



