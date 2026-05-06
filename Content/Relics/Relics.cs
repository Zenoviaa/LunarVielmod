using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.BossBannerSystem;
using Stellamod.Content.Areas.SpringHills.BossesSH.Ravager;
using Stellamod.Content.BossPages;
using Stellamod.Core.Camera;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Relics;

public abstract class AbstractRelicItem<ItemClass, TileClass> : ModItem
    where TileClass : ModTile
    where ItemClass : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.master = true;
        Item.masterOnly = true;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<TileClass>();
    }
}


public class RelicSummon : ModProjectile
{
    private bool _hasSummoned;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private float Time => 165;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.penetrate = -1;
        Projectile.timeLeft = (int)Time;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle summonSound = new SoundStyle("Stellamod/Assets/Sounds/RisingSummon");
            SoundEngine.PlaySound(summonSound, Projectile.position);
        }
        ShakeModSystem.Shake = 3;
        CameraTargetSystem.AddTarget(Projectile.Center);
        if(Timer % 8 == 0)
        {
            Vector2 pos = Projectile.Center + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(100, 250);
            Vector2 vel = (Projectile.Center - pos) * 0.03f;
            SparkleParticle sp = SparkleParticle.Spawn(pos, vel, Color.Red, 0.3f);
            sp.outerColor = Color.Red;
            sp.innerColor = Color.White;
            sp.fast = true;
            sp.noTileCollide = true;
            sp.gravity = 0;

            pos = Projectile.Center + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(100, 250);
            vel = (Projectile.Center - pos) * 0.03f;
            var gp = FXUtil.GlowStretch(pos, vel);
            gp.OuterGlowColor = Color.Red;
        }

        if(Timer >= Time && !_hasSummoned)
        {
            _hasSummoned = true;
            if (this.OwnedByLocalClient())
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    MultiplayerHelper.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, (int)Projectile.ai[1], (int)Projectile.position.X, (int)Projectile.position.Y);
                }
                else
                {
                    NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.position.X, (int)Projectile.position.Y, (int)Projectile.ai[1]);
                }
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            var b = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Red, Color.DarkRed);
            b.Scale *= 2;


            float numDust = 48;
            for(float f = 0; f < numDust; f++)
            {
                Vector2 vel = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(8f, 12f);
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, vel, Color.Red, Main.rand.NextFloat(0.6f, 1f));
                sp.outerColor = Color.Red;
                sp.innerColor = Color.White;
                sp.fast = true;
                sp.dampening = 0.05f;
                sp.noTileCollide = true;
                sp.gravity = 0;
            }
            for (float f = 0; f < numDust; f++)
            {
                Dust d  = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(24, 24), Scale: Main.rand.NextFloat(0.6f, 2f));
                d.noGravity = true;
            }
            Projectile.Kill();
        }

    }

    private void RenderGlowingBall(SpriteBatch sb, Vector2 sp)
    {
        Asset<Texture2D> glowBallAsset = AssetManager.GlowMask.SimpleGlowCircle;
        float ratio = Timer / Time;
        float ease = EasingFunction.InOutSine(ratio);
        Color color = Color.Lerp(Color.Red * 0.5f, Color.Red, ease);
        Vector2 scale = Vector2.Lerp(Vector2.Zero, Vector2.One, ease);
        SpritebatchDrawer dw = SpritebatchDrawer.FromTextureAsset(glowBallAsset, Projectile.Center);
        dw.color = color;
        dw.color.A = 0;
        dw.scale = scale;
        sb.Draw(dw);
        dw.color = Color.White;
        dw.color.A = 0;
        dw.scale *= 0.5f;
        sb.Draw(dw);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(RenderGlowingBall);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public abstract class AbstractRelicTile<ItemType, BossType> : ModTile
    where ItemType : ModItem
    where BossType : BossPage
{
    public override string Texture => this.GetTypeDirectoryWithSlash() + "RelicPedestal";
    public Asset<Texture2D> RelicTextureAsset;
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileShine[Type] = 400;
        Main.tileTable[Type] = true;
        Main.tileSolidTop[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = false;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
        MineResist = 4f;
        MinPick = 200;

        DustType = ModContent.DustType<Dusts.GunFlash>();
        AdjTiles = new int[] { TileID.Bookcases };
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.Width = 6;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
        TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
        TileObjectData.newTile.StyleMultiplier = 2; //same as above
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.Origin = new Point16(3, 1);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();

        // Register map name and color
        // "MapObject.Relic" refers to the translation key for the vanilla "Relic" text
        AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        RegisterItemDrop(ModContent.ItemType<ItemType>());
    }

    public override void Load()
    {
        base.Load();
        if (!Main.dedServ)
        {
            RelicTextureAsset = ModContent.Request<Texture2D>(base.Texture);
        }
    }

    public override void Unload()
    {
        base.Unload();
        RelicTextureAsset = null;
    }
    public override void MouseOver(int i, int j)
    {

    }

    public override void MouseOverFar(int i, int j)
    {
        MouseOver(i, j);
        Player player = Main.LocalPlayer;
        if (player.cursorItemIconText == "")
        {
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = ItemID.None;
        }
    }


    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.2f;
        g = 0.165f;
        b = 0.12f;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {

    }
    public override bool CanExplode(int i, int j) => false;
    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }


    public override bool RightClick(int i, int j)
    {
        Point pointToClickFrom = new Point(i, j);
        Tile tile = Main.tile[pointToClickFrom];
        int a = 0;
        while(tile.TileFrameX != 0)
        {
            a++;
            if (a > 100)
                break;
            pointToClickFrom.X--;
            tile = Main.tile[pointToClickFrom];
        }
        while (tile.TileFrameY != 0)
        {
            a++;
            if (a > 100)
                break;
            pointToClickFrom.Y--;
            tile = Main.tile[pointToClickFrom];
        }
        Vector2 worldPos = pointToClickFrom.ToWorldCoordinates(48, -32);

        int bossType = ModContent.GetInstance<BossType>().bossNPC.Type;
        Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), worldPos, Vector2.Zero, ModContent.ProjectileType<RelicSummon>(), 1, 1, Main.LocalPlayer.whoAmI, ai1: bossType);
        return true;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {

        // Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
        // Therefore we register the top-left of the tile as a "special point"
        // This allows us to draw things in SpecialDraw
        int frameWidth = 16 * 6;
        int frameHeight = 16 * 2;
        if (drawData.tileFrameX % frameWidth == 0 && drawData.tileFrameY % frameHeight == 0)
        {
            Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }
    }

    public virtual Vector2 DrawOffset()
    {
        return Vector2.Zero;
    }
    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        // This is lighting-mode specific, always include this if you draw tiles manually
        Vector2 offScreen = new Vector2(Main.offScreenRange);
        if (Main.drawToScreen)
        {
            offScreen = Vector2.Zero;
        }

        // Take the tile, check if it actually exists
        Point p = new Point(i, j);
        Tile tile = Main.tile[p.X, p.Y];
        if (tile == null || !tile.HasTile)
        {
            return;
        }

        // Get the initial draw parameters
        Texture2D texture = RelicTextureAsset.Value;


        Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height);
        Vector2 worldPos = p.ToWorldCoordinates(48, 64f);
        worldPos += DrawOffset();

        Color color = Lighting.GetColor(p.X, p.Y);


        SpriteEffects effects = SpriteEffects.None;
        // Some math magic to make it smoothly move up and down over time
        const float TwoPi = (float)Math.PI * 2f;
        float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
        Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);
  
        // Draw the main texture
        spriteBatch.Draw(texture, drawPos, null, color, 0f, origin, 1f, effects, 0f);

        // Draw the periodic glow effect
        float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
        Color effectColor = color;
        effectColor.A = 0;
        effectColor = effectColor * 0.1f * scale;
        for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
        {
            spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), null, effectColor, 0f, origin, 1f, effects, 0f);
        }

        if (!ModContent.RequestIfExists<Texture2D>
            (this.GetTypeDirectoryWithSlash() + "StarRank", out var starRankTextureAsset))
            return;

        Rectangle frame = new Rectangle(0, 0, 64, 50);
        frame.Y = (ModContent.GetInstance<BossType>().StarRanking-1) * frame.Height;
        drawPos.Y -= texture.Height;
        origin = frame.Size() * 0.5f;
        spriteBatch.Draw(starRankTextureAsset.Value, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
        for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
        {
            spriteBatch.Draw(starRankTextureAsset.Value, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        /*
        Vector2 worldPos = (new Vector2(i, j+1) + VeilGen.TileAdj) * 16;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(RelicTextureAsset.Value, worldPos);
        drawer.color = Lighting.GetColor(new Point(i, j));
        drawer.BottomCenterOrigin();
        spriteBatch.Draw(drawer);

        Asset<Texture2D> tileTextureAsset = TextureAssets.Tile[Type];
        SpritebatchDrawer statueDrawer = SpritebatchDrawer.FromTextureAsset(tileTextureAsset, worldPos);
        statueDrawer.BottomCenterOrigin();
        statueDrawer.color = drawer.color;
        statueDrawer.worldPosition.Y += ExtraMath.Osc(0f, -8f, speed: 2);
        spriteBatch.Draw(statueDrawer);

        statueDrawer.color = Color.Lerp(Color.Black, Color.White, ExtraMath.Osc(0f, 1f, offset: i)) * 0.66f;
        statueDrawer.color.A = 0;
        spriteBatch.Draw(statueDrawer);*/

        return true;
    }
}


//Woodland Ravager
public class WoodlandRavagerRelic :
    AbstractRelicTile<WoodlandRavagerRelicItem, WoodlandRavagerPage>
{

}

public class WoodlandRavagerRelicItem :
    AbstractRelicItem<WoodlandRavagerRelicItem, WoodlandRavagerRelic>
{

}

//Punker Prime
public class PunkerPrimeRelic :
    AbstractRelicTile<PunkerPrimeRelicItem, PunkerPrimePage>
{

}

public class PunkerPrimeRelicItem :
    AbstractRelicItem<PunkerPrimeRelicItem, PunkerPrimeRelic>
{

}

//Descending Twins
public class DescendingTwinsRelic :
    AbstractRelicTile<DescendingTwinsRelicItem, DescendingTwinsPage>
{

}

public class DescendingTwinsRelicItem :
    AbstractRelicItem<DescendingTwinsRelicItem, DescendingTwinsRelic>
{

}

//Steamroller
public class SteamrollerRelic :
    AbstractRelicTile<SteamrollerRelicItem, SteamrollerPage>
{

}

public class SteamrollerRelicItem :
    AbstractRelicItem<SteamrollerRelicItem, SteamrollerRelic>
{

}


public class VerliaRelic : AbstractRelicTile<VerliaRelicItem, VerliaPage>
{

}

public class VerliaRelicItem : AbstractRelicItem<VerliaRelicItem, VerliaRelic>
{

}
public class CelestiaRelic : AbstractRelicTile<CelestiaRelicItem, CelestiaPage>
{
    public override Vector2 DrawOffset()
    {
        return new Vector2(-20, 0);
    }

}

public class CelestiaRelicItem : AbstractRelicItem<CelestiaRelicItem, CelestiaRelic>
{

}
public class CariyaRelicItem : AbstractRelicItem<CariyaRelicItem, CariyaRelic>
{

}
public class CariyaRelic : AbstractRelicTile<CariyaRelicItem, CariyaPage>
{

}
public class KingJellyfishRelicItem : AbstractRelicItem<KingJellyfishRelicItem, KingJellyfishRelic>
{

}
public class KingJellyfishRelic : AbstractRelicTile<KingJellyfishRelicItem, KingJellyfishPage>
{

}