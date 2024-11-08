using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Template
{
    public class Camera
    {
        public Camera(Vector3 position, float ratio)
        {
            Position = position;
            Ratio = ratio;
            UpdateVectors();
        }
        public Vector3 Position { get; set; }
        public float Ratio { get; set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Forward { get; private set; }
        private float _yaw = -MathHelper.PiOver2; // Yaw is initialized to -90 degrees
        private float _pitch = 0.0f;
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                _pitch = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        private void UpdateVectors()
        {
            Forward = new Vector3(
                MathF.Cos(_pitch) * MathF.Cos(_yaw),
                MathF.Sin(_pitch),
                MathF.Cos(_pitch) * MathF.Sin(_yaw)
            );
            Forward = Vector3.Normalize(Forward);
            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
        }
        public Matrix4 ViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Forward, Up);
        }
        public Matrix4 ProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), Ratio, 0.1f, 1000);
        }
        public void HandleInput(KeyboardState keyboardState, float deltaTime)
        {
            const float speed = 10f;
            const float sensitivity = 10f; // Increased sensitivity for more noticeable rotation
            if (keyboardState.IsKeyDown(Keys.W))
                Position += Forward * speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.S))
                Position -= Forward * speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.A))
                Position -= Right * speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.D))
                Position += Right * speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.Space))
                Position += Up * speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.LeftShift))
                Position -= Up * speed * deltaTime;
            // Handle rotation
            if (keyboardState.IsKeyDown(Keys.Left))
                Yaw -= sensitivity * deltaTime;
            if (keyboardState.IsKeyDown(Keys.Right))
                Yaw += sensitivity * deltaTime;
            if (keyboardState.IsKeyDown(Keys.Down))
                Pitch -= sensitivity * deltaTime;
            if (keyboardState.IsKeyDown(Keys.Up))
                Pitch += sensitivity * deltaTime;
            // Clamp pitch to prevent flipping
            Pitch = MathHelper.Clamp(Pitch, -89f, 89f);
        }
    }
}