using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.Pixelation;
using Stellamod.Core.TriggersSystem.Triggers;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.CariyaBoss;

public class CariyaSitting : VeilTownNPC,
        INPCSpawnCondition
{
    private int _frame;
    private ref float Timer => ref NPC.ai[0];
    private ref float State => ref NPC.ai[1];
    private Chain _chain;
    private Chain Chain
    {
        get
        {
            if (_chain == null)
            {

                _chain = new Chain(NPC.Center, 2, 128);
            }
            return _chain;
        }
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 11;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 50;
        NPC.defense = 15;
        NPC.lifeMax = 6000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.dontCountMe = true;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.scale = 1f;
        NPC.aiStyle = -1;
        HasTownDialogue = true;
        OnlyInteract = true;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        if(State == 1 && _frame < 10)
        {
            NPC.frameCounter += 0.15f;
            if(NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0;
            }
        }
        NPC.frame.Y = _frame * frameHeight;
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case 0:
                AI_Sit();
                break;
            case 1:
                AI_Stand();
                break;
        }


        if (DownedBossTracker.IsDowned(DownedBossFlag.Cariya) ||
            NPC.AnyNPCs(ModContent.NPCType<Cariya>()))
        {
            NPC.active = false;
        }
        SimulateHair();
    }


    private void AI_Sit()
    {

    }

    private void AI_Stand()
    {
        Timer++;
    }

    public override bool CheckActive()
    {
        return false;
    }

    private void StartDialogue()
    {
        DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
        dialogueSystem.StartDialogueSequence(ModContent.GetInstance<CariyaStartDialogue>());
    }
    #region Hair Rendering
    private void SimulateHair()
    {
        Chain.points[0] = NPC.Center;
        Chain.points[0].Y -= 4 + ExtraMath.Osc(0f, 16, speed: 2);
        Chain.pinned[0] = true;

        for (int i = 0; i < 6; i++)
        {
            Chain.points[i].Y += ExtraMath.Osc(-8f, 8f, speed: 0.5f, offset: i);
        }
        for (int i = 0; i < Chain.points.Length; i++)
        {
            Chain.points[i].Y += MathHelper.Lerp(0.2f, 1f, (float)i / (float)Chain.points.Length);
        }
        for (int i = 0; i < 32; i++)
        {
            Chain.ResolveBackToRoot();
        }
    }
    private float GetHairWidth(float ratio)
    {
        return MathHelper.SmoothStep(48, 0, ratio) * EasingFunction.QuadraticBump(ratio);
    }
    private Color GetHairColor(float ratio)
    {
        return Color.DarkGray * EasingFunction.OutExpo(ratio + 0.5f);
    }

    private void DrawHair(GraphicsDevice gDevice)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        TrailDrawer.Draw(Main.spriteBatch, Chain.points, GetHairColor, GetHairWidth, shader);
    }
    #endregion
    public override void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        if (!_drawOutlines)
            return;

        //base.DrawOutlines(spriteBatch, screenPos, lightColor);
        OutlineRenderer.Queue(DrawWhite);
        _drawOutlines = false;
    }

    public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
        //SO multiple people can't open the dialogue at the same time
        if (State == 1)
            return;

        StartDialogue();
        Main.CloseNPCChatOrSign();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.BehindNPCsWithOutline);
        DrawSprite(spriteBatch);
       
        return false;
    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        DrawSprite(spriteBatch);
    }
    private void DrawSprite(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromNPC(NPC);
        sbDrawer.drawOrigin = new Vector2(99, 140);
        sbDrawer.worldPosition = NPC.Bottom;
        spriteBatch.Draw(sbDrawer);
    }

    public bool CanSpawn()
    {
        return !DownedBossTracker.IsDowned(DownedBossFlag.Cariya)
            && !NPC.AnyNPCs(ModContent.NPCType<CariyaSitting>())
            && !NPC.AnyNPCs(ModContent.NPCType<Cariya>());
    }
}
