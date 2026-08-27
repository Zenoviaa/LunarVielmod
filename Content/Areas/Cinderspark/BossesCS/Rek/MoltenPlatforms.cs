using ReLogic.Content;
using Stellamod.Common.Platforms;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;



public class BigMoltenPlatform : AbstractPlatformNPC
{
    public override bool CheckActive()
    {
        if (NPC.AnyNPCs(ModContent.NPCType<RekBoss>()))
            return false;
        return base.CheckActive();
    }
    private Asset<Texture2D> _glowMaskTexture;
    private Asset<Texture2D> _decorationTexture;
    public override void OnKill()
    {
        base.OnKill();
        if (Main.netMode == NetmodeID.Server)
            return;
        PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.Transparent, 60, 384);
        for (int k = 0; k < 48; k++)
        {
            Vector2 pos = NPC.position;
            pos.X += Main.rand.Next(0, NPC.width);
            pos.Y += Main.rand.Next(0, NPC.height);
            var sp = Particle<SmokeParticle>.SpawnInAlphaLayer(pos, -Vector2.UnitY * Main.rand.NextFloat(3f,7f), Color.Black, Scale: 4.5f);
            sp.fadeToColor = Color.Black;
            sp.initialColor = Color.Lerp(Color.RosyBrown, Color.Black, 0.5f);
            sp.fast = Main.rand.NextBool(2);
            sp.dampening = 0.1f;
        }

        for(int k = 0; k < 32; k++)
        {
            Vector2 dpos = NPC.Center + Main.rand.NextVector2Circular(512, 128);
            Vector2 dvel = -Vector2.UnitY * Main.rand.NextFloat(5f, 10f);
            DustParticle sp = Particle<DustParticle>.Spawn(dpos, dvel, Scale: Main.rand.NextFloat(0.5f, 1.5f));
            sp.gravity = 0.5f;
            sp.dampening = 0.1f;
        }
        ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
        screenShaderSystem.TintScreen(Color.Red, 0.12f, 120);

        int leftGore = Mod.Find<ModGore>($"{Name}_Gore_0").Type;
        int rightGore = Mod.Find<ModGore>($"{Name}_Gore_1").Type;
        int midGore = Mod.Find<ModGore>($"{Name}_Gore_2").Type;

        // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
        Vector2 vel = new Vector2(0, -15);
        Vector2 rightVel = vel;
        rightVel.X = 2;
        Vector2 leftVel = vel;
        leftVel.X = -2;
        Gore.NewGore(NPC.GetSource_Death(), NPC.Center - new Vector2(128, 0), leftVel, leftGore, 1f);
        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, vel, rightGore);
        Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(128, 0), rightVel, midGore);
    }

    public override Point GetPlatformSize()
    {
        return new Point(856, 358);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _glowMaskTexture ??= ModContent.Request<Texture2D>($"{Texture}_Glow");
        _decorationTexture ??= ModContent.Request<Texture2D>($"{Texture}_Decoration");
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_decorationTexture, NPC.Center);
        drawer.texture = _decorationTexture.Value;
        drawer.worldPosition += new Vector2(0, -218);
        drawer.sourceRect = null;
        drawer.color = Color.White;
        drawer.spriteEffects = SpriteEffects.None;
        drawer.scale = Vector2.One;
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.worldPosition = NPC.position;
        drawer.drawOrigin = Vector2.Zero;
        //drawer.texture = _decorationTexture.Value;
        drawer.color = Color.White;
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromNPC(NPC);
        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            drawer.worldPosition = NPC.position;
            drawer.worldPosition += f.ToRotationVector2() * 2;
            drawer.drawOrigin = Vector2.Zero;
            drawer.texture = _glowMaskTexture.Value;
            drawer.color = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 0.5f, speed: 3)) * 0.1f;
            drawer.color.A = 0;
            //drawer.texture = _decorationTexture.Value;
            spriteBatch.Draw(drawer);
        }

        return false;
    }

}

public class SmallMoltenPlatform : AbstractPlatformNPC
{
    private Asset<Texture2D> _glowMaskTexture;
    public override bool CheckActive()
    {
        if (NPC.AnyNPCs(ModContent.NPCType<RekBoss>()))
            return false;
        return base.CheckActive();
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _glowMaskTexture ??= ModContent.Request<Texture2D>($"{Texture}_Glow");
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.worldPosition = NPC.position;
        drawer.drawOrigin = Vector2.Zero;
        drawer.color = Color.White;
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.color = Color.White;
        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            drawer.worldPosition = NPC.position;
            drawer.worldPosition += f.ToRotationVector2() * 2;
            drawer.drawOrigin = Vector2.Zero;
            drawer.texture = _glowMaskTexture.Value;
            drawer.color = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 0.5f, speed: 3)) * 0.1f;
            drawer.color.A = 0;
            //drawer.texture = _decorationTexture.Value;
            spriteBatch.Draw(drawer);
        }


        return false;
    }

    public override Point GetPlatformSize()
    {
        return new Point(146, 150);
    }
}