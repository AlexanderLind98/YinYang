using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using YinYang.Behaviors;
using YinYang.Rendering;

namespace YinYang.Particles
{
    public abstract class BaseParticleSystem : Behaviour
    {
        protected int particleCount;
        protected int ssboHandle;
        protected ComputeShader computeShader;
        protected Shader renderShader;
        private int vao;

        public BaseParticleSystem(GameObject gameObject, Game window, int count) 
            : base(gameObject, window)
        {
            particleCount = count;
            LoadShaders();
            InitParticleBuffer();
            WarmupParticles();

            // Create a VAO for the particle system
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Subclasses must fill the initial particle data here.
        /// </summary>
        protected abstract void PopulateInitialParticles(Span<byte> bufferData);

        /// <summary>
        /// Subclasses must load/compile their specific shaders here.
        /// </summary>
        protected abstract void LoadShaders();

        /// <summary>
        /// Returns the size in bytes of one particle.
        /// </summary>
        protected abstract int GetParticleSize();

        /// <summary>
        /// Called each frame to update particle logic on GPU.
        /// </summary>
        public override void Update(FrameEventArgs args)
        {
            computeShader.Use();
            computeShader.SetFloat("deltaTime", (float)args.Time);
            computeShader.BindSSBO(0, ssboHandle);
            computeShader.Dispatch((particleCount + 255) / 256);
            computeShader.Barrier();
        }
        
        private void WarmupParticles()
        {
            computeShader.Use();
            computeShader.SetFloat("deltaTime", 0.016f);
            computeShader.BindSSBO(0, ssboHandle);
            computeShader.Dispatch((particleCount + 255) / 256);
            computeShader.Barrier();
        }

        /// <summary>
        /// Draws all particles as instanced geometry.
        /// </summary>
        public virtual void DrawParticles(RenderContext context)
        {
            renderShader.Use();
            renderShader.SetMatrix("viewProj", context.ViewProjection);
            renderShader.SetVector3("cameraPos", context.Camera.Position);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ssboHandle);
            GL.BindVertexArray(vao); 
            GL.DrawArraysInstanced(PrimitiveType.Points, 0, 1, particleCount);
            GL.BindVertexArray(0);
        }

        private void InitParticleBuffer()
        {
            int totalSize = GetParticleSize() * particleCount;
            ssboHandle = GL.GenBuffer();

            byte[] buffer = new byte[totalSize];
            PopulateInitialParticles(buffer);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssboHandle);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, totalSize, buffer, BufferUsageHint.DynamicDraw);
        }
    }
}
