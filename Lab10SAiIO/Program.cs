using System;
using System.Linq;

namespace Lab10SAiIO
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] matrix = {
                {-3, -4, -2, -6, -7},
                {1, 0, -1, 0, -3},
                {-4, -5, -2, -3, -2},
                {3, -4, 1, 1, -3},
                {-8, -3, -1, -6, -7}
            };
            int[,] reverseMatrix = {
                {-3, 1, -4, 3, -8},
                {-4, 0, -5, -4, -3},
                {-2, -1, -2, 1, -1},
                {-6, 0, -3, 1, -6},
                {-7, -3, -2, -3, -7}
            };
            int min = Int32.MaxValue;
            for (int indexFirst = 0; indexFirst < matrix.GetLength(0); indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < matrix.GetLength(1); indexSecond++)
                {
                    if (matrix[indexFirst,indexSecond] < min)
                    {
                        min = matrix[indexFirst, indexSecond];
                    }
                }
            }
            double[] functionCoeff = new double[matrix.GetLength(0)];
            var limitationCoeff = new double[matrix.GetLength(0)][];
            for (int index = 0; index < functionCoeff.Length; index++)
            {
                functionCoeff[index] = 1;
                limitationCoeff[index] = new double[matrix.GetLength(1) + 1];
                for (int indexSecond = 0; indexSecond < matrix.GetLength(1); indexSecond++)
                {
                    limitationCoeff[index][indexSecond] = matrix[index, indexSecond] - min;
                }
            }
            var table = new Table(limitationCoeff);
            for (int index = 0; index < limitationCoeff.Length; index++)
            {
                table.ExtendColumn();                
            }
            table.ExtendLine();
            for (var index = 0; index < table.Matrix.Length - 1; index++)
            {
                table.Matrix[index][table.Matrix[index].Length - 1] =
                    table.Matrix[index][limitationCoeff[0].Length - 1];
                table.Matrix[index][limitationCoeff[0].Length - 1] = 0;
            }
            for (var index = 0; index < functionCoeff.Length; index++)
            {
                table.Matrix[^1][index] = -1;
                table.Matrix[index][^1] = 1;
            }
            for (var index = 0; index < limitationCoeff.Length; index++)
            {
                table.Matrix[index][index + limitationCoeff[0].Length - 1] = 1;
            }
            int[] basic = { 5, 6, 7, 8, 9};
            table.ResetOldMatrix();
            table.Simplex(basic, out basic);
            var simplexResult = new double[functionCoeff.Length];
            for (var index = 0; index < simplexResult.Length; index++)
            {
                simplexResult[index] = basic.Contains(index) ? table.Matrix[basic.ToList().IndexOf(index)][^1] : 0;
            }
            Console.WriteLine("Решение прямой задачи:");
            Console.WriteLine("Значения основных переменных: ");
            for (var index = 0; index < simplexResult.Length; index++)
            {
                Console.WriteLine("X" + index + "=" + simplexResult[index]);
            }
            var functionValue = simplexResult.Select((t, index) => (t * functionCoeff[index])).Sum();
            Console.WriteLine("Функция равна " + functionValue);
            Console.WriteLine("Решение двойственной задачи:");
            var simplexResultDual = new double[functionCoeff.Length];
            for (int index = 0; index < functionCoeff.Length; index++)
            {
                simplexResultDual[index] = table.Matrix[^1][functionCoeff.Length + index];
            }
            Console.WriteLine("Значения основных переменных: ");
            for (var index = 0; index < simplexResult.Length; index++)
            {
                Console.WriteLine("X" + index + "=" + simplexResultDual[index]);
            }
            var functionValueDual = simplexResultDual.Select((t, index) => (t * functionCoeff[index])).Sum();
            Console.WriteLine("Функция равна " + functionValueDual);
            Console.WriteLine("Цена игры: " + 1.0 / functionValueDual);
            Console.WriteLine("Оптимальная смешанная стратегия первого игрока:");
            for (int index = 0; index < functionCoeff.Length; index++)
            {
                Console.Write((simplexResultDual[index] / functionValueDual) + "\t");
            }
            Console.WriteLine();
            Console.WriteLine("Оптимальная смешанная стратегия второго игрока:");
            for (int index = 0; index < functionCoeff.Length; index++)
            {
                Console.Write((simplexResult[index] / functionValueDual) + "\t");
            }
        }
    }
}