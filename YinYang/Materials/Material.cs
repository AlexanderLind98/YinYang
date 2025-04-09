using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials
{
    /// <summary>
    /// Represents a material with a shader and uniforms.
    /// </summary>
    public class Material : IDisposable
    {
        protected Shader shader;
        protected Dictionary<string, object> uniforms = new();
        private Dictionary<int, Texture> textures = new();

        /// <summary>Enables GL error debug output during SetUniform().</summary>
        public static bool MaterialDebug = true;

        /// <summary>Indicates whether this material uses scene lighting.</summary>
        public virtual bool UsesLighting => true;

        /// <summary>
        /// Called only if <c>UsesLighting</c> is true.
        /// Materials can override to push lighting-specific uniforms.
        /// </summary>
        public virtual void PrepareLighting(RenderContext context) { }

        
        public void UpdateUniforms()
        {
            foreach (KeyValuePair<string, object> uniform in uniforms)
            {
                SetUniform(uniform.Key, uniform.Value);
            }
        }

        public Material(string vertPath, string fragPath)
        {
            shader = new Shader(vertPath, fragPath);
        }

        public Material(string vertPath, string fragPath, Dictionary<string, object> uniforms)
        {
            shader = new Shader(vertPath, fragPath);
            foreach (KeyValuePair<string, object> uniform in uniforms)
            {
                SetUniform(uniform.Key, uniform.Value);
            }
        }

        public Material()
        {
            shader = new Shader("Shaders/LitGeneric.vert", "Shaders/LitGeneric.frag");
        }

        /// <summary>
        /// Sets a uniform for the shader.
        /// </summary>
        /// <param name="name">The uniform name.</param>
        /// <param name="uniform">The uniform value.</param>
        public void SetUniform(string name, object uniform)
        {
            if (shader == null)
            {
                Console.WriteLine("[Material ERROR] Tried to set uniform on null or disposed shader.");
                return;
            }

            shader.Use();

            if (uniform is int uniformInt)
            {
                shader.SetInt(name, uniformInt);
            }
            else if (uniform is float uniformFloat)
            {
                shader.SetFloat(name, uniformFloat);
            }
            else if (uniform is Vector3 uniformVec3)
            {
                shader.SetVector3(name, uniformVec3);
            }
            else if (uniform is Matrix4 uniformMatrix)
            {
                shader.SetMatrix(name, uniformMatrix);
            }
            else if (uniform is Texture tex)
            {
                int textureUnit = GetTextureUnitFor(name); 

                if (textureUnit < 0 || textureUnit > 31)
                {
                    Console.WriteLine($"[Material ERROR] Invalid texture unit for '{name}'. Value '{textureUnit}'. Skipped.");
                    return;
                }
                
                if (tex?.Handle == 0)
                {
                    Console.WriteLine($"[Material WARNING] Texture for '{name}' is null or uninitialized.");
                    return;
                }

                if (!textures.ContainsKey(textureUnit))
                {
                    shader.SetInt(name, textureUnit);
                    textures.Add(textureUnit, tex);
                }
                else
                {
                    // Already exists — just update the uniform binding
                    shader.SetInt(name, textureUnit);
                    textures[textureUnit] = tex;
                }
            }
            else
            {
                Console.WriteLine($"[Material ERROR] Unsupported uniform type for '{name}'.");
                return;
            }

            uniforms[name] = uniform;

            if (MaterialDebug)
            {
                var err = GL.GetError();
                if (err != ErrorCode.NoError)
                    Console.WriteLine($"[GL ERROR] after SetUniform('{name}') → {err}");
            }
        }

        /// <summary>
        /// Activates the shader and binds textures.
        /// </summary>
        public void UseShader()
        {
            foreach (var kv in textures)
            {
                TextureUnit unit = TextureUnit.Texture0 + kv.Key;

                if (kv.Value?.Handle == 0)
                {
                    Console.WriteLine($"[Material WARNING] Texture unit {kv.Key} is null or uninitialized.");
                    continue;
                }

                kv.Value.Use(unit);
            }

            shader.Use();
        }

        public void Dispose()
        {
            shader?.Dispose();
        }

        private int GetTextureUnitFor(string name)
        {
            if(name.Contains("pointCubeMaps"))
                return 4;
            
            return name switch
            {
                "material.diffTex" => 0,
                "material.specTex" => 1,
                "shadowMap"        => 2,
                "cubeMap"          => 3,
                /*"pointCubeMaps"    => 4,*/
                _ => -1 // unknown name → error
            };
        }
    }
}
