using YinYang.Components;
using YinYang.Shapes;
using OpenTK.Mathematics;
using YinYang.Materials;
using YinYang.Rendering;

namespace YinYang;

/// <summary>
/// Provides methods to load different game objects.
/// </summary>
public static class GameObjectFactory
{
    /// <summary>
    /// Creates a simple triangle GameObject with default material and mesh.
    /// </summary>
    /// <param name="gameInstance">The parent game instance.</param>
    /// <returns>A GameObject representing a triangle.</returns>
    public static GameObject CreateTriangle(Game gameInstance)
    {
        Material triangleMaterial = new Material("Shaders/shader.vert", "Shaders/shader.frag");
        Renderer triangleRenderer = new Renderer(triangleMaterial, new TriangleMesh());
        GameObject triangleObject = new GameObject(gameInstance)
        {
            Renderer = triangleRenderer
        };
        triangleObject.Transform.Position = new Vector3(0, 0, -5);
        return triangleObject;
    }

    /// <summary>
    /// Creates a textured cube GameObject with a wall texture and shader material.
    /// </summary>
    /// <param name="gameInstance">The parent game instance.</param>
    /// <returns>A GameObject representing a textured cube.</returns>
    public static GameObject CreateCube(Game gameInstance)
    {
        Texture wallTexture = new Texture("Textures/wall.jpg");
        var uniforms = new Dictionary<string, object> { { "texture0", wallTexture } };
        Material cubeMaterial = new Material("Shaders/shader.vert", "Shaders/shader.frag", uniforms);
        Renderer cubeRenderer = new Renderer(cubeMaterial, new CubeMesh());
        GameObject cubeObject = new GameObject(gameInstance)
        {
            Renderer = cubeRenderer
        };
        cubeObject.Transform.Position = new Vector3(0, 0, 0);
        //cubeObject.AddComponent<MoveObjectBehaviour>();
        return cubeObject;
    }
    
    public static (GameObject, Mesh) CreateTBNObjModel(Game gameInstance, string modelName)
    {
        var loader = new OBJLoader();
        loader.Load($"Models/{modelName}.obj");
        loader.ComputeTangentSpace();
        var mesh = loader.BuildMesh();

        var go = new GameObject(gameInstance);
        return (go, mesh);
    }

    /// <summary>
    /// Loads a 3D model from an OBJ file, computes smooth normals for lighting,
    /// and builds a GameObject along with its corresponding Mesh.
    /// </summary>
    /// <param name="gameInstance">The game instance that owns this object.</param>
    /// <param name="modelName">The filename of the model to load (without .obj extension).</param>
    /// <returns>A tuple containing the GameObject and the Mesh.</returns>
    public static (GameObject, Mesh) CreateObjModel(Game gameInstance, string modelName)
    {
        // Load the model data from the OBJ file
        var objLoader = new OBJLoader();
        objLoader.Load($"Models/{modelName}.obj");
    
        // Extract relevant geometry data into a model structure
        var modelData = new Model
        {
            Vertices = objLoader.Vertices,
            TextureCoords = objLoader.TextureCoords,
            Indices = objLoader.Indices.Select(i => (uint)i).ToList(), // Convert to uint
            TextureIndices = objLoader.TextureIndices,
            NormalIndices = objLoader.NormalIndices
        };
    
        // Step 1: Calculate smooth normals per vertex
        var smoothNormals = ComputeSmoothNormals(modelData);
    
        // Step 2: Build a final vertex buffer and index list
        var (finalVertexData, finalIndices) = BuildVertexBuffer(modelData, smoothNormals);
    
        // Create a mesh using the packed vertex data (8 floats per vertex)
        Mesh mesh = new Mesh(finalVertexData.ToArray(), finalIndices.ToArray(), 8);
    
        // Create a GameObject that can hold this mesh
        GameObject modelObject = new GameObject(gameInstance);
    
        // Return the GameObject and Mesh as a tuple
        return (modelObject, mesh);
    }
    
    public static Mesh CreateModel(string modelName)
    {
        // Load the model data from the OBJ file
        var objLoader = new OBJLoader();
        objLoader.Load($"Models/{modelName}.obj");

        // Extract relevant geometry data into a model structure
        var modelData = new Model
        {
            Vertices = objLoader.Vertices,
            TextureCoords = objLoader.TextureCoords,
            Indices = objLoader.Indices.Select(i => (uint)i).ToList(), // Convert to uint
            TextureIndices = objLoader.TextureIndices,
            NormalIndices = objLoader.NormalIndices
        };

        // Step 1: Calculate smooth normals per vertex
        var smoothNormals = ComputeSmoothNormals(modelData);

        // Step 2: Build a final vertex buffer and index list
        var (finalVertexData, finalIndices) = BuildVertexBuffer(modelData, smoothNormals);

        // Create a mesh using the packed vertex data (8 floats per vertex)
        Mesh mesh = new Mesh(finalVertexData.ToArray(), finalIndices.ToArray(), 8);

        // Return the GameObject and Mesh as a tuple
        return mesh;
    }

    public static Mesh CreateTBNModel(string modelName)
    {
        var loader = new OBJLoader();
        loader.Load($"Models/{modelName}.obj");
        loader.ComputeTangentSpace();
        var mesh = loader.BuildMesh();
        
        return mesh;
    }

    /// <summary>
    /// Computes smooth normals for a model by averaging the face normals
    /// of all triangles sharing each vertex.
    /// </summary>
    /// <param name="modelData">The model data with vertex positions and triangle indices.</param>
    /// <returns>A dictionary mapping each vertex index to a smoothed normal vector.</returns>
    private static Dictionary<int, Vector3> ComputeSmoothNormals(Model modelData)
    {
        // Stores the sum of all normals per vertex
        var accumulatedNormals = new Dictionary<int, Vector3>();

        // Stores how many normals have been added per vertex (for averaging)
        var normalContributionCounts = new Dictionary<int, int>();

        // Loop through each triangle (every 3 indices)
        for (int index = 0; index < modelData.Indices.Count; index += 3)
        {
            // Extract the three vertex indices of the triangle
            int index0 = (int)modelData.Indices[index];
            int index1 = (int)modelData.Indices[index + 1];
            int index2 = (int)modelData.Indices[index + 2];

            // Get the 3D positions of each vertex
            var vertex0 = modelData.Vertices[index0];
            var vertex1 = modelData.Vertices[index1];
            var vertex2 = modelData.Vertices[index2];

            // Compute two edge vectors of the triangle
            var edge1 = vertex1 - vertex0;
            var edge2 = vertex2 - vertex0;

            // Compute the face normal (cross product of the edges, normalized)
            var faceNormal = Vector3.Cross(edge1, edge2).Normalized();

            // Accumulate this normal for each vertex of the triangle
            foreach (int vertexIndex in new[] { index0, index1, index2 })
            {
                // If this is the first time we're adding a normal for this vertex else add to the existing normal
                if (accumulatedNormals.TryAdd(vertexIndex, faceNormal))
                {
                    normalContributionCounts[vertexIndex] = 1;
                }
                else
                {
                    accumulatedNormals[vertexIndex] += faceNormal;
                    normalContributionCounts[vertexIndex]++;
                }
            }
        }

        // Average the accumulated normals per vertex
        foreach (var entry in accumulatedNormals.ToList())
        {
            // Retrieve the vertex index and summed normal
            int vertexIndex = entry.Key;
            var summedNormal = entry.Value;
            int count = normalContributionCounts[vertexIndex];

            // Normalize the average to get the final smooth normal
            accumulatedNormals[vertexIndex] = (summedNormal / count).Normalized();
        }

        return accumulatedNormals;
    }

    /// <summary>
    /// Packs vertex position, texture coordinates, and smooth normals
    /// into a flat vertex buffer, and builds a clean index list.
    /// </summary>
    /// <param name="modelData">The raw geometry data from the loaded model.</param>
    /// <param name="smoothNormals">Computed smooth normals for each vertex.</param>
    /// <returns>
    /// A tuple containing:
    /// - A float list of interleaved vertex attributes (pos, uv, normal),
    /// - A list of indices used to draw the mesh.
    /// </returns>
    private static (List<float> vertexData, List<uint> indices) BuildVertexBuffer(
        Model modelData,
        Dictionary<int, Vector3> smoothNormals)
    {
        // Final interleaved vertex buffer (position, UV, normal)
        List<float> vertexData = new();

        // Final index list used for rendering
        List<uint> indexList = new();

        // Keeps track of already added (vertexIndex, texCoordIndex) pairs
        Dictionary<(int vertexIndex, int texCoordIndex), uint> vertexMap = new();

        // Loop through every original index in the model
        for (int i = 0; i < modelData.Indices.Count; i++)
        {
            int vertexIndex = (int)modelData.Indices[i];

            // Get the texture coordinate index, or default to 0 if not available
            int texCoordIndex = modelData.TextureIndices.Count > i
                ? modelData.TextureIndices[i]
                : 0;

            // Create a key that uniquely identifies a vertex + UV combination
            var uniqueKey = (vertexIndex, texCoordIndex);

            // If we've already packed this vertex combo, reuse the index
            if (!vertexMap.TryGetValue(uniqueKey, out uint mappedIndex))
            {
                // Get vertex position
                var position = modelData.Vertices[vertexIndex];

                // Get texture coordinate or default to (0,0) if missing
                var texCoord = modelData.TextureCoords.Count > texCoordIndex
                    ? modelData.TextureCoords[texCoordIndex]
                    : Vector2.Zero;

                // Get the smoothed normal for the vertex
                var normal = smoothNormals[vertexIndex];

                // Add position (x, y, z)
                vertexData.Add(position.X);
                vertexData.Add(position.Y);
                vertexData.Add(position.Z);

                // Add texture coordinate (u, v)
                vertexData.Add(texCoord.X);
                vertexData.Add(texCoord.Y);

                // Add normal (x, y, z)
                vertexData.Add(normal.X);
                vertexData.Add(normal.Y);
                vertexData.Add(normal.Z);

                // Compute the new vertex index
                mappedIndex = (uint)(vertexData.Count / 8 - 1);

                // Store it so we can reuse if needed
                vertexMap[uniqueKey] = mappedIndex;
            }

            // Add index to the final list
            indexList.Add(mappedIndex);
        }

        return (vertexData, indexList);
    }
}