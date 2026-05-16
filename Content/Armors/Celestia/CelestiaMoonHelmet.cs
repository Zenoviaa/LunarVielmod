using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Celestia
{
    public class Celestially : ModBuff { }

    public class CelestiallyDown : ModBuff { }
    public class CelestiaMoonAura : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float AlphaTimer => ref Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            base.AI();
            CelestiaMoonPlayer moonPlayer = Owner.GetModPlayer<CelestiaMoonPlayer>();
            if (!moonPlayer.hasCelestiaMoonSetBonus)
                return;
            if (Owner.HasBuff<CelestiallyDown>())
            {
                AlphaTimer++;
                
            }
            else
            {
                AlphaTimer--;
            }

            AlphaTimer = MathHelper.Clamp(AlphaTimer, 0f, 30f);
                Projectile.timeLeft = 2;
            Timer++;
            if(Main.rand.NextBool(32) && AlphaTimer <= 0)
            {
                Vector2 initialVelocity = -Vector2.UnitY * 4;
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.LightGreen,
                    outerColor = Color.Turquoise
                };


                Vector2 pos = Owner.position + new Vector2(Main.rand.Next(0, Owner.width), Main.rand.Next(0, Owner.height));
                DustParticle dp = DustParticle.Spawn(pos, initialVelocity, spawnParams);
                dp.gravity = 0f;
                dp.dampening = 0.05f;
            }
            Projectile.Center = Owner.Center;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
            return false;
        }
        private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> noise = AssetManager.Noise.Whirly;
            Vector2 drawOrigin = noise.Size() / 2f;
            Texture2D texture = noise.Value;

            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            drawCenter.Y += Owner.gfxOffY;
            Color drawColor = Color.Lerp(Color.White, Color.Black, EasingFunction.InOutSine(AlphaTimer / 30f));
            drawColor.A = 0;

            var shader = CelestialAuraShader.Instance;
            shader.InnerColor = Color.Lerp(Color.LightGreen, Color.Turquoise, ExtraMath.Osc(0f, 1f, 0.5f));
            shader.OuterColor = Color.Black;
            shader.Time = -Main.GlobalTimeWrappedHourly * 0.3f;
            shader.Tiling = Vector2.One * 0.3f;
            spriteBatch.Restart(effect: shader.Effect);
            for (float f = 0; f < 4; f++)
            {
                Color glowColor = drawColor;
                glowColor = Color.Lerp(glowColor, Color.Black, 0.4f);
                glowColor.A = 0;
                float rotOffset = (f / 4f) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset + 0.5f, drawOrigin, new Vector2(0.8f, 1f) * 0.25f * 0.75f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset, drawOrigin, new Vector2(0.8f, 1f) * 0.25f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
    public class CelestiaMoonPlayer : ModPlayer
    {
        public bool hasCelestiaMoonSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasCelestiaMoonSetBonus = false;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (!hasCelestiaMoonSetBonus)
                return;
            if (!Player.HasBuff<CelestiallyDown>())
            {
                Player.AddBuff(ModContent.BuffType<Celestially>(), 2);
            }

            if (Main.myPlayer != Player.whoAmI)
                return;
            int type = ModContent.ProjectileType<CelestiaMoonAura>();
            if (Player.ownedProjectileCounts[type] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, type, 1, 1, Player.whoAmI);
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
            if (!hasCelestiaMoonSetBonus)
                return;
            if (Player.HasBuff<CelestiallyDown>())
                return;

            int id = CombatText.NewText(Player.getRect(), Color.Turquoise, LangText.Common("Resisted"));
            var text = Main.combatText[id];
            text.lifeTime *= 3;

            SoundStyle impactSound = new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Rune_Fade");
            impactSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(impactSound, Player.position);
            for(float n = 0; n < 16; n++)
            {
                Vector2 initialVelocity = Main.rand.NextVector2Circular(16, 16);
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.LightGreen,
                    outerColor = Color.Turquoise
                };

                DustParticle.Spawn(Player.Center, initialVelocity, spawnParams);
            }
            FXUtil.ShakeCamera(Player.Center, 1024, 8);
            modifiers.FinalDamage *= 0.5f;
            Player.AddBuff(ModContent.BuffType<CelestiallyDown>(), 60 * 30);
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class CelestiaMoonHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorSetSystem.RegisterArmorSet<CelestiaMoonHelmet, CelestiaMoonBreastplate, CelestiaMoonLegs>(ArmorGroup.Act_I);
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 5;
            stats.healthBonus += 35;
            stats.accessorySlots++;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<CelestiaMoonBreastplate>() && legs.type == ModContent.ItemType<CelestiaMoonLegs>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            CelestiaMoonPlayer celestiaMoonPlayer = player.GetModPlayer<CelestiaMoonPlayer>();
            celestiaMoonPlayer.hasCelestiaMoonSetBonus = true;
        }
    }

    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
    public class CelestiaMoonBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 7;
            stats.generalEndurance += 0.2f;
            stats.accessorySlots++;
        }
    }  
    
    [AutoloadEquip(EquipType.Legs)]
    public class CelestiaMoonLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.criticalStrikeChance += 0.03f;
            stats.defenseBonus += 3;
            stats.accessorySlots++;
        }
    }
}