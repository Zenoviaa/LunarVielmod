using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.ArmorRework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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
        private ref float KillMyselfTimer => ref NPC.ai[0];

        private ref float Lifetime => ref NPC.ai[1];

        private ref float IFrameTimer => ref NPC.ai[2];
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

            Lifetime--;
            NPC.life = (int)Lifetime;
            if(NPC.life <= 0)
            {
                NPC.active = false;
            }

            IFrameTimer--;
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
         //   modifiers.FinalDamage *= 0;
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

    public abstract class AbstractBellSummon : ModProjectile,
        IDrawSpectral,
        ITargetable
    {
        private Vector2 _teleportationPoint;
        private bool _spawnedMinionNPC;
        private int _npcWhoAmI = -1;
        private Player Owner => Main.player[Projectile.owner];
        public static event Action<Projectile> OnKillMinion;
        public float lifetime;
        public virtual int GetAggro()
        {
            return -500;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_spawnedMinionNPC);
            writer.Write(_npcWhoAmI);
            writer.WriteVector2(_teleportationPoint);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _spawnedMinionNPC = reader.ReadBoolean();
            _npcWhoAmI = reader.ReadInt32();
            _teleportationPoint = reader.ReadVector2();
        }

        private void ManageHealthbar()
        {
            if (!_spawnedMinionNPC && MultiplayerHelper.IsHost)
            {
                _npcWhoAmI = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y,
                    ModContent.NPCType<DummyNPC>(), ai1: lifetime);
                _spawnedMinionNPC = true;
                Projectile.netUpdate = true;
            }
            if (_npcWhoAmI == -1)
                return;
            NPC npc = Main.npc[_npcWhoAmI];
            npc.Center = Projectile.Center;
            npc.AddBuff(ModContent.BuffType<SpectralMinion>(), 2);
            if (!npc.active)
            {
                Death();
            }
        }

        public void Teleport(Vector2 teleportCenter)
        {
            if (!this.OwnedByLocalClient())
                return;
            _teleportationPoint = teleportCenter;
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            base.AI();
            if(_teleportationPoint.X != 0 || _teleportationPoint.Y != 0)
            {
                Projectile.Center = _teleportationPoint;
                _teleportationPoint = Vector2.Zero;
            }

            Owner.GetModPlayer<BellPlayer>().hasBellMinions = true;
            if (!SummonHelper.CheckMinionActive<BellBlessing>(Owner, Projectile))
                return;

            ManageHealthbar();
        }
        public virtual void Death()
        {
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.Blue);
            Projectile.Kill();
            OnKillMinion?.Invoke(Projectile);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
        public virtual void DrawSpectralWhites(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Point p = Projectile.position.ToTileCoordinates();
            Color lightColor = Lighting.GetColor(p.X, p.Y);
            Color finalColor = Color.White.MultiplyRGB(lightColor);

            spriteBatch.Draw(texture, drawPos - Vector2.UnitX * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitX * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos - Vector2.UnitY * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitY * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        public virtual void DrawSpectral(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Point p = Projectile.position.ToTileCoordinates();
            Color lightColor = Lighting.GetColor(p.X, p.Y);
            Color finalColor = Color.White.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, drawPos, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage.Base += Owner.GetModPlayer<ArmorStatsPlayer>().summonDamage;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

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
            _needsFixing[_playerIndex] = true;
            _playerIndex++;
            return playerToUse;
        }

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();

            //Starting from index 20 just so it doesn't conflict with most multiplayer playthroughs by default
            //But in the case it does I think it's fine?
            _playerIndex = 20;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is ITargetable targetable)
                {
                    Player player = GetFreePlayer();
                    player.active = true;
                    player.dead = false;
                    player.position = proj.position;

                    ArmorStatsPlayer statsPlayer = Main.player[proj.owner].GetModPlayer<ArmorStatsPlayer>();
                    int baseAggro = targetable.GetAggro();
                    player.aggro += baseAggro + statsPlayer.minionAggressiveness;
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
