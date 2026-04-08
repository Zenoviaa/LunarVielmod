using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Catacombs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Fishy
{
    public class BubbledFish : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int Frame => (int)Projectile.ai[1];
        private ref float ScaleVariance => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle bubbleSound = SoundID.Item85;
                bubbleSound.Volume = 0.3f;
                SoundEngine.PlaySound(bubbleSound, Projectile.position);
                if (this.OwnedByLocalClient())
                {
                    ScaleVariance = Main.rand.NextFloat(0.7f, 1f);
                    Projectile.ai[1] = Main.rand.Next(3);
                    Projectile.netUpdate = true;
                }
                Projectile.scale = 0.0001f;
            }

            if(Timer % 24 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.BubbleBlock);
            }

            Projectile.rotation = Projectile.velocity.X * 0.02f;
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.frame = Frame;
            if(Timer < 30)
            {
                Projectile.velocity *= 0.94f;
            }
            
            if(Timer == 31)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            if(Timer > 31)
            {
                if (Projectile.velocity.Length() < 10)
                    Projectile.velocity *= 1.05f;

                NPC target = NPCHelper.FindClosestNPC(Projectile.Center, 1024);
                if(target != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, target.Center);
                }
            }
            Projectile.scale = MathHelper.Lerp(Projectile.scale, ScaleVariance, 0.1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            Main.spriteBatch.Draw(drawer);

            Asset<Texture2D> bubbleTexture = ModContent.Request<Texture2D>(Texture + "_Bubble");
            SpritebatchDrawer bubbleDrawer = SpritebatchDrawer.FromTextureAsset(bubbleTexture, Projectile.Center);
            bubbleDrawer.scale = Projectile.scale * Vector2.Lerp(
                new Vector2(1.1f, 0.9f), 
                new Vector2(0.9f, 1.1f), 
                ExtraMath.Osc(0f, 1f, speed: 2));
            Main.spriteBatch.Draw(bubbleDrawer);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundStyle bubblePop = SoundID.Item54;
            bubblePop.PitchVariance = 0.3f;
            SoundEngine.PlaySound(bubblePop, Projectile.position);
        }
    }

    public class FishyPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (hasSetBonus)
            {
                Vector2 fireDir = velocity.RotatedBy(-MathHelper.PiOver2 * Player.direction);
                Projectile.NewProjectile(source, position, fireDir.RotatedByRandom(MathHelper.ToRadians(30)), 
                    ModContent.ProjectileType<BubbledFish>(), (int)(damage * 0.5f), knockback, Player.whoAmI);
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class FishyHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<FishyHead, FishyBody, FishyLegs>();
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.healthBonus += 100;
            stats.defenseBonus += 4;
            stats.accessorySlots++;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FishyBody>() && legs.type == ModContent.ItemType<FishyLegs>();
        }


        public override void UpdateArmorSet(Player player)
        {
            player.GetJumpState<TyphoonJump>().Enable();
            player.GetModPlayer<FishyPlayer>().hasSetBonus = true;
            player.accFlipper = true; 
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class FishyBody : ModItem
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
            stats.defenseBonus += 6;
            stats.bossEndurance += 0.25f;
            stats.accessorySlots += 2;
            stats.stamina += 1;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class FishyLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.inventorySlots += 10;
            stats.defenseBonus += 4;
        }
    }
}
