using OpenTK.Mathematics;
using static Template.SceneGraph;
namespace Template
{
    public class SceneGraph
    {
        // Node class representing each mesh in the hierarchy
        public class Node
        {
            public Mesh Mesh { get; set; }
            public Texture Texture { get; set; }
            public Transform Transform { get; set; }
            public List<Node> Children { get; set; } = new List<Node>();
            public bool Root;
            public Node(Mesh mesh, Texture texture, Transform transform)
            {
                Mesh = mesh;
                Texture = texture;
                Transform = transform;
                _ = Root;
            }
        }

        public class Transform
        {
            public Vector3 Translation = (0, 0, 0);
            public Vector3 Rotation = (0, 0, 0);
            public Vector3 Scale = (1, 1, 1);

            public Matrix4 LocalMatrix = Matrix4.Identity;
            public Matrix4 GlobalMatrix = Matrix4.Identity;
            public Transform(Vector3 translation, Vector3 rotation, Vector3 scale, Matrix4 localMatrix)
            {
                Translation = translation;
                Rotation = rotation;
                Scale = scale;

                LocalMatrix = localMatrix;
                _ = GlobalMatrix;
            }
        }
        private Node root;
        // Constructor to initialize the root node
        public SceneGraph(Mesh rootMesh, Texture ?texture, Transform ?transform)
        {
            root = new Node(rootMesh, texture, transform);
            root.Root = true;
        }
        // Method to add a child node
        public void AddChild(Node parentNode, Mesh childMesh, Texture ?texture, Transform ?transform)
        {
            Node childNode = new Node(childMesh, texture, transform);
            parentNode.Children.Add(childNode);
        }
        // Method to find a node by its mesh
        private Node FindNode(Node node, Mesh mesh)
        {
            if (node.Mesh == mesh) return node;
            foreach (var child in node.Children)
            {
                Node result = FindNode(child, mesh);
                if (result != null) return result;
            }
            return null;
        }
        // Method to add a child mesh by finding its parent node
        public void AddChild(Mesh parentMesh, Mesh childMesh, Texture ?texture, Transform ?transform)
        {
            Node parentNode = FindNode(root, parentMesh);
            if (parentNode != null)
            {
                AddChild(parentNode, childMesh, texture, transform);
            }
            else
            {
                throw new KeyNotFoundException("Parent mesh not found in the scene graph.");
            }
        }
        public void MakeLocalModelmatrix(Node node)
        {
            node.Transform.LocalMatrix =
                    Matrix4.CreateScale(node.Transform.Scale)

                    * (Matrix4.CreateRotationY(node.Transform.Rotation.X) 
                    * Matrix4.CreateRotationX(node.Transform.Rotation.Y) 
                    * Matrix4.CreateRotationZ(node.Transform.Rotation.Z))

                    *Matrix4.CreateTranslation(node.Transform.Translation);
        }
        public void Update(Node node, Matrix4 lastMatrix)
        {
            MakeLocalModelmatrix(node);
            if (node.Root == false)
            {
                node.Transform.GlobalMatrix = node.Transform.LocalMatrix * lastMatrix;
            }
            else node.Transform.GlobalMatrix = node.Transform.LocalMatrix;

            foreach (var child in node.Children)
            {
                Update(child, node.Transform.GlobalMatrix);
            }
        }
        // Render method to recursively process nodes in the tree
        public void Render(Shader shader, Matrix4 cameraMatrix, Light[] dirLight, Light[] pointLight, Light[] spotLight, Vector3 camera)
        {
            Update(root, root.Transform.LocalMatrix);
            RenderNode(root, shader, cameraMatrix, root.Transform.LocalMatrix, root.Texture, dirLight, pointLight, spotLight, camera);
        }
        private void RenderNode(Node node, Shader shader, Matrix4 parentMatrix, Matrix4 parentWorldMatrix, Texture ?texture, Light[] dirLight, Light[] pointLight, Light[] spotLight, Vector3 camera)
        {
            Matrix4 currentMatrix = node.Transform.GlobalMatrix * parentMatrix;
            node.Mesh.Render(shader, currentMatrix, node.Transform.GlobalMatrix, texture, dirLight, pointLight, spotLight, camera);
            foreach (var child in node.Children)
            {
                RenderNode(child, shader, currentMatrix, node.Transform.GlobalMatrix, child.Texture, dirLight, pointLight, spotLight, camera);
            }
        }
    }
}