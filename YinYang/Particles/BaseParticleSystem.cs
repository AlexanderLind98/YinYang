using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using YinYang.Behaviors;
using YinYang.Rendering;

namespace YinYang.Particles
{
    /// <summary>
    /// Base class for GPU-driven particle systems using OpenGL with clean buffer management.
    /// </summary>
    public abstract class BaseParticleSystem : Behaviour
    {
        protected int particleCount;
        protected int ssboHandle;
        protected ComputeShader computeShader;
        protected Shader renderShader;
        private int vaoHandle;
        private int dummyVBO;

        public BaseParticleSystem(GameObject gameObject, Game window, int count)
            : base(gameObject, window)
        {
            particleCount = count;
            LoadShaders();
            InitializeBuffers();
            WarmupParticles();
            InitializeVAO();
        }

        /// <summary>
        /// Subclasses must provide the per-particle struct size.
        /// </summary>
        protected abstract int GetParticleSize();

        /// <summary>
        /// Subclasses must populate the particle data buffer with initial state.
        /// </summary>
        protected abstract void PopulateInitialParticles(Span<byte> bufferData);

        /// <summary>
        /// Subclasses must load their compute and render shaders.
        /// </summary>
        protected abstract void LoadShaders();

        /// <summary>
        /// Subclasses can override this to define a unique SSBO binding index.
        /// </summary>
        protected virtual int GetBindingIndex() => 0;

        private void InitializeBuffers()
        {
            int totalSize = particleCount * GetParticleSize();
            ssboHandle = GL.GenBuffer();

            byte[] bufferData = new byte[totalSize];
            PopulateInitialParticles(bufferData);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssboHandle);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, totalSize, bufferData, BufferUsageHint.DynamicDraw);
        }

        private void InitializeVAO()
        {
            vaoHandle = GL.GenVertexArray();
            dummyVBO = GL.GenBuffer();

            GL.BindVertexArray(vaoHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, dummyVBO);

            float[] dummyData = new float[3];
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 3, dummyData, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void WarmupParticles()
        {
            computeShader.Use();
            computeShader.SetFloat("deltaTime", 0.016f);
            computeShader.BindSSBO(GetBindingIndex(), ssboHandle);
            computeShader.Dispatch((particleCount + 255) / 256);
            computeShader.Barrier();
        }

        public override void Update(FrameEventArgs args)
        {
            computeShader.Use();
            computeShader.SetFloat("deltaTime", (float)args.Time);
            computeShader.SetVector3("spawnOrigin", gameObject.Transform.Position);

            computeShader.BindSSBO(GetBindingIndex(), ssboHandle);
            computeShader.Dispatch((particleCount + 255) / 256);
            computeShader.Barrier();
        }

        public virtual void DrawParticles(RenderContext context)
        {
            renderShader.Use();
            renderShader.SetMatrix("viewProj", context.ViewProjection);
            renderShader.SetVector3("cameraPosition", context.Camera.Position);
            renderShader.SetFloat("fadeDistance", 20.0f);

            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, GetBindingIndex(), ssboHandle);
            GL.BindVertexArray(vaoHandle);
            GL.DrawArraysInstanced(PrimitiveType.Points, 0, 1, particleCount);
            GL.BindVertexArray(0);

            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
        }

        public virtual void DebugFirstParticle() { }
    }
}
