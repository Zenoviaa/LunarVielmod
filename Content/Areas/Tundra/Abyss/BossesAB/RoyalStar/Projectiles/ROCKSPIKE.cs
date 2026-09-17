using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Gores;
using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class ROCKSPIKE : ModNPC,
    IDrawToRenderTarget
{
    private int _frame;
    private Vector2 _scale;
    private float InTime => 34;
    private ref float Timer => ref NPC.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_frame);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _frame = reader.ReadInt32();
    }
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _frame = Main.rand.Next(3);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 3;
        NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        NPCSets.Heavy[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 128;
        NPC.lifeMax = 10;
        NPC.damage = 35;
        NPC.defense = 9999;
        NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
      //  NPC.DeathSound = AssetReferences.Assets.Sounds.RockBreak.Asset with { PitchVariance = 0.6f };
        NPC.dontCountMe = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.knockBackResist = 0;
    }

    public override void AI()
    {
        base.AI();



        Timer++;
        if(Timer == 1)
        {
            var rockSpikeAsound = AssetReferences.Assets.Sounds.STARR.RockSmash.Asset with { PitchVariance = 1f };
            SoundEngine.PlaySound(rockSpikeAsound, NPC.position);
            CrackVFX(NPC.Bottom);
            DustVFX(NPC.Bottom, -Vector2.UnitY * 7);
            for (int i = 0; i < 4; i++)
            {
                FXUtil.MakeSoilParticle(NPC.Bottom + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1f) * 15);
            }
            for(int i = 0; i < 5; i++)
            {
                MakeRockGore(NPC.Bottom + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY.RotatedByRandom(0.6f) * Main.rand.NextFloat(10f, 15f));
            }
            FXUtil.ShakeCamera(NPC.position, 1024, 16);
        }

        ShakeScreenPosition.Shake = MathHelper.Lerp(4f, 0f, EasingFunction.OutExpo(Timer / InTime));

        float ratio = Timer / InTime;
        float ease = EasingFunction.OutExpo(ratio);

        Vector2 lerp1 = Vector2.Lerp(Vector2.One * 0.6f, new Vector2(1.2f) * new Vector2(1f, 1.42f), ease);
        Vector2 lerp2 = Vector2.Lerp(new Vector2(1.2f), Vector2.One, ease);
        Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, EasingFunction.InOutSine(ratio));
        _scale = lerp3;
    }

    public void MakeRockGore(in Vector2 position, in Vector2 velocity)
    {
        Gore.NewGore(NPC.GetSource_FromThis(), position, velocity, ModContent.GoreType<IceRockGore>());
    }

    public void DustVFX(Vector2 position, Vector2 velocity)
    {
        for (float f = 0; f < 8; f++)
        {
            Vector2 spawnPosition = position;
            spawnPosition.X += Main.rand.NextFloat(-512, 512);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);
            spawnVelocity += velocity;
            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }
    }
    public void CrackVFX(Vector2 position)
    {
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, timeLeft = 200 });
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Draw();
        return false;
    }

    private void Draw(Color? overrideColor = null)
    {
        SpritebatchDrawer spikeDrawer = SpritebatchDrawer.FromNPC(NPC);
        spikeDrawer.worldPosition = NPC.Bottom;
        spikeDrawer.BottomCenterOrigin();
        spikeDrawer.scale *= _scale;
        spikeDrawer.VerticalFrame(_frame, 3);
        if (overrideColor.HasValue)
            spikeDrawer.color = overrideColor.Value;
        Main.spriteBatch.Draw(spikeDrawer);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if(NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            FXUtil.ShakeCamera(NPC.position, 128, 4);
            for (int i = 0; i < 4; i++)
            {
                Vector2 position = NPC.Center + Main.rand.NextVector2Circular(48, 48);
                Vector2 velocity = Main.rand.NextVector2Circular(5, 5);
                Gore.NewGore(NPC.GetSource_FromThis(), position, velocity, ModContent.GoreType<IceRockGore>());
            }

            for (int f = 0; f < 2; f++)
            {
                Vector2 spawnPosition = NPC.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPosition = NPC.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(spawnPosition, DustID.Stone, spawnVelocity);
            }
        }
    }
    public override void OnKill()
    {
        base.OnKill();
        if (Main.netMode == NetmodeID.Server)
            return;

    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        Draw(Color.Red);
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawWhite); 
    }
}
