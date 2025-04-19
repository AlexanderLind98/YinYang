using System.Globalization;
using OpenTK.Mathematics;

namespace YinYang
{
    /// <summary>
    /// A loader for OBJ files, supporting vertices, texture coordinates, normals, and faces.
    /// </summary>
    public class OBJLoader
    {
        public List<Vector3> Vertices { get; private set; } = new();
        public List<Vector2> TextureCoords { get; private set; } = new();
        public List<Vector3> Normals { get; private set; } = new();

        public List<int> Indices { get; private set; } = new();
        public List<int> TextureIndices { get; private set; } = new();
        public List<int> NormalIndices { get; private set; } = new();
        public List<Vector3> Tangents { get; private set; } = new();
        public List<Vector3> Bitangents { get; private set; } = new();  

        /// <summary>
        /// Loads an OBJ file from the specified file path.
        /// </summary>
        /// <param name="filePath">The path to the OBJ file.</param>
        public void Load(string filePath)
        {
            using StreamReader reader = new StreamReader(filePath);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    continue;

                try
                {
                    switch (parts[0])
                    {
                        case "v":
                            Vertices.Add(new Vector3(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture)
                            ));
                            break;

                        case "vt":
                            TextureCoords.Add(new Vector2(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture)
                            ));
                            break;

                        case "vn":
                            Normals.Add(new Vector3(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture)
                            ));
                            break;

                        case "f":
                            if (parts.Length < 4)
                            {
                                Console.WriteLine($"Warning: Invalid face line: {line}");
                                continue;
                            }

                            var vIndices = new List<int>();
                            var vtIndices = new List<int>();
                            var vnIndices = new List<int>();

                            for (int i = 1; i < parts.Length; i++)
                            {
                                var subParts = parts[i].Split('/');

                                int vi = int.Parse(subParts[0]) - 1;
                                vIndices.Add(vi);

                                if (subParts.Length > 1 && !string.IsNullOrWhiteSpace(subParts[1]))
                                {
                                    int vti = int.Parse(subParts[1]) - 1;
                                    vtIndices.Add(vti);
                                }

                                if (subParts.Length > 2 && !string.IsNullOrWhiteSpace(subParts[2]))
                                {
                                    int vni = int.Parse(subParts[2]) - 1;
                                    vnIndices.Add(vni);
                                }
                            }

                            // Triangulér med triangle fan
                            for (int i = 1; i < vIndices.Count - 1; i++)
                            {
                                Indices.Add(vIndices[0]);
                                Indices.Add(vIndices[i]);
                                Indices.Add(vIndices[i + 1]);

                                if (vtIndices.Count == vIndices.Count)
                                {
                                    TextureIndices.Add(vtIndices[0]);
                                    TextureIndices.Add(vtIndices[i]);
                                    TextureIndices.Add(vtIndices[i + 1]);
                                }

                                if (vnIndices.Count == vIndices.Count)
                                {
                                    NormalIndices.Add(vnIndices[0]);
                                    NormalIndices.Add(vnIndices[i]);
                                    NormalIndices.Add(vnIndices[i + 1]);
                                }
                            }
                            
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing line: {line}\nException: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Computes tangents and bitangents for each vertex index based on UV mapping.
        /// </summary>
        public void ComputeTangentSpace()
        {
            // Create temp lists to store tangents and bitangents
            Tangents = new Vector3[Vertices.Count].ToList();
            Bitangents = new Vector3[Vertices.Count].ToList();
            
            // Iterate through each face of the model
            for (int i = 0; i < Indices.Count; i += 3)
            {
                // Get the indices of the vertices of the face
                uint index0 = (uint)Indices[i];
                uint index1 = (uint)Indices[i + 1];
                uint index2 = (uint)Indices[i + 2];

                // Get the vertices and texture coordinates of the face
                Vector3 v0 = Vertices[(int)index0];
                Vector3 v1 = Vertices[(int)index1];
                Vector3 v2 = Vertices[(int)index2];

                Vector2 uv0 = TextureCoords[TextureIndices[(int)index0]];
                Vector2 uv1 = TextureCoords[TextureIndices[(int)index1]];
                Vector2 uv2 = TextureCoords[TextureIndices[(int)index2]];

                // Calculate the edges of the triangle
                Vector3 edge1 = v1 - v0;
                Vector3 edge2 = v2 - v0;

                // Calculate the delta UV coordinates
                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                // Calculate the determinant
                float det = deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X;
                
                if (Math.Abs(det) < float.Epsilon)
                    continue; // Degenerate triangle

                float f = 1.0f / det;

                // Calculate the tangent and bitangent vectors
                Vector3 tangent = f * (deltaUV2.Y * edge1 - deltaUV1.Y * edge2);
                Vector3 bitangent = f * (-deltaUV2.X * edge1 + deltaUV1.X * edge2);

                // Add the tangent and bitangent to the respective lists
                Tangents[(int)index0] += tangent;
                Tangents[(int)index1] += tangent;
                Tangents[(int)index2] += tangent;

                Bitangents[(int)index0] += bitangent;
                Bitangents[(int)index1] += bitangent;
                Bitangents[(int)index2] += bitangent;
            }
            
                // Normalize the tangent and bitangent vectors
                for (int i = 0; i < Vertices.Count; i++)
                {
                    Tangents[i] = Vector3.Normalize(Tangents[i]);
                    Bitangents[i] = Vector3.Normalize(Bitangents[i]);
                }
        }

        /// <summary>
        /// Converts the loaded and processed data into a flat Mesh ready for GPU upload.
        /// </summary>
        public Mesh BuildMesh()
        {
            var vertexData = new List<float>();
            var newIndices = new List<uint>();

            for (int i = 0; i < Indices.Count; i++)
            {
                int vIdx = Indices[i];
                int uvIdx = TextureIndices[i];
                int nIdx = NormalIndices[i];

                Vector3 pos = Vertices[vIdx];
                Vector2 uv = TextureCoords[uvIdx];
                Vector3 normal = Normals[nIdx];
                Vector3 tangent = Tangents[vIdx];
                Vector3 bitangent = Bitangents[vIdx];

                vertexData.AddRange(new float[]
                {
                    pos.X, pos.Y, pos.Z,
                    uv.X, uv.Y,
                    normal.X, normal.Y, normal.Z,
                    tangent.X, tangent.Y, tangent.Z,
                    bitangent.X, bitangent.Y, bitangent.Z
                });

                newIndices.Add((uint)i); // ét vertex per hjørne
            }

            return new Mesh(
                vertexData.ToArray(),
                newIndices.Select(i => (uint)i).ToArray(),
                14
            );
        }
        
        public static Mesh LoadWithTBN(string filePath)
        {
            var loader = new OBJLoader();
            loader.Load(filePath);
            loader.ComputeTangentSpace();
            return loader.BuildMesh();
        }

    }
}
