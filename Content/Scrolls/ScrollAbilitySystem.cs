using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.Scrolls.Buffing;
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

public class AngerPlayer : ModPlayer
{
    public int stacks = 0;
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        if (!Player.HasBuff<Anger>())
        {
            stacks = 0;
        }
    }
}
public class ScrollAbilityDrawPlayer : ModPlayer
{
    private float _timer;
    private Asset<Texture2D> _angerSymbol;
    public override void Load()
    {
        base.Load();
        _angerSymbol = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "AngerSymbol");
    }
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        _timer += Player.HasBuff<Anger>() ? 1 : -1;
        _timer = MathHelper.Clamp(_timer, 0f, 60);
        
    }
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;
        _angerSymbol ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "AngerSymbol");
        SpriteBatch spriteBatch = Main.spriteBatch;
        if (drawInfo.drawPlayer.HasBuff<Anger>())
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_angerSymbol, drawInfo.drawPlayer.Center + new Vector2(18, -36));
            drawer.color = Color.Red * EasingFunction.InOutSine(_timer / 60f) * ExtraMath.Osc(0.5f, 1f, speed: 12);
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
    }
}

public class ScrollAbilitySystem : ModSystem
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
 //       Main.NewText(enchant);

        /*
        foreach(var item in ModContent.GetContent<ScrollItem>())
        {
            Main.NewText(item.Ability);
        }*/
        if (!IsEnchanting())
        {
            Alpha = MathHelper.Lerp(Alpha, 0f, 0.1f);
            return;
        }
        //    return;
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
                    hintColor = Color.Red;
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EnragingFlames>(), damage, knockback, player.whoAmI);
                }
                break;
            case ScrollAbility.Anger:
                {
                    player.AddBuff(ModContent.BuffType<Anger>(), 600);
                    player.GetModPlayer<AngerPlayer>().stacks++;
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
