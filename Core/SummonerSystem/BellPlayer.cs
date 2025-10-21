using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Shaders;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using System;
using System.Collections.Generic;

using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.SummonerSystem
{
    public class SummoningBeam : ModProjectile
    {
        private Vector2 _scale;
        private ref float Timer => ref Projectile.ai[0];
        private int MinionToSummon => (int)Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            float ticks = 30f;
            float lerp = Timer / ticks;
            float interp = EasingFunction.QuadraticBump(lerp);
            _scale = Vector2.Lerp(new Vector2(0f, 1f), Vector2.One, interp);
            if(Timer == 15)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        MinionToSummon, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Console.WriteLine(MinionToSummon);
                    Owner.AddBuff(ModContent.BuffType<BellBlessing>(), 25);
                }
                SoundStyle cast = new SoundStyle("Stellamod/Assets/Sounds/Aurora");
                cast.PitchVariance = 0.2f;
                SoundEngine.PlaySound(cast, Projectile.position);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Black);
                for(float f = 0; f < Main.rand.Next(3, 7); f++)
                {
                    FXUtil.GlowStretch(Projectile.Center, -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(3f, 6f));
                }
                for (float f = 0; f < Main.rand.Next(3, 7); f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f, 6f), 
                        newColor: Color.White, 
                        Scale: Main.rand.NextFloat(0.5f, 1f));
                }
            }

            if(Timer >= ticks)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Time = Timer * 0.3f;
            sparkyShader.Distortion = -0.15f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(effect: sparkyShader.Effect, blendState: BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, null,
                Color.White,
                0, texture.Size() / 2f,
            _scale, SpriteEffects.None, 0);

            spriteBatch.Restart(blendState: BlendState.Additive, effect: null);
            return false;
        }
    }

    public class SummoningCircle : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Owner.HasBuff<BellSummoning>())
                Projectile.timeLeft = 30;
            Projectile.Center = Owner.Bottom;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ringTexture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = RadiantShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.LightBlue;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, shader.Effect, Main.GameViewMatrix.TransformationMatrix);

            Color auraColor = Color.White;
            auraColor *= Timer / 30f;
            auraColor *= Projectile.timeLeft / 30f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle? frameRect = null;
            Vector2 scale = new Vector2(1f, 0.2f);
            Vector2 drawScale = scale * Vector2.One;
            drawScale *= MathHelper.Lerp(0.8f, 1f, ExtraMath.Osc(0f, 1f));

            float drawRotation = Projectile.rotation;
            Vector2 drawOrigin = ringTexture.Size() / 2f;
            spriteBatch.Draw(ringTexture, drawPos, frameRect, auraColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Draw(ringTexture, drawPos, frameRect, auraColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

        }
    }
    public class BellExhaust : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.pvpBuff[Type] = true; // This buff can be applied by other players in Pvp, so we need this to be true.

        }
    }
    public class BellSummoning : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.moveSpeed *= 0.5f;
        }
    }

    public class BellPlayer : ModPlayer
    {
        private List<int> _itemTypes = new List<int>();
        private List<Item> _minions = new List<Item>();
        private List<Item> _unlockedminions = new List<Item>();
        public float castTimer;
        public float castingTime;
        public bool isSummoning;
        public bool hasBellMinions;
        public override void ResetEffects()
        {
            base.ResetEffects();
            castingTime = 60;
            isSummoning = false;
            hasBellMinions= false;  
        }


        public override void PreUpdateBuffs()
        {
            base.PreUpdateBuffs();
            if (LunarVeilKeybinds.BellKeybind.Current && Main.myPlayer == Player.whoAmI)
            {
                Player.AddBuff(ModContent.BuffType<BellSummoning>(), 2);
            }
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Player.whoAmI)
                    continue;
                if (proj.ModProjectile is KillableMinion)
                    hasBellMinions = true;
            }
            isSummoning = Player.HasBuff<BellSummoning>() && !Player.HasBuff<BellExhaust>();
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
        
            if(isSummoning 
                && Player.ownedProjectileCounts[ModContent.ProjectileType<SummoningCircle>()] == 0 
                && Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, 
                    ModContent.ProjectileType<SummoningCircle>(), 1, 1, Player.whoAmI);
            }

            if (isSummoning)
            {
                castTimer++;
                if(castTimer == 1)
                {
                    SoundStyle castingStart = new SoundStyle("Stellamod/Assets/Sounds/AuroraEnd");
                    castingStart.PitchVariance = 0.2f;
                    SoundEngine.PlaySound(castingStart, Player.position);
                }
                if(castTimer >= castingTime)
                {
                    CompleteSummon();
                    castTimer = 0;
                }
            }
            else
            {
                castTimer = 0;
            }
        }

        public void CompleteSummon()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            foreach(var minionItem in _minions)
            {
                int newDamage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(minionItem.damage);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Bottom - new Vector2(0, 50), Vector2.Zero, 
                    ModContent.ProjectileType<SummoningBeam>(), newDamage, minionItem.knockBack, Player.whoAmI,
                    ai1: minionItem.shoot);
            }
            Player.AddBuff(ModContent.BuffType<BellExhaust>(), 600);
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["minions"] = _minions;
            tag["unlockedminions"] = _unlockedminions;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            _minions = new List<Item>();
            _minions = tag.Get<List<Item>>("minions");

            var u = tag.Get<List<Item>>("unlockedminions");
            _unlockedminions = u;
            ManageUnlockedMinions();
        }

        private void ManageUnlockedMinions()
        {
            _unlockedminions.RemoveAll(x => x.IsAir);
            _unlockedminions = _unlockedminions.Distinct().ToList();
            _itemTypes.Clear();
            foreach (var item in _unlockedminions)
            {
                _itemTypes.Add(item.type);
            }
        }
        public List<Item> GetMinions()
        {
            return _minions;
        }

        public void SetMinionAtIndex(Item item, int index)
        {
            List<Item> minions = GetMinions();
            while (minions.Count <= index)
            {
                Item emptyItem = new Item();
                emptyItem.SetDefaults(0);
                minions.Add(emptyItem);
            }
            minions[index] = item;
        }

        public Item GetMinionAtIndex(int index)
        {
            List<Item> minions = GetMinions();
            if (minions.Count > index)
            {
                return minions[index];
            }
            Item air = new Item(0);
            air.SetDefaults(0);
            return air;
        }

        public bool HasUnlocked(Item item)
        {
            return _itemTypes.Contains(item.type);
        }

        public bool HasUnlockedBell()
        {
            return true;
        }

        public void UnlockMinion(Item item)
        {
            _unlockedminions.Add(item);
            ManageUnlockedMinions();
        }
        public void UnlockFlask()
        {

        }

        public void ResetProgress()
        {
            _unlockedminions.Clear();
            ManageUnlockedMinions();
        }

        public void GrantAllProgress()
        {
            _unlockedminions.Clear();
            IEnumerable<ModItem> insources = ModContent.GetContent<BaseBellMinionItem>();
            foreach (var insource in insources)
            {
                _unlockedminions.Add(insource.Item);
            }

            ManageUnlockedMinions();
        }

        public bool CanUseFlask()
        {
            return true;
        }
    }
}
