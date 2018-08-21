using System;
using System.Linq;

namespace Gergov_Servebeer_Tasks._01.Basics
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] array = { 5, 12, 2, 46, 9, 13, 32, 1, 8 };

            SolveTask1(array);
            SolveTask2(array);
            SolveTask3(array);
            SolveTask4(array);
            SolveTask5(array);
        }

        static void SolveTask1(int[] array)
        {
            Console.WriteLine("Írasd ki a képernyõre a tömb elsõ 5 elemét!");

            foreach (var element in array.Take(5))
            {
                Console.WriteLine(element);
            }
        }

        static void SolveTask2(int[] array)
        {
            Console.WriteLine("Írasd ki a tömb elemeit fordított sorrendben!");

            foreach (var element in array.Reverse())
            {
                Console.WriteLine(element);
            }
        }

        static void SolveTask3(int[] array)
        {
            Console.WriteLine("Írasd ki a képernyõre a tömb elsõ, majd minden 3. elemét!");

            foreach (var element in array.Where((item, index) => index % 3 == 0))
            {
                Console.WriteLine(element);
            }
        }

        static void SolveTask4(int[] array)
        {
            Console.WriteLine("Írasd ki a tömb páros elemeit!");

            foreach (var element in array.Where(item => item % 2 == 0))
            {
                Console.WriteLine(element);
            }
        }

        static void SolveTask5(int[] array)
        {
            Console.WriteLine("Írasd ki a tömb elsõ n elemét, egészen addig, amíg nem találsz egy 40-nél nagyobbat!");

            foreach (var element in array.TakeWhile(item => item <= 40))
            {
                Console.WriteLine(element);
            }
        }
    }
}
