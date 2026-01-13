using Stellamod.Common.ArmorRework;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
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
        private float _totalAmountHealed;
        private ref float KillMyselfTimer => ref NPC.ai[0];
        private ref float Lifetime => ref NPC.ai[1];
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
        private float _damageBoostTimer;
        private bool _spawnedMinionNPC;
        private int _npcWhoAmI = -1;
        private Vector2 _teleportationPoint;

        protected Player Owner => Main.player[Projectile.owner];

        public float lifetime;
        public bool isGuardian;
        public static event Action<Projectile> OnKillMinion;
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
            writer.Write(_damageBoostTimer);
            writer.Write(isGuardian);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _spawnedMinionNPC = reader.ReadBoolean();
            _npcWhoAmI = reader.ReadInt32();
            _teleportationPoint = reader.ReadVector2();
            _damageBoostTimer = reader.ReadSingle();
            isGuardian = reader.ReadBoolean();
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
        public void DamageBuff()
        {
            _damageBoostTimer += 120;
            Projectile.netUpdate = true;
        }

        public void Heal()
        {
            NPC npc = Main.npc[_npcWhoAmI];
            npc.AddBuff(ModContent.BuffType<RuneHealing>(), 30);
        }



        public void Teleport(Vector2 teleportCenter)
        {
            if (!this.OwnedByLocalClient())
                return;
            _teleportationPoint = teleportCenter;
            Projectile.netUpdate = true;
        }


        //TODO: Seal this so you can't accidentally override it
        public override void AI()
        {
            base.AI();
            AI_BellMinionLogic();
        }

        private void AI_BellMinionLogic()
        {
            if (_damageBoostTimer > 0)
            {
                if (Main.rand.NextBool(4))
                {
                    var spawnParams = new DustParticleSpawnParams
                    {
                        innerColor = Color.Red,
                        outerColor = Color.DarkRed,
                        scaleRange = new Vector2(0.1f, 1f)

                    };
                    var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), -Vector2.UnitY * 4 * Main.rand.NextFloat(0.5f, 1f), spawnParams);
                    dp.gravity = 0;
                    dp.fast = true;
                    dp.dampening = 0.05f;
                }

                _damageBoostTimer--;
            }
            if (_teleportationPoint.X != 0 || _teleportationPoint.Y != 0)
            {
                Projectile.Center = _teleportationPoint;
                _teleportationPoint = Vector2.Zero;
            }

            Owner.GetModPlayer<BellPlayer>().hasBellMinions = true;
            if (isGuardian)
            {
                Owner.GetModPlayer<BellPlayer>().hasGuardian = true;
            }
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

            Color baseColor = isGuardian ? Color.Red : Color.White;
            Color finalColor = baseColor.MultiplyRGB(lightColor);

            spriteBatch.Draw(texture, drawPos - Vector2.UnitX * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitX * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos - Vector2.UnitY * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitY * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
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
            if (_damageBoostTimer > 0)
            {
                Color flickerColor = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16));
                flickerColor = flickerColor.MultiplyRGB(lightColor);
                finalColor = flickerColor;
            }

            spriteBatch.Draw(texture, drawPos, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage.Base += Owner.GetModPlayer<ArmorStatsPlayer>().summonDamage;
            if (_damageBoostTimer > 0)
            {
                modifiers.FinalDamage.Base += 0.15f;
            }
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
