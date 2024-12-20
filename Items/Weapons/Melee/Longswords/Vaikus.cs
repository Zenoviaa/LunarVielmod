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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee.Longswords
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod


    public class Vaikus : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Ranged;
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
            Item.knockBack = 3;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<VaikusSwordSlash>();
            Item.autoReuse = true;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 100;
            //Set max combo
            maxCombo = 15;





            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 1;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<VaikusSwordStaminaSlash>();
        }
    }

    public class VaikusSwordSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Vaikus";

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
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.Orange;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }
    public class VaikusSwordStaminaSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere()+"/Vaikus";

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
        public bool A;
        public bool B;
        public bool C;
        public bool D;
        public bool E;
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


            if (_smoothedLerpValue >= 0.5f)
            {



                if (!A)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, swingDirection * 24, ModContent.ProjectileType<VaikusProj>(), Projectile.damage /= 2, 0f, Projectile.owner, 0f, 0f);

                    A = true;
                }


            }

            if (_smoothedLerpValue >= 0.45f)
            {
                if (!B)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, swingDirection * 20, ModContent.ProjectileType<VaikusProj>(), Projectile.damage /= 2, 0f, Projectile.owner, 0f, 0f);

                    B = true;
                }

            }

            if (_smoothedLerpValue >= 0.4f)
            {
                if (!C)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, swingDirection * 16, ModContent.ProjectileType<VaikusProj>(), Projectile.damage /= 2, 0f, Projectile.owner, 0f, 0f);

                    C = true;
                }

            }

            if (_smoothedLerpValue >= 0.35f)
            {
                if (!D)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, swingDirection * 12, ModContent.ProjectileType<VaikusProj>(), Projectile.damage /= 2, 0f, Projectile.owner, 0f, 0f);

                    D = true;
                }

            }

            if (_smoothedLerpValue >= 0.3f)
            {
                if (!E)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, swingDirection * 8, ModContent.ProjectileType<VaikusProj>(), Projectile.damage /= 2, 0f, Projectile.owner, 0f, 0f);

                    E = true;
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
                swingTime = 100,
                stabRange = 100,
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

            SoundStyle spearHit = SoundRegistry.FireHit1;
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
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.Orange;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }





    internal class VaikusProj : ModProjectile
    {
        bool Moved;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.timeLeft = 660;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.velocity *= .96f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);

                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;

                Moved = true;
            }

            Vector2 ParOffset;
            if (Projectile.ai[1] >= 60)
            {
                ParOffset.X = Projectile.Center.X - 18;
                ParOffset.Y = Projectile.Center.Y;
                if (Main.rand.NextBool(1))
                {
                    int dustnumber = Dust.NewDust(ParOffset, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, Color.OrangeRed, 3f);
                    Main.dust[dustnumber].velocity *= 0.3f;
                    Main.dust[dustnumber].noGravity = true;
                }
                Projectile.velocity.Y += 2.4f;
            }
            if (Projectile.ai[1] >= 20)
            {
                Projectile.rotation /= 1.20f;
                alphaCounter -= 0.09f;
                Projectile.tileCollide = true;
            }
            else
            {
                if (alphaCounter <= 2)
                {
                    alphaCounter += 0.15f;
                }

            }

            Projectile.spriteDirection = Projectile.direction;
        }

        Vector2 BombOffset;
        public override void OnKill(int timeLeft)
        {
            SoundStyle spearHit = SoundRegistry.FireHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            var EntitySource = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<VaikusExplode>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
            Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<SimpleOrangeExplosion>(), Projectile.damage * 1, 1, Projectile.owner, 0, 0);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8f);

        }

        float alphaCounter = 0;
        Vector2 DrawOffset;
        public override bool PreDraw(ref Color lightColor)
        {

            if (Projectile.spriteDirection != 1)
            {
                DrawOffset.X = Projectile.Center.X - 18;
                DrawOffset.Y = Projectile.Center.Y;
            }
            else
            {
                DrawOffset.X = Projectile.Center.X - 25;
                DrawOffset.Y = Projectile.Center.Y;
            }


            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Color.Goldenrod;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;


            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, Color.OrangeRed, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(254, 231, 97), new Color(247, 118, 34), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }
    }
}



