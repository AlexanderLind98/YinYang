using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Materials;
using YinYang.Shapes;

namespace YinYang.Rendering;

/// <summary>
/// Renders radial light shafts from the sun's screen position to simulate volumetric lighting.
/// </summary>
public class GodRayPass : RenderPass
{
    private Material lightShaftMaterial;
    private Material maskMaterial;
    private int lightShaftFBO;
    private int lightShaftTexture;
    private int blurredLightShaftFBO;
    private int blurredLightShaftTexture;
    private bool initialized = false;

    private QuadMesh screenQuad = new();

    public override string Name => "GodRayPass";

    public override Matrix4? Execute(RenderContext context, ObjectManager objects)
    {
        if (!initialized)
        {
            Init(context.Camera.RenderWidth, context.Camera.RenderHeight);
            initialized = true;
        }

        // STEP 1: Render occlusion mask into lightShaftTexture (black geometry on white background)
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, lightShaftFBO);
        GL.Viewport(0, 0, context.Camera.RenderWidth / 2, context.Camera.RenderHeight / 2);
        GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f); // White background
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);

        foreach (var obj in objects.GameObjects)
        {
            if (obj.Renderer == null) continue;

            Matrix4 model = obj.Transform.CalculateModel();
            Matrix4 mvp = model * context.ViewProjection;

            maskMaterial.UseShader();
            maskMaterial.SetUniform("mvp", mvp);
            maskMaterial.SetUniform("transform", model);
            maskMaterial.SetUniform("color", Vector3.Zero);
            maskMaterial.UpdateUniforms();

            obj.Renderer.Mesh.Draw(); 
        }

        // STEP 2: Apply radial blur into blurredLightShaftTexture
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, blurredLightShaftFBO);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Disable(EnableCap.DepthTest);

        lightShaftMaterial.UseShader();
        lightShaftMaterial.SetUniform("sceneTex", new Texture(lightShaftTexture));
        lightShaftMaterial.SetUniform("lightPos", ProjectSunToScreen(context));
        lightShaftMaterial.UpdateUniforms();

        screenQuad.Draw();

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, context.Camera.RenderWidth, context.Camera.RenderHeight);

        return context.LightSpaceMatrix;
    }

    private Vector2 ProjectSunToScreen(RenderContext context)
    {
        Vector4 sunWorldPos = new Vector4(context.World.DirectionalLight.Transform.Position, 1.0f);
        Vector4 clip = sunWorldPos * context.ViewProjection;
        Vector3 ndc = clip.Xyz / clip.W;
        return new Vector2(ndc.X * 0.5f + 0.5f, ndc.Y * 0.5f + 0.5f);
    }

    private void Init(int width, int height)
    {
        lightShaftFBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, lightShaftFBO);

        lightShaftTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, lightShaftTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width / 2, height / 2, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D, lightShaftTexture, 0);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
            throw new Exception("GodRayPass framebuffer not complete");
        }

        // Create blur target
        blurredLightShaftFBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, blurredLightShaftFBO);

        blurredLightShaftTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, blurredLightShaftTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width / 2, height / 2, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D, blurredLightShaftTexture, 0);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
            throw new Exception("GodRayPass blur framebuffer not complete");
        }

        lightShaftMaterial = new Material("Shaders/Fullscreen.vert", "Shaders/godRays.frag");
        maskMaterial = new Material("Shaders/unlitgeneric.vert", "Shaders/unlitgeneric.frag");
    }

    public override void Dispose()
    {
        GL.DeleteFramebuffer(lightShaftFBO);
        GL.DeleteFramebuffer(blurredLightShaftFBO);
        GL.DeleteTexture(lightShaftTexture);
        GL.DeleteTexture(blurredLightShaftTexture);
        lightShaftMaterial?.Dispose();
        maskMaterial?.Dispose();
    }

    public int LightShaftTexture => blurredLightShaftTexture;
    public int LightShaftMaskTexture => lightShaftTexture;
}
