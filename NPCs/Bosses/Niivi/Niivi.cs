using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Fenix;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi : ModNPC
    {
        public enum ActionState
        {
            Roaming,
            Obliterate,
            Sleeping,
            BossFight
        }

        public enum BossActionState
        {
            Idle,
            Frost_Breath,
            Laser_Blast,
            Star_Wrath,
            Charge,
            Thunderstorm,
            Baby_Dragons,
            Swoop_Out,
            PrepareAttack
        }

        public ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = (float)value;
            }
        }

        ref float Timer => ref NPC.ai[1];
        ref float AttackTimer => ref NPC.ai[2];

        BossActionState BossState;
        BossActionState NextAttack = BossActionState.Frost_Breath;
        int ScaleDamageCounter;
        int AggroDamageCounter;



        public override void SetStaticDefaults()
        {
            //Don't want her to be hit by any debuffs
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = Total_Segments;
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Sylia/SyliaPreview";
            drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Niivi, The First Dragon")
            });
        }

        public override void SetDefaults()
        {
            //Stats
            NPC.lifeMax = 192000;
            NPC.defense = 75;
            NPC.damage = 240;
            NPC.width = (int)NiiviHeadSize.X;
            NPC.height = (int)NiiviHeadSize.Y;

            //It won't be considered a boss or take up slots until the fight actually starts
            //So the values are like this for now
            NPC.boss = false;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;

            //She'll tile collide and have gravity while on the ground, but not while airborne.
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;

            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;


        }

        public override bool CheckActive()
        {
            //Return false to prevents despawning.
            //She'll have custom code when she's in her fight phase
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //Return false for no Contact Damage
            return false;
        }

        private void ResetState(ActionState actionState)
        {
            State = actionState;
            if(State == ActionState.BossFight)
            {
                // Custom boss bar
                NPC.BossBar = ModContent.GetInstance<QueenBossBar>();
                NPC.boss = true;
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Niivi");
            }
            else
            {
                NPC.BossBar = Main.BigBossProgressBar.NeverValid;
                Music = -1;
            }
        }

        private void ResetState(BossActionState bossActionState)
        {
            BossState = bossActionState;
            Timer = 0;
            AttackTimer = 0;
            AttackCount = 0;
            NPC.netUpdate = true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int lifeToGiveIllurineScale = NPC.lifeMax / 300;
            int lifeToGiveIllurineScaleInBoss = NPC.lifeMax / 100;
            if (StellaMultiplayer.IsHost)
            {
                AggroDamageCounter += hit.Damage;
                ScaleDamageCounter += hit.Damage;

                int lifeToGive = State == ActionState.BossFight ? lifeToGiveIllurineScaleInBoss : lifeToGiveIllurineScale;
                if (ScaleDamageCounter >= lifeToGive)
                {
                    Vector2 velocity = -Vector2.UnitY;
                    velocity *= Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, velocity,
                        ModContent.ProjectileType<NiiviScaleProj>(), 0, 1, Main.myPlayer);
                    ScaleDamageCounter = 0;
                }

                if (AggroDamageCounter >= lifeToGiveIllurineScale * 15 && State != ActionState.BossFight)
                {
                    ResetState(ActionState.BossFight);
                    ResetState(BossActionState.Idle);
                    AggroDamageCounter = 0;
                }
            }
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Roaming:
                    AIRoaming();
                    break;
                case ActionState.Obliterate:
                    AIObliterate();
                    break;
                case ActionState.Sleeping:
                    AISleeping();
                    break;
                case ActionState.BossFight:
                    AIBossFight();
                    break; 
            }
        }
    }
}
