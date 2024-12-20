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
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Melee.Spears
{
    public class CrystalPointer : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Ranged;
        public override void SetDefaults()
        {
            Item.damage = 8;
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
            Item.shoot = ModContent.ProjectileType<CrystalPointerStab>();
            Item.autoReuse = true;

            comboWaitTime = 70;
            maxCombo = 9;

            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 2;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<CrystalPointerStaminaStab>();
        }
    }

    public class CrystalPointerStab : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/CrystalPointer";

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
        }

        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            if (ComboIndex == 6)
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
            float trailWidth = MathHelper.Lerp(0, 252, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightCyan, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DeepSkyBlue, _smoothedLerpValue);
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
            shader.SecondaryColor = Color.DarkSlateBlue;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
    }

    public class CrystalPointerStaminaStab : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/CrystalPointer";

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
            swings.Add(new OvalSwingStyle
            {
                swingTime = 60,
                swingXRadius = 128,
                swingYRadius = 48,
                swingRange = MathHelper.ToRadians(2100),
                easingFunc = (float lerpValue) => lerpValue,
                swingSound = nSpin,
                swingSoundLerpValue = 0.15f
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 20,
                stabRange = 100,
                thrustSpeed = 15,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
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
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, (Main.MouseWorld - Owner.Center), Projectile.type, Projectile.damage, Projectile.knockBack,
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            if (ComboIndex == 0)
            {
                SoundStyle spearHit = SoundRegistry.SpearHit1;
                spearHit.PitchVariance = 0.1f;
                SoundEngine.PlaySound(spearHit, Projectile.position);
            }

            if (ComboIndex == 1)
            {
                SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
                spearHit2.PitchVariance = 0.2f;
                SoundEngine.PlaySound(spearHit2, Projectile.position);
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
            float trailWidth = MathHelper.Lerp(0, 252, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightCyan, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DeepSkyBlue, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.CrystalTrail2;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.DarkSlateBlue;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
    }
}