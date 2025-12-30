using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class WondrousShieldPlayer : ModPlayer
    {
        public int stacks;
        public Asset<Texture2D> BubbleTextureAsset;

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
            if (stacks > 0)
            {
                modifiers.IncomingDamageMultiplier *= 0.1f;
                stacks--;

                SoundStyle arcaneExplode = new SoundStyle("Stellamod/Assets/Sounds/ArcaneExplode");
                arcaneExplode.PitchVariance = 0.3f;
                SoundEngine.PlaySound(arcaneExplode, Player.position);


                FXUtil.ShakeCamera(Player.Center, 1024, 8);
                FXUtil.GlowCircleBoom(Player.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.28f);
            }
        }
        public override void PostUpdateBuffs()
        {
            base.PostUpdateBuffs();
            if (stacks > 0)
            {
                Player.AddBuff(ModContent.BuffType<WondrousShieldInsourceBuff>(), 2);
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            if (stacks > 0 && drawInfo.shadow == 0f)
            {
                string path = this.GetType().DirectoryHere() + "/WondrousShieldBubble";
                BubbleTextureAsset ??= ModContent.Request<Texture2D>(path);
                SpriteBatch spriteBatch = Main.spriteBatch;

                Vector2 drawPosition = Player.Center - Main.screenPosition;
                Point tilePosition = Player.position.ToTileCoordinates();
                Color lightingColor = Lighting.GetColor(tilePosition.X, tilePosition.Y);
                Color drawColor = Color.White.MultiplyRGB(lightingColor);
                Vector2 drawOrigin = BubbleTextureAsset.Size() / 2f;
                Vector2 drawScale = Vector2.Lerp(new Vector2(0.75f), Vector2.One, ExtraMath.Osc(0f, 1f));
                spriteBatch.Draw(BubbleTextureAsset.Value, drawPosition, null, drawColor, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
    }

    public class WondrousShieldInsourceBuff : ModBuff
    {

    }

    public class WondrousShieldInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 120;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            WondrousShieldPlayer shieldPlayer = flaskPlayer.Player.GetModPlayer<WondrousShieldPlayer>();
            shieldPlayer.stacks++;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MiracleThread, BlankBrooch>();
        }
    }
}
