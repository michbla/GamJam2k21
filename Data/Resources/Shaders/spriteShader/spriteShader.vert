#version 330 core
layout (location = 0) in vec2 vertPos; // <vec2 position, vec2 texCoords>
layout (location = 1) in vec2 tex;

out vec2 texCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform vec2 texOffset;

void main()
{
    gl_Position = vec4(vertPos, 0.0, 1.0) * model * view * projection;

    texCoords = tex + texOffset;
}