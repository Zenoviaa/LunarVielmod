using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.NPCs;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public class GothiviaIdle : VeilTownNPC
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 61;
    }
    private ref float Timer => ref NPC.ai[0];
    public bool ThreeQ = false;
    public bool FourQ = false;
    public bool NoWings = false;

    // Current state
    public float squish = 0f;
    private int _wingFrameCounter;
    private int _wingFrameTick;

    // Current frame
    public int frameCounter;
    // Current frame's progress
    public int frameTick;
    // Current state's timer
    public float timer;

    public override void SetDefaults()
    {
        // Sets NPC to be a Town NPC
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 106;
        NPC.height = 92;
        NPC.aiStyle = -1;
        NPC.damage = 90;
        NPC.defense = 42;
        NPC.lifeMax = 9000;
        NPC.knockBackResist = 0.5f;
        NPC.npcSlots = 0;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        SpawnAtPoint = true;
    }
    public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
    {
        base.SetPointSpawnerDefaults(ref spawner);
        spawner.isGlobal = true;
        StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
        Point spawnPoint = stellaWorld.MarshLocation + stellaWorld.GothiviaSpawnOffset;
        spawner.spawnTileOffset = spawnPoint;
    }
    public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
        //Set buttons
        buttons.Add(new Tuple<string, Action>("Talk", Talk));

        portrait = "ZuiPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiOpenDialogue1";
    }

    public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
    {
        base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
        portrait = "ZuiPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiIdleChat1";


    }
    public override void FindFrame(int frameHeight)
    {
        /*
        NPC.frameCounter += 1f;
        NPC.frameCounter %= Main.npcFrameCount[NPC.type];
        int frame = (int)NPC.frameCounter;
        NPC.frame.Y = frame * frameHeight;*/
    }

    public override bool CanChat()
    {
        return true;
    }
    //This prevents the NPC from despawning
    public override bool CheckActive()
    {
        return false;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "You sense a strange godly prescence coming from Gothivia")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "???", "2"))
        });
    }


    public override string GetChat()
    {
        WeightedRandom<string> chat = new WeightedRandom<string>();

        int partyGirl = NPC.FindFirstNPC(NPCID.Steampunker);

        // These are things that the NPC has a chance of telling you when you talk to it.
        chat.Add("...");
        chat.Add(LangText.Chat(this, "Basic1"));
        chat.Add(LangText.Chat(this, "Basic2"));
        chat.Add(LangText.Chat(this, "Basic3"), 1.0);
        chat.Add(LangText.Chat(this, "Basic4"), 1.0);


        return chat; // chat is implicitly cast to a string.
    }

    public override List<string> SetNPCNameList()
    {
        return new List<string>() {
            "Gothivia The Enraged",
            "Gothivia The Enraged"
        };
    }

    // The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
    // Returning false will allow you to manually draw your NPC
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        Vector2 size = new Vector2(166, 96);


        Player player = Main.player[NPC.target];
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        SpriteEffects effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Rectangle rect;


        Vector2 drawPosition = NPC.Center - screenPos;
        Vector2 origin = new Vector2(83, 48);
        Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/Gwings4Q").Value;
        int wingFrameSpeed = 1;
        int wingFrameCount = 60;
        spriteBatch.Draw(syliaWingsTexture, drawPosition,
            syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
            drawColor, NPC.rotation, origin, 2f, effects, 0f);

        rect = new(0, 16 * 96, 166, 7 * 96);
        spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 7, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
        return false;
    }

    public override void AI()
    {
        Timer++;
        NPC.TargetClosest();

        float yVelocity = MathF.Sin(Timer * 0.05f);
        NPC.velocity.Y = yVelocity;
        NPC.spriteDirection = NPC.direction;
        Player target = Main.player[NPC.target];

        if (NPC.AnyNPCs(ModContent.NPCType<Gothivia>()))
        {
            NPC.active = false;
        }

        Vector3 RGB = new(2.30f, 2.21f, 2.72f);
        Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
    }
}