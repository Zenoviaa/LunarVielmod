using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    /// <summary>
    /// Inherit from this to make a projectile able to be targeted by NPCs, it works by moving the position of players in the player array to basically fake a player being there
    /// </summary>
    public interface ITargetable
    {
        int GetAggro();
    }

    public class DummyNPC : ModNPC
    {
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
            NPC.aiStyle = 7;
            NPC.ShowNameOnHover = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
    }

    public abstract class KillableMinion : ModProjectile,
        IDrawSpectral,
        ITargetable
    {
        private bool _spawnedMinionNPC;
        private int _npcWhoAmI = -1;
        public virtual int GetAggro()
        {
            return -500;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_spawnedMinionNPC);   
            writer.Write(_npcWhoAmI);   
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _spawnedMinionNPC = reader.ReadBoolean();
            _npcWhoAmI = reader.ReadInt32();
        }

        private void ManageHealthbar()
        {
            if (!_spawnedMinionNPC && StellaMultiplayer.IsHost)
            {
                _npcWhoAmI = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y,
                    ModContent.NPCType<DummyNPC>());
                _spawnedMinionNPC = true;
                Projectile.netUpdate = true;
            }
            if (_npcWhoAmI == -1)
                return;
            NPC npc = Main.npc[_npcWhoAmI];
            npc.Center = Projectile.Center;
            if (!npc.active)
            {
                Death();
            }
        }
        public override void AI()
        {
            base.AI();
            ManageHealthbar();
        }
        public virtual void Death()
        {
            Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
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

            spriteBatch.Restart(effect: SpriteWhiteShader.Instance.Effect, blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos - Vector2.UnitX * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitX * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos - Vector2.UnitY * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitY * 2, frame, Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);


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
            for(int i =0; i < 256; i++)
            {
                _needsFixing[i] = false;
                _playerArrClone[i] = null;
            }
            _fakePlayerQueue.Clear();
        }
        private Player GetFreePlayer()
        {
            if(_playerIndex >= 255)
            {
                //idk just a failsafe
                return Main.player[Main.myPlayer];
            }

            _playerArrClone[_playerIndex] = Main.player[_playerIndex];
            if(_fakePlayerQueue.Count <= 0)
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
            foreach(var proj in Main.ActiveProjectiles)
            {
                if(proj.ModProjectile is ITargetable targetable)
                {
                    Player player = GetFreePlayer();
                    player.active = true;
                    player.dead = false;
                    player.position = proj.position;
                    player.aggro = targetable.GetAggro();
                }
            }
        }

        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();
            for(int i = 0; i < _needsFixing.Length; i++)
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
