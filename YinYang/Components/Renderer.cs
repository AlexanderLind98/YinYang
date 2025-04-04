using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using YinYang.Materials;
using YinYang.Worlds;

namespace YinYang.Components
{
    public class Renderer
    {
        private Material _material;
        private bool DepthTest = true;

        public Material Material
        {
            get => _material;
            set
            {
                if (_material != null && _material != value)
                {
                    (_material as IDisposable)?.Dispose();
                }
                _material = value;
            }
        }

        public Mesh Mesh { get; set; }
        
        public Renderer(Material material, Mesh mesh)
        {
            Material = material;
            Mesh = mesh;
        }

        public void Draw(Matrix4 mvp, Matrix4 lightSpaceMatrix, Matrix4 model, Camera camera, int currentDebugMode, World currentWorld)
        {
            Material.UseShader();
            Material.UpdateUniforms();
            Material.SetUniform("mvp", mvp);
            Material.SetUniform("model", model);
            Material.SetUniform("lightSpaceMatrix", lightSpaceMatrix);
            Material.SetUniform("dirShadowMap", currentWorld.dir_depthMap);
            Material.SetUniform("pointShadowMap", currentWorld.point_depthMap);
            Material.SetUniform("far_plane", currentWorld.depthFarPlane);

            Matrix4 normalMatrix = Matrix4.Invert(model);

            Material.SetUniform("normalMatrix", normalMatrix); // Correct normal matrix passed to the shader

            SetSun(currentWorld);
            SpotLights(camera, currentWorld);
            PointLights(currentWorld);

            Material.SetUniform("viewPos", (Vector3)camera.Position);
            Material.SetUniform("debugMode", currentDebugMode);
            
            Mesh.Draw();
        }

        private void PointLights(World currentWorld)
        {
            int numPointLights = currentWorld.PointLights.Count;
            
            Material.SetUniform("numPointLights", numPointLights);
            
            for (int i = 0; i < numPointLights; i++)
            {
                //Position
                Material.SetUniform($"pointLights[{i}].position", currentWorld.PointLights[i].Transform.Position);
                
                //Colors
                Material.SetUniform($"pointLights[{i}].ambient", currentWorld.GetSkyColor() / 255);
                Material.SetUniform($"pointLights[{i}].diffuse", currentWorld.PointLights[i].LightColor);
                Material.SetUniform($"pointLights[{i}].specular", currentWorld.PointLights[i].LightColor);
                
                //Fall-off
                Material.SetUniform($"pointLights[{i}].constant", currentWorld.PointLights[i].Constant);
                Material.SetUniform($"pointLights[{i}].linear", currentWorld.PointLights[i].Linear);
                Material.SetUniform($"pointLights[{i}].quadratic", currentWorld.PointLights[i].Quadratic);
            }
        }

        private void SpotLights(Camera camera, World currentWorld)
        {
            int numSpotLights = currentWorld.SpotLights.Count;
            Material.SetUniform("numSpotLights", numSpotLights);
            
            for (int i = 0; i < numSpotLights; i++)
            {
                //Position & rotation
                if (i == 0) // spotlight 0 is ALWAYS player flashlight!!
                {
                    Material.SetUniform($"spotLights[{i}].position", camera.Position);
                    Material.SetUniform($"spotLights[{i}].direction", Vector3.Normalize(camera.Front));
                }
                else //All other spotlights
                {
                    Material.SetUniform($"spotLights[{i}].position", currentWorld.SpotLights[i].Transform.Position);
                    Material.SetUniform($"spotLights[{i}].direction", currentWorld.SpotLights[i].Transform.Rotation);
                }

                //Cone radius
                Material.SetUniform($"spotLights[{i}].cutOff", (float)MathHelper.Cos(MathHelper.DegreesToRadians(
                    currentWorld.SpotLights[i].InnerRadius)));
                
                Material.SetUniform($"spotLights[{i}].outerCutOff", (float)MathHelper.Cos(MathHelper.DegreesToRadians(
                    currentWorld.SpotLights[i].OuterRadius)));
                
                //Color
                Material.SetUniform($"spotLights[{i}].ambient", currentWorld.GetSkyColor() / 255);
                Material.SetUniform($"spotLights[{i}].diffuse", currentWorld.SpotLights[i].LightColor);
                Material.SetUniform($"spotLights[{i}].specular", currentWorld.SpotLights[i].LightColor);
                
                //Fall-off
                Material.SetUniform($"spotLights[{i}].constant", currentWorld.SpotLights[i].Constant);
                Material.SetUniform($"spotLights[{i}].linear", currentWorld.SpotLights[i].Linear);
                Material.SetUniform($"spotLights[{i}].quadratic", currentWorld.SpotLights[i].Quadratic);
            }
        }

        private void SetSun(World currentWorld)
        {
            Material.SetUniform("dirLight.direction", currentWorld.DirectionalLight.Transform.Rotation);
            Material.SetUniform("dirLight.ambient", currentWorld.GetSkyColor() / 2);
            Material.SetUniform("dirLight.diffuse", currentWorld.DirectionalLight.LightColor);
            Material.SetUniform("dirLight.specular", currentWorld.DirectionalLight.LightColor);
        }

        public void RenderDepth(Shader shader, Matrix4 model)
        {
            shader.SetMatrix("model", model);
            GL.DepthMask(DepthTest);
            Mesh.Draw();
            
            GL.DepthMask(true);
        }
    }
}