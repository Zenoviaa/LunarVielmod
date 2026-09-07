using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Core.Godrays;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.RibbonSystem;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
// ModMapLayers are used to draw icons and other things over the map. Pylons and spawn/bed icons are examples of vanilla map layers. This example adds an icon over the dungeon.
public class BellFlowerMapLayer : ModMapLayer
{
    // Here you can define where to put this layer in the vanilla map layer order.
    // In this case we go before the pings layer since all other vanilla map layers are ordered before pings,
    public override Position GetDefaultPosition() => new Before(IMapLayer.Pings);

    // In the Draw method, we draw everything. Consulting vanilla examples in the source code is a good resource for properly using this Draw method.
    public override void Draw(ref MapOverlayDrawContext context, ref string text)
    {
        // Here we define the scale that we wish to draw the icon when hovered and not hovered.
        const float scaleIfNotSelected = 1f;
        const float scaleIfSelected = scaleIfNotSelected * 2f;


        // Here we retrieve the texture of the Skeletron boss head so that we can draw it. Remember that not all textures are loaded by default, so you might need to do something like `Main.instance.LoadItem(ItemID.BoneKey);` in your code to ensure the texture is loaded.
        var bellFlowerTexture = AssetReferences.Content.Areas.Tundra.Abyss.EnemiesAB.BellFlower_MapIcon.Asset;
        foreach(PlacedBellFlower flower in BellFlowerSystem.BellFlowers)
        {
            byte frame = 0;
            if (flower.rung)
                frame = 1;
            /*
            if (!flower.discovered)
                continue;*/
            // The MapOverlayDrawContext.Draw method used here handles many of the small details for drawing an icon and should be used if possible. It'll handle scaling, alignment, culling, framing, and accounting for map zoom. Handling these manually is a lot of work.
            // Note that the `position` argument expects tile coordinates expressed as a Vector2. Don't scale tile coordinates to world coordinates by multiplying by 16.
            // The return of MapOverlayDrawContext.Draw has a field that indicates if the mouse is currently over our icon.
            if (context.Draw(bellFlowerTexture.Value, flower.spawnPosition.ToTileCoordinates().ToVector2(), Color.White,
                new SpriteFrame(1, 2, 0, frame), scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver)
            {
                // When the icon is being hovered by the users mouse, we set the mouse text to the localized text for "The Dungeon"
                text = LangText.Common("BellFlower");
            }
        }
    }
}
public class PlacedBellFlowerSerializer : TagSerializer<PlacedBellFlower, TagCompound>
{
    public override PlacedBellFlower Deserialize(TagCompound tag)
    {
        Vector2 position = tag.Get<Vector2>("pos");
        bool discovered = tag.Get<bool>("discovered");
        bool rung = tag.Get<bool>("rung");
        float activeTimer = tag.Get<float>("activeTimer");
        return new PlacedBellFlower
        {
            spawnPosition = position,
            discovered = discovered,
            rung = rung,
            activeTimer = activeTimer
        };
    }

    public override TagCompound Serialize(PlacedBellFlower value)
    {
        return new TagCompound
        {
            ["pos"] = value.spawnPosition,
            ["discovered"] = value.discovered,
            ["rung"] = value.rung,
            ["activeTimer"] = value.activeTimer
        };
    }
}
public class PlacedBellFlower
{
    public Vector2 spawnPosition;
    public bool discovered;
    public bool rung;
    public float activeTimer;
}

public class BellFlowerSystem : ModSystem
{
    public static readonly List<PlacedBellFlower> BellFlowers = new();
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (MultiplayerHelper.IsHost)
        {
            bool anyInAbyss = false;
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.InModBiome<AbyssBiome>())
                {
                    anyInAbyss = true;
                    break;
                }
            }

            if (!anyInAbyss)
                return;

            int i = 0;
            foreach (PlacedBellFlower bellFlower in BellFlowers)
            {
                bellFlower.activeTimer--;
                if (bellFlower.activeTimer <= 0)
                {
                    NPC.NewNPC(new EntitySource_Misc(""),
                        (int)bellFlower.spawnPosition.X,
                        (int)bellFlower.spawnPosition.Y,
                        ModContent.NPCType<BellFlower>(),
                        ai2: i);
                    bellFlower.activeTimer = 30;
                }
                i++;
            }
        }


    }
    public static void ClearBellFlowers()
    {
        BellFlowers.Clear();
    }
    public static void CreateBellFlower(Point tilePoint)
    {
        PlacedBellFlower bellFlower = new PlacedBellFlower
        {
            spawnPosition = tilePoint.ToWorldCoordinates(),
            activeTimer = 0,
            discovered = false,
            rung = false
        };
        PlaceBellFlower(bellFlower);
    }

    public static void PlaceBellFlower(PlacedBellFlower bellFlower)
    {
        BellFlowers.Add(bellFlower);
    }

    public static void DiscoverBellFlower(int index)
    {
        if (BellFlowers.Count <= index)
            return;
        BellFlowers[index].discovered = true;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
    public static void RingBellFlower(int index)
    {
        if (BellFlowers.Count <= index)
            return;
        BellFlowers[index].rung = true;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.Write(BellFlowers.Count);
        foreach(PlacedBellFlower flower in BellFlowers)
        {
            writer.WriteVector2(flower.spawnPosition);
            writer.Write(flower.discovered);
            writer.Write(flower.rung);
            writer.Write(flower.activeTimer);
        }
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        int bCount = reader.ReadInt32();
        BellFlowers.Clear();

        for(int i = 0; i < bCount; i++)
        {
            Vector2 pos = reader.ReadVector2();
            bool disc = reader.ReadBoolean();
            bool run = reader.ReadBoolean();
            float timer = reader.ReadSingle();
            BellFlowers.Add(new PlacedBellFlower
            {
                spawnPosition = pos,
                discovered = disc,
                rung = run,
                activeTimer = timer
            });
        }
    }
    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        tag["bellFlowers"] = BellFlowers;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        List<PlacedBellFlower> bellFlowers = tag.Get<List<PlacedBellFlower>>("bellFlowers");
        BellFlowers.Clear();
        if (bellFlowers != null)
            BellFlowers.AddRange(bellFlowers);
    }
}

public class BellFlower : ModNPC
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

    private int Index
    {
        get => (int)NPC.ai[2];
        set => NPC.ai[2] = value;
    }

    public const float TRIGGER_RANGE = 256 * 256;
    private Player MyTarget => Main.player[NPC.target];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 2;
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
        NPC.height = 64;
        NPC.lifeMax = 100;
        NPC.damage = 1;
        NPC.defense = 9999;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.knockBackResist = 0;
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();

        if (BellFlowerSystem.BellFlowers.Count > Index)
        {
            PlacedBellFlower placed = BellFlowerSystem.BellFlowers[Index];
            float dist = Vector2.Distance(NPC.Center, placed.spawnPosition);
            if (dist > 500)
                NPC.active = false;
            placed.activeTimer = 30;
        }
        else
        {
            NPC.active = false;
        }
        if (Main.rand.NextBool(100) && Main.hasFocus && Main.netMode != NetmodeID.Server)
        {
            GodrayRenderer godrayRenderer = ModContent.GetInstance<GodrayRenderer>();
            Vector2 centerPos = NPC.Center;

           
            godrayRenderer.AddGodrayParticle(centerPos + Main.rand.NextVector2Circular(64, 64));
        }

        if (Main.rand.NextBool(16))
        {
            var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(72, 72), Vector2.Zero);
            sp.gravity = 0;
            sp.Scale *= 0.5f;
            sp.fast = true;
            sp.outerColor = Color.Blue;
        }
        Vector2 ground = MovementUtilities.FindFloorWet(NPC.Center);
        ground.Y -= 64;
        NPC.velocity = ground - NPC.Center;
        if (Main.rand.NextBool(32))
        {
            var d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, Scale: 0.8f);
            d.noGravity = true;
        }
        NPC.TargetClosest();
        NPC.life = NPC.lifeMax;
        float distanceSquared = Vector2.DistanceSquared(NPC.Center, MyTarget.Center);
        NPC.dontTakeDamage = distanceSquared > TRIGGER_RANGE;
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

    private void AI_Ring()
    {
        Timer++;
        if(Timer == 1)
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.TintScreen(Color.White, 0.12f, 45);
            BellFlowerSystem.RingBellFlower(Index);
            var sound = AssetReferences.Assets.Sounds.TheWorld.Asset with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound);
        }
        if(Timer % 10 == 0)
        {
            Particles.RoarDust.Spawn(RoarDustData.Default with { position = NPC.Center, timeLeft = 24 });
        }

        float range = MathHelper.Lerp(0.25f, 0f, Timer / 60f);
        NPC.rotation = ExtraMath.Osc(-range, range, speed: 6);
        ShakeScreenPosition.Shake = MathHelper.Lerp(4, 0, Timer / 60f);
        if(Timer >= 60)
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

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 drawOrigin = new Vector2(42, 32);
        Vector2 drawOffset = new Vector2(-16, 0);
        SpritebatchDrawer rootDrawer = SpritebatchDrawer.FromNPC(NPC);
        rootDrawer.VerticalFrame(0, 2);
        rootDrawer.drawOrigin = drawOrigin;
        rootDrawer.rotation = 0;
        rootDrawer.worldPosition -= drawOrigin;
        rootDrawer.worldPosition += drawOffset;
        spriteBatch.Draw(rootDrawer);

        SpritebatchDrawer bellDrawer = SpritebatchDrawer.FromNPC(NPC);
        bellDrawer.drawOrigin = drawOrigin;
        bellDrawer.VerticalFrame(1, 2);
        bellDrawer.worldPosition -= drawOrigin;
        bellDrawer.worldPosition += drawOffset;
        spriteBatch.Draw(bellDrawer);

        bellDrawer.color = Color.White * ExtraMath.Osc(0.32f, 0.6f, speed: 2);
        bellDrawer.color.A = 0;
        spriteBatch.Draw(bellDrawer);
        DrawUtilities.DrawBasicGlow(spriteBatch, NPC.Center, 0.4f, Color.Blue * 0.4f * ExtraMath.Osc(0.8f, 1f, speed: 2));
        DrawUtilities.DrawBasicGlow(spriteBatch, NPC.Center, 1.5f, Color.Blue * 0.4f * ExtraMath.Osc(0.8f, 1f, speed: 2));


        //Godrays here
        Asset<Texture2D> godrayTexture = AssetManager.GlowMask.SimpleGlowCircle;
        Vector2 origin = godrayTexture.Size() * 0.5f;
        Vector2 godrayScale = new Vector2(0.35f, 2f);

        float godrayOffset = 150;
        SpritebatchDrawer sbDrawer2 = SpritebatchDrawer.FromTextureAsset(godrayTexture, NPC.Center);
        sbDrawer2.color = Color.White * 0.2f * ExtraMath.Osc(0f, 1f, speed: 1) * LightingHelper.DayLightEase;
        sbDrawer2.color.A = 0;
        sbDrawer2.rotation -= MathHelper.ToRadians(25);
        sbDrawer2.scale *= godrayScale;
        sbDrawer2.worldPosition.Y -= godrayOffset;
        sbDrawer2.worldPosition.X -= 64;
        Main.spriteBatch.Draw(sbDrawer2);

        sbDrawer2.color = Color.White * 0.2f * ExtraMath.Osc(0f, 1f, speed: 1, offset: 1) * LightingHelper.DayLightEase;
        sbDrawer2.color.A = 0;
        sbDrawer2.worldPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 1) * 64;
        sbDrawer2.worldPosition.Y -= godrayOffset;
        sbDrawer2.worldPosition.X -= 64;
        Main.spriteBatch.Draw(sbDrawer2);
        return false;
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}
