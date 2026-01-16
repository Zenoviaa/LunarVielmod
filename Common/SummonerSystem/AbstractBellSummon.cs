using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    public abstract class AbstractBellSummon : ModProjectile,
        IDrawSpectral,
        IDrawOutlines,
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
            return -50;
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

        public NPC GetAttachedNPC()
        {
            return Main.npc[_npcWhoAmI];
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
            if (isGuardian)
            {
                DrawSprite();
            }
            return false;
        }

        private void DrawSprite()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
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
        public virtual void DrawSpectralWhites(SpriteBatch spriteBatch)
        {
            if (isGuardian)
                return;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Point p = Projectile.position.ToTileCoordinates();
            Color lightColor = Lighting.GetColor(p.X, p.Y);

            Color baseColor = isGuardian ? Color.Green : Color.White;
            Color finalColor = baseColor.MultiplyRGB(lightColor);

            spriteBatch.Draw(texture, drawPos - Vector2.UnitX * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitX * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos - Vector2.UnitY * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitY * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        public virtual void DrawSpectral(SpriteBatch spriteBatch)
        {
            if (isGuardian)
                return;

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

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (!isGuardian)
                return;

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Point p = Projectile.position.ToTileCoordinates();

            Color baseColor = Color.LightGreen;
            Color finalColor = baseColor.MultiplyRGB(lightColor);

            spriteBatch.Draw(texture, drawPos - Vector2.UnitX * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitX * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos - Vector2.UnitY * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitY * 2, frame, finalColor, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
