using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Core.Godrays;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.NPCHelpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Stellamod.Content.Areas.SpecialTiles;



// This item shows off using 1 class to load multiple items. This is an alternate to typical inheritance.
// Read the comments in this example carefully, as there are many parts necessary to make this approach work.
// The real strength of this approach is when you have many items that vary by small changes, like how these 2 trap items vary only by placeStyle.
public class CheckpointBell : ModItem
{
    public override void SetDefaults()
    {
        // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
        Item.DefaultToPlaceableTile(ModContent.TileType<CheckpointBellTile>());
        Item.mech = true; // lets you see wires while holding.
    }
}
public class CheckpointBellTileEntity : ModTileEntity
{
    int _activeNpc = -1;
    float CheckpointDistance => 1024 * 1024;
    public override void Update()
    {
        base.Update();
        if (_activeNpc!= -1)
        {

            var npc = Main.npc[_activeNpc];
            if (!npc.active || npc.type != ModContent.NPCType<BellPhone>())
                _activeNpc = -1;
            return;
        }

        if (Main.GameUpdateCount % 60 != 0)
            return;

        var worldCoordinates = Position.ToWorldCoordinates();
        var nearestPlayer = PlayerHelper.FindClosestPlayer(worldCoordinates, 999999);
        if (nearestPlayer == null)
            return;

        var sqrDistToPlayer = Vector2.DistanceSquared(worldCoordinates, nearestPlayer.Center);
        if (sqrDistToPlayer > CheckpointDistance)
            return;
        _activeNpc =  NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)worldCoordinates.X, (int)worldCoordinates.Y, ModContent.NPCType<BellPhone>());
    }

    public override void OnNetPlace()
    {
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
        }
    }

    public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
            int width = 4;
            int height = 4;
            NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

            // Sync the placement of the tile entity with other clients
            // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
            NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            return -1;
        }

        // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
        int placedEntity = Place(i, j);

        return placedEntity;
    }


    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        //The MyTile class is shown later
        return tile.HasTile && tile.TileType == ModContent.TileType<CheckpointBellTile>();
    }
}

public class BellPhonePlayer : ModPlayer
{
    int _bellPhoneType = -1;
    int BellPhoneType
    {
        get
        {
            if (_bellPhoneType == -1)
            {
                _bellPhoneType = ModContent.NPCType<BellPhone>();
            }
            return _bellPhoneType;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (target.type == BellPhoneType)
        {
            Player.SetBellRespawnPoint(target.Bottom.ToTileCoordinates() + new Point(0, -4));
        }
    }
  
}

public class BellPhone : ModNPC
{
    private enum AIState
    {
        Sway,
        Ring
    }
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float PulseTime => ref NPC.ai[2];

    public const float TRIGGER_RANGE = 256 * 256;
    private Player MyTarget => Main.player[NPC.target];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCSets.Heavy[Type] = true;
    }


    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 134;
        NPC.lifeMax = 100;
        NPC.damage = 1;
        NPC.defense = 9999;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.knockBackResist = 0;
        
    }
    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    bool IsActive() => Vector2.DistanceSquared(NPC.Center, Main.LocalPlayer.GetModPlayer<TotemOfTheVeilRespawnPlayer>().respawnPoint) < 100 * 100;
    public override void AI()
    {
        base.AI();
        if (Main.rand.NextBool(100) && Main.hasFocus && Main.netMode != NetmodeID.Server)
        {
            GodrayRenderer godrayRenderer = ModContent.GetInstance<GodrayRenderer>();
            Vector2 centerPos = NPC.Center;
            godrayRenderer.AddGodrayParticle(centerPos + Main.rand.NextVector2Circular(64, 64));
        }


        if (IsActive())
        {
            PulseTime+=0.05f;
        }
        else
        {
            PulseTime-=0.05f;
        }
        PulseTime = MathHelper.Clamp(PulseTime, 0f, 1f);
        if (Main.rand.NextBool(64))
        {
            var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(72, 72), Vector2.Zero);
            sp.gravity = 0;
            sp.Scale *= 0.35f;
            sp.fast = true;
            sp.outerColor = Color.Gold;
        }

        if (Main.GameUpdateCount % 32 == 0 && IsActive())
        {
            Particles.MusicNote.Spawn(new()
            {
                position = NPC.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = -Vector2.UnitY * Main.rand.NextFloat(1f, 2f),
                color = Color.Gold,
                timeLeft = 90
            });
        }

        Vector2 ground = MoonUtils.FindFloorWet(NPC.Center);
        ground.Y -= 64;
        NPC.velocity = ground - NPC.Center;

        NPC.TargetClosest();
        NPC.life = NPC.lifeMax;
        float distanceSquared = Vector2.DistanceSquared(NPC.Center, MyTarget.Center);
        NPC.dontTakeDamage = distanceSquared > TRIGGER_RANGE;
        Lighting.AddLight(NPC.Center, TorchID.Torch);
        switch (State)
        {
            case AIState.Sway:
                AI_Sway();
                break;
            case AIState.Ring:
                AI_Ring();
                break;
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void AI_Sway()
    {
        Timer++;
        NPC.rotation = ExtraMath.Osc(-0.05f, 0.05f);
    }

    float RingTime => 120f;
    float PulseInterp => Timer / RingTime;

    private void AI_Ring()
    {
        Timer++;
        if (Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.Checkpointbell.Asset with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound);
        }

        float range = MathHelper.Lerp(0.5f, 0f, Timer / RingTime);
        NPC.rotation = ExtraMath.Osc(-range, range, speed: 6);
        ShakeScreenPosition.Shake = MathHelper.Lerp(4, 0, Timer / RingTime);
        if (Timer >= RingTime)
        {
            SwitchState(AIState.Sway);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);

        if (State != AIState.Ring)
            SwitchState(AIState.Ring);
    }

    void DrawGoldenPulse(SpriteBatch spriteBatch)
    {
        if (State != AIState.Ring)
            return;
        var goldenPulsePass = AssetReferences.Effects.Generic.GoldenPulse.CreatePixelPass();
        goldenPulsePass.Parameters.time = PulseInterp * 3;
        goldenPulsePass.Apply();
        using(spriteBatch.Ctx(spriteBatch.Parameters with { effect = goldenPulsePass.Shader, blendState = BlendState.Additive }))
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
            drawer.color = Color.Lerp(Color.Transparent, Color.DarkOrange, EasingFunction.QuickOutSlowIn(PulseInterp));
            drawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.OutSine(PulseInterp));
            Main.spriteBatch.Draw(drawer);

       
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var blackCircle = AssetReferences.Assets.GlowMasks.WhiteCircle.Asset;
        var blackDrawer = SpritebatchDrawer.FromTextureAsset(blackCircle, NPC.Center);
        blackDrawer.color = Color.Black;
        blackDrawer.scale *= 1f;
        spriteBatch.Draw(blackDrawer);
//DrawUtilities.DrawBasicGlow(spriteBatch, NPC.Center, 0.4f, Color.DarkOrange * 0.24f * ExtraMath.Osc(0.8f, 1f, speed: 2));
    //    DrawUtilities.DrawBasicGlow(spriteBatch, NPC.Center, 1.5f, Color.DarkOrange * 0.24f * ExtraMath.Osc(0.8f, 1f, speed: 2));

        NPC.spriteDirection = 1;
        Vector2 drawOrigin = new Vector2(45, 32);
        Vector2 drawOffset = new Vector2(36, 0);
        SpritebatchDrawer rootDrawer = SpritebatchDrawer.FromNPC(NPC);
        rootDrawer.VerticalFrame(0, Main.npcFrameCount[Type]);
        rootDrawer.drawOrigin = drawOrigin;
        rootDrawer.rotation = 0;
        rootDrawer.worldPosition -= drawOrigin;
        rootDrawer.worldPosition += drawOffset;
        spriteBatch.Draw(rootDrawer);

        SpritebatchDrawer bellDrawer = SpritebatchDrawer.FromNPC(NPC);
        bellDrawer.drawOrigin = drawOrigin;
        bellDrawer.VerticalFrame(1, Main.npcFrameCount[Type]);
        bellDrawer.worldPosition -= drawOrigin;
        bellDrawer.worldPosition += drawOffset;
        spriteBatch.Draw(bellDrawer);

        var outlineDrawer = bellDrawer;
        outlineDrawer.color = Color.Gold * ExtraMath.Osc(0.5f, 1f, speed: 6);
        outlineDrawer.VerticalFrame(2, Main.npcFrameCount[Type]);
        spriteBatch.Draw(outlineDrawer);

        if (IsActive())
        {
            SpritebatchDrawer beamDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, NPC.Bottom);
            beamDrawer.color = Color.Goldenrod * ExtraMath.Osc(0.5f, 1f, speed: 3) * PulseTime;
            beamDrawer.color.A = 0;
            beamDrawer.BottomCenterOrigin();
            spriteBatch.Draw(beamDrawer);
        }
        /*
        bellDrawer.color *= 0.3f;
        bellDrawer.drawOrigin = drawOrigin;
        bellDrawer.VerticalFrame(2, Main.npcFrameCount[Type]);
        bellDrawer.scale *= ExtraMath.Osc(1.2f, 1.4f);
        spriteBatch.Draw(bellDrawer);
        */

        //Godrays here
        Asset<Texture2D> godrayTexture = AssetManager.GlowMask.SimpleGlowCircle;
        Vector2 origin = godrayTexture.Size() * 0.5f;
        Vector2 godrayScale = new Vector2(0.35f, 2f);

        float godrayOffset = 150;
        var sbDrawer2 = SpritebatchDrawer.FromTextureAsset(godrayTexture, NPC.Center);
        sbDrawer2.color = Color.White * 0.2f * ExtraMath.Osc(0f, 1f, speed: 1);
        sbDrawer2.color.A = 0;
        sbDrawer2.scale *= godrayScale;
        sbDrawer2.worldPosition += drawOffset;
        sbDrawer2.worldPosition.Y -= godrayOffset;
        spriteBatch.Draw(sbDrawer2);
        DrawGoldenPulse(spriteBatch);
        return false;
    }
    public override void OnKill()
    {
        base.OnKill();
    }

}
public class CheckpointBellTile : ModTile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        LocalizedText name = CreateMapEntryName();

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.Height = 1;
        TileObjectData.newTile.Width = 1;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        // MyTileEntity refers to the tile entity mentioned in the previous section
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<CheckpointBellTileEntity>().Hook_AfterPlacement, -1, 0, true);
        // This is required so the hook is actually called.
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.addTile(Type);


        AddMapEntry(new Color(178, 220, 190), name);
        MineResist = 1f;
        MinPick = 225;
        // These 2 AddMapEntry and GetMapOption show off multiple Map Entries per Tile. Delete GetMapOption and all but 1 of these for your own ModTile if you don't actually need it.
        AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap")); // localized text for "Trap"
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {

        return base.PreDraw(i, j, spriteBatch);
    }
    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        base.KillMultiTile(i, j, frameX, frameY);
        // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
        // The tile coordinate parameters already refer to the top-left corner of the multitile
        ModContent.GetInstance<CheckpointBellTileEntity>().Kill(i, j);
    }
    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
        // The tile coordinate parameters already refer to the top-left corner of the multitile
        ModContent.GetInstance<CheckpointBellTileEntity>().Kill(i, j);
    }
    public override bool IsTileDangerous(int i, int j, Player player) => true;


    // PlaceInWorld is needed to facilitate styles and alternates since this tile doesn't use a TileObjectData. Placing left and right based on player direction is usually done in the TileObjectData, but the specifics of that don't work for how we want this tile to work. 
    public override void PlaceInWorld(int i, int j, Item item)
    {
        int style = Main.LocalPlayer.HeldItem.placeStyle;
        Tile tile = Main.tile[i, j];
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        }
    }
}

