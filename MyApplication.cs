using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Template
{
    public class MyApplication
    {
        // member variables
        public Surface screen;                         // background surface for printing etc.
        Mesh? teapot, floor;                           // meshes to draw using OpenGL
        float a = 0;                                   // teapot rotation angle
        readonly Stopwatch timer = new();              // timer for measuring frame duration
        Shader? shader;                                // shader to use for rendering
        Shader? postproc;                              // shader to use for post processing
        Texture? wood, frog;                           // texture to use for rendering
        SceneGraph.Transform[] transformations;        // array of transformations for meshes
        public RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                              // screen filling quad for post processing
        readonly bool useRenderTarget = true;          // required for post processing
        public Camera camera;                          // camera
        Light[] dirLight;                              // array of dirLights to use for rendering
        Light[] pointLight;                            // array of pointLights to use for rendering
        Light[] spotLight;                             // single spotLight to use for rendering
        private SceneGraph sceneGraph;                 // scene graph for managing hierarchy of meshes
        public bool play = false;                      // bool if true moves and changes lights
        Random rnd = new Random();
        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            camera = new Camera(new Vector3(0f, -8f, 30f), screen.width / (float)screen.height);
            // load teapot and floor
            teapot = new Mesh("../../../assets/teapot.obj");
            floor = new Mesh("../../../assets/floor.obj");
            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            // load textures
            wood = new Texture("../../../assets/wood.jpg");
            frog = new Texture("../../../assets/frog.jpg");
            // add array of dirLights
            dirLight = new Light[]
            {
               new Light(new Vector3(10f, 2f, -15f), new Vector3(0.2f, 0.25f, 0.15f), new Vector3(-1f, 0f, -5f), 0f, 0f, 0f, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(1f, 1f, 1f), 0f, 0f)
            };
            // add array of pointLights
            pointLight = new Light[]
            {
               new Light(new Vector3(1f, 0f, 0f), new Vector3(2f, 2f, 2f), new Vector3(0f, 0f, 0f), 1f, 0.09f, 0.05f, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), 0f, 0f),
               new Light(new Vector3(3f, 2f, -1f), new Vector3(0f, 0f, 2f), new Vector3(0f, 0f, 0f), 1f, 0.09f, 0.05f, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), 0f, 0f),
               new Light(new Vector3(1f, 0f, 2f), new Vector3(0f, 2f, 0f), new Vector3(0f, 0f, 0f), 1f, 0.09f, 0.05f, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), 0f, 0f),
               new Light(new Vector3(-2f, 2f, 2f), new Vector3(2f, 0f, 0f), new Vector3(0f, 0f, 0f), 1f, 0.09f, 0.05f, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), 0f, 0f)
            };
            // add array of spotLight
            spotLight = new Light[]
            {
               new Light(new Vector3(20f, -12f, 0f), new Vector3(3.5f, 0.0f, 3.5f), new Vector3(-1f, -1f, 0f), 1f, 0.09f, 0.05f, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), 8f, 10f),
               new Light(new Vector3(-25f, 1f, -15f), new Vector3(5f, 5f, 3f), new Vector3(1f, -1f, 0.8f), 1f, 0.09f, 0.08f, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), 10f, 13f),
               new Light(new Vector3(0f, -1f, 0f), new Vector3(0f, 5f, 5f), new Vector3(-1f, -1f, 0f), 1f, 0.09f, 0.05f, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), 15f, 19f)
            };
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();
            // add array of transformations
            transformations = new SceneGraph.Transform[]
            {
               new SceneGraph.Transform(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(5.0f, 5.0f, 1.0f), Matrix4.Identity),
               new SceneGraph.Transform(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.2f, 0.2f, 1.0f), Matrix4.Identity),
               new SceneGraph.Transform(new Vector3(0.4f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), Matrix4.Identity),
               new SceneGraph.Transform(new Vector3(-0.4f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f , 1.0f), Matrix4.Identity)
            };
            // initialize scene graph with teapot as the root
            sceneGraph = new SceneGraph(floor, wood, transformations[0]);
            sceneGraph.AddChild(floor, teapot, wood, transformations[1]);
            sceneGraph.AddChild(teapot, teapot, wood, transformations[2]);
            sceneGraph.AddChild(teapot, teapot, frog, transformations[3]);
        }
        // tick for background surface
        public void Tick(float deltaTime, KeyboardState keyboardState)
        {
            screen.Clear();
            camera.HandleInput(keyboardState, deltaTime);
        }
        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            // prepare matrix for vertex shader
            float angle90degrees = MathF.PI / 2;
            Matrix4 worldToCamera = camera.ViewMatrix();
            Matrix4 cameraToScreen = camera.ProjectionMatrix();
            // update rotation
            a += 0.0001f * frameDuration;
            if (a > 2 * MathF.PI) a -= 2 * MathF.PI;
            // moves lights and changes colors (pointLights)
            if (play == true)
            {
                for (int i = 0; i < pointLight.Length; i++)
                {
                    pointLight[i].position.X = rnd.NextSingle() * rnd.Next(10);
                    pointLight[i].position.Y = rnd.NextSingle() * rnd.Next(10);
                    pointLight[i].position.Z = rnd.NextSingle();
                    pointLight[i].color = new Vector3(rnd.NextSingle() * rnd.Next(3), rnd.NextSingle() * rnd.Next(2), rnd.NextSingle() * rnd.Next(4));
                }
            }
            if (useRenderTarget && target != null && quad != null)
            {
                // enable render target
                target.Bind();
                // render scene to render target
                if (shader != null && wood != null && frog != null)
                {
                    sceneGraph.Render(shader, worldToCamera * cameraToScreen, dirLight, pointLight, spotLight, camera.Position);
                }
                // render quad
                target.Unbind();
                if (postproc != null)
                    quad.Render(postproc, target.GetTextureID(), new Vector2(screen.width, screen.height));
            }
            else
            {
                // render scene directly to the screen
                if (shader != null && wood != null && frog != null)
                {
                    sceneGraph.Render(shader, worldToCamera * cameraToScreen, dirLight, pointLight, spotLight, camera.Position);
                }
            }
        }
    }
}