using Microsoft.Xna.Framework;
using Stellamod.Common.Lights;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee.Shields;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.EliteCommander.Projectiles;
using Stellamod.NPCs.Colosseum.Common;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.EliteCommander
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
    public class EliteCommander : BaseColosseumNPC
    {
        private int _frame;
        private enum AIState
        {
            Idle,
            Walk,
            Jump,
            Land,
            Summon,
            Despawn
        }

        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float AbovePlayerTimer => ref NPC.ai[2];
        private ref float JumpCount => ref NPC.ai[3];

        private Player Target => Main.player[NPC.target];
        private float DirectionToTarget
        {
            get
            {
                if (Target.Center.X < NPC.Center.X)
                {
                    return -1;
                }
                return 1;
            }
        }

        private int ShockwaveDamage => 40;
        private int HandDamage => 12;

        private bool InSecondPhase => NPC.life < NPC.lifeMax / 2;
        private float JumpTarget;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(JumpTarget);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            JumpTarget = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Verlia of The Moon");

            Main.npcFrameCount[Type] = 42;

            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Stellamod/NPCs/Event/Gintzearmy/BossGintze/GintziaPreview",
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "He's evil yet so weak. This fellow betrayed Gothivia during their war against Fenix and his part of the army joined the other side for power.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Commander Gintzia, the betrayer", "2"))
            });
        }

        public override void SetDefaults()
        {
            NPC.Size = new Vector2(42, 67);
            NPC.damage = 24;
            NPC.defense = 10;
            NPC.lifeMax = 900;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;

            // Custom boss bar
            NPC.BossBar = ModContent.GetInstance<EliteCommanderBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Gintzicane");
            }
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.25f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0;
            }

            switch (State)
            {
                case AIState.Idle:
                    _frame = 0;
                    break;
                case AIState.Walk:
                    if (_frame < 29)
                    {
                        _frame = 29;
                    }
                    if (_frame >= 42)
                    {
                        _frame = 29;
                    }
                    break;
                case AIState.Jump:
                    if (_frame < 0 || _frame >= 10)
                    {
                        _frame = 0;
                    }

                    if (_frame >= 8)
                    {
                        _frame = 4;
                    }
                    break;
                case AIState.Land:
                    if (_frame < 8)
                    {
                        _frame = 8;
                    }

                    if (_frame >= 12)
                    {
                        _frame = 11;
                    }
                    break;
                case AIState.Summon:
                    if (_frame < 12 || _frame >= 31)
                    {
                        _frame = 12;
                    }

                    if (_frame >= 30)
                    {
                        _frame = 29;
                    }
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }
        public override void AI()
        {
            base.AI();
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }
            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Walk:
                    AI_Walk();
                    break;
                case AIState.Jump:
                    AI_Jump();
                    break;
                case AIState.Land:
                    AI_Land();
                    break;
                case AIState.Summon:
                    AI_Summon();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
            }
        }

        private void AI_Idle()
        {
            Timer++;
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                SwitchState(AIState.Despawn);
            }

            if (Timer >= 60)
            {
                if (JumpCount < 3)
                {
                    SwitchState(AIState.Walk);
                }
                else
                {
                    JumpCount = 0;
                    SwitchState(AIState.Summon);
                }
            }
        }

        private void AI_Walk()
        {
            Timer++;
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;

            float moveSpeed = 1f;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.3f);
            if (Target.Top.Y < NPC.Top.Y)
            {
                AbovePlayerTimer++;
            }
            if (Timer > JumpTarget || AbovePlayerTimer > 60)
            {
                if (StellaMultiplayer.IsHost)
                {
                    JumpTarget = Main.rand.NextFloat(180, 240);
                    if (InSecondPhase)
                    {
                        JumpTarget *= 0.5f;
                    }
                    NPC.netUpdate = true;
                }
                SwitchState(AIState.Jump);
            }
        }

        private void AI_Jump()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.spriteDirection = NPC.direction;
                JumpCount++;
            }

            if (Timer == 10)
            {
                if (StellaMultiplayer.IsHost)
                {
                    float distance = Main.rand.NextFloat(2, 5);
                    float jumpHeight = -14f;
                    if (InSecondPhase)
                    {
                        jumpHeight = Main.rand.NextFloat(-5f, -10f);
                    }
                    NPC.velocity = new Vector2(NPC.direction * distance, jumpHeight);
                    NPC.netUpdate = true;
                }
                
            }

            if (Timer > 20 && NPC.collideY)
            {
                SwitchState(AIState.Land);
            }
        }

        private void AI_Land()
        {
            Timer++;
            if (Timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    //This is the part where you spawn the cool ahh shockwaves
                    //But we have to make cool ahh shockwaves :(
                    int shockwaveDamage = ShockwaveDamage;
                    int knockback = 1;
                    Vector2 velocity = Vector2.UnitX;
                    velocity *= 4;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                        ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                    velocity = -velocity;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                   ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                }

                SpecialEffectsPlayer specialEffectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                specialEffectsPlayer.rippleCount = 20;
                specialEffectsPlayer.rippleSize = 5;
                specialEffectsPlayer.rippleSpeed = 15;
                specialEffectsPlayer.rippleDistortStrength = 300f;
                specialEffectsPlayer.rippleTimer = 180f;

                SoundStyle landingSound = new SoundStyle($"Stellamod/Assets/Sounds/Verifall");
                landingSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(landingSound, NPC.position);

                for (int i = 0; i < 16; i++)
                {
                    Vector2 dustVelocity = -Vector2.UnitY;
                    dustVelocity *= Main.rand.NextFloat(3f, 7f);
                    dustVelocity = dustVelocity.RotatedByRandom(MathHelper.ToRadians(30));
                    Dust.NewDustPerfect(NPC.Bottom, DustID.GemDiamond, dustVelocity);
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector2 dustVelocity = -Vector2.UnitX;
                    dustVelocity *= Main.rand.NextFloat(3f, 7f);
                    dustVelocity.Y -= Main.rand.NextFloat(1f, 2f);
                    if (i % 2 == 0)
                    {
                        dustVelocity.X = -dustVelocity.X;
                    }
                    Dust.NewDustPerfect(NPC.Bottom, DustID.GemDiamond, dustVelocity);
                }
            }

            NPC.velocity.X = 0;
            float target = 60;
            if (InSecondPhase)
            {
                target *= 0.5f;
            }

            if (Timer >= target)
            {
                if (InSecondPhase)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            SwitchState(AIState.Jump);
                        } else
                        {
                            SwitchState(AIState.Idle);
                        }
                    }
                }
                else
                {
                    SwitchState(AIState.Idle);
                }
              
            }
        }

        private void AI_Summon()
        {
            Timer++;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
            if (Timer == 60)
            {
                if (StellaMultiplayer.IsHost)
                {
                    int damage = HandDamage;
                    int knockback = 1;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EliteCommanderHand>(), damage, knockback, Main.myPlayer,
                        ai1: NPC.whoAmI);

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EliteCommanderHand>(), damage, knockback, Main.myPlayer,
                        ai0: 60,
                        ai1: NPC.whoAmI);
                }
            }

            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Despawn()
        {
            Timer++;
            NPC.noTileCollide = true;
            NPC.EncourageDespawn(60);
        }

        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                AbovePlayerTimer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            if (NPC.HasValidTarget && Target.Top.Y > NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                return true;
            }

            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 4; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Copper, 0f, -2f, 180, default, .6f);
                    Main.dust[num].noGravity = true;
                    Dust expr_62_cp_0 = Main.dust[num];
                    expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    Dust expr_92_cp_0 = Main.dust[num];
                    expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    if (Main.dust[num].position != NPC.Center)
                    {
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            //Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            //npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.GintzeBossRel>()));
        }


        public override void OnKill()
        {
            base.OnKill();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedGintzlBoss, -1);
            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
            }
        }
    }
}
