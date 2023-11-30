using System;
using System.Text;
using System.Runtime.CompilerServices;

namespace Sudoku;

public class Sudoku {
    readonly Cell[,] field;
    
    public Sudoku() {
        field = new Cell[9, 9];
        for(int i = 0; i < 9; i++)
        for(int j = 0; j < 9; j++)
            field[i, j] = new Cell(CellType.Regular, 0);
    }


    public int this[int i, int j] {
        get { return field[i, j].Digit; }
        set { field[i, j] = new Cell(CellType.Clue, value); }
    }

    public bool Solve() {
        for(int i = 0; i < 9; i++)
        for(int j = 0; j < 9; j++) {
            // 'Clue' cells are read-only
            if(field[i, j].Type == CellType.Clue) continue;

            // try set 1..9 digits in the (i,j) cell
            while(!TrySetDigitInCell(i, j)) {
                // if we are here, we can't find an appropriate digit, so do a backtracking

                // find an appropriate cell
                Cell cell;

                do {
                    j--;
                    if(j == -1) {
                        j = 8;
                        i--;
                    }
                    if(i == -1) return false;
                    cell = field[i, j];
                }
                while(cell.Digit == 9 || field[i, j].Type == CellType.Clue);

                // cell is found
                // set a backtracking flag for it
                cell.Type = CellType.Backtracking;
            }
        }
        return true;
    }
    bool TrySetDigitInCell(int i, int j) {
        Cell cell = field[i, j];

        int startDigit;
        if(cell.Type == CellType.Backtracking) {
            startDigit = cell.Digit + 1;
            cell.Type = CellType.Regular;
        }
        else
            startDigit = 1;

        for(int digit = startDigit; digit <= 9; digit++) {
            if(IsDigitValid(i, j, digit)) {
                cell.Digit = digit;
                return true;
            }
        }
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsDigitValid(int i, int j, int digit) {
        // row should contain unique digits
        for(int n = 0; n < j; n++) {
            if(field[i, n].Digit == digit) return false;
        }
        for(int n = j + 1; n < 9; n++) {
            Cell c = field[i, n];
            if(c.Type == CellType.Clue && c.Digit == digit) return false;
        }

        // column should contain unique digits
        for(int n = 0; n < i; n++) {
            if(field[n, j].Digit == digit) return false;
        }
        for(int n = i + 1; n < 9; n++) {
            Cell c = field[n, j];
            if(c.Type == CellType.Clue && c.Digit == digit) return false;
        }

        // current small 3x3 square should contain unique digits
        int squareStartRow = (i / 3) * 3;
        int squareStartCol = (j / 3) * 3;
        int squareEndRow = squareStartRow + 2;
        int squareEndCol = squareStartCol + 2;

        for(int k = squareStartRow; k <= squareEndRow; k++)
        for(int l = squareStartCol; l <= squareEndCol; l++) {
            if(k == i && l == j) {
                k = 9; // to exit both external & internal loops
                break; 
            }
            if(field[k, l].Digit == digit) return false;
        }

        for(int k = i; k <= squareEndRow; k++)
        for(int l = j + 1; l <= squareEndCol; l++) {
            Cell c = field[k, l];
            if(c.Type == CellType.Clue && c.Digit == digit) return false;
        }
        return true;
    }

    public void Print(TextWriter textWriter) {
        StringBuilder sb = new(256);
        
        for(int i = 0; i < 9; i++) {
            for(int j = 0; j < 9; j++) {
                sb.Append(field[i, j].Digit);
                sb.Append(" ");
            }
            sb.AppendLine();
        }
        textWriter.Write(sb.ToString());
    }


    class Cell {
        public Cell(CellType type, int digit) {
            Type = type;
            Digit = digit;
        }
        public CellType Type;
        public int Digit;
    }

    enum CellType {
        Regular,
        Clue,
        Backtracking
    }
}
