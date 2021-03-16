#version 330 core
in vec2 texCoords;
out vec4 color;

uniform sampler2D texture0;
uniform vec3 spriteColor;

void main()
{    
    vec4 texColor = vec4(spriteColor, 1.0) * texture(texture0, texCoords);
    if(texColor.a < 0.1)
        discard;
    color = texColor;
}  