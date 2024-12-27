// https://adventofcode.com/2024/day/6

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

//var (obstacles, position, direction, boundary) = GetData(input);

var (obstacles, position, direction, boundary) = GetData(File.ReadAllLines("input.txt").ToArray());

// foreach (var obstacle in obstacles)
// {
//     Console.WriteLine($"Obstacle: ({obstacle.x},{obstacle.y})");
// }
//
// Console.WriteLine($"Position: ({position.Value.x},{position.Value.y})");
// Console.WriteLine($"Direction: ({direction.Value.dx},{direction.Value.dy})");

// 4515

var uniqueSteps = FindWay(obstacles, position, direction, boundary);

Console.WriteLine($"Part 1. Unique positions: {uniqueSteps.Count}");


// Part 2 

// get potential obstacles along the path 

var potentialObstacles = GetPotentialObstacles(uniqueSteps, obstacles, boundary);
var additionalObstacles = new HashSet<(int x, int y)>();

foreach (var potentialObstacle in potentialObstacles)
{
    HashSet<(int x, int y)> tmp = new HashSet<(int x, int y)>(obstacles);
    tmp.Add(potentialObstacle);
    //tmp.Add((3,6));
    
    if (HasLoop(tmp, position, direction, boundary))
    {
        additionalObstacles.Add(potentialObstacle);
    }
}

// 1309
Console.WriteLine($"Part 2. Extra obstacles: {additionalObstacles.Count}");

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

bool HasLoop(HashSet<(int x, int y)> obstacles, (int x, int y) position, (int dx, int dy) direction, (int x, int y) boundary)
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
            // if (steps.Contains(nextPosition) && steps.Contains(currentPosition))
            //     return true;

            //previousPosition = currentPosition;
            currentPosition = nextPosition;

            if(!steps.Add(currentPosition))
                i++;
        }
        
        // todo: find better way to find end of loop
        //  e.g keep direction with position - if the see the same position with the same direction we have a loop.

        if (i > 800)
            return true;

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

// todo: encapsulate direction somehow


// Get the list of obstacles, and boundaries of the grid
(HashSet<(int x, int y)>, (int x, int y) position, (int dx, int dy) direction, (int x, int y) boundary) GetData(string[] input)
{
    // todo: refactor to remove null
    // todo: introduce object abstractions
    
    var obstacles = new HashSet<(int x, int y)>();
    (int x, int y)? position = null;
    (int dx, int dy)? direction = null;
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

    return (obstacles, position.Value, direction.Value, boundary);
} 