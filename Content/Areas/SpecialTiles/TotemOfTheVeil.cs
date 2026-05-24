using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Stellamod.Content.Areas.SpecialTiles;

public class NoGravestonesGlobalProjectile : GlobalProjectile
{
    public override bool PreAI(Projectile projectile)
    {
        if (ProjectileID.Sets.IsAGravestone[projectile.type])
        {
            projectile.active = false;
            return false;
        }
        return base.PreAI(projectile);
    }
}

public class TotemOfTheVeilRespawnPlayer : ModPlayer
{
    public Vector2 respawnPoint;
    public Vector2 oldRespawnPoint;
    public Point deactivated;

    public bool shouldGoToTotemSpot;
    public float totemAlpha;
    public float totemInterp;
    public float blessTimer;
    public override void UpdateDead()
    {
        base.UpdateDead();
        shouldGoToTotemSpot = true;
    }
    public override void OnRespawn()
    {
        base.OnRespawn();

    }

    public float GetInterp(Point point)
    {
        float dist = Vector2.Distance(point.ToWorldCoordinates(), oldRespawnPoint);
        if (dist > 128)
            return 0f;
        return totemInterp;
    }

    public void Toggle(Point point)
    {
        if (!IsTotemActive(point))
        {
            deactivated = Point.Zero;
        }
        else
        {
            deactivated = point;
            SoundStyle offSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead2") with { PitchVariance = 0.5f };
            offSound.Volume = 0.35f;
            SoundEngine.PlaySound(offSound);
        }
    }
    public bool IsTotemActive(Point point)
    {
        if (point == deactivated)
            return false;

        //If this one is deactived then yeah return
        float dist = Vector2.Distance(deactivated.ToWorldCoordinates(), point.ToWorldCoordinates());
        if (dist <= 128)
        {
            return false;
        }
        return true;
    }
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Player.whoAmI != Main.myPlayer)
            return;

        if(blessTimer > 0)
        {
            if(blessTimer % 5 == 0)
            {
                Vector2 pos = new Vector2();
                pos.X = Main.rand.Next(0, Player.width);
                pos.Y = Main.rand.Next(0, Player.height);
                pos += Player.position;
                var dp = DustParticle.Spawn(pos, -Vector2.UnitY);
                dp.outerColor = Color.Goldenrod;
                dp.Scale *= 0.3f;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.01f;
            }

            blessTimer--;
        }

        if (shouldGoToTotemSpot)
        {
            if (respawnPoint != default(Vector2))
            {
                Player.Teleport(respawnPoint, TeleportationStyleID.DebugTeleport);
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, respawnPoint.X, respawnPoint.Y, 1);
                SoundStyle respawnSound = new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Spawn");
                SoundEngine.PlaySound(respawnSound);
            }

            shouldGoToTotemSpot = false;
        }

        (Point topLeft, Point bottomRight) = TileUtilities.CenterTileBounds(Player.Center, width: 252, height: 252);
        Point totemOfTheVeilPoint = Point.Zero;
        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile && tile.TileType == ModContent.TileType<TotemOfTheVeilTile>())
                {
                    totemOfTheVeilPoint = new Point(x, y);
                    break;
                }
            }
        }

        if (respawnPoint != default(Vector2))
        {
            totemAlpha++;
        }

        totemAlpha = MathHelper.Clamp(totemAlpha, 0f, 60f);
        totemInterp = totemAlpha / 60f;
        if (totemOfTheVeilPoint == default(Point))
        {
            totemAlpha--;
            return;
        }
        if (totemOfTheVeilPoint == deactivated)
        {
            totemAlpha--;
            return;
        }

        //If this one is deactived then yeah return
        float dist = Vector2.Distance(deactivated.ToWorldCoordinates(), totemOfTheVeilPoint.ToWorldCoordinates());
        if (dist <= 128)
        {
            totemAlpha--;
            respawnPoint = Vector2.Zero;
            return;
        }

        Vector2 proposedTotemPoint = totemOfTheVeilPoint.ToWorldCoordinates();
        float distToNewPoint = Vector2.Distance(respawnPoint, proposedTotemPoint);
        if (distToNewPoint <= 128)
            return;

        respawnPoint = proposedTotemPoint;
        oldRespawnPoint = respawnPoint;
        SoundStyle blessSound = new SoundStyle("Stellamod/Assets/Sounds/CorsageRune1");
        SoundEngine.PlaySound(blessSound);
        blessTimer = 90;
    }

    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
        tag["respawnPoint"] = respawnPoint;
    }
    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);
        respawnPoint = tag.Get<Vector2>("respawnPoint");
    }
}

public class TotemOfTheVeil : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<TotemOfTheVeilTile>());
    }
}

public class TotemOfTheVeilTile : ModTile
{
    public Asset<Texture2D> WingsTextureAsset;
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
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.Width = 2;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
        TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
        TileObjectData.newTile.StyleMultiplier = 2; //same as above
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.Origin = new Point16(1, 5);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();

        // Register map name and color
        // "MapObject.Relic" refers to the translation key for the vanilla "Relic" text
        AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        RegisterItemDrop(ModContent.ItemType<TotemOfTheVeil>());
    }

    public override void Load()
    {
        base.Load();
        if (!Main.dedServ)
        {
            WingsTextureAsset = ModContent.Request<Texture2D>($"{Texture}_Wings");
        }
    }

    public override void Unload()
    {
        base.Unload();
        WingsTextureAsset = null;
    }
    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<TotemOfTheVeil>();
    }

    public override void MouseOverFar(int i, int j)
    {
        MouseOver(i, j);
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
        Main.LocalPlayer.GetModPlayer<TotemOfTheVeilRespawnPlayer>().Toggle(pointToClickFrom);// = pointToClickFrom;
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
        Vector2 worldPos = (new Vector2(i, j + 1) + VeilGen.TileAdj) * 16;
        worldPos.X += 10;
        worldPos.Y += 16;

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(WingsTextureAsset.Value, worldPos);
        drawer.color = Lighting.GetColor(new Point(i, j));
        drawer.CenterOrigin();
        spriteBatch.Draw(drawer);

        SpritebatchDrawer beamDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, worldPos + new Vector2(0, 64));
        beamDrawer.color = Color.Goldenrod * ExtraMath.Osc(0.5f, 1f, speed: 3) * Main.LocalPlayer.GetModPlayer<TotemOfTheVeilRespawnPlayer>().GetInterp(new Point(i, j));
        beamDrawer.color.A = 0;
        beamDrawer.BottomCenterOrigin();
        spriteBatch.Draw(beamDrawer);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {


        return true;
    }
}
