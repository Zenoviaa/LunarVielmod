using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Explosions;
using Stellamod.Visual.GIFEffects;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Swords
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod


    public class Vael : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetDefaults()
        {
            Item.damage = 24;
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
            Item.shoot = ModContent.ProjectileType<VaelSwordSlash>();
            Item.autoReuse = true;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 60;
            //Set max combo
            maxCombo = 6;





            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 2;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<VaelSwordStaminaSlash>();
        }
    }

    public class VaelSwordSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Vael";

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
            Projectile.height = 39;
            Projectile.width = 39;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);

            SoundStyle swingSound1 = SoundRegistry.NSwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.NSwordSlash2;
            swingSound2.PitchVariance = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new CircleSwingStyle
            {
                swingTime = 24,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
                swingXRadius = 128 / 1.5f,
                swingYRadius = 64 / 1.5f,
                swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
                swingXRadius = 128 / 1.5f,
                swingYRadius = 64 / 1.5f,
                swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 24,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 7),
                swingSound = swingSound1
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 24,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 7),
                swingSound = swingSound1
            });

            float circleRange = MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.TwoPi;
            swings.Add(new CircleSwingStyle
            {
                swingTime = 40,
                startSwingRotOffset = -circleRange,
                endSwingRotOffset = circleRange,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound3
            });
        }


        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            if (ComboIndex == 5)
            {
                Projectile.localNPCHitCooldown = 2 * ExtraUpdateMult;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;
            }


        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);


            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }

        //TRAIL VISUALS
        #region Visuals
        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 80;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 384, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Orange, p);
            Color fadeColor = Color.Lerp(trailColor, Color.Red, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);

            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.GlowTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.Orange;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }
    public class VaelSwordStaminaSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Vael";

        public bool Hit;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            _lightningTrail = new();
            holdOffset = 40;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 38;
            Projectile.width = 38;
            Projectile.friendly = true;
            Projectile.scale = 1.3f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        private bool _thrust;
        private bool _spawnedExplosion;
        public float thrustSpeed = 5;
        public float stabRange;
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private float _lightningTrailTimer;
        private bool _hasSpawnedSecondKnife;

        public override void AI()
        {
            base.AI();
            _lightningTrailTimer++;

            //How often it recalculates the lightning trail
            float randomizeRate = 8;
            if (_lightningTrailTimer % randomizeRate == 0)
            {
                _lightningTrail.RandomPositions(_trailPoints);
            }


            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);

            if (_smoothedLerpValue > 0.5f)
            {
                if (!_thrust)
                {
                    _thrust = true;
                    if (ComboIndex == 0)
                    {
                        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        float offsetRadians = MathHelper.ToRadians(30);
                        Vector2 offsetVelocity = direction.RotatedBy(-offsetRadians / 2f) * 17f;



                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, offsetVelocity,
                            ModContent.ProjectileType<VaelProj>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);

                        offsetVelocity = direction.RotatedBy(offsetRadians / 2f) * 17f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, offsetVelocity,
                           ModContent.ProjectileType<VaelProj>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                    }
                }

                Vector2 swingDirection2 = Projectile.velocity.SafeNormalize(Vector2.Zero);
                if (_smoothedLerpValue > 0.5f)
                {
                    if (!_spawnedExplosion)
                    {
                        Owner.velocity += swingDirection2 * thrustSpeed;

                        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), TrueProjectileCenter, Vector2.Zero,
                            ModContent.ProjectileType<VaelExplosion>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);


                        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, swingDirection2 * 17,
                            ModContent.ProjectileType<VaelSpecial>(), Projectile.damage * 2, 0f, Projectile.owner, ai1: Projectile.whoAmI);
                        p.rotation = direction.ToRotation();

                        _spawnedExplosion = true;

                    }
                }


            }
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {

            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;


            base.SetComboDefaults(swings);
            swings.Add(new SpearSwingStyle
            {
                swingTime = 80,
                stabRange = 160,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.QuickInFadeOut(lerpValue),
                swingSound = swingSound1
            });


        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<VaelExplosion>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);

            }


        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit = SoundRegistry.MagicShockwaveExplosion;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

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
        #region Visuals

        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 80;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 484, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Orange, p);
            Color fadeColor = Color.Lerp(trailColor, Color.Red, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.GlowTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.Orange;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }

    public class VaelProj : ModProjectile
    {
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private float _timer;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningTrail = new();
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            base.AI();
            _timer++;
            if (_timer % 8 == 0)
            {
                _lightningTrail.RandomPositions(Projectile.oldPos);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(36, 0, completionRatio);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.Orange, Color.Purple, p);
            return trailColor;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        private void DrawTrail()
        {
            //Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = MagicVaellusShader.Instance;
            shader.PrimaryTexture = TrailRegistry.LightningTrail2;
            shader.NoiseTexture = TrailRegistry.LightningTrail3;
            shader.OutlineTexture = TrailRegistry.LightningTrail2Outline;
            shader.PrimaryColor = new Color(69, 70, 159);
            shader.NoiseColor = new Color(224, 107, 10);
            shader.OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            shader.Alpha = 1f;

            _lightningTrail ??= new();
            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 1;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 2;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            _lightningTrail.Draw(spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void DrawSprite(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;



            var shader = PixelMagicVaellusShader.Instance;
            shader.PrimaryTexture = TrailRegistry.CloudsSmall;
            shader.NoiseTexture = TrailRegistry.Clouds3;
            shader.OutlineTexture = TrailRegistry.LightningTrail2Outline;
            shader.PrimaryColor = new Color(69, 70, 159);
            shader.NoiseColor = new Color(224, 107, 10);
            shader.OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0f;
            shader.Power = 3f;
            shader.Blend = 0.4f;
            shader.Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, shader.Effect, Main.GameViewMatrix.ZoomMatrix);

            spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, shader.Effect, Main.GameViewMatrix.ZoomMatrix);

            for (float i = 0; i < 2f; i++)
            {
                spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, drawOrigin, drawScale * 0.85f, SpriteEffects.None, 0f);

            }

            spriteBatch.End();
            spriteBatch.Begin();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            DrawSprite(ref lightColor);
            return false;
        }
    }
}