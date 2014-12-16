public class sonar : RMUD.MudObject
{
    private static int MapWidth = 50;
    private static int MapHeight = 25;
    
    public override void Initialize()
    {
        Short = "devine sonar device";
        Nouns.Add("devine", "sonar", "device", "map");

        Perform<RMUD.MudObject, RMUD.MudObject>("describe")
            .Do((viewer, thing) =>
            {
                var builder = new System.Text.StringBuilder();
                builder.Append("As you gaze into the depths of the devine sonar device, a map forms in your mind...\r\n\r\n");

                var mapGrid = new int[MapWidth, MapHeight];
                for (int y = 0; y < MapHeight; ++y)
                    for (int x = 0; x < MapWidth; ++x)
                        mapGrid[x, y] = ' ';

                for (int y = 1; y < MapHeight - 1; ++y)
                {
                    mapGrid[0, y] = '|';
                    mapGrid[MapWidth - 1, y] = '|';
                }

                for (int x = 1; x < MapWidth - 1; ++x)
                {
                    mapGrid[x, 0] = '-';
                    mapGrid[x, MapHeight - 1] = '-';
                }

                mapGrid[0, 0] = '+';
                mapGrid[0, MapHeight - 1] = '+';
                mapGrid[MapWidth - 1, 0] = '+';
                mapGrid[MapWidth - 1, MapHeight - 1] = '+';

                if (viewer.Location is RMUD.Room) MapLocation(mapGrid, (MapWidth / 2), (MapHeight / 2), viewer.Location as RMUD.Room, '@');

                for (int y = 0; y < MapHeight; ++y)
                {
                    for (int x = 0; x < MapWidth; ++x)
                        builder.Append((char)mapGrid[x, y]);
                    builder.Append("\r\n");
                }

                RMUD.Mud.SendMessage(viewer, builder.ToString());
                return RMUD.PerformResult.Continue;
            });
    }

    private static void MapLocation(int[,] MapGrid, int X, int Y, RMUD.Room Location, int Symbol)
    {
        if (X < 1 || X >= MapWidth - 1 || Y < 1 || Y >= MapHeight - 1) return;

        if (MapGrid[X, Y] != ' ') return;
        if (Symbol == ' ')
        {
            var spacer = Location.Short.LastIndexOf('-');
            if (spacer > 0 && spacer < Location.Short.Length - 2)
                Symbol = Location.Short.ToUpper()[spacer + 2];
            else
                Symbol = Location.Short.ToUpper()[0];
        }
        MapGrid[X, Y] = Symbol;

        foreach (var link in Location.Links)
        {
            var destination = RMUD.Mud.GetObject(link.Destination) as RMUD.Room;
            var directionVector = RMUD.Link.GetAsVector(link.Direction);
            PlaceEdge(MapGrid, X + directionVector.X, Y + directionVector.Y, link.Direction);

            if (destination.RoomType == Location.RoomType)
                MapLocation(MapGrid, X + (directionVector.X * 3), Y + (directionVector.Y * 3), destination, ' ');
        }
    }

    private static void PlaceEdge(int[,] MapGrid, int X, int Y, RMUD.Direction Direction)
    {
        if (X < 1 || X >= MapWidth - 1 || Y < 1 || Y >= MapHeight - 1) return;

        switch (Direction)
        {
            case RMUD.Direction.NORTH:
            case RMUD.Direction.SOUTH:
                MapGrid[X, Y] = '|';
                break;
            case RMUD.Direction.EAST:
            case RMUD.Direction.WEST:
                MapGrid[X, Y] = '-';
                break;
            case RMUD.Direction.NORTHEAST:
            case RMUD.Direction.SOUTHWEST:
                MapGrid[X, Y] = '/';
                break;
            case RMUD.Direction.NORTHWEST:
            case RMUD.Direction.SOUTHEAST:
                MapGrid[X, Y] = '\\';
                break;
            default:
                MapGrid[X, Y] = '*';
                break;
        }
    }
}