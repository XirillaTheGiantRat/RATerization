using OpenTK.Mathematics;

namespace Template
{
    public class Light
    {
        // standard light values
        public Vector3 position;
        public Vector3 color;
        public Vector3 direction;
        // attenuation values
        public float constant;
        public float linear;
        public float quadratic;
        // light type strenght values
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        // spotLight values
        public float angle;
        public float outerAngle;
        // light strenght value (coming soon (tm))

        public Light(Vector3 position, Vector3 color, Vector3 direction, float constant, float linear, float quadratic, Vector3 ambient, Vector3 diffuse, Vector3 specular, float angle, float outerAngle)
        {
            this.position = position;
            this.color = color;
            this.direction = direction;
            this.constant = constant;
            this.linear = linear;
            this.quadratic = quadratic;
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.angle = angle;
            this.outerAngle = outerAngle;
        }
    }
}
