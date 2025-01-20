using System.Text;

class BigWarehouse : Warehouse
{
    protected override void Move(Direction direction)
    {
        if (direction == Direction.Left || direction == Direction.Right)
            MoveHorizontal(direction);
        else
            MoveVertical(direction);
    }

    void MoveVertical(Direction moveDirection)
    {
        var nextPos = Robot.Position + moveDirection;

        switch (GetValue(nextPos))
        {
            case '.':
            {
                Swap(nextPos, Robot.Position);
                Robot.Position = nextPos;
                return;
            }
            case '#':
                return;
            case '[':
                MoveIfPossible(nextPos, Direction.Right, moveDirection);
                return;
            case ']':
                MoveIfPossible(nextPos, Direction.Left, moveDirection);
                return;
        }

        void MoveIfPossible(Point point, Direction adjacent, Direction dir)
        {
            if (CanMove(point, dir) && CanMove(point + adjacent, dir))
            {
                Move(Robot.Position, dir);
                Robot.Position = point;
            }
        }
    }

    bool CanMove(Point pos, Direction dir)
    {
        return GetValue(pos) switch
        {
            '#' => false,
            '.' => true,
            '[' => CanMove(pos + dir, dir) && CanMove(pos + dir + Direction.Right, dir),
            ']' => CanMove(pos + dir, dir) && CanMove(pos + dir + Direction.Left, dir),
            _ => throw new InvalidDataException("Invalid map value")
        };
    }

    void Move(Point pos, Direction dir)
    {
        switch (GetValue(pos + dir))
        {
            case '[':
                Move(pos + dir + Direction.Right, dir);
                Move(pos + dir, dir);
                break;
            case ']':
                Move(pos + dir + Direction.Left, dir);
                Move(pos + dir, dir);
                break;
        }
        
        Swap(pos, pos + dir);
    }

    private void MoveHorizontal(Direction direction)
    {
        var nextPos = Robot.Position + direction;
        var currentPos = nextPos;

        // check for any crates in the direction of move
        while (GetValue(currentPos) == '[' || GetValue(currentPos) == ']')
        {
            currentPos += direction;
        }

        // ignore move
        if (GetValue(currentPos) == '#')
            return;

        // we reached empty space - swap backwards
        while (currentPos != Robot.Position)
        {
            var tmp = currentPos - direction;
            Swap(currentPos, tmp);
            currentPos = tmp;
        }

        Robot.Position = nextPos;   
    }

    public IEnumerable<int> GetGpsCoordinates() 
        => base.GetGpsCoordinates('[');

    public static BigWarehouse Create(char[][] map)
    {
        var m = ScaleUp(map);
        
        return new BigWarehouse
        {
            Map = m,
            Size = new Size(m.Length, m[0].Length),
            Robot = Robot.Create(m)
        };
    }
    
    static char[][] ScaleUp(char[][] map)
    {
        var bigMap = new List<string>();
        
        for (var row = 0; row < map.Length; row++)
        {
            var sb = new StringBuilder(map.Length * 2);
            
            for (var col = 0; col < map.Length; col++)
            {
                sb.Append(ScaleUp(map[row][col]));
            }
            
            bigMap.Add(sb.ToString());
        }

        return bigMap.Select(o => o.ToCharArray()).ToArray();

        string ScaleUp(char o) => o switch
        {
            '#' => "##",
            'O' => "[]",
            '.' => "..",
            _   => "@."
        };
    }        

}
