using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang;
using YinYang.Particles;
using YinYang.Rendering;

namespace YinYang.Particles
{
    /// <summary>
    /// Particle system for mist or splash at the base of a waterfall.
    /// Emits short-lived upward particles with turbulence.
    /// </summary>
    public class WaterfallMistParticleSystem : BaseParticleSystem
    {
        private readonly Random rng = new Random();

        [StructLayout(LayoutKind.Sequential)]
        private struct Particle
        {
            public Vector4 Position; // xyz = position, w = lifetime
            public Vector4 Velocity; // xyz = direction
        }

        public WaterfallMistParticleSystem(GameObject gameObject, Game window, int count)
            : base(gameObject, window, count) { }

        protected override int GetParticleSize() => Marshal.SizeOf<Particle>();

        protected override void LoadShaders()
        {
            computeShader = new ComputeShader("Shaders/Particles/computeWaterfallMist.comp");
            renderShader = new Shader("Shaders/Particles/renderMagicParticles.vert", "Shaders/Particles/renderWaterMist.frag");
        }

        protected override void PopulateInitialParticles(Span<byte> bufferData)
        {
            for (int i = 0; i < particleCount; i++)
            {
                float delay = (float)(rng.NextDouble() * 1.0);

                var p = new Particle
                {
                    Position = new Vector4(0f, 0f, 0f, delay),
                    Velocity = new Vector4(0.0f, 1.5f, 0.0f, 0.0f)
                };

                int offset = i * GetParticleSize();
                MemoryMarshal.Write(bufferData.Slice(offset), ref p);
            }
        }
    }
}