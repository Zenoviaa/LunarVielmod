using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Common.Lights;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Swords;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Swords
{
    public class AssassinsSlash : BaseSwingItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 7;
            Item.mana = 6;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ivyen Saber");
            // Tooltip.SetDefault("Has a chance to poison enemies.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
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
            Item.shoot = ModContent.ProjectileType<AssassinsSlashSlash>();
            Item.autoReuse = true;

            //This is only going to affect the tooltip :P
            meleeWeaponType = MeleeWeaponType.Sword;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 60;
            //Set max combo
            maxCombo = 5;


            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 1;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<AssassinsSlashStaminaSlash>();
        }
    }

    public class AssassinsSlashSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/AssassinsSlash";
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
            SoundStyle swingSound1 = SoundRegistry.NSwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.NSwordSlash2;
            swingSound2.PitchVariance = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 100,
                swingYRadius = 48,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 100,
                swingYRadius = 48,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 100,
                swingYRadius = 64,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 100,
                swingYRadius = 64,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            float circleRange = MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.TwoPi;
            swings.Add(new CircleSwingStyle
            {
                swingTime = 32,
                startSwingRotOffset = -circleRange,
                endSwingRotOffset = circleRange,
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingDistance=16,
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

        public override void AI()
        {
            base.AI();

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
        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 32, Easing.InExpo(completionRatio) * t) * Easing.SpikeOutCirc(uneasedLerpValue);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.White, Color.Red, completionRatio), completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 252);
        }
    }

    public class AssassinsSlashStaminaSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/AssassinsSlash";

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
        public float thrustSpeed = 8;
        public float stabRange;
        public override void AI()
        {
            base.AI();

            if (_smoothedLerpValue > 0.1f)
            {
                if (!_thrust)
                {
                    Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Owner.velocity += swingDirection * thrustSpeed;
                    _thrust = true;
                }
            }
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;


            base.SetComboDefaults(swings);
            swings.Add(new OvalSwingStyle
            {
                swingTime = 48,
                swingXRadius = 160 / 1.5f,
                swingYRadius = 24,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f

            });
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Owner.velocity -= swingDirection * thrustSpeed * 2;
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<Assassinate>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }

        //TRAIL VISUALS
        //TRAIL VISUALS
        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 32, completionRatio * t);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.Red, Color.Black, completionRatio), Easing.SpikeOutCirc(completionRatio) * Timer / 60f);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 155);
        }
    }

    public class Assassinate : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int NPC => (int)Projectile.ai[1];
        private ref float SlashCount => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            base.AI();
            NPC myNpc = Main.npc[NPC];
            if (!myNpc.active)
            {
                Projectile.Kill();
            }

            Timer++;
            if(Timer == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
      ModContent.ProjectileType<AssassinsSpawnEffect>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
            }
            if(Timer <= 10)
            {
                SpecialEffectsPlayer player = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                player.blackWhiteStrength = 0.66f;
                player.blackWhiteThreshold = 0.5f;
            }
            if(Timer >= 20)
            {
                SpecialEffectsPlayer player = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                player.blackWhiteStrength = 1f;
                player.blackWhiteThreshold = 0.5f;
            }
            if (Timer == 25)
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(myNpc.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
                    ModContent.ProjectileType<AssassinsSpawnEffect>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
                        ModContent.ProjectileType<AssassinsSlashProj>(), 0, 1, Projectile.owner, 0, 0);
                    SlashCount++;
                    if (SlashCount >= 10)
                    {
                        Projectile.Kill();
                    }
                }
            }
            if (Timer >= 25)
            {
                Timer = 20;
            }
        }
    }
}