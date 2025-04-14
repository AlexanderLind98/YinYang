using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang;
using YinYang.Particles;
using YinYang.Rendering;

namespace YinYang.Particles
{
    public class MagicParticleSystem : BaseParticleSystem
    {
        public Random rng = new Random();
        
        public MagicParticleSystem(GameObject gameObject, Game window, int count) 
            : base(gameObject, window, count) { }

        // tell the compiler to lay out the struct in memory in the order we define it
        [StructLayout(LayoutKind.Sequential)] private struct Particle
        {
            // xyz = position, w = lifetime
            public Vector4 Position; 
            
            // xyz = unused, w = current angle
            public Vector4 Velocity;  
        }

        protected override int GetParticleSize() => Marshal.SizeOf<Particle>();

        protected override void PopulateInitialParticles(Span<byte> bufferData)
        {
            Vector3 spawnPos = gameObject.Transform.Position;

            for (int i = 0; i < particleCount; i++)
            {
                float delay = (float)(rng.NextDouble() * 2.0); // 0 til 2 sekunders delay
                var p = new Particle
                {
                    // pos, lifetime
                    Position = new Vector4(0, 0, 0, delay),
                    // op
                    Velocity = new Vector4(0.0f, 0.5f, 0.0f, 0.0f) 
                };

                int offset = i * GetParticleSize();
                MemoryMarshal.Write(bufferData.Slice(offset), ref p);
            }
        }


        protected override void LoadShaders()
        {
            computeShader = new ComputeShader("Shaders/computeMagicParticles.comp");
            renderShader = new Shader("Shaders/renderMagicParticles.vert", "Shaders/renderMagicParticles.frag");
        }
    }
}