using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.Tundra.Abyss.AccAB;
using Stellamod.Content.Dialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.Particles;
using Stellamod.Core.TriggersSystem.Triggers;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;

public class VerliaIdle : VeilTownNPC,
         INPCSpawnCondition
{
    private Asset<Texture2D> _wingTextureAsset;
    private Asset<Texture2D> _wingOutlineTextureAsset;
    private Asset<Texture2D> _wingTextureAsset2;
    private ref float Timer => ref NPC.ai[0];
    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[Type] = 3;
        Main.npcFrameCount[NPC.type] = 2;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.ActsLikeTownNPC[Type] = true;
        NPCID.Sets.SpawnsWithCustomName[Type] = true;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 32;
        NPC.height = 100;
        NPC.damage = 32;
        NPC.defense = 0;
        NPC.lifeMax = 1100;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.npcSlots = 10f;
        NPC.aiStyle = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.BossBar = Main.BigBossProgressBar.NeverValid;
        NPC.dontTakeDamageFromHostiles = true;
        HasTownDialogue = true;
    }


    public override void SetChatButtons(ref string button, ref string button2)
    { // What the chat buttons are when you open up the chat UI
        button2 = Language.GetTextValue("LegacyInterface.28");
        button = LangText.Chat(this, "Button");
    }


    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += 0.07f;
        NPC.frameCounter %= Main.npcFrameCount[NPC.type];
        int frame = (int)NPC.frameCounter;
        NPC.frame.Y = frame * frameHeight;
    }

    public override bool CheckActive()
    {
        return false;
    }
    public override List<string> SetNPCNameList()
    {
        return new List<string>() {
                "Verlia of the Moon",
            };
    }


    public override void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        if (!_drawOutlines)
            return;

        //base.DrawOutlines(spriteBatch, screenPos, lightColor);
        OutlineRenderer.Queue(DrawWhite);
        _drawOutlines = false;
    }
    private void DrawWhite(SpriteBatch spriteBatch)
    {
        DrawSprite(spriteBatch, Vector2.Zero, Color.White);
    }


    public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
        //Set buttons
        buttons.Add(new Tuple<string, Action>("Talk", Talk));
        buttons.Add(new Tuple<string, Action>("Shop", OpenShop));
        buttons.Add(new Tuple<string, Action>("Kill", KillYourFriend));

        portrait = "VerliaPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "VerliaOpenDialogue";
    }

    public override void Talk()
    {
        base.Talk();
        OpenTalkOptions(
            ModContent.GetInstance<VerliaHappenedDialogue>(),
            ModContent.GetInstance<VerliaFamilyDialogue>(),
            ModContent.GetInstance<VerliaWingsDialogue>());
    }

    private void KillYourFriend()
    {
        DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
        dialogueSystem.StartDialogueSequence(ModContent.GetInstance<VerliaKillDialogue>());
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A traveller of the lands who may hold great power")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Zui the Traveller", "2"))
            });
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            var npc = NPC;
            float numDust = 16;
            for (int n = 0; n < numDust; n++)
            {
                Vector2 position = npc.position;
                position.X += Main.rand.Next(0, npc.width);
                position.Y += Main.rand.Next(0, npc.height);

                var smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(position, -Vector2.UnitY, Color.White, Scale: Main.rand.NextFloat(0.66f, 1.75f));
                smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.14f);
                smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                smokeParticle.fadeToColor = Color.Black;
            }
        }

        Vector2 velocity = new Vector2();
        velocity.Y = MathF.Sin(Timer * 0.05f) * 0.2f;
        NPC.velocity = velocity;
        NPC.spriteDirection = NPC.direction;
        if (NPC.AnyNPCs(ModContent.NPCType<Verlia>()) ||
            DownedBossTracker.IsDowned(DownedBossFlag.Verlia) ||
            !DownedBossTracker.IsDowned(DownedBossFlag.VerliaPrison))
        {
            NPC.active = false;
        }
    }
    public override void AddShops()
    {
        var npcShop = new NPCShop(Type, "Shop")
         .Add(new Item(ModContent.ItemType<MoonFlight>())
         {
             shopCustomPrice = 30,
             shopSpecialCurrency = Stellamod.MedalCurrencyID
         });
        npcShop.Register();
    }
    public bool CanSpawn()
    {
        if (NPC.AnyNPCs(ModContent.NPCType<VerliaIdle>()))
            return false;
        if (NPC.AnyNPCs(ModContent.NPCType<Verlia>()))
            return false;
        if (DownedBossTracker.IsDowned(DownedBossFlag.Verlia))
            return false;
        if (!DownedBossTracker.IsDowned(DownedBossFlag.VerliaPrison))
            return false;
        return true;
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawWings(spriteBatch, screenPos, drawColor);
        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        return false;
    }


    private float HeightOffset
    {
        get
        {
            return 0;
        }
    }

    private float WingOffset
    {
        get
        {
            return -24;
        }
    }

    private float WingVelocity
    {
        get
        {
            return 15;
        }
    }
    private Vector2 LeftWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.6f, EasingFunction.Clamp(NPC.velocity.X / -WingVelocity));
            //   leftWingScale *= _wingScale;
            return leftWingScale;
        }
    }
    private float LeftWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(12), EasingFunction.Clamp(NPC.velocity.X / -WingVelocity));
            return rot;
        }
    }
    private Vector2 RightWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.6f, EasingFunction.Clamp(NPC.velocity.X / WingVelocity));
            //   leftWingScale *= _wingScale;
            return leftWingScale;
        }
    }
    private float RightWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(-12), EasingFunction.Clamp(NPC.velocity.X / -WingVelocity));
            return rot;
        }
    }

    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.sourceRect = NPC.frame;

        Vector2 drawOrigin = new Vector2(100, 114);
        drawer.drawOrigin = drawOrigin;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor;
        drawer.worldPosition += screenPos;
        spriteBatch.Draw(drawer);
    }

    private void DrawWings_Inner(SpriteBatch spriteBatch)
    {

        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.color = Color.DeepSkyBlue * 0.4f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.Y *= 0.66f;
        glowDrawer.scale *= 0.6f;
        //   glowDrawer.scale *= _wingScale;
        glowDrawer.rotation = MathHelper.ToRadians(degrees);

        glowDrawer.drawOrigin = new Vector2(AssetManager.GlowMask.SimpleGlowCircle.Width() * 0.2f, AssetManager.GlowMask.SimpleGlowCircle.Height() * 0.5f);
        spriteBatch.Draw(glowDrawer);



        glowDrawer.rotation = MathHelper.ToRadians(-degrees);
        glowDrawer.drawOrigin.X = glowDrawer.texture.Size().X - glowDrawer.drawOrigin.X;
        //    glowDrawer.drawOrigin = new Vector2(AssetManager.GlowMask.SimpleGlowCircle.Width() * 0.2f, AssetManager.GlowMask.SimpleGlowCircle.Height() * 0.5f);
        spriteBatch.Draw(glowDrawer);


        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        // wingDrawer.drawOrigin.X = WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.DarkBlue;

        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.scale = LeftWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);
    }

    private void DrawWings(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        string texture = ModContent.GetInstance<Verlia>().Texture;
        _wingTextureAsset ??= ModContent.Request<Texture2D>(texture + "_Wing");
        _wingOutlineTextureAsset ??= ModContent.Request<Texture2D>(texture + "_WingOutline");
        _wingTextureAsset2 ??= ModContent.Request<Texture2D>(texture + "_WingSprite");

        VerlianWingsShader wingShader = VerlianWingsShader.Instance;
        wingShader.BloomColorStart = Color.White;
        wingShader.BloomColorEnd = Color.Lerp(Color.Lerp(Color.Blue, Color.Black, 0.5f), Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 2));
        wingShader.PerlinNoiseTexture = AssetManager.Noise.Whirly.Value;
        wingShader.ScrollingTexture = TrailRegistry.WaterTrail.Value;
        wingShader.DistortionStrength = 0.15f;
        wingShader.MaskSize = _wingTextureAsset.Size();
        wingShader.Frequency = 1f;
        wingShader.Tiling = Vector2.One * 2.5f;
        wingShader.ScrollOffset = new Vector2(-Main.GlobalTimeWrappedHourly * 0.4f, 0.0f);

        //     DrawWings_Inner2(spriteBatch);

        DrawWings_Inner(spriteBatch);
        //return;

        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        spriteBatch.Restart(effect: wingShader.Effect, sortMode: SpriteSortMode.Immediate);

        SpritebatchDrawer wingDrawer;

        //Draw main wings
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.5f) * 0.5f;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        wingDrawer.scale = RightWingScale;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.scale = LeftWingScale;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);

        //Draw stars in wings
        spriteBatch.Restart(effect: wingShader.Effect, sortMode: SpriteSortMode.Immediate);
        wingShader.Tiling = Vector2.One * 16f;
        wingShader.ScrollingTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise2").Value;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.35f) * 0.6f;
        wingDrawer.color.A = 0;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);



        wingShader.BloomColorEnd = Color.White;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingOutlineTextureAsset, NPC.Center);

        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.White;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);

        spriteBatch.RestartDefaults();

    }
}