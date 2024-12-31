using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    internal class IrradiaElectricBoxConnectorProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        bool ConnectToStart => true;
        bool Init;

        NPC[] Nodes;
        List<Point> Connections;
        List<Point> ConnectionsToRemove;
        List<Vector2[]> LightningPos;
        bool[] NodesThatDied;

        int FrameTick;
        int FrameCounter;


        internal PrimitiveTrail BeamDrawer;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }
        public int Ty = 0;
        public override void AI()
        {
            Ty++;

           if (Ty == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ElectricShuffle"), Projectile.position);
            }


            AI_FillPoints();
        }



        private void AI_FillPoints()
        {
            Timer++;
            if (!Init)
            {
                LightningPos = new List<Vector2[]>();
                ConnectionsToRemove = new List<Point>();
                Connections = new List<Point>();
                var nodes = new List<NPC>();

                for(int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active)
                        continue;
                    if (npc.type != ModContent.NPCType<IrradiaElectricBoxNode>())
                        continue;
                    //Already connected
                    if (npc.ai[1] == 1)
                        continue;

                    nodes.Add(npc);
                    npc.ai[1] = 1;
                }

                if(nodes.Count == 0)
                {
                    Projectile.Kill();
                    return;
                }
       
                Nodes = nodes.ToArray();
                NodesThatDied = new bool[Nodes.Length];
                for (int i = 0; i < Nodes.Length - 1; i++)
                {
                    Connections.Add(new Point(i, i + 1));
                }
                if (ConnectToStart)
                {
                    Connections.Add(new Point(Nodes.Length - 1, 0));
                }
                Init = true;
            }

            //Check active
            if(Nodes.Length > 0)
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    NPC node = Nodes[i];
                    if (!node.active)
                    {
                        NodesThatDied[i] = true;
                    }
                }
            }

            foreach(Point connection in Connections)
            {
                if (NodesThatDied[connection.X] || NodesThatDied[connection.Y])
                    ConnectionsToRemove.Add(connection);
            }

            foreach(Point removeConnection in ConnectionsToRemove)
            {
                Connections.Remove(removeConnection);
            }
            ConnectionsToRemove.Clear();


            if(Timer % 3 == 0)
            {
                LightningPos.Clear();
                for (int i = 0; i < Connections.Count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 position = Nodes[Connections[i].Y].Center;
                        Vector2 previousPosition = Nodes[Connections[i].X].Center;
                        List<Vector2> points = new List<Vector2>();

                        Vector2 currentPosition = previousPosition;

                        float distanceToPosition = Vector2.Distance(currentPosition, position);
                        points.Add(previousPosition);

                        while (distanceToPosition > 16)
                        {
                            Vector2 directionToPosition = currentPosition.DirectionTo(position);
                            Vector2 direction = directionToPosition.RotatedByRandom(MathHelper.ToRadians(9));
                            float distance = Main.rand.NextFloat(2, 64);
                            if (distance >= distanceToPosition)
                                distance = distanceToPosition;

                            currentPosition = currentPosition + direction * distance;
                            points.Add(currentPosition);
                            distanceToPosition = Vector2.Distance(currentPosition, position);
                        }

                        points.Add(position);
                        LightningPos.Add(points.ToArray());
                    }
                }
            }
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 8;
            return 3.5f + Easing.SpikeInOutCirc(completionRatio) * baseWidth;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Orange;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            float collisionPoint = 0;
            for (int i = 0; i < Connections.Count; i++)
            {
                Vector2 position = Nodes[Connections[i].Y].Center;
                Vector2 previousPosition = Nodes[Connections[i].X].Center;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Connections.Count == 0)
                return false;

            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);

            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailRegistry.LaserShader.UseColor(Color.Orange);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.VortexTrail);

            for(int i = 0; i < LightningPos.Count; i++)
            {
                BeamDrawer.DrawPixelated(LightningPos[i], -Main.screenPosition, LightningPos[i].Length);
            }

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
