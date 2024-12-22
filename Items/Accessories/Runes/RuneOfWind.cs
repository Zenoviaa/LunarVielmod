using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfWindBlowNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public Vector2 BlowVelocity;
        public override void AI(NPC npc)
        {
            base.AI(npc);
            npc.velocity += BlowVelocity;
            BlowVelocity *= 0.6f;
        }

    }

    internal class RuneOfWindShield : ModProjectile
    {
        private Vector2[] _windPos;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            _windPos = new Vector2[32];
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 10;
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            RuneOfWindPlayer runeOfWindPlayer = player.GetModPlayer<RuneOfWindPlayer>();
            if (runeOfWindPlayer.hasRuneOfWind)
            {
                Projectile.timeLeft = 2;
            }

            if (runeOfWindPlayer.hardBlowingTimer > 0)
            {
                Timer += 3;
            }
            else
            {
                Timer--;
            }
            if (Timer > 60)
                Timer = 60;
            if (Timer <= 0)
                Timer = 0;

            Projectile.Center = player.Center;
            for (int i = 0; i < _windPos.Length; i++)
            {
                float progress = (float)i / (float)_windPos.Length;
                float range = 80 + Timer * 2;
                Vector2 start = player.Center - new Vector2(range, 0);
                Vector2 end = player.Center + new Vector2(range, 0);
                Vector2 windPos = Vector2.Lerp(start, end, progress);
                _windPos[i] = windPos;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0f, 80, Easing.SpikeOutCirc(completionRatio));
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, Easing.SpikeOutCirc(completionRatio));
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            Projectile.oldPos = _windPos;
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            }

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Vector2 trailOffset = -Main.screenPosition;
            TrailDrawer.DrawPrims(_windPos, trailOffset, 155);

            return false;
        }
    }

    internal class RuneOfWindPlayer : ModPlayer
    {
        public bool hasRuneOfWind;
        public float blowingStrength;
        public float hardBlowingTimer;
        public float hardBlowDuration => 90;
        public override void ResetEffects()
        {
            base.ResetEffects();
            blowingStrength = 1f;
            hasRuneOfWind = false;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
            if (hasRuneOfWind)
            {
                hardBlowingTimer = hardBlowDuration;
            }
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            hardBlowingTimer--;
            if (hardBlowingTimer <= 0)
                hardBlowingTimer = 0;

            if (hasRuneOfWind)
            {
                float blowingHardProgress = hardBlowingTimer / hardBlowDuration;
                float blowStrength = blowingStrength + blowingHardProgress * blowingStrength;
                NPC[] npcsInRange = NPCHelper.FindNPCsInRange(Player.Center, maxDetectDistance: 80 + blowingHardProgress * 60, -1);
                foreach (NPC npc in npcsInRange)
                {
                    Vector2 blowVelocity = npc.Center - Player.Center;
                    blowVelocity = blowVelocity.SafeNormalize(Vector2.Zero);
                    blowVelocity *= blowingStrength;
                    npc.GetGlobalNPC<RuneOfWindBlowNPC>().BlowVelocity = blowVelocity;
                }
            }
        }
    }

    internal class RuneOfWind : BaseRune
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 2);
            Item.defense = 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RuneOfWindPlayer runeOfWindPlayer = player.GetModPlayer<RuneOfWindPlayer>();
            runeOfWindPlayer.hasRuneOfWind = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RuneOfWindShield>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<RuneOfWindShield>(), 0, 0, player.whoAmI);
            }
        }
    }
}