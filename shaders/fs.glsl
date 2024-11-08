#version 330
 
const int maxLights = 30;

// structs of 3 different types of light the shader has functions for
struct dirLight
{
	vec3 position;
	vec3 color;
	vec3 direction;

	vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct pointLight
{
	vec3 position;
	vec3 color;
    
    float constant;
    float linear;
    float quadratic;  

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct spotLight
{
	vec3 position;
	vec3 color;
	vec3 direction;

	float constant;
    float linear;
    float quadratic;  

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

	float angle;
	float outerAngle;
};

// shader inputs
in vec4 positionWorld;					       // fragment position in World Space
in vec4 normalWorld;					       // fragment normal in World Space
in vec2 uv;					   		   	       // fragment uv texture coordinates
uniform sampler2D diffuseTexture;		       // texture sampler

uniform int numberDirLights;                   // number of actual dirLights
uniform dirLight allDirLights[maxLights];      // array of dirLights
uniform int numberPointLights;    	           // number of actual pointLights
uniform pointLight allPointLights[maxLights];  // array of pointLights
uniform int numberSpotLights;                  // number of actual spotLights
uniform spotLight allSpotLights[maxLights];    // array of spotLights
uniform vec3 camera;                           // vector3 of camera info

// shader output
out vec4 outputColor;

// functions
vec4 CalcDirLight(int count, vec3 normal, vec3 camera);
vec4 CalcPointLight(int count, vec3 normal, vec3 fragPos, vec3 camera);
vec4 CalcSpotLight(int count, vec3 normal, vec3 fragPos, vec3 camera);

// fragment shader main function
void main()
{
	vec3 normal = normalize(normalWorld.xyz);

	// vec4 that gets all light values for fragment added to it
	vec4 combinedLights = vec4(0);

	// goes over all dirLights and adds light value of fragment to vec4 combinedLights
	for (int i = 0; i < numberDirLights; i++)
	{
		combinedLights += CalcDirLight(i, normal, camera);
	}

	// goes over all spotLights and adds light value of fragment to vec4 combinedLights
	for (int i = 0; i < numberSpotLights; i++)
	{
		combinedLights += CalcSpotLight(i, normal, vec3(positionWorld), camera);
	}

	// goes over all pointLights and adds light value of fragment to vec4 combinedLights
	for (int i = 0 ; i < numberPointLights ; i++)
	{
		combinedLights += CalcPointLight(i, normal, vec3(positionWorld), camera);
	};

    outputColor = texture(diffuseTexture, uv) * combinedLights;
}

// light calculations for dirLights
vec4 CalcDirLight(int count, vec3 normal, vec3 camera)
{
	dirLight light = allDirLights[count];

	// ambient (should be colorMaterial * colorLight * percentage, but no material setup so...)
	vec3 ambient = 1 * light.color;

	// diffuse
	float diffuseStrength = max(0.0, dot(light.position, normal));
	vec3 diffuse = diffuseStrength * light.color;

	// specular
	vec3 viewDirection = normalize(camera);
	vec3 reflectPosition = normalize(reflect(-light.position, normal));
	float specularStrength = max(0.0, dot(viewDirection, reflectPosition));
	specularStrength = pow(specularStrength, 100.0);// change number to change strength of effect
	vec3 specular = specularStrength * light.color;

    // combine results
    ambient  *= light.ambient;
    diffuse  *= light.diffuse;
    specular *= light.specular;

	// return final light value
	return vec4((ambient + diffuse + specular), 1);
}

// light calculations for pointLights
vec4 CalcPointLight(int count, vec3 normal, vec3 fragPos, vec3 camera)
{
	pointLight light = allPointLights[count];

	// ambient (should be colorMaterial * colorLight * percentage, but no material setup so...)
	vec3 ambient = 1 * light.color;

	// diffuse
	float diffuseStrength = max(0.0, dot(light.position, normal));
	vec3 diffuse = diffuseStrength * light.color;

	// specular
	vec3 viewDirection = normalize(camera);
	vec3 reflectPosition = normalize(reflect(-light.position, normal));
	float specularStrength = max(0.0, dot(viewDirection, reflectPosition));
	specularStrength = pow(specularStrength, 100.0);// change number to change strength of effect
	vec3 specular = specularStrength * light.color;

	// attenuation
    float distance = length(light.position - fragPos);
	// simple or more complex attenuantion function
    //float attenuation = 1.0 / (distance * distance);   
	float attenuation= 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    

    // combine results
    ambient  *= attenuation * light.ambient;
    diffuse  *= attenuation * light.diffuse;
    specular *= attenuation * light.specular;

	// return final light value
	return vec4((ambient + diffuse + specular), 1);
}

// light calculations for spotLights
vec4 CalcSpotLight(int count, vec3 normal, vec3 fragPos, vec3 camera)
{
	spotLight light = allSpotLights[count];

	// ambient (should be colorMaterial * colorLight * percentage, but no material setup so...)
	vec3 ambient = 1 * light.color;

	// diffuse
	float diffuseStrength = max(0.0, dot(light.position, normal));
	vec3 diffuse = diffuseStrength * light.color;

	// specular
	vec3 viewDirection = normalize(camera);
	vec3 reflectPosition = normalize(reflect(-light.position, normal));
	float specularStrength = max(0.0, dot(viewDirection, reflectPosition));
	specularStrength = pow(specularStrength, 100.0);// change number to change strength of effect
	vec3 specular = specularStrength * light.color;

	// attenuation
    float distance = length(light.position - fragPos);
	// simple or more complex attenuantion function
    //float attenuation = 1.0 / (distance * distance);   
	float attenuation= 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    

	// spotLight calculations
	vec3 lightDir = normalize(light.position - fragPos);
	// inner circle
	float theta = dot(lightDir, normalize(-light.direction));
	// outer circle
	float epsilon = light.angle - light.outerAngle;
	// interpolation based on degree away from inner circle
	float intensity = clamp((theta - light.outerAngle) / epsilon, 0.0, 1.0);

	// combine results
    ambient  *= attenuation * light.ambient;
    diffuse  *= attenuation * light.diffuse * intensity;
    specular *= attenuation * light.specular * intensity;

	// return final light value
	return vec4((ambient + diffuse + specular), 1);
}