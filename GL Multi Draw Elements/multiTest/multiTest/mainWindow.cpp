#define _CRT_SECURE_NO_WARNINGS
#define VERTICES 0
#define COLORS 1
#define INDICES 2
#include "Header.h"
#include <iostream>
#include <string>
#include <fstream>
#include <vector>

using namespace std;

const int MAX_VERTS = 1000;
const float SCREEN_WIDTH = 800;
const float SCREEN_HEIGHT = 600;
const int SIZE_OF_GL_FLOAT = sizeof(GLfloat);

GLuint vao;
GLuint vbo[3];

//attribute locations in the vertex shader
GLuint vPosition, vColor;
//current positions in the respective buffers
GLintptr vertexOffset = 0, colorOffset = 0, indexOffset = 0;
//number of bytes being added to the respective buffers
GLsizeiptr vertexChangeSize = 0, colorChangeSize = 0, indexChangeSize = 0;
int polygonCount = 0, index = 0, vertexCount = 0;
float normalizedX = 0.0f, normalizedY = 0.0f;

vector<GLfloat> vertexData;
vector<GLfloat> colorData;
vector<GLuint> indexData;
//holds the pointers, in bytes, to the indices
vector<GLuint> IBO_pointer_indices;
//holds the number of vertices in the polygon(s)
vector<GLsizei> counts;

//holds the points on screen where the user has clicked
//done for feedback only
GLuint pointsVao, pointsVbo[2];


/*******************************************************/
//Generates and allocates empty buffers on GPU
void InitBuffers()
{
	//clear color set to white
	glClearColor(1.0f, 1.0f, 1.0f, 1.0f);

	glGenVertexArrays(1, &vao);
	glGenBuffers(3, vbo);

	//allocate empty buffers of apt. size
	glBindVertexArray(vao);
	//vertices
	glBindBuffer(GL_ARRAY_BUFFER, vbo[VERTICES]);	
	glBufferData(GL_ARRAY_BUFFER, MAX_VERTS * SIZE_OF_GL_FLOAT, NULL, GL_DYNAMIC_DRAW);
	glVertexAttribPointer(vPosition, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vPosition);
	// colors
	glBindBuffer(GL_ARRAY_BUFFER, vbo[COLORS]);	
	glBufferData(GL_ARRAY_BUFFER, MAX_VERTS * SIZE_OF_GL_FLOAT, NULL, GL_DYNAMIC_DRAW);
	glVertexAttribPointer(vColor, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vColor);
	//indices
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, vbo[INDICES]);	
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, MAX_VERTS * sizeof(GLuint), NULL, GL_DYNAMIC_DRAW);

	////////////////////////
	//for points
	glGenVertexArrays(1, &pointsVao);
	glGenBuffers(2, pointsVbo);

	glBindVertexArray(pointsVao);
	//vertices
	glBindBuffer(GL_ARRAY_BUFFER, pointsVbo[VERTICES]);	
	glBufferData(GL_ARRAY_BUFFER, MAX_VERTS * SIZE_OF_GL_FLOAT, NULL, GL_DYNAMIC_DRAW);
	glVertexAttribPointer(vPosition, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vPosition);
	// colors
	glBindBuffer(GL_ARRAY_BUFFER, pointsVbo[COLORS]);	
	glBufferData(GL_ARRAY_BUFFER, MAX_VERTS * SIZE_OF_GL_FLOAT, NULL, GL_DYNAMIC_DRAW);
	glVertexAttribPointer(vColor, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vColor);

}

//converts screen coordinates to GL coordinates i.e from -1 to +1
void ScreenToNormalized(const int& x, const int& y, float& normX, float& normY)
{
	normX = ((2.0f * x) / SCREEN_WIDTH) - 1.0f;
	normY = ((-2.0f * y) / SCREEN_HEIGHT) + 1.0f;
	return;
}

//left click handles clicks on the canvas
//right click pushes data to GPU
void HandleMouse(int button, int state, int x, int y)
{
	switch(state)
	{
	case GLUT_DOWN:

		switch(button)
		{
		case GLUT_LEFT_BUTTON:
			//if we aren't reaching the vetex limit
			if(vertexData.size() + 1 <= MAX_VERTS)
			{
				//vertices
				ScreenToNormalized(x, y, normalizedX, normalizedY);
				vertexData.push_back((GLfloat)normalizedX);//x
				vertexData.push_back((GLfloat)normalizedY);//y
				vertexData.push_back((GLfloat)0.0f);//z
				vertexData.push_back((GLfloat)1.0f);//w
				vertexChangeSize += 4 * SIZE_OF_GL_FLOAT;

				//colors
				//pushing same values as x & y to red & green 
				//channel for difference in colors
				colorData.push_back(abs(normalizedX));//r
				colorData.push_back(abs(normalizedY));//g
				colorData.push_back(0.65f);//b
				colorData.push_back(1.0f);//a
				colorChangeSize += 4 * SIZE_OF_GL_FLOAT;

				//indices
				indexData.push_back(index++);
				indexChangeSize += sizeof(GLuint);

				vertexCount++;

				HandlePointsData();
			}
			else
			{
				PushData();
			}
			break;

		case GLUT_RIGHT_BUTTON:
			//using LINE_LOOP so there have to be a minimum of 2 vertices per primitive
			if(vertexCount >= 2)
			{
				PushData();
			}
			break;
		}
	break;
	}
}

//sends captured mouse clicks to GPU
void PushData()
{
	counts.push_back(vertexCount);
	vertexCount = 0;

	//stores the number of bytes we had added till the (n-1)th polygon
	//these are the pointers into the index array
	IBO_pointer_indices.push_back(indexOffset);

	//push data to GPU
	glBindVertexArray(vao);
	//vertex
	glBindBuffer(GL_ARRAY_BUFFER, vbo[VERTICES]);
	glBufferSubData(GL_ARRAY_BUFFER, vertexOffset, vertexChangeSize, vertexData._Myfirst);
	glVertexAttribPointer(vPosition, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vPosition);
	vertexOffset += vertexChangeSize;
	vertexChangeSize = 0;

	//color
	glBindBuffer(GL_ARRAY_BUFFER, vbo[COLORS]);
	glBufferSubData(GL_ARRAY_BUFFER, colorOffset, colorChangeSize, colorData._Myfirst);
	glVertexAttribPointer(vColor, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vColor);
	colorOffset += colorChangeSize;
	colorChangeSize = 0;

	//index
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, vbo[INDICES]);
	glBufferSubData(GL_ELEMENT_ARRAY_BUFFER, indexOffset, indexChangeSize, indexData._Myfirst);
	indexOffset += indexChangeSize;
	indexChangeSize = 0;

	polygonCount++;
	vertexData.clear();
	colorData.clear();
	indexData.clear();
}

//draws the polygons stored in the GPU
void Display()
 {    
	glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );
	glEnable(GL_LINE_SMOOTH);
	glLineWidth(2.0f);

	if(vertexCount)
	{
		glEnable( GL_PROGRAM_POINT_SIZE );
		glBindVertexArray(pointsVao);
		glDrawArrays(GL_POINTS, 0, vertexCount);
	}

	if(polygonCount)
	{
		glBindVertexArray(vao);
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, vbo[2]);
		glMultiDrawElements(GL_LINE_LOOP, counts._Myfirst, GL_UNSIGNED_INT, (const GLvoid **)IBO_pointer_indices._Myfirst, polygonCount);
	}
	
    glFlush();
	glutPostRedisplay(); 
	glutSwapBuffers();
}

//pushes data to GPU to draw points
void HandlePointsData()
{
	glBindVertexArray(pointsVao);
	//points
	glBindBuffer(GL_ARRAY_BUFFER, pointsVbo[VERTICES]);
	//4 floats, xyzw, for each point
	glBufferSubData(GL_ARRAY_BUFFER, 0, 4 * vertexCount * SIZE_OF_GL_FLOAT, vertexData._Myfirst);
	glVertexAttribPointer(vPosition, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vPosition);
	//colors
	glBindBuffer(GL_ARRAY_BUFFER, pointsVbo[COLORS]);
	//4 floats, rgba, for each point
	glBufferSubData(GL_ARRAY_BUFFER, 0, 4 * vertexCount * SIZE_OF_GL_FLOAT, colorData._Myfirst);
	glVertexAttribPointer(vColor, 4, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(vColor);
}

//Code courtsey of Dr. K.R. Subramanian
int main(int argc, char *argv[]) {

	// initialize glut
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_RGBA | GLUT_DOUBLE | GLUT_DEPTH);
	glutInitWindowSize(SCREEN_WIDTH, SCREEN_HEIGHT);
	glutInitWindowPosition(100, 100);
	glutCreateWindow("glMultiDrawElements Example");

	// specify glut call backs
	glutDisplayFunc(Display);
	glutMouseFunc(HandleMouse);

	// check to see opengl 2.0 is available using glew
	glewInit();

	const char* version = (const char*)glGetString(GL_VERSION); 
    cout << "OpenGL Version: " << string(version) << endl;

    version = (const char*)glGetString(GL_SHADING_LANGUAGE_VERSION); 
    cout << "GLSL Version: " << string(version) << endl;

	

	// initialize shaders
	GLuint shader_program = InitShader("draw.vert", "draw.frag");
	
	// activate shader program
	glUseProgram(shader_program);
	
	//allocate the buffers
	InitBuffers();

    // enter the event loop
	glutMainLoop();

    return 0;
}

//Code courtsey of Dr. K.R. Subramanian
GLuint InitShader(const char* vs_file, const char* fs_file) {

	char *vs_str = NULL, *fs_str = NULL;

	GLuint vs = glCreateShader(GL_VERTEX_SHADER);
	GLuint fs = glCreateShader(GL_FRAGMENT_SHADER);

	vs_str = TextFileRead(vs_file);
	fs_str = TextFileRead(fs_file);

	const char *vv = vs_str;
	const char *ff = fs_str;

	glShaderSource(vs, 1, &vv, NULL);
	glShaderSource(fs, 1, &ff, NULL);

	free(vs_str); free(fs_str); 

	glCompileShader(vs);

	int IsCompiled;
	glGetShaderiv(vs, GL_COMPILE_STATUS, &IsCompiled);
	cout << "Compiled VS:" << IsCompiled << endl << flush;
	if(!IsCompiled)
    {
		int maxLength;
       glGetShaderiv(vs, GL_INFO_LOG_LENGTH, &maxLength);
 
       /* The maxLength includes the NULL character */
       char *vertexInfoLog = (char *)malloc(maxLength);
 
       glGetShaderInfoLog(fs, maxLength, &maxLength, vertexInfoLog);
 
    	cout << vertexInfoLog << endl;
       /* Handle the error in an appropriate way such as displaying a message or writing to a log file. */
       /* In this simple program, we'll just leave */
       	free(vertexInfoLog);

		cout << "Vertex Shader Failure!" << endl;
//		return -1;
    }
	glCompileShader(fs);
	IsCompiled = 0;
	glGetShaderiv(fs, GL_COMPILE_STATUS, &IsCompiled);

	cout << "Compiled FS:" << IsCompiled << endl << flush;
	if(!IsCompiled)
    {
		int maxLength;
       glGetShaderiv(fs, GL_INFO_LOG_LENGTH, &maxLength);
 
       /* The maxLength includes the NULL character */
       char *fragmentInfoLog = (char *)malloc(maxLength);
 
       glGetShaderInfoLog(fs, maxLength, &maxLength, fragmentInfoLog);
 
    cout << fragmentInfoLog << endl;
						// Handle the error in an appropriate way such as 
						// displaying a message or writing to a log file. */
       /* In this simple program, we'll just leave */
       free(fragmentInfoLog);
		cout << "Fragment Shader Failure!" << endl;
       return -1;
    }

	glEnable( GL_DEPTH_TEST );

//	printShaderInfoLog(vs);
//	printShaderInfoLog(fs);

	GLuint shader_program = glCreateProgram();
	glAttachShader(shader_program, vs);
	glAttachShader(shader_program, fs);

	glBindAttribLocation(shader_program, 0, "vPosition");
	glBindAttribLocation(shader_program, 1, "vColor");

	glLinkProgram(shader_program);

	glUseProgram(shader_program);

						// set up attribute locations, color and position
	vPosition = glGetAttribLocation( shader_program, "vPosition" );
	vColor    = glGetAttribLocation( shader_program, "vColor" );

	return shader_program;
}

//Code courtsey of Dr. K.R. Subramanian
char* TextFileRead(const char *fn) {


	FILE *fp;
	char *content = NULL;

	int count=0;

	if (fn != NULL) {
		fp = fopen(fn,"rt");

		if (fp != NULL) {
      
      fseek(fp, 0, SEEK_END);
      count = ftell(fp);
      rewind(fp);

			if (count > 0) {
				content = (char *)malloc(sizeof(char) * (count+1));
				count = fread(content,sizeof(char),count,fp);
				content[count] = '\0';
			}
			fclose(fp);
		}
	}
	for (int k = 0; k < count; k++)
		cout << content[k];
	return content;
}