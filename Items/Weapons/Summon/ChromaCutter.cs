﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Projectiles.Slashers.Ixy;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Swords.Ripper;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    internal class ChromaCutter : ModItem
    {
        private int _chromaCounter = 0;
        public int AttackCounter = 1;
        public int combowombo = 0;
        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 98;
            Item.damage = 71;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 5;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.noUseGraphic = true;
            Item.shootSpeed = 20f;
            Item.buffType = ModContent.BuffType<ChromaCutterMinionBuff>();
            Item.shoot = ModContent.ProjectileType<ChromaCutterProj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "R", "Red - No Effect")
            {
                OverrideColor = Color.DarkRed
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "O", "Orange - Explodes")
            {
                OverrideColor = Color.Orange
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "Y", "Yellow - Teleports after hitting an enemy")
            {
                OverrideColor = Color.Yellow
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "G", "Green - High damage")
            {
                OverrideColor = Color.Green
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "C", "Cyan - Causes several debuffs")
            {
                OverrideColor = Color.Cyan
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "B", "Blue - Homing")
            {
                OverrideColor = Color.Blue
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "V", "Purple - Summons powerful blades upon hitting an enemy")
            {
                OverrideColor = Color.Purple
            };

            tooltips.Add(line);

        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<ChromaCutterProj>();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                bool doSummonMinions = player.ownedProjectileCounts[ModContent.ProjectileType<ChromaCutterMinion>()] == 0;
                if (doSummonMinions)
                {
                    player.AddBuff(Item.buffType, 2);
                    // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
                    // 7 Minions
                    position = Main.MouseWorld;

                    float remainingSlots = player.maxMinions - player.slotsMinions;

                    for (int i = 0; i < 7; i++)
                    {
                        var projectile = Projectile.NewProjectileDirect(source, position, velocity,
                            ModContent.ProjectileType<ChromaCutterMinion>(), damage, knockback, player.whoAmI, ai0: i);
                        if(i == 0)
                        {
                            projectile.minionSlots = remainingSlots;
                        }

                        projectile.originalDamage = Item.damage + (int)(9 * remainingSlots);
                    }

                    return false;
                }
                else
                {
                    int dir = AttackCounter;
                    if (player.direction == 1)
                    {
                        player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter;
                    }
                    else
                    {
                        player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter * -1;

                    }
                    AttackCounter = -AttackCounter;
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);

                    int chromaCount = 0;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<ChromaCutterMinion>())
                        {
                            if (_chromaCounter == chromaCount)
                            {
                                if (Vector2.Distance(player.Center, proj.Center) > 128)
                                {
                                    _chromaCounter++;
                                    chromaCount++;
                                    continue;
                                }

                                //Make it attack
                                proj.ai[1] = 1;
                                proj.netUpdate = true;
                                _chromaCounter++;
                                break;
                            }
                            else
                            {
                                chromaCount++;
                            }
                        }
                    }

                    if (_chromaCounter >= 7)
                        _chromaCounter = 0;

                    switch (Main.rand.Next(0, 2))
                    {
                        case 0:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg"), player.position);
                            break;
                        case 1:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg2"), player.position);
                            break;
                    }

                    return false;
                }
            }

            return false;
        }
    }

    internal class ChromaCutterMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ChromaCutterMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    internal class ChromaCutterMinion : ModProjectile
    {
        private Vector2 _prepareCenter;
        private Vector2 _targetCenter;
        private float _slowdown;
        public enum ActionState
        {
            Red,
            Orange,
            Yellow,
            Green,
            Cyan,
            Blue,
            Violet
        }

        public ActionState State
        {
            get
            {
                return (ActionState)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = (float)value;
            }
        }


        public bool Attack
        {
            get
            {
                return Projectile.ai[1] == 1;
            }
            set
            {
                if (value)
                {
                    Projectile.ai[1] = 1;
                }
                else
                {
                    Projectile.ai[1] = 0;
                }
            }
        }

        public ref float AI_Timer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 2;
            switch (State)
            {
                case ActionState.Red:
                    Projectile.width = 88;
                    Projectile.height = 94;
                    break;
                case ActionState.Orange:
                    Projectile.width = 74;
                    Projectile.height = 72;
                    break;
                case ActionState.Yellow:
                    Projectile.width = 34;
                    Projectile.height = 36;
                    Projectile.localNPCHitCooldown = 5;
                    break;
                case ActionState.Green:
                    Projectile.width = 58;
                    Projectile.height = 58;
                    break;
                case ActionState.Cyan:
                    Projectile.width = 42;
                    Projectile.height = 42;
                    break;
                case ActionState.Blue:
                    Projectile.width = 58;
                    Projectile.height = 56;
                    break;
                case ActionState.Violet:
                    Projectile.width = 62;
                    Projectile.height = 54;
                    break;
            }
    
        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool MinionContactDamage()
        {
            return Attack;
        }

        private void Idle()
        {


            AI_Timer = 0;
            Player owner = Main.player[Projectile.owner];

            //Hovering
            float hoverSpeed = 1;
            float hoverRange = 32;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            float distanceFromPlayer = 80 + y;

            Vector2 targetCenter = owner.Center;
            Vector2 targetCenterOffset = targetCenter + new Vector2(-distanceFromPlayer, 0);

            float degreesDiff = 180 / 7;
            float degreesToRotateBy = degreesDiff * (float)State;
            Vector2 idleTargetCenter = targetCenterOffset.RotatedBy(MathHelper.ToRadians(degreesToRotateBy+45/2), targetCenter);
            Projectile.rotation = MathHelper.ToRadians(degreesToRotateBy - 135);
            Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, idleTargetCenter, 1350);
        }

        public override void AI()
        {
            //This code here just makes sure the projectile desummons if you don't have the buff.
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<ChromaCutterMinionBuff>(owner, Projectile))
                return;


            int offset = 64;
            Vector2 world = new Vector2(Main.maxTilesX - 16, Main.maxTilesY - 16).ToWorldCoordinates();
            if (Projectile.position.X < offset || Projectile.position.Y < offset || Projectile.position.X > world.X || Projectile.position.Y > world.Y)
            {
                AI_Reset();
                Idle();
            }
            if (Attack)
            {
                AI_Attack();
            }
            else
            {
                Idle();
            }

            Visuals();
        }

        private void AI_Reset()
        {
            AI_Timer = 0;
            Attack = false;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
            }
        }

        private void PlayShootSound()
        {
            switch(Main.rand.Next(0, 3))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Chroma3") with { PitchVariance = 0.1f }, Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Chroma2") with { PitchVariance = 0.1f }, Projectile.position);
                    break;
                case 2:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Chroma1") with { PitchVariance = 0.1f }, Projectile.position);
                    break;
            }
        }

        private void AI_Attack()
        {
            Player owner = Main.player[Projectile.owner];
            AI_Timer++;
            switch (State)
            {
                default:
                case ActionState.Red:
                    if(AI_Timer < 12)
                    {
                        Vector2 directionToMouse = owner.Center.DirectionTo(Main.MouseWorld);
                        _prepareCenter = owner.Center - (directionToMouse * 128 * (AI_Timer / 12));
                        Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, _prepareCenter, 45);
                        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.ToRadians(45);
                    }
                    else if (AI_Timer == 12)
                    {
                        _targetCenter = Main.MouseWorld;
                        Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.Center, _targetCenter, 36);
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
                        PlayShootSound();
                    } 
                    else if (AI_Timer > 64)
                    {
                        AI_Reset();
                    }

                    break;
                case ActionState.Yellow:
                    if (AI_Timer < 12)
                    {
                        Vector2 directionToMouse = owner.Center.DirectionTo(Main.MouseWorld);
                        _prepareCenter = owner.Center - (directionToMouse * 128 * (AI_Timer / 12));
                        Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, _prepareCenter, 45);
                        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.ToRadians(45);
                    }
                    else if (AI_Timer == 12)
                    {
                        _targetCenter = Main.MouseWorld;
                        Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.Center, _targetCenter, 36);
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
                        PlayShootSound();
                    }
                    else if (AI_Timer > 128)
                    {
                        AI_Reset();
                    }

                    break;
                case ActionState.Green:
                    if (AI_Timer < 12)
                    {
                        Vector2 directionToMouse = owner.Center.DirectionTo(Main.MouseWorld);
                        _prepareCenter = owner.Center - (directionToMouse * 128 * (AI_Timer / 12));
                        Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, _prepareCenter, 45);
                        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.ToRadians(45);
                    }
                    else if (AI_Timer == 12)
                    {
                        _targetCenter = Main.MouseWorld;
                        Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.Center, _targetCenter, 36);
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
                        PlayShootSound();
                    } else if (AI_Timer > 120)
                    {
                        AI_Reset();
                    }
           
       
                    break;
                case ActionState.Blue:
                    if (AI_Timer < 12)
                    {
                        Vector2 directionToMouse = owner.Center.DirectionTo(Main.MouseWorld);
                        _prepareCenter = owner.Center - (directionToMouse * 128 * (AI_Timer / 12));
                        Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, _prepareCenter, 45);
                        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.ToRadians(45);
                    }
                    else if (AI_Timer == 12)
                    {
                        _targetCenter = Main.MouseWorld;
                        Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.Center, _targetCenter, 36);
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
                        PlayShootSound();
                    }
                    else if (AI_Timer < 70)
                    {

                    }
                    else if (AI_Timer < 120)
                    {
                        SummonHelper.SearchForTargets(owner, Projectile,
                              out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
                        if (foundTarget)
                        {
                            AI_Movement(targetCenter, 45);
                        }
                        else
                        {
                            AI_Movement(owner.Center, 12);
                        }
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
                    }
                    else if (AI_Timer > 120)
                    {
                        AI_Reset();
                    }
      
                    break;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (!Attack)
                return;
            switch (State)
            {
                case ActionState.Green:
                    modifiers.FinalDamage = modifiers.FinalDamage.Scale(3);
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Attack)
                return;
            switch (Main.rand.Next(0, 2))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit"), target.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2"), target.position);
                    break;
            }

            Player owner = Main.player[Projectile.owner];
            switch (State)
            {
                case ActionState.Red:

                    break;

                case ActionState.Orange:
                    owner.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<CombustionBoomMini>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                        ParticleManager.NewParticle(Projectile.Center, velocity, ParticleManager.NewInstance<UnderworldParticle1>(),
                            Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
                    }

                    for (int i = 0; i < Main.rand.Next(1, 3); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                            ProjectileID.WandOfSparkingSpark, Projectile.damage, 0f, Projectile.owner);
                    }

                    AI_Reset();
                    break;

                case ActionState.Yellow:
                    Vector2 randYellowOffset = Main.rand.NextVector2CircularEdge(256f, 256f);
                    int count = 16;
                    for (int i = 0; i < count; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                        var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, speed, Scale: 1.5f);
                        d.noGravity = true;
                    }

                    Projectile.Center = target.Center + randYellowOffset;
                    Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.Center, target.Center, 40);
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
                    for (int i = 0; i < count; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                        var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, speed, Scale: 1.5f);
                        d.noGravity = true;
                    }
                    break;

                case ActionState.Green:
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner, 0f, 0f);

                    break;

                case ActionState.Cyan:
                    target.AddBuff(BuffID.Frostburn2, 240);
                    target.AddBuff(BuffID.Poisoned, 240);
                    target.AddBuff(BuffID.Slow, 240);
                    target.AddBuff(BuffID.OnFire3, 240);
                    target.AddBuff(BuffID.Electrified, 240);
                    switch (Main.rand.Next(0, 2))
                    {
                        case 0:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CyroBolt1"), target.position);
                            break;
                        case 1:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CyroBolt2"), target.position);
                            break;
                    }
                    break;

                case ActionState.Blue:


                    break;

                case ActionState.Violet:
                    float swordCount = 3;
                    for (int i = 0; i < swordCount; i++)
                    {
                        //360 degrees in circle :P
                        float degreesBetween = 360 / swordCount;
                        float degrees = degreesBetween * i;
                        float circleDistance = 256;
                        float swordSpeed = 16;
                        Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0)
                            .RotatedBy(MathHelper.ToRadians(degrees));

                        //I divide by 100 here cause I want there to be a delay before the swords converge
                        Vector2 velocity = (circlePosition.DirectionTo(Main.MouseWorld) * swordSpeed) / 100;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                            ModContent.ProjectileType<ChromaCutterPurpleSwordProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                    break;
            }

        }

        #region Draw Code

        private Texture2D GetChromaCutterTexture()
        {
            Texture2D swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionRed").Value;
            switch (State)
            {
                case ActionState.Red:
                    swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionRed").Value;
                    break;

                case ActionState.Orange:
                    swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionOrange").Value;
                    break;

                case ActionState.Yellow:
                    swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionYellow").Value;
                    break;

                case ActionState.Green:
                    swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionGreen").Value;
                    break;

                case ActionState.Cyan:
                    swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionCyan").Value;
                    break;

                case ActionState.Blue:
                    swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionBlue").Value;
                    break;

                case ActionState.Violet:
                    swordTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/ChromaCutterMinionPurple").Value;
                    break;
            }

            return swordTexture;
        }

        private void PreDrawAfterImage()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Color startColor; 
            Color endColor;
            switch (State)
            {
                default:
                case ActionState.Red:
                    startColor = Color.Red;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Orange:
                    startColor = Color.Orange;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Yellow:
                    startColor = Color.Yellow;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Green:
                    startColor = Color.Green;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Cyan:
                    startColor = Color.Cyan;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Blue:
                    startColor = Color.Blue;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Violet:

                    startColor = Color.Violet;
                    endColor = Color.Transparent;
                    break;
            }

            Texture2D texture = GetChromaCutterTexture();
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(startColor, endColor, 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private void PreDrawGlow()
        {

        }

        private void PreDrawLighting()
        {
            Color startColor;
            Color endColor;
            switch (State)
            {
                default:
                case ActionState.Red:
                    startColor = Color.Red;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Orange:
                    startColor = Color.Orange;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Yellow:
                    startColor = Color.Yellow;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Green:
                    startColor = Color.Green;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Cyan:
                    startColor = Color.Cyan;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Blue:
                    startColor = Color.Blue;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Violet:

                    startColor = Color.Violet;
                    endColor = Color.Transparent;
                    break;
            }

            Lighting.AddLight(Projectile.position, startColor.ToVector3() * Main.essScale * 1.0f);
        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 8;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor;
            Color endColor;
            switch (State)
            {
                default:
                case ActionState.Red:
                    startColor = Color.Red;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Orange:
                    startColor = Color.Orange;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Yellow:
                    startColor = Color.Yellow;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Green:
                    startColor = Color.Green;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Cyan:
                    startColor = Color.Cyan;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Blue:
                    startColor = Color.Blue;
                    endColor = Color.Transparent;
                    break;
                case ActionState.Violet:

                    startColor = Color.Violet;
                    endColor = Color.Transparent;
                    break;
            }

            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public static PrimDrawer TrailDrawer { get; private set; } = null;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = GetChromaCutterTexture();
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            PreDrawLighting();
            PreDrawAfterImage();
            PreDrawGlow();
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            TrailDrawer.WidthFunc = WidthFunction;
            TrailDrawer.ColorFunc = ColorFunction;
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, drawOrigin - Main.screenPosition, 155);

            //This code here draws the main blade
            //Since it can have 7 different sprites and they're all different sizes we're doing it a bit weirdly.
            spriteBatch.Draw(texture, drawPosition, sourceRectangle, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return base.PreDraw(ref lightColor);
        }
        #endregion
        private void Visuals()
        {
        }
    }
}
