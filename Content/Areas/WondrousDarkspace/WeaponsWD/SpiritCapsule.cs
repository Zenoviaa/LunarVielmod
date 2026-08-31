using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;


public class Charm : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        if (Main.rand.NextBool(3))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.LightPink * 0.4f;
            sp.expand = true;
        }

        if (Main.rand.NextBool(6))
        {
            int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Gold);
            Main.dust[d].noGravity = true;
        }
    }
}
public class CharmGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public bool charmed;
    public static bool makeSpawnedProjectilesCharmed;
    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        base.SendExtraAI(projectile, bitWriter, binaryWriter);
        binaryWriter.Write(charmed);
    }
    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        base.ReceiveExtraAI(projectile, bitReader, binaryReader);
        charmed = binaryReader.ReadBoolean();
    }
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        base.OnSpawn(projectile, source);
        if (makeSpawnedProjectilesCharmed)
        {
            charmed = true;
        }
    }
    public override bool PreAI(Projectile projectile)
    {
        if (charmed && projectile.hostile)
        {
            projectile.friendly = true;
            projectile.hostile = false;
        }
        return base.PreAI(projectile);
    }

    public override bool? CanHitNPC(Projectile projectile, NPC target)
    {
        IEntitySource source = projectile.GetSource_FromThis();
        if (source is EntitySource_Parent src)
        {
            if (src.Entity is NPC npc)
            {
                if (npc == target)
                {
                    return false;
                }
            }
        }
        return base.CanHitNPC(projectile, target);
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(projectile, target, ref modifiers);

    }
}
public class CharmGlobalNPC : GlobalNPC
{
    private static DummyPlayer _dummyPlayer;
    public override bool PreAI(NPC npc)
    {
       
        if (npc.HasBuff<Charm>() && !npc.boss)
        {
            CharmGlobalProjectile.makeSpawnedProjectilesCharmed = true;
            _dummyPlayer = DummyPlayerHelper.RequestDummyPlayer();
            npc.target = _dummyPlayer.playerIndex;

            float closestDistance = float.MaxValue;
            NPC closestNPC = npc;
            foreach (NPC otherNPC in Main.ActiveNPCs)
            {
                if (npc.whoAmI == otherNPC.whoAmI)
                    continue;

                float dist = Vector2.Distance(npc.Center, otherNPC.Center);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestNPC = otherNPC;
                }
          
            }

   

            _dummyPlayer.player.Center = closestNPC.Center; ;

            /*
            Vector2 pos = _dummyPlayer.player.Center;
            PixelationManager.QueueSpritebatchDrawAction((SpriteBatch sb, Vector2 screenPos) =>
            {
                SpritebatchDrawer testDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, pos);
                testDrawer.scale *= 0.3f;
                testDrawer.color = Color.Green;
                testDrawer.blackIsTransparency = true;
                sb.Draw(testDrawer);
            });
          */
        }
        return base.PreAI(npc);
    }

    public override void PostAI(NPC npc)
    {
        base.PostAI(npc);
        if (npc.HasBuff<Charm>() && !npc.boss)
        {
            CharmGlobalProjectile.makeSpawnedProjectilesCharmed = false;
            DummyPlayerHelper.ReturnDummyPlayer(_dummyPlayer);
        }
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        if (npc.HasBuff<Charm>())
        {
            void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
            {
                Asset<Texture2D> noise = AssetManager.GlowMask.MagicCircle2;
                Vector2 drawOrigin = noise.Size() / 2f;
                Texture2D texture = noise.Value;

                Vector2 drawCenter = npc.Center - Main.screenPosition;
                drawCenter.Y += npc.gfxOffY;

          
                Color drawColor = Color.White;
                drawColor.A = 0;
                Color drawColor2 = Color.Blue;
                drawColor2.A = 0;
                //     drawColor *= 0.5f;

                Vector2 scale = Vector2.One;
                scale *= ExtraMath.Osc(0.5f, 1f);
                scale *= 4;
                var shader = CelestialAuraShader.Instance;
                shader.InnerColor = Color.Pink;
                shader.OuterColor = Color.Black;
                shader.Time = -Main.GlobalTimeWrappedHourly;
                shader.Tiling = Vector2.One * 0.1f;
                spriteBatch.Restart(effect: shader.Effect);
                for (float f = 0; f < 3; f++)
                {
                    Color glowColor = Color.Lerp(drawColor, drawColor2, (f + 1) / 3f);
                    glowColor.A = 0;
                    float rotOffset = (f / 4f) * MathHelper.TwoPi;
                    spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset + 0.5f, drawOrigin,
                        new Vector2(0.8f, 1f) * 0.25f * 0.75f * scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset, drawOrigin,
                        new Vector2(0.8f, 1f) * 0.25f * scale, SpriteEffects.None, 0);
                }

                spriteBatch.RestartDefaults();
            }
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
        }
    }


}

public class SpiritCapsule : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToCombatTool(0, 0, ammoCount: 3);
        Item.shoot = ModContent.ProjectileType<SpiritCapsuleP>();
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankJuggler>(),
            material: ModContent.ItemType<MiracleThread>());
    }
}

public class SpiritCapsuleP : ModProjectile
{
    private float Timer;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.BouncingShield);
        Projectile.width = 8;
        Projectile.height = 8;
        AIType = ProjectileID.BouncingShield;
        Projectile.penetrate = 5;
    }

    public override void PostAI()
    {
        base.PostAI();
        Timer++;
        if (Timer % 12 == 0)
        {
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Pink, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
            else
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowHeartDust>(), Projectile.velocity * 0.1f, 0, Color.Pink, Main.rand.NextFloat(1f, 2f)).noGravity = true;
        }
        Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 1.75f * Main.essScale);
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(32, 0f, completionRatio);
    }

    private Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Pink, Color.Transparent, completionRatio);
    }


    private void DrawTrail(GraphicsDevice graphicDevice)
    {
        var shader = BlackFireShader.Instance;
        shader.InnerColor = Color.LightPink * 0.15f;
        shader.OuterColor = Color.Pink * 0.15f;
        shader.BackColor = Color.Violet * 0.15f;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader,
            offset: Projectile.Size / 2f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
            ModContent.ProjectileType<SpiritualBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
        for (float f = 0; f < 16; f++)
        {
            float p = f / 16f;
            Vector2 spawnPoint = Projectile.Center + VectorHelper.PointOnHeart(p * 8, 6);
            Vector2 vel = (spawnPoint - Projectile.Center).SafeNormalize(Vector2.Zero) * 5;
            Dust.NewDustPerfect(spawnPoint, ModContent.DustType<GlowHeartDust>(), vel, 0, Color.Pink, 2f).noGravity = true;
        }
        SoundStyle explosionSound = new SoundStyle($"Stellamod/Assets/Sounds/Briskfly");
        explosionSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(explosionSound, Projectile.position);
        target.AddBuff(ModContent.BuffType<Charm>(), 18000);
    }
}









