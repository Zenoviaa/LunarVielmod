using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    internal class IrradiaElectricBoxConnectorProj : ModProjectile
    {

        bool ConnectToStart => true;
        bool Init;

        NPC[] Nodes;
        List<Point> Connections;
        List<Point> ConnectionsToRemove;
        bool[] NodesThatDied;

        int FrameTick;
        int FrameCounter;


        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 720;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            AI_FillPoints();
        }



        private void AI_FillPoints()
        {
            if (!Init)
            {
                ConnectionsToRemove = new List<Point>();
                Connections = new List<Point>();
                var nodes = new List<NPC>();
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.type != ModContent.NPCType<IrradiaElectricBoxNode>())
                        continue;
                    //Already connected
                    if (npc.ai[1] == 1)
                        continue;

                    nodes.Add(npc);
                    npc.ai[1] = 1;
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
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 8;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Yellow;
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
            return base.Colliding(projHitbox, targetHitbox);
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

            for (int i = 0; i < Connections.Count; i++)
            {
                Vector2 position = Nodes[Connections[i].Y].Center;
                Vector2 previousPosition = Nodes[Connections[i].X].Center;
                DrawHelper.DrawSupernovaChains(chainTexture, previousPosition, position, animationFrame, Projectile.alpha / 255f);
            }
          
            return false;
        }
    }
}
