class SmallWarehouse : Warehouse
{
    protected override void Move(Direction direction)
    {
        var nextPos = Robot.Position + direction;
        var currentPos = nextPos;

        // check for any crates in the direction of move
        while (GetValue(currentPos) == 'O')
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
        => base.GetGpsCoordinates('O');    

    public static SmallWarehouse Create(char[][] map)
    {
        return new SmallWarehouse
        {
            Map = map,
            Size = new Size(map.Length, map.Length),
            Robot = Robot.Create(map)
        };
    }
}