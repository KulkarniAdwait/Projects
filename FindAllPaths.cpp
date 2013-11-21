#include <iostream>
#include <vector>
//X and Y dimensions of the map
#define mapX 3
#define mapY 3
#define startX 0
#define startY 0
#define destX 2
#define destY 2

struct position {
	int X;
	int Y;
};


position* makePos(int x, int y) {
	position *pos = new position;
	pos->X = x;
	pos->Y = y;

	return pos;
}

//find neighbors of the node
std::vector<position> getNeighbors(position node) {
	std::vector<position> ret;
	//up
	if(node.X >= 1) {
		position *pos = makePos(node.X - 1, node.Y);
		ret.push_back(*pos);
	}
	//down
	if(node.X < mapX - 1) {
		position *pos = makePos(node.X + 1, node.Y);
		ret.push_back(*pos);
	}
	//left
	if(node.Y >= 1) {
		position *pos = makePos(node.X, node.Y - 1);
		ret.push_back(*pos);
	}
	//right
	if(node.Y < mapY - 1) {
		position *pos = makePos(node.X, node.Y + 1);
		ret.push_back(*pos);
	}
	return ret;
}

//prints the number of paths or actual paths
int pathCnt = 0;
void print(std::vector<position> path) {
	pathCnt++;
	std::vector<position>::iterator iter = path.begin();
	std::cout << pathCnt << "| ";
	while(iter != path.end() ) {
		std::cout << " ("<< (*iter).X << "," << (*iter).Y << ") ";
		++iter;
	}
	std::cout << std::endl;
}


//finds all the paths from (sX,sY) to (dX, dY)
//visitedMap tells if (x,y) has been visited or not
void findAllPaths(int sX, int sY, int dX, int dY, std::vector<position> path, bool visitedMap[mapX][mapY]) {
	
	//bounds check
	if((sX < 0 || sY < 0 || dX < 0 || dY < 0 ) || (sX >= mapX || sY >= mapY || dX >= mapX || dY >= mapY )) {
		std::cout << "Bounds check fail!" << std::endl;
		getchar();
		exit(1);
	}

	//add current node to the path and mark it as visited
	position curr = path.back();
	visitedMap[curr.X][curr.Y] = true;
	std::vector<position> neighbors = getNeighbors(curr);

	//check path from every neighbor to the destination
	for(int i=0; i< neighbors.size(); i += 1) {
		//if neighbor is the destination then print path
		if(neighbors[i].X == dX && neighbors[i].Y == dY) {
			path.push_back(neighbors[i]);
			print(path);
			path.pop_back();
			continue;
		}
		//if unvisited neighbor
		if(visitedMap[neighbors[i].X][neighbors[i].Y] == false) {
			//mark as visited
			visitedMap[neighbors[i].X][neighbors[i].Y] = true;
			//add to path
			path.push_back(neighbors[i]);
			//find all paths from thie neighbor to destination
			findAllPaths(neighbors[i].X, neighbors[i].Y, dX, dY, path, visitedMap);
			//remove neighbor from path and mark as unvisited
			visitedMap[neighbors[i].X][neighbors[i].Y] = false;
			path.pop_back();
		}
	}
}

int main() {
	std::vector<position> path;
	position start;
	start.X = startX;
	start.Y = startY;
	path.push_back(start);
	bool visitedMap[mapX][mapY];
	for(int i=0; i < mapX; i+=1) {
		for(int j=0; j < mapY; j+=1) {
			visitedMap[i][j] = false;
		}
	}
	findAllPaths(start.X, start.Y, destX, destY, path, visitedMap);
	std::cout << std::endl << pathCnt << " paths" << std::endl;
	getchar();
}