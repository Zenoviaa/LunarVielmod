using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using Stellamod.NPCs.Town;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.RenderTargetSystem
{

    /// <summary>
    /// Wrapper class for a render target that automatically gets resized and disposed
    /// </summary>
    public class ManagedRenderTarget : IDisposable
    {
        private Point _oldScreenSize;
        private readonly int _downSamples;
        private readonly ResizeFunction _resizeFunction;
        private RenderTarget2D _renderTarget;
        private bool _mipMap;
        private SurfaceFormat _surfaceFormat;
        private DepthFormat _depthFormat;
        private ManagedRenderTarget(ResizeFunction resizeFunction, int downSamples = 1, bool mipMap = true, SurfaceFormat surfaceFormat = SurfaceFormat.Color, DepthFormat depthFormat = DepthFormat.None)
        {
            _resizeFunction = resizeFunction;
            _mipMap = mipMap;
            _surfaceFormat = surfaceFormat;
            _depthFormat = depthFormat;
            _downSamples = downSamples;

            //Setting to 1 here just incase we get a division by 0 somewhere for like a single frame
            Width = 1;
            Height = 1;
        }

        public delegate Point ResizeFunction();
    
        public int Width { get; private set; }
        public int Height { get; private set; }
        private void Resize()
        {
            Point screenSize = _resizeFunction();
            Point newSize = new Point(screenSize.X / _downSamples, screenSize.Y / _downSamples);
            _renderTarget.Release();
            _renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, newSize.X, newSize.Y, mipMap: _mipMap, _surfaceFormat, _depthFormat);
            Width = newSize.X;
            Height = newSize.Y;
        }

        public void QueueResize(Point screenSize)
        {
            if(_oldScreenSize == screenSize) 
                return;
            
            _oldScreenSize = screenSize;
            Main.QueueMainThreadAction(Resize);

        }


        public static ManagedRenderTarget New( ResizeFunction resizeFunction, int downSamples = 1, bool mipMap = true, SurfaceFormat surfaceFormat = SurfaceFormat.Color, DepthFormat depthFormat = DepthFormat.None)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            ManagedRenderTarget managedRenderTarget = new ManagedRenderTarget(resizeFunction, downSamples, mipMap, surfaceFormat, depthFormat);

            ManagedRenderTargetManager managedRenderTargetManager = ModContent.GetInstance<ManagedRenderTargetManager>();
            managedRenderTargetManager.AddManagedRenderTarget(managedRenderTarget);
            return managedRenderTarget;
        }

        public void Dispose()
        {
            _renderTarget.Release();
        }

        public static implicit operator RenderTarget2D(ManagedRenderTarget managedRenderTarget)
        {
            //This looks weird, it's just so something gets output while the render target is loading
            //Might cause flickers when you resize the screen depending on what you're using the render target for
            //But that's such a non issue
            if (managedRenderTarget._renderTarget == null)
            {
                return Main.screenTargetSwap;
            }

            return managedRenderTarget._renderTarget;
        }
    }

    /// <summary>
    /// Automatically handles creating and resizing render targets so we don't have to duplicate the code everywhere
    /// </summary>
    public class ManagedRenderTargetManager : ModSystem
    {
        private Point _oldScreenSize;
        private List<ManagedRenderTarget> _managedRenderTargets;
        public override void Unload()
        {
            base.Unload();

            //Release all render targets
            for(int i = 0; i < _managedRenderTargets.Count; i++)
            {
                _managedRenderTargets[i].Dispose();
            }
            _managedRenderTargets.Clear();
        }

     
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTargets();
        }

        public void AddManagedRenderTarget(ManagedRenderTarget managedRenderTarget)
        {
            //Lazy loading
            _managedRenderTargets ??= new List<ManagedRenderTarget>();
            _managedRenderTargets.Add(managedRenderTarget);
        }

        private void ResizeRenderTargets()
        {
            Point screenSize = Main.ScreenSize;
            if (_oldScreenSize == screenSize)
                return;

            for(int i = 0; i < _managedRenderTargets.Count; i++)
            {
                ManagedRenderTarget managedRenderTarget = _managedRenderTargets[i];
                managedRenderTarget.QueueResize(screenSize);
            }
            _oldScreenSize = screenSize;
        }
    }
}
