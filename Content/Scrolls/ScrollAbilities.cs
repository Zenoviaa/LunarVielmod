using Stellamod.Assets;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.Scrolls.Projectiles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Content.Scrolls;
public class ScrollBuffPlayer : ModPlayer
{
    public int angerStacks = 0;
    public int enrageStacks = 0;
    public int enduranceStacks = 0;
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        if (!Player.HasBuff<Endurance>())
        {
            enduranceStacks = 0;
        }
        
        if (!Player.HasBuff<Enrager>())
        {
            enrageStacks = 0;
        }

        if (!Player.HasBuff<Anger>())
        {
            angerStacks = 0;
        }
    }
}


public class ScrollAbilities : ModSystem
{
    public static Dictionary<ScrollAbility, Item> scrollsToContentTemplates;
    public static float Alpha { get; private set; }
    public static ScrollAbility enchant;
    public static ScrollItem usingScroll;
    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        scrollsToContentTemplates = new Dictionary<ScrollAbility, Item>();
        foreach(var scroll in ModContent.GetContent<ScrollItem>())
        {
            scrollsToContentTemplates.TryAdd(scroll.Ability, scroll.Item);
        }
    }
    public override void Unload()
    {
        base.Unload();
        scrollsToContentTemplates?.Clear();
        scrollsToContentTemplates = null;
        usingScroll = null;
    }
    public override void Load()
    {
        base.Load();
        On_Main.DrawPlayers_AfterProjectiles += RenderBlackOverlay;
        //We're not autoloading scrolls because the items are being auto-generated from the enum
        //This removes a lot of boiler plate with setting up item classes and having overrides etc
        //And it's a bit easier to edit how they work in bulk if need-be
        ScrollAbility[] abilities = Enum.GetValues<ScrollAbility>();
        for (int i = 0; i < abilities.Length; i++)
        {
            string a = abilities[i].ToString();
            if (a[0] == '_')
                continue;
            ScrollItem scrollItem = new ScrollItem(abilities[i]);
            Mod.AddContent(scrollItem);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        base.ModifyInterfaceLayers(layers);
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Scarlet Sun: Enchanter",
                delegate
                {
                    if (IsEnchanting())
                    {
                        SpriteBatch spriteBatch = Main.spriteBatch;
                        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Main.screenPosition + Main.MouseScreen);
                        glowDrawer.color = Main.DiscoColor * ExtraMath.Osc(0.6f, 1f, speed: 12);
                        glowDrawer.color.A = 0;
                        glowDrawer.scale *= 0.25f;
                        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
                        spriteBatch.Draw(glowDrawer);

                        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Main.screenPosition + Main.MouseScreen);
                        glowDrawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 12);
                        glowDrawer.color.A = 0;
                        glowDrawer.scale *= 0.15f;
                        spriteBatch.Draw(glowDrawer);
                        string name = (usingScroll != null && usingScroll.DisplayName != null) ? usingScroll.DisplayName.Value : string.Empty;
                        string remainingPoints = LangText.Common("ScrollEnchantHelp", name);
                        Vector2 textPosition = Main.MouseScreen;
                        textPosition.X += 24;
                        textPosition.Y += ExtraMath.Osc(-2f, 2f, speed: 3);
                        textPosition.Y += 16;
                        Vector2 t = FontAssets.MouseText.Value.MeasureString(remainingPoints) * new Vector2(0f, 0.5f);
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, remainingPoints, textPosition,
                            Color.White * ExtraMath.Osc(0.48f, 1f, speed: 3), 0, t, Vector2.One * 1f);
                    }

                    return true;
                },
                InterfaceScaleType.UI));
        }
    }

    private void RenderBlackOverlay(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        Rectangle dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
        spriteBatch.Draw(TextureAssets.BlackTile.Value, dstRect, null, Color.Black * Alpha, 0, Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.End();
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (!IsEnchanting())
        {
            Alpha = MathHelper.Lerp(Alpha, 0f, 0.1f);
            return;
        }
  
        Alpha = MathHelper.Lerp(Alpha, 0.6f, 0.1f);
        if (Main.rand.NextBool(4))
        {
            var sp = SparkleParticle.Spawn(Main.LocalPlayer.Center + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY);
            sp.outerColor = Color.Blue;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.dampening = 0.02f;
            sp.Scale *= 0.3f;
        }
    }


    public static bool IsEnchanting()
    {
        return enchant != ScrollAbility._None;
    }
    public static int GetStaminaCost(ScrollAbility ability)
    {
        byte abilityByte = (byte)ability;
        if (abilityByte < (byte)ScrollAbility._ACT_2)
        {
            return 1;
        }
        else if (abilityByte < (byte)ScrollAbility._ACT_3)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    public static void UseAbility(Item item,
        Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback,
        ScrollAbility ability)
    {
        Color hintColor = Color.White; ;
        switch (ability)
        {
            default:
                break;
            case ScrollAbility.Enrager:
                {
                    player.AddBuff(ModContent.BuffType<Enrager>(), 600);
                    player.GetModPlayer<ScrollBuffPlayer>().enrageStacks++;
                    hintColor = Color.Red;
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EnragingFlames>(), damage, knockback, player.whoAmI);
                }
                break;

            case ScrollAbility.Anger:
                {
                    player.AddBuff(ModContent.BuffType<Anger>(), 600);
                    player.GetModPlayer<ScrollBuffPlayer>().angerStacks++;
                    hintColor = Color.Red;
                    SoundStyle useSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Spawn") with { Volume = 0.5f } ;
                    SoundEngine.PlaySound(useSound, player.position);
                    FXUtil.GlowCircleBoom(player.Center, Color.White, Color.Red, Color.DarkRed, 12, 0.18f);
                    PixelPrimitiveCircleFactory.CreateGenericBoom(player.Center, Color.Red, Color.DarkRed, 18, 100);
                    ShakeScreenPosition.Shake = 2;
                    for(int i = 0; i < 7; i++)
                    {
                        var dp = DustParticle.Spawn(player.Center + Main.rand.NextVector2Circular(40, 40), -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(3f, 6f));
                        dp.innerColor = Color.Red;
                        dp.dampening = 0.08f;
                        dp.Scale *= 0.5f;
                    }
                }
                break;

            case ScrollAbility.Endurance:
                {
                    player.AddBuff(ModContent.BuffType<Endurance>(), 300);
                    player.GetModPlayer<ScrollBuffPlayer>().enduranceStacks++;
                    hintColor = Color.SkyBlue;
                    SoundStyle useSound = new SoundStyle("Stellamod/Assets/Sounds/TOTS1") with { Volume = 0.5f };
                    SoundEngine.PlaySound(useSound, player.position);
                    FXUtil.GlowCircleBoom(player.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 12, 0.18f);
                    PixelPrimitiveCircleFactory.CreateGenericBoom(player.Center, Color.SkyBlue, Color.DarkBlue, 18, 100);
                    ShakeScreenPosition.Shake = 2;
                    for (int i = 0; i < 7; i++)
                    {
                        var dp = DustParticle.Spawn(player.Center + Main.rand.NextVector2Circular(40, 40), -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(3f, 6f));
                        dp.innerColor = Color.SkyBlue;
                        dp.dampening = 0.08f;

                    }
                    break;
                }
            
            case ScrollAbility.Flame:
                {
                    hintColor = Color.OrangeRed;
                    player.AddBuff(ModContent.BuffType<Flame>(), 60 * 20);
                    SoundStyle fireSound = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1") with { PitchVariance = 0.3f };
                    SoundEngine.PlaySound(fireSound, position);
                }
                break;
            
            case ScrollAbility.Poison:
                {
                    hintColor = Color.DarkGreen;
                    player.AddBuff(ModContent.BuffType<Poison>(), 60 * 20);
                    SoundStyle greenSound = new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Flare1") with { PitchVariance = 0.5f, Volume = 0.3f };
                    SoundEngine.PlaySound(greenSound, position);
                }
                break;

            case ScrollAbility.SimpleHome:
                {
                    hintColor = Color.LightGray;
                    for (float f = 0; f < 3; f++)
                    {
                        float progress = f / 3f;
                        float radians = MathHelper.TwoPi * progress;
                        Vector2 offset = radians.ToRotationVector2();
                        offset *= 48;
                        Vector2 fireVelocity = Vector2.Zero;
                        Projectile.NewProjectile(source, player.Center + offset, fireVelocity,
                            ModContent.ProjectileType<SimpleWhiteHomingBolt>(), damage, knockback, player.whoAmI);
                    }
                }
                break;

            case ScrollAbility.SimpleFireball:
                {
                    hintColor = Color.OrangeRed;
                    Projectile.NewProjectile(source, position, velocity.Resize(15), ModContent.ProjectileType<SimpleFireball>(), damage, knockback, player.whoAmI);
                }
                break;

            case ScrollAbility.SimpleMeteor:
                {
                    hintColor = Color.Purple;
                    Projectile.NewProjectile(source, Main.MouseWorld - new Vector2(0, 1000), Vector2.UnitY * 5, ModContent.ProjectileType<SimpleMeteor>(), damage * 2, knockback, player.whoAmI);
                }
                break;

            case ScrollAbility.SimpleSpikeball:
                {
                    hintColor = Color.DarkGray;
                    for(int i = 0; i < Main.rand.Next(3, 5); i++)
                    {
                        Vector2 throwVelocity = velocity;
                        throwVelocity = throwVelocity.RotatedByRandom(MathHelper.ToRadians(35)) * Main.rand.NextFloat(0.35f, 1f);
                        throwVelocity.Y -= 2;
                        Projectile.NewProjectile(source, position, throwVelocity, ModContent.ProjectileType<SimpleSpikeball>(), damage, knockback, player.whoAmI);
                    }
                }
                break;


            case ScrollAbility.MyScarab:
                {
                    hintColor = Color.Gold;
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GoldenScarab>(), damage, knockback, player.whoAmI);
                }
                break;
        }

        //Use Animation
        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: 1);
        var p2 = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ScrollMagicCircle>(), 1, 1, player.whoAmI);
        if(p2.ModProjectile is ScrollMagicCircle circle)
        {
            circle.hintColor = hintColor;
        }
    }

    public static bool IsApplicable(Item item)
    {
        if (item.DamageType == null)
            return false;
        if (item.damage == -1)
            return false;
        if (item.IsACoin)
            return false;
        if (item.FitsAmmoSlot())
            return false;
        //TODO:
        return true;
        //throw new NotImplementedException();
    }

    public static void ConsumeEnchantment(Item item)
    {
        if (item.TryGetGlobalItem<ScrollGlobalItem>(out ScrollGlobalItem sgi))
        {
            sgi.scroll = enchant;
        }

        SoundStyle enchantSound = new SoundStyle("Stellamod/Assets/Sounds/bloodlamp") with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(enchantSound);
        ShakeScreenPosition.Shake = 4;
        enchant = ScrollAbility._None;
        //throw new NotImplementedException();
    }
}
