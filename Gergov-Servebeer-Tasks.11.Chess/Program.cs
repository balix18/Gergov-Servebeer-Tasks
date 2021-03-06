﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Gergov_Servebeer_Tasks._11.Chess
{
    class Program
    {
        class ChessPieceType
            : SafeEnumeration<ChessPieceType.Meta, ChessPieceType.Meta.ValuesEnum, ChessPieceType>
        {
            public class Meta
            {
                public enum ValuesEnum
                {
                    Bishop,
                    King,
                    Rook
                }

                public static ChessPieceType Bishop { get; } = new ChessPieceType(ValuesEnum.Bishop, 'B', "Futó");
                public static ChessPieceType King { get; } = new ChessPieceType(ValuesEnum.King, 'K', "Király");
                public static ChessPieceType Rook { get; } = new ChessPieceType(ValuesEnum.Rook, 'R', "Bástya");
            }

            public Meta.ValuesEnum Value { get; }

            public char ShortName { get; }
            public string CsvName { get; }

            public static IEnumerable<ChessPieceType> TypeList => GetAll();

            protected ChessPieceType(Meta.ValuesEnum value, char shortName, string csvName)
                : base(value.IntValue(), value.ToString())
            {
                Value = value;
                ShortName = shortName;
                CsvName = csvName;
            }

            public override string ToString() => Value.ToString();
        }

        interface IChessPieceCollusion
        {
            IEnumerable<(int row, int column)> GetCollusionIndices((int width, int height) tableSize);
        }

        abstract class ChessPiece
            : IChessPieceCollusion
        {
            public ChessPieceType Type { get; }

            public char PrettyColumn { get; }
            public int PrettyRow { get; }

            public (int row, int column) Index => (PrettyRowToIndex, PrettyColumnToIndex);
            public int PrettyRowToIndex => Converter.PrettyRowToIndexConverter(PrettyRow);
            public int PrettyColumnToIndex => Converter.PrettyColumnToIndexConverter(PrettyColumn);

            public static class Converter
            {
                public static (int row, int column) PrettyToIndexConverter(int prettyRow, char prettyColumn) => (PrettyRowToIndexConverter(prettyRow), PrettyColumnToIndexConverter(prettyColumn));
                public static int PrettyRowToIndexConverter(int prettyRow) => prettyRow - 1;
                public static int PrettyColumnToIndexConverter(char prettyColumn) => prettyColumn - 'A';
            }

            public static class InvertedConverter
            {
                public static (int prettyRow, char prettyColumn) IndexToPrettyConverter(int row, int column) => (IndexToPrettyRowConverter(row), IndexToPrettyColumnConverter(column));
                public static int IndexToPrettyRowConverter(int row) => row + 1;
                public static char IndexToPrettyColumnConverter(int column) => (char)(column + 'A');
            }

            public abstract IEnumerable<(int row, int column)> GetCollusionIndices((int width, int height) tableSize);

            public ChessPiece(ChessPieceType type, char prettyColumn, int prettyRow)
            {
                Type = type;

                PrettyColumn = prettyColumn;
                PrettyRow = prettyRow;
            }

            public ChessPiece(ChessPieceType type, (int row, int column) positionIndex)
                : this(type, InvertedConverter.IndexToPrettyColumnConverter(positionIndex.column), InvertedConverter.IndexToPrettyRowConverter(positionIndex.row))
            { }

            public override string ToString() => $"Type: {Type, 7}, PrettyColumn: {PrettyColumn}, PrettyRow: {PrettyRow}, Index: {Index}";
        }

        class BishopChessPiece
            : ChessPiece
        {
            public BishopChessPiece(char prettyColumn, int prettyRow)
                : base(ChessPieceType.Meta.Bishop, prettyColumn, prettyRow)
            { }

            public BishopChessPiece((int row, int column) positionIndex)
                : base(ChessPieceType.Meta.Bishop, positionIndex)
            { }

            public override IEnumerable<(int row, int column)> GetCollusionIndices((int width, int height) tableSize)
            {
                var indices = new List<(int row, int column)>();

                // Bal oldalt lefelé
                {
                    (int row, int column) current = (Index.row - 1, Index.column - 1);

                    while (current.row >= 0 && current.column >= 0)
                    {
                        indices.Add(current);

                        current.row--;
                        current.column--;
                    }
                }

                // Jobb oldalt felfelé
                {
                    (int row, int column) current = (Index.row + 1, Index.column + 1);

                    while (current.row < tableSize.height && current.column < tableSize.width)
                    {
                        indices.Add(current);

                        current.row++;
                        current.column++;
                    }
                }

                // Bal oldalt felfelé
                {
                    (int row, int column) current = (Index.row + 1, Index.column - 1);

                    while (current.row < tableSize.height && current.column >= 0)
                    {
                        indices.Add(current);

                        current.row++;
                        current.column--;
                    }
                }

                // Jobb oldalt lefelé
                {
                    (int row, int column) current = (Index.row - 1, Index.column + 1);

                    while (current.row >= 0 && current.column < tableSize.width)
                    {
                        indices.Add(current);

                        current.row--;
                        current.column++;
                    }
                }

                return indices;
            }
        }

        class KingChessPiece
            : ChessPiece
        {
            public KingChessPiece(char prettyColumn, int prettyRow)
                : base(ChessPieceType.Meta.King, prettyColumn, prettyRow)
            { }

            public KingChessPiece((int row, int column) positionIndex)
                : base(ChessPieceType.Meta.King, positionIndex)
            { }

            public override IEnumerable<(int row, int column)> GetCollusionIndices((int width, int height) tableSize)
            {
                var indices = new List<(int row, int column)>();

                var neighbourOffsetList = new List<(int rowOffset, int columnOffset)>()
                {
                     (1, -1),     (1, 0),      (1, 1),
                     (0, -1),   /* center */   (0, 1),
                    (-1, -1),    (-1, 0),     (-1, 1),
                };

                foreach (var (rowOffset, columnOffset) in neighbourOffsetList)
                {
                    (int row, int column) finalIndex = (Index.row + rowOffset, Index.column + columnOffset);

                    if (finalIndex.row < 0 || finalIndex.column < 0 || finalIndex.row >= tableSize.height || finalIndex.column >= tableSize.width)
                    {
                        continue;
                    }

                    indices.Add(finalIndex);
                }

                return indices;
            }
        }

        class RookChessPiece
            : ChessPiece
        {
            public RookChessPiece(char prettyColumn, int prettyRow)
                : base(ChessPieceType.Meta.Rook, prettyColumn, prettyRow)
            { }

            public RookChessPiece((int row, int column) positionIndex)
                : base(ChessPieceType.Meta.Rook, positionIndex)
            { }

            public override IEnumerable<(int row, int column)> GetCollusionIndices((int width, int height) tableSize)
            {
                var indices = new List<(int row, int column)>();

                // Lentről fölfelé
                for (int row = 0; row < tableSize.height; row++)
                {
                    if (row != Index.row)
                    {
                        indices.Add((row, Index.column));
                    }
                }

                // Balról jobbra
                for (int column = 0; column < tableSize.width; column++)
                {
                    if (column != Index.column)
                    {
                        indices.Add((Index.row, column));
                    }
                }

                return indices;
            }
        }

        class ChessTable
        {
            public int Width { get; }
            public int Height { get; }

            public ChessPiece[,] Table { get; }

            public ChessTable(IEnumerable<ChessPiece> placement, int width = 8, int height = 8)
            {
                Width = width;
                Height = height;
                Table = new ChessPiece[Width, Height];

                FillTable(placement);
            }

            private void FillTable(IEnumerable<ChessPiece> placement)
            {
                foreach (var piece in placement)
                {
                    var (rowIndex, columnIndex) = piece.Index;
                    Table[rowIndex, columnIndex] = piece; 
                }
            }

            public ChessPiece this[char prettyColumn, int prettyRow]
            {
                get
                {
                    var (rowIndex, columnIndex) = ChessPiece.Converter.PrettyToIndexConverter(prettyRow, prettyColumn);
                    return Table[rowIndex, columnIndex];
                }
            }

            public IEnumerable<ChessPiece> GetEveryChessPieceOnTable() => Table.Cast<ChessPiece>().Where(chessPiece => chessPiece != null).ToList();

            public bool IsCellEmpty(int row, int column) => Table[row, column] == null;

            public bool IsRookInCollusion(ChessPiece chessPiece, IEnumerable<ChessPieceType> allowedCollusion)
            {
                if (chessPiece?.Type != ChessPieceType.Meta.Rook) throw new ArgumentException("Chess piece does not exist or not rook type.");

                var indices = (chessPiece as RookChessPiece).GetCollusionIndices((Width, Height));
                return IsInCollusionByIndices(indices, allowedCollusion);
            }

            public bool IsBishopInCollusion(ChessPiece chessPiece, IEnumerable<ChessPieceType> allowedCollusion)
            {
                if (chessPiece?.Type != ChessPieceType.Meta.Bishop) throw new ArgumentException("Chess piece does not exist or not rook type.");

                var indices = (chessPiece as BishopChessPiece).GetCollusionIndices((Width, Height));
                return IsInCollusionByIndices(indices, allowedCollusion);
            }

            private bool IsInCollusionByIndices(IEnumerable<(int row, int column)> indices, IEnumerable<ChessPieceType> allowedCollusion)
            {
                foreach (var (row, column) in indices)
                {
                    if (!IsCellEmpty(row, column) && allowedCollusion.Any(chessPieceType => chessPieceType == Table[row, column].Type))
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool IsKingInCheck(ChessPiece king)
            {
                if (king?.Type != ChessPieceType.Meta.King) throw new ArgumentException("Provided ChessPiece is not a king");

                var allowedCollusions = new List<ChessPieceType>() { ChessPieceType.Meta.King };

                {
                    var rookList = GetEveryChessPieceOnTable().Where(chessPiece => chessPiece?.Type == ChessPieceType.Meta.Rook);
                    foreach (var rook in rookList) {
                        if (IsRookInCollusion(rook, allowedCollusions))
                        {
                            return true;
                        }
                    }
                }

                {
                    var bishopList = GetEveryChessPieceOnTable().Where(chessPiece => chessPiece?.Type == ChessPieceType.Meta.Bishop);
                    foreach (var bishop in bishopList)
                    {
                        if (IsBishopInCollusion(bishop, allowedCollusions))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            public IEnumerable<ChessPiece> GetKingAttackPossibilities(ChessPiece king)
            {
                if (king?.Type != ChessPieceType.Meta.King) throw new ArgumentException("Provided ChessPiece is not a king");

                var possibleHits = new List<ChessPiece>();

                var indices = king.GetCollusionIndices((Width, Height));

                foreach (var (row, column) in indices)
                {
                    var originalNeighbour = Table[row, column];
                    if (originalNeighbour == null)
                    {
                        continue;
                    }

                    // Le kell venni a királyt és a szomszédos figurát a tábláról
                    var originalKing = king;
                    Table[originalKing.Index.row, originalKing.Index.column] = null;
                    Table[originalNeighbour.Index.row, originalNeighbour.Index.column] = null;

                    // Oda kell tenni a szomszédos figura helyére, és meg kell nézni, hogy ott sakk-ban lenne-e
                    var movedKing = new KingChessPiece((row, column));
                    Table[row, column] = movedKing;

                    if (!IsKingInCheck(movedKing))
                    {
                        possibleHits.Add(originalNeighbour);
                    }

                    // Vissza kell rendezni az eredeti állapotot
                    Table[originalKing.Index.row, originalKing.Index.column] = originalKing;
                    Table[originalNeighbour.Index.row, originalNeighbour.Index.column] = originalNeighbour;
                }

                return possibleHits;
            }

            #region Printing functions
            public IEnumerable<IEnumerable<char>> PlacementToConsoleFormat(char placeholder = ' ')
            {
                var output = new List<List<char>>();

                for (int i = Height - 1; i >= 0; i--)
                {
                    var currentLine = new List<char>();
                    for (int j = 0; j < Width; j++)
                    {
                        currentLine.Add(Table[i, j]?.Type.ShortName ?? placeholder);
                    }
                    output.Add(currentLine);
                }

                return output;
            }

            public void PrintPlacementToConsole()
            {
                var placement = PlacementToConsoleFormat();
                foreach (var row in placement)
                {
                    foreach (var element in row)
                    {
                        Console.Write(element);
                    }
                    Console.WriteLine();
                }
            }

            public void PrintPrettyPlacementToConsole()
            {
                var placement = PlacementToConsoleFormat();
                foreach (var row in placement)
                {
                    Console.WriteLine(PureHorizontalLine());
                    Console.WriteLine(StrippedHorizontalLine(row));
                }
                Console.WriteLine(PureHorizontalLine());

                string PureHorizontalLine()
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < Width; i++)
                    {
                        sb.Append("+-");
                    }
                    sb.Append("+");
                    return sb.ToString();
                }

                string StrippedHorizontalLine(IEnumerable<char> line)
                {
                    var sb = new StringBuilder();
                    foreach (var element in line)
                    {
                        sb.Append($"|{element}");
                    }
                    sb.Append("|");
                    return sb.ToString();
                }
            }
            #endregion
        }

        static void Main(string[] args)
        {
            var filename = $"input{Path.DirectorySeparatorChar}placement.csv";

            var placement = LoadPlacementFromFile(filename, ChessPieceType.TypeList);

            Console.WriteLine($"Input placement: (count: {placement.Count})");
            foreach (var piece in placement)
            {
                Console.WriteLine($"  - {piece}");
            }

            var table = new ChessTable(placement);

            Console.WriteLine($"{Environment.NewLine}Table:");
            table.PrintPrettyPlacementToConsole();

            Console.WriteLine(table.IsRookInCollusion(table['C', 8], ChessPieceType.TypeList) ?
                "Rook is colliding" :
                "Rook isnt colliding"
            );

            Console.WriteLine(table.IsBishopInCollusion(table['C', 5], ChessPieceType.TypeList) ?
                "Bishop is colliding" :
                "Bishop isnt colliding"
            );

            var king = table.GetEveryChessPieceOnTable().Where(chessPiece => chessPiece?.Type == ChessPieceType.Meta.King).First();
            Console.WriteLine(table.IsKingInCheck(king) ?
                "King is in check" :
                "King is not in check"
            );

            var attackPossibilitiesList = table.GetKingAttackPossibilities(king);
            Console.WriteLine($"Attack possibilities for the king: (count: {attackPossibilitiesList.Count()})");
            foreach (var attackPossibilities in attackPossibilitiesList)
            {
                Console.WriteLine($"  - {attackPossibilities}");
            }
        }

        static List<ChessPiece> LoadPlacementFromFile(string filename, IEnumerable<ChessPieceType> chessPieceTypeList)
        {
            var lines = File.ReadAllLines(filename);
            if (lines.Count() == 0)
            {
                throw new Exception($"{filename} file is empty");
            }

            return lines
                .Select(line =>
                {
                    var splittedLine = line.Split(';');

                    var lineStruct = new
                    {
                        ChessPieceName = splittedLine[0],
                        Column = char.Parse(splittedLine[1]),
                        Row = int.Parse(splittedLine[2])
                    };

                    var matchingChessPieceType = chessPieceTypeList
                        .Where(piece => piece.CsvName.Equals(lineStruct.ChessPieceName))
                        .FirstOrDefault();

                    if (matchingChessPieceType == null) throw new ArgumentException($"Invalid data. (Line: {line})");

                    return CreateMatchingObject(matchingChessPieceType, lineStruct.Column, lineStruct.Row);
                })
                .ToList();

            ChessPiece CreateMatchingObject(ChessPieceType matchingChessPieceType, char prettyColumn, int prettyRow)
            {
                if (matchingChessPieceType == ChessPieceType.Meta.Bishop) return new BishopChessPiece(prettyColumn, prettyRow);
                else if (matchingChessPieceType == ChessPieceType.Meta.King) return new KingChessPiece(prettyColumn, prettyRow);
                else if (matchingChessPieceType == ChessPieceType.Meta.Rook) return new RookChessPiece(prettyColumn, prettyRow);
                else throw new ArgumentException($"Invalid ChessPieceType : {matchingChessPieceType}");
            }
        }
    }
}
