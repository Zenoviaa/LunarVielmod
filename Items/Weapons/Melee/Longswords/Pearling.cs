using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee.Longswords
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod


    public class Pearling : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Ranged;
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 126;
            Item.useAnimation = 126;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<PearlingSwordSlash>();
            Item.autoReuse = true;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 100;
            //Set max combo
            maxCombo = 15;





            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 10;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<PearlingSwordStaminaSlash>();
        }
    }

    public class PearlingSwordSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Pearling";

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

            SoundStyle swingSound1 = SoundRegistry.NSwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;

            swings.Add(new SpearSwingStyle
            {
                swingTime = 10,
                stabRange = 50,
                thrustSpeed = 2,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 10,
                stabRange = 50,
                thrustSpeed = 2,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 10,
                stabRange = 50,
                thrustSpeed = 2,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 40,
                stabRange = 90,
                thrustSpeed = 3,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });



            swings.Add(new OvalSwingStyle
            {
                swingTime = 40,
                swingXRadius = 90,
                swingYRadius = 20,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 40,
                swingXRadius = 90,
                swingYRadius = 20,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 40,
                swingXRadius = 70,
                swingYRadius = 40,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 40,
                swingXRadius = 70,
                swingYRadius = 40,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 10,
                stabRange = 50,
                thrustSpeed = 2,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 10,
                stabRange = 50,
                thrustSpeed = 2,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 30,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1
            });

            swings.Add(new CircleSwingStyle
            {
                swingTime = 40,
                startSwingRotOffset = -MathHelper.ToRadians(135),
                endSwingRotOffset = MathHelper.ToRadians(135),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 40,
                swingXRadius = 100,
                swingYRadius = 20,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 40,
                swingXRadius = 100,
                swingYRadius = 20,
                swingRange = MathHelper.Pi / 2f,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                ovalRotOffset = ovalRotOffset,
                swingSound = spearSlash1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 60,
                stabRange = 90,
                thrustSpeed = 4,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });



        }



        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            /*
              if (ComboIndex == 5)
              {
                  Projectile.localNPCHitCooldown = 2 * ExtraUpdateMult;
              }
            */
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

            /*
                        if (ComboIndex == 5)
                        {
                            modifiers.FinalDamage *= 2;
                        }
            */
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
            float trailWidth = MathHelper.Lerp(0, 284, p);
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
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.DarkSlateBlue;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }
    public class PearlingSwordStaminaSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Pearling";

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
            Projectile.scale = 1.3f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        private bool _thrust;
        public float thrustSpeed = 0;
        public float stabRange;
        public override void AI()
        {
            base.AI();

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (_smoothedLerpValue > 0.5f)
            {
                if (!_thrust)
                {
                    Owner.velocity += swingDirection * thrustSpeed;
                    _thrust = true;
                }
            }



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

            SoundStyle swingSound1 = SoundRegistry.NSwordSlash1;
            swingSound1.PitchVariance = 0.5f;


            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;

            spearSlash2.PitchVariance = 0.5f;
            nSpin.PitchVariance = 0.2f;

            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });



            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });



            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });


            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 9,
                stabRange = 45,
                thrustSpeed = 0,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = spearSlash2
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
            }


        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit = SoundRegistry.CrystalHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 2;
            modifiers.Knockback *= 0.5f;

        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = (int)(ComboIndex + 1);
            int dir = comboPlayer.ComboDirection;


            if (ComboIndex < 10)
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
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.DarkSlateBlue;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }
}