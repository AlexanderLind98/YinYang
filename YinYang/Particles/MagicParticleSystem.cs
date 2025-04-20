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
    /// A specific particle system with randomized circular emission and upward motion.
    /// </summary>
    public class MagicParticleSystem : BaseParticleSystem
    {
        private readonly Random rng = new Random();

        [StructLayout(LayoutKind.Sequential)]
        private struct Particle
        {
            public Vector4 Position;
            public Vector4 Velocity;
        }

        public MagicParticleSystem(GameObject gameObject, Game window, int count)
            : base(gameObject, window, count) { }

        protected override int GetParticleSize() => Marshal.SizeOf<Particle>();

        protected override void LoadShaders()
        {
            computeShader = new ComputeShader("Shaders/Particles/computeMagicParticles.comp");
            renderShader = new Shader("Shaders/Particles/renderMagicParticles.vert", "Shaders/Particles/renderMagicParticles.frag");
        }

        protected override void PopulateInitialParticles(Span<byte> bufferData)
        {
            for (int i = 0; i < particleCount; i++)
            {
                float delay = (float)(rng.NextDouble() * 2.0);

                var p = new Particle
                {
                    Position = new Vector4(0f, 0f, 0f, delay),
                    Velocity = new Vector4(0.0f, 0.5f, 0.0f, 0.0f)
                };

                int offset = i * GetParticleSize();
                MemoryMarshal.Write(bufferData.Slice(offset), ref p);
            }
        }

        public override void DebugFirstParticle()
        {
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssboHandle);
            IntPtr ptr = GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
            if (ptr != IntPtr.Zero)
            {
                Particle p = Marshal.PtrToStructure<Particle>(ptr);
                Console.WriteLine($"[DEBUG] P0 Position: {p.Position.X}, {p.Position.Y}, {p.Position.Z}, lifetime: {p.Position.W}");
                GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
            }
        }
    }
}