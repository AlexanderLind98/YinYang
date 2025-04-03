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
    }
}
