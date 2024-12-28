// https://adventofcode.com/2024/day/6

using System.Diagnostics;

string[] input = new[]
{
    "....#.....", // 0
    ".........#", // 1
    "..........", // 2
    "..#.......", // 3
    ".......#..", // 4
    "..........", // 5
    ".#..^.....", // 6
    "........#.", // 7
    "#.........", // 8
    "......#..."  // 9
};

Stopwatch timer = Stopwatch.StartNew();

//var (obstacles, position, direction, boundary) = GetData(input);
var (obstacles, position, direction, boundary) = GetData(File.ReadAllLines("input.txt").ToArray());

// Part 1
var uniqueSteps = FindWay(obstacles, position, direction, boundary);

// 4515
Console.WriteLine($"Part 1. Unique positions: {uniqueSteps.Count}");


// Part 2 
// get potential obstacles along the path 
var potentialObstacles = GetPotentialObstacles(uniqueSteps, obstacles, boundary);
var additionalObstacles = new HashSet<(int x, int y)>();

// check if any potential obstacle generate a loop
foreach (var potentialObstacle in potentialObstacles)
{
    HashSet<(int x, int y)> tmp = new HashSet<(int x, int y)>(obstacles);
    tmp.Add(potentialObstacle);
    
    if (HasLoopStorePosition(tmp, position, direction, boundary))
    //if (HasLoopStorePositionAndDirection(tmp, position, direction, boundary))
    {
        additionalObstacles.Add(potentialObstacle);
    }
}

// 1309
Console.WriteLine($"Part 2. Extra obstacles: {additionalObstacles.Count}");

timer.Stop();

Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} ms");




HashSet<(int x, int y)> GetPotentialObstacles(HashSet<(int x, int y)> uniqueSteps, HashSet<(int x, int y)> obstacles, (int x, int y) boundary)
{
    var potentialObstacles = new HashSet<(int x, int y)>();
    
    (int dx, int dy)[] directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];
    
    // for each point
    foreach (var step in uniqueSteps)
    {
        foreach (var direction in directions)
        {
            (int x, int y) nextPosition = (step.x + direction.dx, step.y + direction.dy);
            
            if (nextPosition.x < 0 || nextPosition.x >= boundary.x  || nextPosition.y < 0 || nextPosition.y >= boundary.y || obstacles.Contains(nextPosition))
                continue;
            
            potentialObstacles.Add(nextPosition);
        }
    }

    return potentialObstacles;
}

HashSet<(int x, int y)> FindWay(HashSet<(int x, int y)> obstacles, (int x, int y) position, (int dx, int dy) direction, (int x, int y) boundary)
{
    var steps = new HashSet<(int x, int y)>();
    var currentPosition = position;
    
    while (true)
    {
        (int x, int y) nextPosition = (currentPosition.x + direction.dx, currentPosition.y + direction.dy);
        
        // have we left boundaries
        if (nextPosition.x < 0 || nextPosition.x >= boundary.x  || nextPosition.y < 0 || nextPosition.y >= boundary.y)
            break;

        if (obstacles.Contains(nextPosition))
        {
            // nextPosition is obstacle change direction - turn right
            direction = TurnRight(direction);
        }
        else
        {
            currentPosition = nextPosition;
            steps.Add(currentPosition);
        }
    }

    return steps;
}

bool HasLoopStorePosition(HashSet<(int x, int y)> obstacles, (int x, int y) position, (int dx, int dy) direction, (int x, int y) boundary)
{
    var steps = new HashSet<(int x, int y)>();
    var currentPosition = position;

    int i = 0;
    while (true)
    {
        (int x, int y) nextPosition = (currentPosition.x + direction.dx, currentPosition.y + direction.dy);
        
        // have we left boundaries
        if (nextPosition.x < 0 || nextPosition.x >= boundary.x || nextPosition.y < 0 || nextPosition.y >= boundary.y)
            return false;

        if (obstacles.Contains(nextPosition))
        {
            // nextPosition is obstacle change direction - turn right
            direction = TurnRight(direction);
        }
        else
        {
            currentPosition = nextPosition;

            if(!steps.Add(currentPosition))
                i++;
        }

        // higher than arbitrary number can assume we are in a loop
        // some wasted cycles but still faster than second attempt with a storing direction
        if (i > boundary.x * boundary.x) 
            return true;

        i++;
    }
}

bool HasLoopStorePositionAndDirection(HashSet<(int x, int y)> obstacles, (int x, int y) position, (int dx, int dy) direction, (int x, int y) boundary)
{
    var steps = new HashSet<(int x, int y, int dx, int dy)>();
    var currentPosition = position;

    while (true)
    {
        (int x, int y) nextPosition = (currentPosition.x + direction.dx, currentPosition.y + direction.dy);
        
        // have we left boundaries
        if (nextPosition.x < 0 || nextPosition.x >= boundary.x || nextPosition.y < 0 || nextPosition.y >= boundary.y)
            return false;

        if (obstacles.Contains(nextPosition))
        {
            // nextPosition is obstacle change direction - turn right
            direction = TurnRight(direction);
        }
        else
        {
            currentPosition = nextPosition;

            // if we are in the same position & direction we have a loop
            if (!steps.Add((currentPosition.x, currentPosition.y, direction.dx, direction.dy)))
                return true;            
        }
    }
}   

return;

(int dx, int dy) TurnRight((int x, int y) direction)
{
    if (direction == (0, -1))
        return (1, 0);

    if (direction == (1, 0))
        return (0, 1);
    
    if (direction == (0, 1))
        return (-1, 0);
    
    return (0, -1);
}

// Get the list of obstacles, and boundaries of the grid
(HashSet<(int x, int y)>, (int x, int y) position, (int dx, int dy) direction, (int x, int y) boundary) GetData(string[] input)
{
    var obstacles = new HashSet<(int x, int y)>();
    (int x, int y) position = (0, 0);
    (int dx, int dy) direction = (0, 0);
    (int x, int y) boundary = (input[0].Length, input.Length);
    
    for (int i = 0; i < boundary.x; i++)
    {
        for (int j = 0; j < boundary.y; j++)
        {
            var o = (j, i);
            
            switch (input[i][j])
            {
                case '.':
                    continue;
                case '#':
                    obstacles.Add(o);
                    continue;
                case '^':
                    position = o;
                    direction = (0, -1);
                    continue;
                case '>':
                    position = o;
                    direction = (1, 0);
                    continue;
                case '<':
                    position = o;
                    direction = (-1, 0);
                    continue;                    
                case 'v' or 'V':
                    position = o;
                    direction = (0, 1);
                    continue;
            }
        }
    }

    return (obstacles, position, direction, boundary);
}