using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Vextin
{

    public class VextinShield : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 196;
            Projectile.height = 196;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            VextinPlayer vextinPlayer = Owner.GetModPlayer<VextinPlayer>();
            if (vextinPlayer.hasVextinSetBonus && vextinPlayer.hitCount % 2 == 1)
                Projectile.timeLeft = 60;
            if (Projectile.timeLeft == 55)
            {
                float numDust = 16;
                for (float f = 0; f < numDust; f++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        innerColor = Color.SandyBrown,
                        outerColor = Color.DarkGoldenrod,
                        gravity = 0,
                        scaleRange = new Vector2(0.3f, 1f)

                    };
                    var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), spawnParams);
                    dp.dampening = 0.05f;
                }
            }

            Projectile.Center = Owner.Center;
            Projectile.rotation += 0.005f;
        }

        private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = AssetManager.GlowMask.SpiralVortex2.Value;
            BubbleShader bubbleShader = BubbleShader.Instance;
            bubbleShader.InnerColor = Color.SandyBrown * 0.5f;
            bubbleShader.OuterColor = Color.DarkGoldenrod * 0.5f;
            bubbleShader.Distortion = 0.05f;
            bubbleShader.Time = Main.GlobalTimeWrappedHourly * -1;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: bubbleShader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White * EasingFunction.InOutSine(Projectile.timeLeft / 60f) * EasingFunction.InOutSine(Timer / 60f), Projectile.rotation, texture.Size() / 2f,
                Projectile.scale * 0.15f, SpriteEffects.None, 0);

            spriteBatch.RestartDefaults();


            Texture2D texture2 = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Color sickColor = Color.SandyBrown * EasingFunction.InOutSine(Projectile.timeLeft / 60f) * EasingFunction.InOutSine(Timer / 60f);
            sickColor.A = 0;
            sickColor *= 0.5f;
            spriteBatch.Draw(texture2, drawPos, null, sickColor, Projectile.rotation, texture2.Size() / 2f, Projectile.scale * 0.4f, SpriteEffects.None, 0);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
            return false;
        }
    }

    public class VextinPlayer : ModPlayer
    {
        public int hitCount;
        public bool hasVextinSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasVextinSetBonus = false;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.myPlayer != Player.whoAmI)
                return;
            if (!hasVextinSetBonus)
                return;
            int type = ModContent.ProjectileType<VextinShield>();
            if (Player.ownedProjectileCounts[type] > 0)
                return;
            if (hitCount % 2 != 1)
                return;


            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, type, 0, 1, Player.whoAmI);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
            if (!hasVextinSetBonus)
                return;
            if (hitCount % 2 == 1)
            {
                modifiers.FinalDamage *= 0.5f;
            }
            hitCount++;
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class VextinMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorSetSystem.RegisterArmorSet<VextinMask, VextinRobe, VextinBoots>();
        }


        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.healthBonus += 25;
            stats.defenseBonus += 4;
            stats.accessorySlots += 1;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VextinRobe>() && legs.type == ModContent.ItemType<VextinBoots>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.desertBoots = true;
            player.GetModPlayer<VextinPlayer>().hasVextinSetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class VextinRobe : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 6;
            stats.accessorySlots += 1;
            stats.generalEndurance += 0.09f;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class VextinBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 5;
            stats.movementSpeedBonus += 0.5f;
            stats.accessorySlots += 1;
        }
    }
}