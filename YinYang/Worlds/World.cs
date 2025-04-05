using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Lights;
using YinYang.Shapes;

namespace YinYang.Worlds;

public abstract class World
{
    public readonly List<GameObject> GameObjects = [];
    public readonly Game Game;

    public virtual string DebugLabel => "Combined";
    public string WorldName { get; set; }

    private Camera camera;

    public Vector3 SunDirection = new Vector3(-0.2f, -1.0f, -0.3f); //Set default sun direction;
    public Vector3 SunColor = new Vector3(2f, 2f, 1.8f); //Set default sun direction;
    public Color4 SkyColor = Color4.CornflowerBlue;
    
    public DirectionalLight DirectionalLight;
    public List<PointLight> PointLights = new();
    public List<SpotLight> SpotLights = new();

    private Shader dir_depthShader;
    private int dir_depthMapFBO;
    public Texture dir_depthMap;
    
    public Shader debugShader;
    public QuadMesh quadMesh;
    private int dirShadowMap;
    private int dirShadowResolution = 2048;
    
    private Shader point_depthShader;
    private int point_depthMapFBO;
    public Texture point_depthMap;
    
    private int pointShadowMap;
    private int pointShadowResolution = 2048;
    private float depthNearPlane = 0.1f;
    public float depthFarPlane = 50.0f;

    protected World(Game game)
    {
        Game = game;
        SetupCamera();
        
        //Basic Sun
        DirectionalLight = new DirectionalLight(this, Color4.LightYellow, 1);
        DirectionalLight.Transform.Rotation = SunDirection;
        DirectionalLight.Transform.Position = new Vector3(2, 5, -2);
        DirectionalLight.UpdateVisualizer(this);
    }

    /// <summary>
    /// Method used for constructing initial world
    /// </summary>
    protected virtual void ConstructWorld() { }
    
    public virtual void HandleInput(KeyboardState input) { }

    public Vector3 GetSkyColor()
    {
        return new Vector3(SkyColor.R, SkyColor.G, SkyColor.B);
    }
    
    public Vector3 GetSkyColor(float intensity)
    {
        return new Vector3(SkyColor.R + intensity, SkyColor.G + intensity, SkyColor.B + intensity);
    }

    public void LoadWorld()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.FramebufferSrgb); //Enabling gamma correction - handled by OpenGL
        GL.ClearColor(SkyColor);

        SetupDirDepthMaps();

        point_depthShader = new Shader("Shaders/PointDepth.vert", "Shaders/PointDepth.frag", "Shaders/PointDepth.geom");
        point_depthMapFBO = GL.GenFramebuffer();
        
        pointShadowMap = GL.GenTexture();
        GL.BindTexture(TextureTarget.TextureCubeMap, pointShadowMap);

        for (int i = 0; i < 6; ++i)
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.DepthComponent, pointShadowResolution, pointShadowResolution, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

        // attach depth texture as FBO's depth buffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, point_depthMapFBO);
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, pointShadowMap, 0);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        point_depthMap = new Texture(pointShadowMap);

        //SetupDebugQuad();
        
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        
        ConstructWorld();
    }

    private void SetupDirDepthMaps()
    {
        //Depth shader
        dir_depthShader = new Shader("Shaders/DirDepth.vert", "Shaders/DirDepth.frag");
        dir_depthMapFBO = GL.GenFramebuffer();
        
        // Opret depth texture
        dirShadowMap = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, dirShadowMap);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent,
            dirShadowResolution, dirShadowResolution, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

        // Konfigurer texture parametre
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 1, 1, 1, 1 });

        // Bind til framebuffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, dir_depthMapFBO);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, dirShadowMap, 0);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        
        dir_depthMap = new Texture(dirShadowMap);
    }

    public void UpdateWorld(FrameEventArgs args)
    {
        foreach (var obj in GameObjects)
        {
            obj.Update(args);
        }
    }

    public void DrawWorld(FrameEventArgs args, int debugMode)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.CullFace(TriangleFace.Front);
        
        Matrix4 lightSpaceMatrix = DirShadowMap();

        PointLight refLight = PointLights[0];
        
        //Create cubemap matrices
        Matrix4 shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f),
            pointShadowResolution / pointShadowResolution, depthNearPlane, depthFarPlane);

        List<Matrix4> shadowTransforms = new List<Matrix4>();
        Vector3 lightPos = refLight.Transform.Position;
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(1.0f, 0.0f, 0.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 1.0f, 0.0f),  new Vector3(0.0f,  0.0f,  1.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f,  0.0f, -1.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, 1.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
        
        //Render to cubemap texture
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, point_depthMapFBO);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        point_depthShader.Use();
        for (int i = 0; i < 6; i++)
            point_depthShader.SetMatrix($"shadowMatrices[{i}]", shadowTransforms[i]);
        point_depthShader.SetFloat("far_plane", depthFarPlane);
        point_depthShader.SetVector3("lightPos", lightPos);
        GL.Viewport(0, 0, pointShadowResolution, pointShadowResolution);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, point_depthMapFBO);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        GameObjects.ForEach(x => x.RenderDepth(point_depthShader));
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        // reset viewport
        GL.CullFace(TriangleFace.Back);
        GL.Viewport(0, 0, Game.Size.X, Game.Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        //Draw scene from Cam perspective
        Matrix4 viewProjection = camera.GetViewProjection();
        
        Vector3 cameraPos = camera.Position;
        foreach (var obj in GameObjects)
        {
            obj.Draw(viewProjection, lightSpaceMatrix, camera, this, debugMode);
        }
    }

    private Matrix4 DirShadowMap()
    {
        // GL.CullFace(TriangleFace.Front);
        
        Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-10.0f, 10.0f, -10, 10, depthNearPlane, depthFarPlane);
        Matrix4 lightView = Matrix4.LookAt(new Vector3(DirectionalLight.Transform.Position),
            new Vector3(DirectionalLight.Transform.Position + DirectionalLight.Transform.Rotation),
            new Vector3(0.0f, 1.0f, 0.0f));
        Matrix4 lightSpaceMatrix = lightView * lightProjection;
        
        dir_depthMap.Use();
        dir_depthShader.Use();
        dir_depthShader.SetMatrix("lightSpaceMatrix", lightSpaceMatrix);
        
        GL.Viewport(0, 0, dirShadowResolution, dirShadowResolution);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, dir_depthMapFBO);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        
        GameObjects.ForEach(x => x.RenderDepth(dir_depthShader));
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        
        return lightSpaceMatrix;
    }

    public void UnloadWorld()
    {
        foreach (var obj in GameObjects)
        {
            if (obj.Renderer != null)
            {
                obj.Renderer.Mesh?.Dispose();

                if (obj.Renderer.Material is IDisposable disposableMat)
                {
                    disposableMat.Dispose();
                }
            }
        }

        foreach (var obj in GameObjects)
        {
            obj.Dispose();
        }
        
        // todo: visualizer dispose
        
        dir_depthShader.Dispose();

        GameObjects.Clear();
    }
    
    /// <summary>
    /// Sets up the main camera.
    /// </summary>
    private void SetupCamera()
    {
        GameObject cameraObject = new GameObject(Game);
        cameraObject.AddComponent<Camera>(60.0f, (float)Game.Size.X, (float)Game.Size.Y, 0.3f, 1000.0f);
        cameraObject.AddComponent<CamMoveBehavior>();
        camera = cameraObject.GetComponent<Camera>();
        GameObjects.Add(cameraObject);

        //Grab focus for cursor, locking it to window
        Game.CursorState = CursorState.Grabbed;
    }
    
    private void RenderDebugQuad()
    {
        // Gem eksisterende viewport
        int[] fullViewport = new int[4];
        GL.GetInteger(GetPName.Viewport, fullViewport);

        // Sæt viewport til 1/4 skærmstørrelse (nederste venstre hjørne)
        GL.Viewport(0, 0, Game.Size.X / 4, Game.Size.Y / 4);

        debugShader.Use();
        debugShader.SetInt("depthMap", 0);
        dir_depthMap.Use(); // binder shadowMap til Texture0
        quadMesh.Draw();
        
        // Console.WriteLine("Rendering debug quad");

        // Gendan viewport til fuld skærm
        GL.Viewport(fullViewport[0], fullViewport[1], fullViewport[2], fullViewport[3]);
    }
    
    private void SetupDebugQuad()
    {
        debugShader = new Shader("Shaders/shadowDebugQuad.vert", "Shaders/shadowDebugQuad.frag");
        quadMesh = new QuadMesh();
    }
}