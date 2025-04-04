using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace YinYang;

public class Shader : IDisposable
{
    public int Handle;
    private readonly string name;
    
    public string Name => name;
    
    public Shader(string vertexPath, string fragmentPath)
    {
        name = Path.GetFileNameWithoutExtension(vertexPath) + "+" + Path.GetFileNameWithoutExtension(fragmentPath);
        
        // The first step in creating a shader is to load the source code from a file.
        string vertexSource = File.ReadAllText(vertexPath);
        
        // The source code is then compiled into a shader object.
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        
        // The source code is then loaded into the shader object.
        GL.ShaderSource(vertexShader, vertexSource);
        
        // The shader object is then compiled.
        CompileShader(vertexShader);
        
        // The same process is then repeated for the fragment shader.
        string fragmentSource = File.ReadAllText(fragmentPath);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        CompileShader(fragmentShader);

        // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        // To do this, create a program...
        Handle = GL.CreateProgram();

        // Attach both shaders...
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        // And then link them together.
        LinkProgram(Handle);

        // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
        // Detach them, and then delete them, to free up memory.
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }
    
    private void CompileShader(int shader)
    {
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
        }
    }
    
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }

    private void LinkProgram(int program)
    {
        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Error occurred whilst linking Program({program}). Info log: {infoLog}");
        }
    }
    
    public void Use()
    {
        GL.UseProgram(Handle);
    }

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            GL.DeleteProgram(Handle);
            disposedValue = true;
        }
    }

    ~Shader()
    {
        if (disposedValue == false)
        {
            Console.WriteLine($"GPU Resource leak in shader: {name} — Did you forget to call Dispose()?");
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }
    
    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, (float)value);
    }
    
    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(location, value);
    }

    public void SetMatrix(string name, Matrix4 transform)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(location, true, ref transform);
    }
}