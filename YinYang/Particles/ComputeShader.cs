using OpenTK.Graphics.OpenGL4;

namespace YinYang.Particles
{
    /// <summary>
    /// Wrapper class for compute shaders in OpenGL.
    /// </summary>
    public class ComputeShader : IDisposable
    {
        private readonly Shader shader;

        /// <summary>
        /// Loads and compiles a compute shader from file.
        /// </summary>
        /// <param name="path">Path to the .comp shader file.</param>
        public ComputeShader(string path)
        {
            shader = new Shader(path, ShaderType.ComputeShader);
        }

        /// <summary>
        /// Activates the shader for use.
        /// </summary>
        public void Use()
        {
            shader.Use();
        }

        /// <summary>
        /// Sets a float uniform.
        /// </summary>
        public void SetFloat(string name, float value)
        {
            shader.SetFloat(name, value);
        }

        /// <summary>
        /// Sets an int uniform.
        /// </summary>
        public void SetInt(string name, int value)
        {
            shader.SetInt(name, value);
        }

        /// <summary>
        /// Sets a Vector3 uniform.
        /// </summary>
        public void SetVector3(string name, OpenTK.Mathematics.Vector3 value)
        {
            shader.SetVector3(name, value);
        }

        /// <summary>
        /// Binds a Shader Storage Buffer Object to a specific binding point.
        /// </summary>
        public void BindSSBO(int binding, int ssboHandle)
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, binding, ssboHandle);
        }

        /// <summary>
        /// Dispatches the compute shader with the specified work group sizes.
        /// </summary>
        public void Dispatch(int groupsX, int groupsY = 1, int groupsZ = 1)
        {
            GL.DispatchCompute(groupsX, groupsY, groupsZ);
        }

        /// <summary>
        /// Inserts a memory barrier to ensure SSBO access completes before next stage.
        /// </summary>
        public void Barrier()
        {
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        }

        public void Dispose()
        {
            shader.Dispose();
        }
    }
}
