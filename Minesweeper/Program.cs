var game = new MinesweeperGame();
game.InitializeGameBoard();
game.PlayMinesweeper();

public partial class MinesweeperGame
{
    private Square[,] Board = new Square[8,8];
    private int GameState = 0;  // 0 = running, 1 = won, 2 = lost

    public void InitializeGameBoard()
    {
        Console.WriteLine();
        Console.WriteLine($"{Environment.NewLine}Starting Minesweeper...");
        PopulateSquares();
        LayMines();
        PopulateNearbyValues();
    }

    public void PlayMinesweeper()
    {
        PrintBoard();

        while (GameState == 0)
        {
            var inputs = GetInputs();
            UpdateBoard(inputs.Item1, inputs.Item2, inputs.Item3);
            UpdateGameState();
            PrintBoard();
        }

        PrintResults();
    }

    private void UpdateBoard(int i, int j, bool isFlag)
    {
        if (isFlag)
        {
            if (!Board[i,j].IsChecked)
                Board[i,j].IsFlagged = true;
            return;
        }

        if (Board[i,j].IsMine)
        {
            GameState = 2;
            return;
        }

        Board[i,j].IsChecked = true;
        Board[i,j].IsFlagged = false;

        if (Board[i,j].NearbyMines == 0)
            UpdateSurroundingSquares(i, j);
    }

    private void UpdateSurroundingSquares(int i, int j)
    {
        var squaresWithNoMinesNearby = new List<(int,int)>();

        if (IsValidPosition(i-1,j-1))
        {
            if (Board[i-1,j-1].NearbyMines == 0 && !Board[i-1,j-1].IsChecked)
                squaresWithNoMinesNearby.Add(new(i-1,j-1));
            Board[i-1,j-1].IsChecked = true;
        }
        if (IsValidPosition(i-1,j))
        {
            if (Board[i-1,j].NearbyMines == 0 && !Board[i-1,j].IsChecked)
                squaresWithNoMinesNearby.Add(new(i-1,j));
            Board[i-1,j].IsChecked = true;
        }
        if (IsValidPosition(i-1,j+1))
        {
            if (Board[i-1,j+1].NearbyMines == 0 && !Board[i-1,j+1].IsChecked)
                squaresWithNoMinesNearby.Add(new(i-1,j+1));
            Board[i-1,j+1].IsChecked = true;
        }
        if (IsValidPosition(i,j-1))
        {
            if (Board[i,j-1].NearbyMines == 0 && !Board[i,j-1].IsChecked)
                squaresWithNoMinesNearby.Add(new(i,j-1));
            Board[i,j-1].IsChecked = true;
        }
        if (IsValidPosition(i,j+1))
        {
            if (Board[i,j+1].NearbyMines == 0 && !Board[i,j+1].IsChecked)
                squaresWithNoMinesNearby.Add(new(i,j+1));
            Board[i,j+1].IsChecked = true;
        }
        if (IsValidPosition(i+1,j-1))
        {
            if (Board[i+1,j-1].NearbyMines == 0 && !Board[i+1,j-1].IsChecked)
                squaresWithNoMinesNearby.Add(new(i+1,j-1));
            Board[i+1,j-1].IsChecked = true;
        }
        if (IsValidPosition(i+1,j))
        {
            if (Board[i+1,j].NearbyMines == 0 && !Board[i+1,j].IsChecked)
                squaresWithNoMinesNearby.Add(new(i+1,j));
            Board[i+1,j].IsChecked = true;
        }
        if (IsValidPosition(i+1,j+1))
        {
            if (Board[i+1,j+1].NearbyMines == 0 && !Board[i+1,j+1].IsChecked)
                squaresWithNoMinesNearby.Add(new(i+1,j+1));
            Board[i+1,j+1].IsChecked = true;
        }
        
        foreach (var square in squaresWithNoMinesNearby)
            UpdateBoard(square.Item1, square.Item2, false);
    }

    private bool IsValidPosition(int i, int j)
    {
        return i >= 0 && j >= 0 && i < 8 && j < 8;
    }

    private Tuple<int,int,bool> GetInputs()
    {
        var row = GetNumberInput("row");
        var column = GetNumberInput("column");
        var flag = GetFlagInput();
        return new Tuple<int,int,bool>(row, column, flag);
    }

    private int GetNumberInput(string inputName)
    {
        bool inputSuccess;

        do
        {
            Console.WriteLine($"Enter {inputName} (1-8): ");
            var input = Console.ReadLine();
            var parseSuccess = int.TryParse(input, out var output);
            inputSuccess = parseSuccess && output >= 1 && output < 9;
            
            if (inputSuccess)
                return output - 1;

        } while (!inputSuccess);

        return -1;
    }

    private bool GetFlagInput()
    {
        Console.WriteLine("Enter 'y' if you want to flag this square, otherwise press enter");
        var input = Console.ReadLine();
        return !string.IsNullOrEmpty(input) && (input[0] == 'y' || input[0] == 'Y');
    }

    private void UpdateGameState()
    {
        if (GameState == 2)
            return;

        var uncheckedSquares = 0;

        for (int i = 0; i < Board.GetLength(0); i++)
            for (int j = 0; j < Board.GetLength(1); j++)
                if (!Board[i,j].IsChecked)
                    uncheckedSquares++;
        
        if (uncheckedSquares == 10)
            GameState = 1;
    }

    private void PrintBoard()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                var square = Board[i,j];
                if (square.IsFlagged && GameState == 0)
                    Console.Write(">  ");
                else if (!square.IsChecked && GameState == 0)
                    Console.Write("#  ");
                else if (square.IsMine)
                    Console.Write("*  ");
                else
                    Console.Write($"{square.NearbyMines}  ");
            }
            
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    private void PrintResults()
    {
        var message = "GAME OVER";

        if (GameState == 1)
            message += " - You Won!";
        else if (GameState == 2)
            message += " - You Lost!";
        else
            message += " - Something Went Terribly Wrong!";
        
        Console.WriteLine(message);
    }

    private void PopulateSquares()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
            for (int j = 0; j < Board.GetLength(1); j++)
                Board[i,j] = new Square();
    }

    private void LayMines()
    {
        var rand = new Random();
        var mineCount = 0;

        while (mineCount < 10)
        {
            var mineRow = rand.NextInt64(0, 8);
            var mineColumn = rand.NextInt64(0, 8);

            var square = Board[mineRow,mineColumn];
            if (!square.IsMine)
            {
                Board[mineRow,mineColumn].IsMine = true;
                mineCount++;
            }
        }
    }

    private void PopulateNearbyValues()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i,j].IsMine)
                    continue;
                
                var count = 0;

                if (IsValidPosition(i-1,j-1) && Board[i-1,j-1].IsMine)
                    count++;
                if (IsValidPosition(i-1,j) && Board[i-1,j].IsMine)
                    count++;
                if (IsValidPosition(i-1,j+1) && Board[i-1,j+1].IsMine)
                    count++;
                if (IsValidPosition(i,j-1) && Board[i,j-1].IsMine)
                    count++;
                if (IsValidPosition(i,j+1) && Board[i,j+1].IsMine)
                    count++;
                if (IsValidPosition(i+1,j-1) && Board[i+1,j-1].IsMine)
                    count++;
                if (IsValidPosition(i+1,j) && Board[i+1,j].IsMine)
                    count++;
                if (IsValidPosition(i+1,j+1) && Board[i+1,j+1].IsMine)
                    count++;
                
                Board[i,j].NearbyMines = count;
            }
        }
    }
}