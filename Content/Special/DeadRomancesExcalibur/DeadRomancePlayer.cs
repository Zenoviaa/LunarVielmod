using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomancePlayer : ModPlayer
{
    public float attackSpeedStacks;
    public float hitResetTimer;
    public bool useGreatBlade;
    public float parryTimer;
    public float parryCooldown;
    public float parryStacks;
    public bool hitParry;
    public bool successfulReflect;
    public Vector2? dashVelocity;
    public float swingRatio => attackSpeedStacks / 20f;
    public int punishNPCIndex;
    public override void ResetEffects()
    {
        base.ResetEffects();
    }

    public override bool FreeDodge(Player.HurtInfo info)
    {
        if (Player.HasBuff<HeavenlyLove>())
            return true;

        punishNPCIndex = info.DamageSource.SourceNPCIndex;
        return base.FreeDodge(info);
    }
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if (dashVelocity.HasValue)
        {
            Vector2 dv = dashVelocity.Value;
            Player.velocity = dv;
            dashVelocity = null;
        }
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (hitResetTimer > 0)
            hitResetTimer--;

        if (Player.HasBuff<HeavenlyLove>())
        {
            for (int i = 0; i < Main.musicFade.Length; i++)
            {
                Main.musicFade[i] = 0;
            }

            if (Main.rand.NextBool(4))
            {
                Vector2 center = Player.Center;
                Rectangle screenRect = new Rectangle();
                screenRect.X = (int)center.X - Main.screenWidth / 2;
                screenRect.Y = (int)center.Y - Main.screenHeight / 2;
                screenRect.Width = Main.screenWidth;
                screenRect.Height = Main.screenHeight;
                Vector2 pos = new Vector2();
                pos.X = Main.rand.Next(screenRect.Left, screenRect.Right);
                pos.Y = Main.rand.Next(screenRect.Top, screenRect.Bottom);
                var sp = SparkleParticle.Spawn(pos, Vector2.Zero, Scale: 0.5f);
                sp.outerColor = Color.Goldenrod;
                sp.gravity = 0;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                ScreenShaderSystem tint = ModContent.GetInstance<ScreenShaderSystem>();
                tint.TintScreen(Color.Goldenrod, 0.2f, 15);
            }
        }

        if (parryStacks >= 5)
        {
            Ascend();
        }
        if (parryTimer > 0)
        {
            parryTimer--;
            if (parryTimer <= 0)
            {
                if (!hitParry)
                {
                    parryStacks = 0;
                }
            }
        }

        if (hitResetTimer <= 0)
        {
            attackSpeedStacks = 0;
        }

        if (attackSpeedStacks >= 28)
            attackSpeedStacks = 28;

        float lerp = attackSpeedStacks / 20f;
        lerp = MathHelper.Clamp(lerp, 0f, 1f);
        Player.GetAttackSpeed(DamageClass.Melee) += MathHelper.Lerp(0f, 2f, lerp);
    }
    public void Ascend()
    {
        SoundEngine.PlaySound(AssetRegistry.Sounds.Melee.ExcaliburAscended);
        Player.AddBuff(ModContent.BuffType<HeavenlyLove>(), 15 * 60);
        parryStacks = 0;
    }

    public void StartParry()
    {
        hitParry = false;
        parryTimer = 24;
    }
    public void ConsumeGreatBlade()
    {
        attackSpeedStacks = 0;
        useGreatBlade = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;
        int maxNumBlades = 5;
        SpriteBatch sb = Main.spriteBatch;
        if (Player.HasBuff<HeavenlyLove>())
        {
            var haloTexture = ModContent.GetInstance<HeavenlyLove>().SigilTextureAsset;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(haloTexture, Player.Center);
            drawer.worldPosition += Vector2.UnitY * -64;
            drawer.blackIsTransparency = true;
            drawer.color = Color.Goldenrod;
            drawer.worldPosition.Y += ExtraMath.Osc(0f, -4f);
            sb.Draw(drawer);
        }
        if (parryStacks <= 0)
            return;
        for (int i = 0; i < maxNumBlades; i++)
        {
            if (i >= parryStacks)
                break;
            float ratio = i / (float)maxNumBlades;
            float radians = ratio * MathHelper.TwoPi;
            radians += Main.GlobalTimeWrappedHourly * 0.5f;
            Vector2 drawCenter = radians.ToRotationVector2() * 32 + drawInfo.drawPlayer.Center;
            for (int j = 0; j < 4; j++)
            {

                float ratio2 = j / (float)4f;
                float radians2 = ratio2 * MathHelper.TwoPi;
                radians2 += Main.GlobalTimeWrappedHourly * 2f;
                Vector2 offset = radians2.ToRotationVector2() * 8;
                Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<DeadRomanceHeavenlySmiteBlade>()].Value;
                SpritebatchDrawer swordDrawer = SpritebatchDrawer.FromTextureAsset(texture, drawCenter + offset);
                float rads = MathHelper.ToRadians(3);
                float osc = ExtraMath.Osc(0f, 1f, offset: j);
                swordDrawer.rotation = MathHelper.PiOver2 + MathHelper.Lerp(-rads, rads, osc);
                swordDrawer.color = Color.Goldenrod;
                swordDrawer.color.A = 0;
                swordDrawer.scale = Vector2.One * 0.3f;
                sb.Draw(swordDrawer);
            }

        }
    }

    public override bool ConsumableDodge(Player.HurtInfo info)
    {
        if (parryTimer > 0 && parryCooldown <= 0)
        {
            parryTimer = 0;
            ParryEffects();
            return true;
        }

        return false;
    }

    public void ParryEffects()
    {
        //Brief invulnerability after parrying
        // Some sound and visual effects
        for (int i = 0; i < 50; i++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
            Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, DustID.GoldFlame, speed * 5, Scale: 1.5f);
            d.noGravity = true;
        }
        SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 0.5f }, Player.position);
        SoundStyle parryHitBack = AssetRegistry.Sounds.Melee.ExcaliburParryHitback;
        parryHitBack.PitchVariance = 0.3f;
        SoundEngine.PlaySound(parryHitBack);

        //Spawn the big verlia slash projectile here
        //Setting the immune time
        hitParry = true;
        parryTimer = 0;
        parryStacks++;
        Player.GetModPlayer<DashPlayer>().DashCount += 2;
        Player.SetImmuneTimeForAllTypes(60);
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        Vector2 velocity = Player.Center.DirectionTo(Main.MouseWorld);
        Projectile.NewProjectile(Player.GetSource_FromThis(),
            Player.Center, velocity, ModContent.ProjectileType<DeadRomancesExcaliburParrySlash>(),
            Player.HeldItem.damage * 5, Player.HeldItem.knockBack, Player.whoAmI, ai1: 1);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            SendExampleDodgeMessage(Player.whoAmI);
        }
    }

    public static void HandleExampleDodgeMessage(BinaryReader reader, int whoAmI)
    {
        int player = reader.ReadByte();
        if (Main.netMode == NetmodeID.Server)
        {
            player = whoAmI;
        }

        DeadRomancePlayer romancePlayer = Main.player[player].GetModPlayer<DeadRomancePlayer>();
        romancePlayer.ParryEffects();

        if (Main.netMode == NetmodeID.Server)
        {
            // If the server receives this message, it sends it to all other clients to sync the effects.
            SendExampleDodgeMessage(player);
        }
    }

    public static void SendExampleDodgeMessage(int whoAmI)
    {
        // This code is called by both the initial 
        ModPacket packet = ModContent.GetInstance<Stellamod>().GetPacket();
        packet.Write((byte)MessageType.RomanceDodge);
        packet.Write((byte)whoAmI);
        packet.Send(ignoreClient: whoAmI);
    }
}
