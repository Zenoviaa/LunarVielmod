using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Gores.NKR;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Explosions;
using Stellamod.Visual.GIFEffects;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Greatswords
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod


    public class NoxianRider : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Throwing;
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
            Item.shoot = ModContent.ProjectileType<NRSwordSlash>();
            Item.autoReuse = true;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 120;
            //Set max combo
            maxCombo = 8;





            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 1;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<NRStaminaSlash>();
        }
    }

    public class NRSwordSlash : BaseSwingProjectile
    {
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private float _lightningTimer;
        private NPCSucker _npcSucker;
        public override string Texture => this.PathHere() + "/NoxianRider";

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
            Projectile.height = 38 * 2;
            Projectile.width = 38 * 2;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);

            SoundStyle swingSound1 = SoundRegistry.MotorcycleSlash1;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.MotorcycleSlash2;
            swingSound2.PitchVariance = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.MotorcycleDrive;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new CircleSwingStyle
            {
                swingDistance = 24,
                swingTime = 48,
                startSwingRotOffset = -MathHelper.ToRadians(175),
                endSwingRotOffset = MathHelper.ToRadians(175),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 90,
                stabRange = 240,
                thrustSpeed = 1,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = swingSound3,
                spinRotationRange = MathHelper.ToRadians(2000),
                spinTrailOffset = 1.15f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 70,
                swingXRadius = 84 / 1.5f,
                swingYRadius = 70 / 1.5f,
                swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 70,
                swingXRadius = 168 / 1.5f,
                swingYRadius = 140 / 1.5f,
                swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4,
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new CircleSwingStyle
            {
                swingDistance = 24,
                swingTime = 48,
                startSwingRotOffset = -MathHelper.ToRadians(175),
                endSwingRotOffset = MathHelper.ToRadians(175),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new CircleSwingStyle
            {
                swingDistance = 32,
                swingTime = 96,
                startSwingRotOffset = -MathHelper.ToRadians(215),
                endSwingRotOffset = MathHelper.ToRadians(215),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new SpearSwingStyle
            {
                swingTime = 100,
                stabRange = 300,
                thrustSpeed = 2,
                easingFunc = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                swingSound = swingSound3,
                spinRotationRange = MathHelper.ToRadians(2000),
                spinTrailOffset = 1.15f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 115,
                swingXRadius = 108 / 1.5f,
                swingYRadius = 80 / 1.5f,
                swingRange = MathHelper.ToRadians(1440),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 7),
                swingSound = swingSound3,
                swingSoundLerpValue = 0.15f
            });
        }

        public override bool? CanDamage()
        {
            return true;
        }
        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            if (ComboIndex == 1 || ComboIndex == 6)
            {
                Projectile.knockBack = 0;
                Projectile.localNPCHitCooldown = 12 * ExtraUpdateMult;
            }

            if (ComboIndex == 7)
            {
                //This npc local hit cooldown time makes it hit multiple times
                Projectile.localNPCHitCooldown = 3 * ExtraUpdateMult;
            }
        }

        public override void AI()
        {
            base.AI();
            _npcSucker ??= new NPCSucker();
            _lightningTimer++;
            if (_lightningTimer % 24 == 0)
            {
                _lightningTrail.RandomPositions(_trailPoints);
            }

            if (Timer % (ExtraUpdateMult) == 0 && uneasedLerpValue > 0.5f)
            {
                _npcSucker.AI(Projectile.Center, strength: 0.8f);
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

            if ((ComboIndex == 1 || ComboIndex == 6) && uneasedLerpValue > 0.5f)
            {
                _npcSucker.AddNPCSuckerTarget(Projectile.Center, target);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            if (ComboIndex == 0)
            {
                modifiers.Knockback *= 2;
            }

            if (ComboIndex == 2)
            {
                modifiers.Knockback *= 2;
            }


            if (ComboIndex == 1 || ComboIndex == 6)
            {
                modifiers.Knockback *= 0;
                modifiers.FinalDamage *= 0.5f;
            }

            if (ComboIndex == 4)
            {
                modifiers.Knockback *= 2;
            }

            if (ComboIndex == 5)
            {
                modifiers.Knockback *= 3;
            }

            if (ComboIndex == 7 && ComboIndex != 6)
            {
                modifiers.FinalDamage *= 2;
            }

        }

        //TRAIL VISUALS
        #region Visuals
        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 100;
        }

        private float WidthFunction(float p)
        {
            if (ComboIndex == 1 || ComboIndex == 6)
            {
                float spinTrailWidth = MathHelper.Lerp(0, 32, p);
                float spinFadeWidth = MathHelper.Lerp(0, spinTrailWidth, _smoothedLerpValue);
                return spinFadeWidth;
            }
            float trailWidth = MathHelper.Lerp(0, 106, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, new Color(207, 150, 140), p);
            return trailColor;
        }
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningBolt2Shader lightningShader = LightningBolt2Shader.Instance;
            lightningShader.PrimaryColor = new Color(207, 150, 140);
            lightningShader.NoiseColor = new Color(60, 107, 128);
            lightningShader.Speed = 5;
            lightningShader.BlendState = BlendState.Additive;

            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 2;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 4;

            _lightningTrail.Draw(spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, lightningShader, offset: GetFramingSize() / 2);
        }
        #endregion
    }
    public class NRStaminaSlash : BaseSwingProjectile
    {
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private float _lightningTimer;
        public override string Texture => this.PathHere() + "/NoxianRider";

        public bool Hit;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            _lightningTrail = new();
            holdOffset = -10;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 38;
            Projectile.width = 38;
            Projectile.friendly = true;
            Projectile.scale = 1.4f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        private bool _thrust;
        public float thrustSpeed = 4;
        public float stabRange;
        public override void AI()
        {
            base.AI();
            _lightningTimer++;
            if (_lightningTimer % 6 == 0)
            {
                _lightningTrail.RandomPositions(_trailPoints);
            }

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (_smoothedLerpValue > 0.5f)
            {
                if (!_thrust)
                {
                    Owner.velocity += swingDirection * thrustSpeed;

                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Vector2 velocity = swingDirection * 15;
                    velocity.Y -= 5;
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, velocity,
                        ModContent.ProjectileType<NKRMoped>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                    p.rotation = direction.ToRotation();

                    _thrust = true;

                }
            }



        }
        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {

            SoundStyle swingSound1 = SoundRegistry.MotorcycleSlash2;
            swingSound1.PitchVariance = 0.5f;


            base.SetComboDefaults(swings);
            swings.Add(new CircleSwingStyle
            {
                swingDistance = 90,
                swingTime = 108,
                startSwingRotOffset = -MathHelper.ToRadians(275),
                endSwingRotOffset = MathHelper.ToRadians(105),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });



        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Hit = true;
                hitstopTimer = 8 * ExtraUpdateMult;
            }


        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3.5f;
            modifiers.Knockback *= 3;

        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

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
            float trailWidth = MathHelper.Lerp(0, 105, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, new Color(207, 150, 140), p);
            return trailColor;
        }
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningBolt2Shader lightningShader = LightningBolt2Shader.Instance;
            lightningShader.PrimaryColor = new Color(207, 150, 140);
            lightningShader.NoiseColor = new Color(60, 107, 128);
            lightningShader.Speed = 5;
            lightningShader.BlendState = BlendState.Additive;

            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 2;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 4;

            _lightningTrail.Draw(spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, lightningShader, offset: GetFramingSize() / 2f);
        }

        #endregion
    }


    public class NKRMopedPlayer : ModPlayer
    {
        public Vector2 StartPosition;
        public Vector2? TargetPosition;
        public float Timer;
        public float LerpTime;
        public float RotationFixTimer;
        public float StartRotation;
        public bool FixRotation;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (TargetPosition != null)
            {
                Timer++;
                if (Timer == 1)
                {
                    StartPosition = Player.Center;
                }

                float progress = Timer / LerpTime;
                float easedRollProgress = progress;
                float rot = MathHelper.Lerp(0, MathHelper.ToRadians(360), easedRollProgress);
                Player.Center = Vector2.Lerp(StartPosition, TargetPosition.Value, progress);
                Player.fullRotation = Player.direction == -1 ? MathHelper.Lerp(MathHelper.TwoPi, 0, easedRollProgress) : MathHelper.Lerp(0, MathHelper.TwoPi, easedRollProgress);
                Player.fullRotationOrigin = new Vector2(Player.width / 2, Player.height / 2);
                Player.velocity = Vector2.Zero;
                if (Timer >= LerpTime)
                {
                    Timer = 0;
                    TargetPosition = null;
                }
            }

            if (FixRotation)
            {
                RotationFixTimer++;
                if (RotationFixTimer == 0)
                {
                    StartRotation = Player.fullRotation;
                }
                Player.fullRotation = MathHelper.Lerp(StartRotation, 0, RotationFixTimer / 30f);
                if (RotationFixTimer >= 30)
                {
                    RotationFixTimer = 0;
                    FixRotation = false;
                }
            }
        }
    }
    public class NKRMoped : ModProjectile
    {
        private NPCSucker _npcSucker;
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        public enum AIState
        {
            Spawn,
            Hop_In,
            Ride
        }


        public override string Texture => this.PathHere() + "/NoxianRider";

        private AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float Timer => ref Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];

        private float SpawnTime => 30;
        private float MountTime => 30;

        private Vector2 InitialVelocity;
        public Vector2 SeatPosition
        {
            get
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 position = Projectile.position + texture.Size() / 2f;
                return position;
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _npcSucker = new NPCSucker();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.timeLeft = 300;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            //Hold item so you can't attack
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Hop_In:
                    AI_HopIn();
                    break;
                case AIState.Ride:
                    AI_Ride();
                    break;
            }

            if (Timer % 6 == 0)
            {
                _lightningTrail ??= new();
                _lightningTrail.RandomPositions(Projectile.oldPos);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<FlameExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NKRExplode>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 16);
            SoundStyle explosionStyle = SoundRegistry.HeavyExplosion1;
            explosionStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(explosionStyle, Projectile.position);
            var entitySource = Projectile.GetSource_Death();
            Owner.GetModPlayer<NKRMopedPlayer>().FixRotation = true;
            Owner.SetImmuneTimeForAllTypes(15);
            Vector2 vel = Projectile.velocity * 0.3f;


            for (int i = 0; i < 2; i++)
            {
                vel.Y -= 7;
                Gore.NewGore(entitySource, Projectile.position, vel, ModContent.GoreType<NoxianFront>());
                Gore.NewGore(entitySource, Projectile.position, vel, ModContent.GoreType<NoxianFuelCan>());
                Gore.NewGore(entitySource, Projectile.position, vel, ModContent.GoreType<NoxianSeat>());
                for (int j = 0; j < 5; j++)
                {
                    Gore.NewGore(entitySource, Projectile.position, vel, ModContent.GoreType<NoxianScrew>());
                }

                for (int j = 0; j < 2; j++)
                {
                    Gore.NewGore(entitySource, Projectile.position, vel, ModContent.GoreType<NoxianStep>());
                }

                for (int j = 0; j < 2; j++)
                {
                    Gore.NewGore(entitySource, Projectile.position, vel, ModContent.GoreType<NoxianTire>());
                }
            }

        }

        private void SwitchState(AIState state)
        {
            State = state;
            Timer = 0;
            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return State == AIState.Ride;
        }

        public override bool? CanDamage()
        {
            return State == AIState.Ride;
        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
            }
            Projectile.velocity *= 0.98f;
            if (Timer >= SpawnTime)
            {
                SwitchState(AIState.Hop_In);
            }
        }

        private void AI_HopIn()
        {
            Timer++;
            if (Timer == 1)
            {
                NKRMopedPlayer mopedPlayer = Owner.GetModPlayer<NKRMopedPlayer>();
                mopedPlayer.TargetPosition = Projectile.Center;
                mopedPlayer.LerpTime = MountTime;
                Owner.SetImmuneTimeForAllTypes((int)MountTime);
            }

            if (Timer >= MountTime)
            {
                SwitchState(AIState.Ride);
            }
        }

        private void AI_Ride()
        {
            Timer++;
            if (Timer == 1)
            {
                Owner.SetImmuneTimeForAllTypes(240);
                Projectile.velocity = InitialVelocity;
                SoundStyle driveStyle = SoundRegistry.MotorcycleDrive;
                driveStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(driveStyle, Projectile.position);
            }

            if (Timer > 10)
            {
                Projectile.tileCollide = true;
            }

            Projectile.velocity.Y += 0.15f;
            Owner.Center = SeatPosition - Vector2.UnitY * 24;
            Owner.fullRotation = Owner.direction == -1 ? Projectile.rotation : -Projectile.rotation;
            Owner.fullRotation *= 0.3f;
            Owner.fullRotation -= MathHelper.ToRadians(45);
            Owner.fullRotationOrigin = new Vector2(Owner.width / 2, Owner.height / 2);
            _npcSucker.AI(Projectile.Center, strength: 1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_npcSucker.HasSuckerTarget(target))
            {
                FXUtil.ShakeCamera(target.Center, 1024, 16);
                FXUtil.PunchCamera(target.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 8, 0.5f, 2, 30);
                _npcSucker.AddNPCSuckerTarget(Projectile.Center, target);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage *= 0.1f;
        }

        private void DrawWindTrail()
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes

            //Main Fill
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, shader, offset: Projectile.Size / 2);
        }

        private void DrawNoxianTrail()
        {
            //Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningBolt2Shader lightningShader = LightningBolt2Shader.Instance;
            lightningShader.PrimaryColor = new Color(207, 150, 140);
            lightningShader.NoiseColor = new Color(60, 107, 128);
            lightningShader.Speed = 5;
            lightningShader.BlendState = BlendState.Additive;

            _lightningTrail ??= new();
            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 32;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 24;
            _lightningTrail.Draw(spriteBatch, Projectile.oldPos, Projectile.oldRot, NoxianTrailColorFunction, NoxianTrailWidthFunction, lightningShader, offset: Projectile.Size / 2f);
        }

        private float NoxianTrailWidthFunction(float completionRatio)
        {
            float progress = completionRatio / 0.3f;
            float rounded = Easing.SpikeOutCirc(progress);
            float spikeProgress = Easing.SpikeOutExpo(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));
            float midWidth = 47;
            return MathHelper.Lerp(0, midWidth, fireball);
        }

        private Color NoxianTrailColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, new Color(207, 150, 140), p);
            return trailColor;
        }

        private Color StripColors(float progressOnStrip)
        {

            Color result = Color.Lerp(Color.LightGray, Color.White,
                Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            return MathHelper.Lerp(26f, 32f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawWindTrail();
            string glowTexture = Texture;
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = whiteTexture.Size();
            Vector2 drawOrigin = textureSize / 2;

            //Lerping

            Color drawColor = Color.White;
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            SpriteEffects spriteEffects = SpriteEffects.None;
            float rotation = Projectile.rotation;
            if (Projectile.velocity.X < 0)
            {
                spriteEffects = SpriteEffects.FlipVertically;
                rotation -= MathHelper.ToRadians(90);
            }

            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, rotation, drawOrigin, Projectile.scale,
                spriteEffects, 0f);
            //     DrawNoxianTrail();
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            switch (State)
            {
                case AIState.Spawn:
                    DrawWhite(lightColor);
                    break;
            }
        }

        private void DrawWhite(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = whiteTexture.Size();
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = Timer / SpawnTime;
            Color drawColor = Color.Lerp(Color.White, Color.Transparent, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            SpriteEffects spriteEffects = SpriteEffects.None;
            float rotation = Projectile.rotation;
            if (Projectile.velocity.X < 0)
            {
                spriteEffects = SpriteEffects.FlipVertically;
                rotation -= MathHelper.ToRadians(90);
            }


            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overPlayers.Add(index);
        }
    }
}


