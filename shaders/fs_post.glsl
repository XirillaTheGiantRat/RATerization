#version 330

// shader inputs
in vec2 uv;						// fragment uv texture coordinates
in vec2 positionFromBottomLeft;
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform vec2 screen;

// shader output
out vec3 outputColor;

float Vingette(float distance)
{ 
	float vingette = smoothstep(0.75, 0.2, distance);
	return vingette;
}

// fragment shader
void main()
{
	// distance from center
	vec2 centeredPosition2DFrag = gl_FragCoord.xy / screen - 0.5;
	float distance = length(centeredPosition2DFrag);

	float effect = 0.003;
	//float effect = smoothstep(0.5, 1.0, distance);
	float r = texture(pixels, uv - vec2(effect, effect)).r;
	float g = texture(pixels, uv).g;
	float b = texture(pixels, uv + vec2(effect, effect)).b;
	outputColor = vec3(r, g, b);

	// output pixel color
	outputColor = mix(outputColor, outputColor * Vingette(distance), 0.8);
}