using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Gergov_Servebeer_Tasks._10.CsvProcessing
{
    class Program
    {
        class Dog
        {
            public string Name { get; }
            public string Breed { get; }
            public int Age { get; }
            public long Id { get; }

            public Dog(string name, string breed, int age, long id)
            {
                Name = name;
                Breed = breed;
                Age = age;
                Id = id;
            }

            public override string ToString() => $"{Name} {Breed} {Age.ToString()} {Id.ToString()}";

            public string ToCsv() => $"{Name};{Breed};{Age.ToString()};{Id.ToString()}";
        }

        static void Main(string[] args)
        {
            var filename = $"input{Path.DirectorySeparatorChar}input.csv";

            // 1. Töltsd be a felügyelt kutyák adatait és tárold el a számítógép memóriájában!
            var dogs = LoadDogsFromFile(filename);

            Console.WriteLine("List of all dogs:");
            foreach (var dog in dogs)
            {
                Console.WriteLine($"  - {dog}");
            }

            // 2. Kérd be a felhasználótól egy kutya nevét, és írasd ki a hozzá tartozó azonosítót!
            Console.WriteLine("Type in a dog's name, and I'll try to find its id:");
            var searchedName = Console.ReadLine();
            var findResultDog = TryToFindSpecificDogByName(dogs, searchedName);
            Console.WriteLine((findResultDog != null) ?
                $"Search successed. {searchedName} => {findResultDog.Id}" :
                $"Search failed. {searchedName} doesn't exists."
            );

            // 3. Írd ki a kutyák nevét az életkoruk szerinti növekvõ sorrendben.
            Console.WriteLine("List of dog's name's by descanding age:");
            foreach (var dog in OrderDogsByAgeDescending(dogs))
            {
                Console.WriteLine($"{dog.Name} ({dog.Age})");
            }

            // 4. Számold meg, hogy hány 4 évnél fiatalabb kutya felügyeletét látja el a cég! Írasd ki a képernyõre!
            Console.WriteLine($"Sum of dogs under 4 years: {SelectDogsUnderExclusiveAge(dogs, 4).Count()}");

            // 5. Kérd be a felhasználótól egy újabb kutya adatait! Írd hozzá a megadott adatokat az input.csv fájlhoz!
            Console.WriteLine("Type in a new dogs's data.");
            AppendNewDogsToFile(filename, new List<Dog> { ReadNewDogFromConsole() });
        }

        static List<Dog> LoadDogsFromFile(string filename)
        {
            var lines = File.ReadAllLines(filename);
            if (lines.Count() <= 1)
            {
                throw new Exception("Not enough lines.");
            }

            // Az első sorban van a fejléc, ami nem érdekel minket
            return lines.Skip(1)
                .Select(line => {
                    var splittedLine = line.Split(';');
                    return new Dog(
                        name: splittedLine[0],
                        breed: splittedLine[1],
                        age: int.Parse(splittedLine[2]),
                        id: long.Parse(splittedLine[3])
                    );
                })
                .ToList();
        }

        static Dog ReadNewDogFromConsole()
        {
            Console.WriteLine("Name:");
            var newName = Console.ReadLine();

            Console.WriteLine("Breed:");
            var newBreed = Console.ReadLine();

            Console.WriteLine("Age:");
            var newAge = int.Parse(Console.ReadLine());

            Console.WriteLine("Id:");
            var newId = long.Parse(Console.ReadLine());

            return new Dog(newName, newBreed, newAge, newId);
        }

        static void AppendNewDogsToFile(string filename, IEnumerable<Dog> dogs)
        {
            File.AppendAllLines(filename, ConvertDogsToFullCsvFormat(dogs));
            Console.WriteLine($"{filename} file updated.");
        }

        static Dog TryToFindSpecificDogByName(IEnumerable<Dog> dogs, string searchedName)
        {
            return dogs.FirstOrDefault(dog => dog.Name.Equals(searchedName));
        }

        static IEnumerable<Dog> OrderDogsByAgeDescending(IEnumerable<Dog> dogs)
        {
            return dogs.OrderByDescending(dog => dog.Age);
        }

        static IEnumerable<Dog> SelectDogsUnderExclusiveAge(IEnumerable<Dog> dogs, int exclusiveMaximumAge)
        {
            return dogs.Where(dog => dog.Age < exclusiveMaximumAge);
        }

        static IEnumerable<string> ConvertDogsToFullCsvFormat(IEnumerable<Dog> dogs)
        {
            return dogs.Select(dog => dog.ToCsv());
        }
    }
}
