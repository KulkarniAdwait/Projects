//Code courtsey of Dr. K.R. Subramanian
#ifndef GL_DEFS_H

#define GL_DEFS_h

#include <GL/glew.h>
#include <GL/gl.h>
#include <GL/glut.h>
#include <GL/glu.h>

//Code courtsey of Dr. K.R. Subramanian
GLuint InitShader(const char* vs_file, const char* fs_file);
//Code courtsey of Dr. K.R. Subramanian
char* TextFileRead(const char *fn);

//Adwait Kulkarni
void InitBuffers();
void HandleMouse(int button, int state, int x, int y);
void PushData();
void ScreenToNormalized(const int& x, const int& y, float& normX, float& normY);
void Display();
void HandlePointsData();

#endif
