#version 150

in  vec4 vPosition;
in  vec4 vColor;
out vec4 color;

void main() 
{
    gl_Position = vPosition;
    color = vColor;
    gl_PointSize = 3.5f;
} 
