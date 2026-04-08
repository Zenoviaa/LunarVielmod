using Microsoft.Build.Framework;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Content.Armors.Veldrin;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Alsis;

public class AlsisAura : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var glowStretch = FXUtil.GlowStretch(Projectile.Center, new Vector2(-1, 1));
            glowStretch.VectorScale.X *= 4;
            glowStretch.OuterGlowColor = Color.Pink;
            SoundStyle spawnSound = new SoundStyle("Stellamod/Assets/Sounds/Parendine2");
            spawnSound.PitchVariance = 0.3f;
            spawnSound.Volume = 0.5f;
            spawnSound.Pitch = -0.3f;
            SoundEngine.PlaySound(spawnSound, Projectile.position);
        }

        if (Timer % 12 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero, Color.White, Scale: 0.5f);
            sp.fast = true;
            sp.gravity = 0;
        }

        if (Main.rand.NextBool(32))
        {
            Vector2 initialVelocity = -Vector2.UnitY * 4;
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.Pink,
                outerColor = Color.DarkViolet
            };

            Vector2 pos = Owner.position + new Vector2(Main.rand.Next(0, Owner.width), Main.rand.Next(0, Owner.height));
            DustParticle dp = DustParticle.Spawn(pos, initialVelocity, spawnParams);
            dp.gravity = 0f;
            dp.dampening = 0.05f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
        return false;
    }

    private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> noise = AssetManager.GlowMask.MagicCircleVampiricVine;
        Vector2 drawOrigin = noise.Size() / 2f;
        Texture2D texture = noise.Value;

        Vector2 drawCenter = Projectile.Center - Main.screenPosition;
        drawCenter.Y += Owner.gfxOffY;

        float ease = EasingFunction.InOutSine((float)Projectile.timeLeft / 60f) * EasingFunction.InOutSine(Timer / 10f);
        Color drawColor = Color.White;
        drawColor.A = 0;
        Color drawColor2 = Color.Pink;
        drawColor2.A = 0;
        //     drawColor *= 0.5f;

        Vector2 scale = Vector2.One;
        scale *= ease;
        scale *= 4;
        var shader = CelestialAuraShader.Instance;
        shader.InnerColor = Color.DarkViolet;
        shader.OuterColor = Color.Black;
        shader.Time = -Timer * 0.05f + 1;
        shader.Tiling = Vector2.One * 0.1f;
        spriteBatch.Restart(effect: shader.Effect);
        for (float f = 0; f < 8; f++)
        {
            Color glowColor = Color.Lerp(drawColor, drawColor2, (f + 1) / 3f);
            glowColor.A = 0;
            float rotOffset = (f / 8) * MathHelper.TwoPi;
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset + 0.5f, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * 0.75f * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * scale, SpriteEffects.None, 0);
        }

        spriteBatch.RestartDefaults();
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class AlsisGlobalProjectile : GlobalProjectile
{
    public bool isEnchanted;
    public override bool InstancePerEntity => true;
    public override void SetDefaults(Projectile entity)
    {
        base.SetDefaults(entity);
        isEnchanted = false;
    }

    public override bool PreAI(Projectile projectile)
    {
        Player player = Main.player[projectile.owner];
        AlsisPlayer alsisPlayer = player.GetModPlayer<AlsisPlayer>();
        if (!isEnchanted && alsisPlayer.hasSetBonus && projectile.friendly)
        {
            int manaCost = projectile.arrow ? 20 : 5;
            if(alsisPlayer.hasSetBonus && !alsisPlayer.exhausted && player.CheckMana(manaCost, true))
            {
                if(player.manaRegenDelay < 140)
                    player.manaRegenDelay = 140;
                isEnchanted = true;
            }
        }
        return base.PreAI(projectile);
    }
    public override void PostAI(Projectile projectile)
    {
        base.PostAI(projectile);
    }
    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(projectile, target, hit, damageDone);
        if (isEnchanted && projectile.type != ModContent.ProjectileType<AlsisAura>())
        {
            Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, 
                ModContent.ProjectileType<AlsisAura>(), projectile.damage, projectile.knockBack, projectile.owner);
        }
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(projectile, target, ref modifiers);
        if (isEnchanted)
        {
            modifiers.FinalDamage *= 3;
        }
    }
}

public class AlsisPlayer : ModPlayer
{
    private int _frame;
    private Asset<Texture2D> _masteryMagicTextureAsset;
    public bool hasSetBonus;
    public bool exhausted;
    public float alphaTimer;
    public float frameTimer;
    public override void Unload()
    {
        base.Unload();
        _masteryMagicTextureAsset = null;
    }

    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }

    
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSetBonus)
            return;

        if(!Player.CheckMana(20, pay: false))
        {
            exhausted = true;
        }

        if (exhausted)
        {
            alphaTimer-=0.05f;
            if(Player.statMana >= Player.statManaMax2)
            {
                exhausted = false;
            }
        }
        else
        {
            alphaTimer += 0.05f;
        }
        frameTimer++;
        if(frameTimer >= 2)
        {
            _frame++;
            if (_frame >= 55)
                _frame = 0;
            frameTimer = 0;
        }
        alphaTimer = MathHelper.Clamp(alphaTimer, 0f, 1f);
        CrossbowPlayer crossbowPlayer = Player.GetModPlayer<CrossbowPlayer>();
        crossbowPlayer.magicCircleColor = Color.Violet;
        crossbowPlayer.magicCircleTextureAsset = AssetManager.GlowMask.AlsisMagicCircle;
        crossbowPlayer.magicCircleScale *= 2;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;

        _masteryMagicTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "MasteryofMagic");
        int frameCount = 55;
        SpriteBatch sb = Main.spriteBatch;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_masteryMagicTextureAsset, drawInfo.drawPlayer.Center);
        Rectangle frame = _masteryMagicTextureAsset.Value.GetFrame(_frame, frameCount);
        drawer.worldPosition.Y += drawInfo.drawPlayer.gfxOffY;
        drawer.color *= alphaTimer;
        drawer.color.A = 0;
        drawer.sourceRect = frame;
        drawer.drawOrigin = frame.Size() * 0.5f;
        sb.Draw(drawer);

    }
}

[AutoloadEquip(EquipType.Head)]
public class AlsisMask : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        ArmorSetSystem.RegisterArmorSet<AlsisMask, AlsisChestplate, AlsisGreaves>();
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Pink; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.rangedGunAmmoAmountPct += 1;
        stats.defenseBonus += 18;
        stats.accessorySlots++;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<AlsisChestplate>() && legs.type == ModContent.ItemType<AlsisGreaves>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<AlsisPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class AlsisChestplate : ModItem
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
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Pink; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.rangedDamage += 0.56f;
        stats.defenseBonus += 19;
        stats.accessorySlots++;
    }


}

[AutoloadEquip(EquipType.Legs)]
public class AlsisGreaves : ModItem
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
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Pink; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 11;
        stats.rangedPiercing += 3;
        stats.accessorySlots++;
    }

}