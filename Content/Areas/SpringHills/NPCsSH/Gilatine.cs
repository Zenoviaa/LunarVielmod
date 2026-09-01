using ReLogic.Content;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class DragonSegment
{
    private DragonSegment _parent;
    public DragonSegment(int segmentLength, float initialAngle = 0)
    {
        length = segmentLength;
        angle = initialAngle;
        //children = new List<DragonSegment>();
        children = new List<DragonSegment>();
    }
    public DragonSegment parent
    {
        get
        {
            return _parent;
        }
        set
        {
            if (_parent != null)
                _parent.children.Remove(this);

            _parent = value;
            if (_parent != null)
                _parent.children.Add(this);
        }
    }
    public List<DragonSegment> children;
    public Vector2 a;
    public Vector2 b;
    public float angle;
    public float length;
    public float totalAngle
    {
        get
        {
            if (_parent != null)
                return _parent.totalAngle + angle;
            return angle;
        }
    }
    public Vector2 Center => (a + b) * 0.5f;
}

public class DragonRig
{
    public DragonSegment root;
    public List<DragonSegment> segments;
    public DragonRig()
    {
        segments = new List<DragonSegment>();
    }

    public void AddSegment(DragonSegment segment)
    {
        segments.Add(segment);
    }

    public void ResolveFK(Vector2 rootPosition)
    {
        root.a = rootPosition;
        root.b = root.a + root.angle.ToRotationVector2() * root.length;
        ResolveInner(root);
    }

    public void ResolveInner(DragonSegment child)
    {
        if (child.parent != null)
            child.a = child.parent.b;

        child.b = child.a + child.totalAngle.ToRotationVector2() * child.length;
        foreach (DragonSegment innerChild in child.children)
        {
            ResolveInner(innerChild);
        }
    }
}
public class Gilatine : VeilTownNPC
{


    private bool _initialized;
    private DragonRig _rig;


    private Asset<Texture2D> _headTextureAsset;
    private Asset<Texture2D>[] _bodyTextureAssets;
    private Asset<Texture2D>[] _frontLegTextureAssets;
    private Asset<Texture2D>[] _backLegTextureAssets;
    private Asset<Texture2D>[] _wingTextureAssets;

    private DragonSegment _headSegment;
    private DragonSegment[] _bodySegments;
    //   private DragonSegment[] _frontLegSegments;
    //  private DragonSegment[] _backLegSegments;
    //   private DragonSegment[] _wingSegments;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 1;
    }

    private void LoadTextureAssets()
    {
        if (_initialized)
            return;
        _headTextureAsset = ModContent.Request<Texture2D>(Texture + "_Head");
        _bodyTextureAssets = new Asset<Texture2D>[5];
        for (int i = 0; i < _bodyTextureAssets.Length; i++)
        {
            _bodyTextureAssets[i] = ModContent.Request<Texture2D>(Texture + "_Body_" + i);
        }
        _frontLegTextureAssets = new Asset<Texture2D>[2];
        for (int i = 0; i < _frontLegTextureAssets.Length; i++)
        {
            _frontLegTextureAssets[i] = ModContent.Request<Texture2D>(Texture + "_FrontLeg_" + i);
        }
        _backLegTextureAssets = new Asset<Texture2D>[2];
        for (int i = 0; i < _backLegTextureAssets.Length; i++)
        {
            _backLegTextureAssets[i] = ModContent.Request<Texture2D>(Texture + "_BackLeg_" + i);
        }
        _wingTextureAssets = new Asset<Texture2D>[2];
        for (int i = 0; i < _wingTextureAssets.Length; i++)
        {
            _wingTextureAssets[i] = ModContent.Request<Texture2D>(Texture + "_Wing_" + i);
        }
        _initialized = true;
    }

    private void SetupRig()
    {
        _rig = new DragonRig();

        //Setup head
        _headSegment = new DragonSegment(segmentLength: 48);
        _rig.AddSegment(_headSegment);
        _rig.root = _headSegment;

        //Setup body
        _bodySegments = new DragonSegment[5];
        int[] bodyWidths = new int[5];
        bodyWidths[0] = 30;
        bodyWidths[1] = 20;
        bodyWidths[2] = 20;
        bodyWidths[3] = 8;
        bodyWidths[4] = 20;
        for (int i = 0; i < _bodySegments.Length; i++)
        {
            DragonSegment bodySegment = new DragonSegment(segmentLength: bodyWidths[i]);
            if (i == 0)
            {
                bodySegment.parent = _headSegment;
            }
            else
            {
                bodySegment.parent = _bodySegments[i - 1];
            }
            _rig.AddSegment(bodySegment);
            _bodySegments[i] = bodySegment;
        }

        //_frontLegSegments
    }
    public override void SetDefaults()
    {
        // Sets NPC to be a Town NPC
        SetupRig();
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 32;
        NPC.height = 32;
        NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
        NPC.damage = 90;
        NPC.defense = 42;
        NPC.lifeMax = 200;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.dontTakeDamageFromHostiles = true;
        SpawnAtPoint = true;
        HasTownDialogue = true;
    }

    public override bool CheckActive()
    {
        return false;
    }

    private void ResolveKinematics()
    {
        _rig.ResolveFK(NPC.Center);
        _headSegment.angle = (-Vector2.UnitX).ToRotation();
    }

    private Vector2 Breathe(float offset)
    {
        Vector2 breathScale = Vector2.Lerp(Vector2.One * 1f, Vector2.One * 1.1f, ExtraMath.Osc(0f, 1f, speed: 2, offset: offset));
        return breathScale;
    }

    private float WingBreathe(float offset)
    {
        float range = MathHelper.ToRadians(5);
        float radians = MathHelper.Lerp(-range, range, ExtraMath.Osc(0f, 1f, speed: 1, offset: offset));
        return radians;
    }

    private float _headAngle;
    private void DrawSegments(SpriteBatch spriteBatch, Vector2 offset)
    {
        //BACK WING

        SpritebatchDrawer backWingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAssets[1], _bodySegments[0].a);
        backWingDrawer.drawOrigin = new Vector2(85, 116);
        backWingDrawer.rotation = _bodySegments[0].totalAngle - MathHelper.Pi;
        float wingAngle2 = MathHelper.WrapAngle(backWingDrawer.rotation + MathHelper.PiOver2);
        if (wingAngle2 < 0)
        {
            backWingDrawer.spriteEffects = SpriteEffects.FlipVertically;
            backWingDrawer.drawOrigin = new Vector2(114, _wingTextureAssets[1].Height() - 116);
        }
        backWingDrawer.scale = Breathe(0);
        backWingDrawer.rotation += WingBreathe(0);
        backWingDrawer.worldPosition += offset;
        spriteBatch.Draw(backWingDrawer);

        //BACK LEG BACK
        SpritebatchDrawer backLegDrawerBack = SpritebatchDrawer.FromTextureAsset(_backLegTextureAssets[1], _bodySegments[2].a);
        backLegDrawerBack.rotation = _bodySegments[2].totalAngle - MathHelper.Pi;
        backLegDrawerBack.drawOrigin = Vector2.Zero;

        float backLegAngle = MathHelper.WrapAngle(backLegDrawerBack.rotation + MathHelper.PiOver2);
        if (backLegAngle < 0)
        {
            backLegDrawerBack.spriteEffects = SpriteEffects.FlipVertically;
            backLegDrawerBack.drawOrigin = new Vector2(0, _backLegTextureAssets[1].Height() - 0);
        }
        backLegDrawerBack.scale = Breathe(1);
        backLegDrawerBack.worldPosition += offset;
        spriteBatch.Draw(backLegDrawerBack);

        //BACK LEG FRONT
        SpritebatchDrawer frontLegDrawerBack = SpritebatchDrawer.FromTextureAsset(_frontLegTextureAssets[1], _bodySegments[0].a);
        frontLegDrawerBack.drawOrigin = Vector2.Zero;
        frontLegDrawerBack.rotation = _bodySegments[0].totalAngle - MathHelper.Pi;
        float frontLegAngle = MathHelper.WrapAngle(frontLegDrawerBack.rotation + MathHelper.PiOver2);
        if (frontLegAngle < 0)
        {
            frontLegDrawerBack.spriteEffects = SpriteEffects.FlipVertically;
            frontLegDrawerBack.drawOrigin = new Vector2(0, _frontLegTextureAssets[1].Height() - 0);
        }
        frontLegDrawerBack.scale = Breathe(1);
        frontLegDrawerBack.worldPosition += offset;
        spriteBatch.Draw(frontLegDrawerBack);

        //BODY
        for (int i = _bodyTextureAssets.Length - 1; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_bodyTextureAssets[i], _bodySegments[i].a);
            float bodyAngle = _bodySegments[i].totalAngle;
            drawer.rotation = bodyAngle;
            float dir = 1;
            if (i == _bodyTextureAssets.Length - 1)
            {
                drawer.drawOrigin = new Vector2(_bodyTextureAssets[i].Width(), 0);
            }

            bodyAngle = MathHelper.WrapAngle(bodyAngle + MathHelper.PiOver2);
            if (bodyAngle < 0)
            {
                dir = -1;
                drawer.spriteEffects = SpriteEffects.FlipVertically;
                drawer.drawOrigin = new Vector2(drawer.drawOrigin.X, _bodyTextureAssets[i].Height() - drawer.drawOrigin.Y);
            }
            if (i == _bodyTextureAssets.Length - 1)
            {
                drawer.rotation -= MathHelper.ToRadians(65 * dir);
                //         drawer.scale *= 4;
            }
            drawer.scale = Breathe(2 + i);
            drawer.worldPosition += offset;
            spriteBatch.Draw(drawer);
        }


        //HEAD
        SpritebatchDrawer headDrawer = SpritebatchDrawer.FromTextureAsset(_headTextureAsset, _headSegment.b);
        float drawAngle = MathHelper.Pi + _headSegment.angle;

        drawAngle = MathHelper.WrapAngle(drawAngle);


        Player player = Main.player[NPC.target];
        Vector2 lookVectory = player.Center - NPC.Center;
        lookVectory = lookVectory.SafeNormalize(Vector2.Zero);

        Vector2 forwardVectory = (_headSegment.a - _headSegment.b).SafeNormalize(Vector2.Zero);
        float dp = Vector2.Dot(forwardVectory, lookVectory);
        if (dp > 0.25f)
        {
            float lookAngle = lookVectory.ToRotation();
            _headAngle = Utils.AngleLerp(_headAngle, lookAngle, 0.1f);

        }
        else
        {
            _headAngle = Utils.AngleLerp(_headAngle, drawAngle, 0.1f);
        }

        headDrawer.rotation = _headAngle;
        headDrawer.LeftCenterOrigin();

        drawAngle = MathHelper.WrapAngle(drawAngle + MathHelper.PiOver2);
        if (drawAngle < 0)
        {
            headDrawer.spriteEffects = SpriteEffects.FlipVertically;
            headDrawer.drawOrigin = new Vector2(headDrawer.drawOrigin.X, _headTextureAsset.Height() - headDrawer.drawOrigin.Y);
        }

        headDrawer.worldPosition += offset;
        headDrawer.scale = Breathe(6);
        spriteBatch.Draw(headDrawer);

        //BACK LEG
        SpritebatchDrawer backLegDrawerFront = SpritebatchDrawer.FromTextureAsset(_backLegTextureAssets[0], _bodySegments[2].a);
        backLegDrawerFront.drawOrigin = Vector2.Zero;
        backLegDrawerFront.rotation = _bodySegments[2].totalAngle - MathHelper.Pi + MathHelper.PiOver2;
        backLegDrawerFront.rotation -= MathHelper.ToRadians(25);
        //backLegDrawerFront.scale *= 4;
        float backDLegAngle = MathHelper.WrapAngle(_bodySegments[3].totalAngle - MathHelper.Pi + MathHelper.PiOver2);
        if (backDLegAngle < 0)
        {
            backLegDrawerFront.rotation += MathHelper.Pi;
            backLegDrawerFront.spriteEffects = SpriteEffects.FlipVertically;
            backLegDrawerFront.drawOrigin = new Vector2(0, _backLegTextureAssets[0].Height() - 0);
        }
        backLegDrawerFront.scale = Breathe(5);
        backLegDrawerFront.worldPosition += offset;
        spriteBatch.Draw(backLegDrawerFront);

        //FRONT LEG
        SpritebatchDrawer frontLegDrawerFront = SpritebatchDrawer.FromTextureAsset(_frontLegTextureAssets[0], _bodySegments[0].a);
        frontLegDrawerFront.drawOrigin = Vector2.Zero;
        frontLegDrawerFront.rotation = _bodySegments[0].totalAngle - MathHelper.Pi + MathHelper.PiOver2;
        float frontDLegAngle = MathHelper.WrapAngle(_bodySegments[0].totalAngle - MathHelper.Pi + MathHelper.PiOver2);
        if (frontDLegAngle < 0)
        {
            frontLegDrawerFront.rotation += MathHelper.Pi;
            frontLegDrawerFront.spriteEffects = SpriteEffects.FlipVertically;
            frontLegDrawerFront.drawOrigin = new Vector2(0, _frontLegTextureAssets[0].Height() - 0);
        }
        frontLegDrawerFront.scale = Breathe(5);
        frontLegDrawerFront.worldPosition += offset;
        spriteBatch.Draw(frontLegDrawerFront);

        //FRONT WING
        SpritebatchDrawer frontWingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAssets[0], _bodySegments[0].a);
        frontWingDrawer.drawOrigin = new Vector2(114, 84);
        frontWingDrawer.rotation = _bodySegments[0].totalAngle - MathHelper.Pi;
        frontWingDrawer.scale = Breathe(5);

        float wingAngle = MathHelper.WrapAngle(frontWingDrawer.rotation + MathHelper.PiOver2);
        if (wingAngle < 0)
        {
            frontWingDrawer.spriteEffects = SpriteEffects.FlipVertically;
            frontWingDrawer.drawOrigin = new Vector2(114, _wingTextureAssets[0].Height() - 84);
        }
        frontWingDrawer.rotation += WingBreathe(0);
        frontWingDrawer.worldPosition += offset;
        spriteBatch.Draw(frontWingDrawer);
    }

    public override void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        if (!_drawOutlines)
            return;
        _drawOutlines = false;
        float o = 2;
        DrawSegments(spriteBatch, -Vector2.UnitX * o);
        DrawSegments(spriteBatch, Vector2.UnitX * o);
        DrawSegments(spriteBatch, Vector2.UnitY * o);
        DrawSegments(spriteBatch, -Vector2.UnitY * o);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        LoadTextureAssets();
        ResolveKinematics();
        DrawSegments(spriteBatch, Vector2.Zero);
        return false;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Freezing to death")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Veldris the assassin", "2"))
            });
    }


    public override List<string> SetNPCNameList()
    {
        return new List<string>() {
                "Gilatine",
            };
    }

    public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
        //Set buttons
        buttons.Add(new Tuple<string, Action>("Talk", Talk));
        buttons.Add(new Tuple<string, Action>("WeaponUpgrade", OpenWeaponUpgradeMenu));

        portrait = "GilatinePortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiOpenDialogue1";
    }

    public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
    {
        base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
        portrait = "GilatinePortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiIdleChat1";
    }

    private void OpenWeaponUpgradeMenu()
    {
        Main.CloseNPCChatOrSign();
        Main.playerInventory = true;
        WeaponUpgradeUISystem uiSystem = ModContent.GetInstance<WeaponUpgradeUISystem>();
        uiSystem.OpenUI();
        CloseTownDialogue();
    }
}