using Microsoft.Xna.Framework.Graphics.PackedVector;
using Stellamod.Assets;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD.Critters;

public class DreamyTentacle : ModNPC
{
    public override string Texture => AssetReferences.Assets.Textures.Darkspace.NPCs.DreamyTentacle.KEY;
    float Glow => ExtraMath.Osc(0.1f, 0.7f, offset: NPC.whoAmI);
    public static readonly Rectangle TopBulbFrame = new Rectangle(28, 12, 14, 16);
    public static readonly Rectangle TopStemFrame = new Rectangle(28, 40, 12, 8);
    public static readonly Rectangle StemFrame = new Rectangle(32, 60, 6, 10);
    public static readonly Rectangle BottomStemFrame = new Rectangle(26, 82, 18, 8);
    public static readonly Rectangle GrassFrame = new Rectangle(8, 100, 56, 26);

    Vector2 _ikTarget;
    int SegmentCount => 32;
    Vector2[] Positions => field ??= new Vector2[SegmentCount];
    FabrikSegment[] Segments
    {
        get
        {
            if (field == null)
            {
                field = new FabrikSegment[SegmentCount];
                for (var i = 0; i < field.Length; i++)
                {
                    ref var segment = ref field[i];
                    segment.position = new Vector2(NPC.Center.X, NPC.Center.Y - i * 7);
                    segment.length = 7;
                }
            }


            return field;
        }
        set => field = value;
    }

    float SqrDistanceToTarget => Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
    float SqrTouchDistance => 180 * 180;
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 32;
        NPC.damage = 32;
        NPC.width = 34;
        NPC.height = 72;
        NPC.lifeMax = 135;
        NPC.defense = 10;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.25f;
        NPC.noGravity = false;
    }

    public override void AI()
    {
        base.AI();
        NPC.TargetClosest();
        var quickTouch = false;
        if (SqrDistanceToTarget < SqrTouchDistance)
        {
            quickTouch = true;
            _ikTarget = NPC.PlayerImTargeting.Center;
        }
        else
        {
            _ikTarget = NPC.Center + new Vector2(ExtraMath.Osc(-16, 16, speed: 0.5F, NPC.whoAmI), -SqrTouchDistance);
        }
        var segments = Segments;
        FabrikSolver.Solve(ref segments, NPC.Bottom, _ikTarget, 4);
        var ratio = quickTouch ? 0.06f : 0.02f;
        for (var i = 0; i < Positions.Length; i++)
        {
            ref var pos = ref Positions[i];
            pos = Vector2.Lerp(pos, segments[i].position, ratio);
        }
        Segments = segments;
    }

    Rectangle GetFrameAtIndex(int index)
    {
        if (index == 0)
        {
            return BottomStemFrame;
        }
        if (index == Positions.Length - 3)
        {
            return TopStemFrame;
        }
        if (index == Positions.Length - 2)
        {
            return TopBulbFrame;
        }
        return StemFrame;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var texture = TextureAssets.Npc[Type].Value;
        for (var i = 0; i < Positions.Length - 1; i++)
        {
            ref var pos = ref Positions[i];
            ref var nextPos = ref Positions[i + 1];
            var stemDrawer = SpritebatchDrawer.FromTextureGetLightingColor(texture, pos);
            stemDrawer.sourceRect = GetFrameAtIndex(i);
            stemDrawer.BottomCenterOrigin();
            stemDrawer.worldPosition = pos;
            stemDrawer.rotation = (nextPos - pos).ToRotation() + MathHelper.PiOver2;
            spriteBatch.Draw(stemDrawer);
        }

        var bottomDrawer = SpritebatchDrawer.FromTextureGetLightingColor(texture, NPC.Bottom);
        bottomDrawer.sourceRect = GrassFrame;
        bottomDrawer.BottomCenterOrigin();
        spriteBatch.Draw(bottomDrawer);
        return false;
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureWhite(glowCircle);
        drawer.worldPosition = NPC.Center;
        drawer.color = Color.White * Glow * 0.2f;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        spriteBatch.Draw(drawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        DarkspaceEnemyCommon.HitAndDeathEffectsMini(NPC);
    }
}
