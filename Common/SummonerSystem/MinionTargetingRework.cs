using Stellamod.Common.ArmorRework;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    /// <summary>
    /// Inherit from this to make a projectile able to be targeted by NPCs, it works by moving the position of players in the player array to basically fake a player being there
    /// </summary>
    public interface ITargetable
    {
        int GetAggro();
    }

    public class SpectralMinion : ModBuff
    {

    }

    public class DummyNPC : ModNPC
    {
        private float _totalAmountHealed;
        private ref float KillMyselfTimer => ref NPC.ai[0];
        private ref float Lifetime => ref NPC.ai[1];
        private int Owner =>  (int)NPC.ai[2];
        private Player MyOwner => Main.player[(int)Owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 1000;
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 1;
            NPC.defense = 0;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
            NPC.takenDamageMultiplier = 0.8f;

        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Lifetime *= 60;
            NPC.lifeMax = (int)(Lifetime);
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            base.AI();
            if (Lifetime >= NPC.lifeMax)
                Lifetime = NPC.lifeMax;
            Lifetime--;
            NPC.life = (int)Lifetime;
            if (NPC.life <= 0)
            {
                NPC.active = false;
            }

            if (NPC.HasBuff<RuneHealing>())
            {
                float healingTime = 120f;
                float totalHealingAMount = NPC.lifeMax * 0.2f;
                float amountToHealPerTick = totalHealingAMount / healingTime;
                Lifetime += amountToHealPerTick;
                _totalAmountHealed += amountToHealPerTick;
                if (_totalAmountHealed >= 100)
                {
                    NPC.HealEffect(100);
                    _totalAmountHealed -= 100;
                }
            }
            else if (_totalAmountHealed > 0)
            {
                NPC.HealEffect((int)_totalAmountHealed);
                _totalAmountHealed = 0;
            }

            if (NPC.HasBuff<SpectralMinion>())
                KillMyselfTimer = 0;
            else
                KillMyselfTimer++;
            if (KillMyselfTimer >= 5)
            {
                NPC.active = false;
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            base.ModifyIncomingHit(ref modifiers);
            BellPlayer bellPlayer = MyOwner.GetModPlayer<BellPlayer>();
            modifiers.FinalDamage *= bellPlayer.incomingDamageMultiplier;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            Lifetime -= hit.SourceDamage;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
    }
    public struct DummyPlayer
    {
        public Player player;
        public Vector2 originalPosition;
        public int playerIndex;
    }
    public class DummyPlayerHelper : ModSystem
    {
        private bool _init;
        public static int CharmPlayerIndex => 254;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();

            if (Main.gameMenu)
                return;

            if (_init)
                return;

            Main.player[CharmPlayerIndex] = (Player)Main.LocalPlayer.Clone();
            Main.player[CharmPlayerIndex].active = false;
            _init = true;
        }

        public static DummyPlayer RequestDummyPlayer()
        {
            Main.player[CharmPlayerIndex].active = true;
            return new DummyPlayer
            {
                player = Main.player[CharmPlayerIndex],
                originalPosition = Main.player[CharmPlayerIndex].position,
                playerIndex = CharmPlayerIndex
            };
        }

        public static void ReturnDummyPlayer(DummyPlayer dummyPlayer)
        {
            Main.player[dummyPlayer.playerIndex].active = false;
            Main.player[dummyPlayer.playerIndex].position = Vector2.Zero;
        }
    }
    public class MinionTargetingRework : ModSystem
    {
        //This should only run on the server btw?
        private int _playerIndex;
        private Player[] _playerArrClone = new Player[256];
        private Queue<Player> _fakePlayerQueue = new Queue<Player>();
        private bool[] _needsFixing = new bool[256];



        public override void OnModLoad()
        {
            base.OnModLoad();
            for (int i = 0; i < 256; i++)
            {
                _needsFixing[i] = false;
                _playerArrClone[i] = null;
            }
            _fakePlayerQueue.Clear();
        }
        private Player GetFreePlayer()
        {
            if (_playerIndex >= 255)
            {
                //idk just a failsafe
                return Main.player[Main.myPlayer];
            }

            _playerArrClone[_playerIndex] = Main.player[_playerIndex];
            if (_fakePlayerQueue.Count <= 0)
            {
                _fakePlayerQueue.Enqueue((Player)Main.LocalPlayer.Clone());
            }

            Main.player[_playerIndex] = _fakePlayerQueue.Dequeue();
            Player playerToUse = Main.player[_playerIndex];
            playerToUse.whoAmI = _playerIndex;
            _needsFixing[_playerIndex] = true;
            _playerIndex++;
            return playerToUse;
        }

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();

            //Starting from index 20 just so it doesn't conflict with most multiplayer playthroughs by default
            //But in the case it does I think it's fine?
            _playerIndex = 21;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is ITargetable targetable)
                {
              
                    Player player = GetFreePlayer();
                  //  player.ResetEffects();
                    player.active = true;
                    player.dead = false;
                    player.position = proj.position;
                    player.name = proj.Name;

                    ArmorStatsPlayer statsPlayer = Main.player[proj.owner].GetModPlayer<ArmorStatsPlayer>();
                    int baseAggro = targetable.GetAggro();

                    AggroSystem aggroSystem = ModContent.GetInstance<AggroSystem>();
                    aggroSystem.aggro[player.whoAmI] += baseAggro + statsPlayer.minionAggressiveness;
                }
            }
        }

        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();
            for (int i = 0; i < _needsFixing.Length; i++)
            {
                if (_needsFixing[i])
                {
                    _fakePlayerQueue.Enqueue(Main.player[i]);
                    Main.player[i] = _playerArrClone[i];
                    _needsFixing[i] = false;
                }
            }
        }
    }
}
