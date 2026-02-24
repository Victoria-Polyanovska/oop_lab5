using System;
using System.Text;

class Date
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }

    public Date() { }

    public Date(int year, int month, int day, int hours, int minutes)
    {
        Year = year;
        Month = month;
        Day = day;
        Hours = hours;
        Minutes = minutes;
    }

    public Date(Date other)
    {
        Year = other.Year;
        Month = other.Month;
        Day = other.Day;
        Hours = other.Hours;
        Minutes = other.Minutes;
    }

    public DateTime ToDateTime()
    {
        return new DateTime(Year, Month, Day, Hours, Minutes, 0);
    }

    public int TotalMinutes()
    {
        return (int)(ToDateTime() - new DateTime(Year, Month, Day)).TotalMinutes;
    }
}

class Airplane
{
    public string StartCity { get; set; }
    public string FinishCity { get; set; }
    public Date Start { get; set; }
    public Date Finish { get; set; }

    public Airplane(string startCity, string finishCity, Date start, Date finish)
    {
        StartCity = startCity;
        FinishCity = finishCity;
        Start = start;
        Finish = finish;
    }
}

class AirplaneService
{
    public int GetTotalTime(Airplane airplane)
    {
        return airplane.Finish.TotalMinutes() - airplane.Start.TotalMinutes();
    }

    public bool IsArrivingToday(Airplane airplane)
    {
        return airplane.Finish.ToDateTime().Date == DateTime.Now.Date;
    }
}

static class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;
        Console.Title = "Лабораторна робота №5";
        Console.SetWindowSize(100, 25);
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Clear();

        Airplane[] airplanes = ReadAirplaneArray();
        if (airplanes.Length == 0)
        {
            Console.WriteLine("Немає введених рейсів.");
            return;
        }

        AirplaneService service = new AirplaneService();
        bool running = true;
        while (running)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Вивести всі рейси");
            Console.WriteLine("2. Знайти найкоротший та найдовший шлях");
            Console.WriteLine("3. Сортувати по даті відправки");
            Console.WriteLine("4. Сортувати за часом польоту");
            Console.WriteLine("5. Вихід");
            Console.Write("Виберіть опцію: ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    PrintAirplanes(airplanes, service);
                    break;
                case "2":
                    int min, max;
                    if (GetAirplaneInfo(airplanes, service, out min, out max))
                        Console.WriteLine($"Найкоротший час: {min} хв, Найдовший час: {max} хв");
                    else
                        Console.WriteLine("Немає достатніх даних для аналізу.");
                    break;
                case "3":
                    SortAirplanesByDate(airplanes);
                    PrintAirplanes(airplanes, service);
                    break;
                case "4":
                    SortAirplanesByTotalTime(airplanes, service);
                    PrintAirplanes(airplanes, service);
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    static Airplane[] ReadAirplaneArray()
    {
        Console.Write("Введіть кількість рейсів: ");
        string input = Console.ReadLine();
        if (!int.TryParse(input, out int n) || n <= 0)
        {
            Console.WriteLine("Невірне значення! Має бути додатне число.");
            return new Airplane[0];
        }

        Airplane[] airplanes = new Airplane[n];
        for (int i = 0; i < n; i++)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nВведення даних для рейсу {i + 1}:");

            Console.Write("Місто відправки: ");
            string startCity = Console.ReadLine();

            Console.Write("Місто прибуття: ");
            string finishCity = Console.ReadLine();

            Console.Write("Дата відправки (рік місяць день година хвилина): ");
            Date startDate = ReadDate();

            Console.Write("Дата прибуття (рік місяць день година хвилина): ");
            Date finishDate = ReadDate();

            airplanes[i] = new Airplane(startCity, finishCity, startDate, finishDate);
        }
        return airplanes;
    }

    static Date ReadDate()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (TryParseDate(input, out Date date))
                return date;

            Console.WriteLine("Невірний формат! Спробуйте ще раз.");
        }
    }

    static bool TryParseDate(string input, out Date date)
    {
        date = null;
        string[] parts = input.Split();
        if (parts.Length == 5 &&
            int.TryParse(parts[0], out int year) &&
            int.TryParse(parts[1], out int month) &&
            int.TryParse(parts[2], out int day) &&
            int.TryParse(parts[3], out int hours) &&
            int.TryParse(parts[4], out int minutes))
        {
            date = new Date(year, month, day, hours, minutes);
            return true;
        }
        return false;
    }

    static void PrintAirplane(Airplane airplane, AirplaneService service)
    {
        Console.WriteLine($"{airplane.StartCity} -> {airplane.FinishCity}, Час польоту: {service.GetTotalTime(airplane)} хв, Прибуття сьогодні: {service.IsArrivingToday(airplane)}");
    }

    static void PrintAirplanes(Airplane[] airplanes, AirplaneService service)
    {
        foreach (var airplane in airplanes)
            PrintAirplane(airplane, service);
    }

    static bool GetAirplaneInfo(Airplane[] airplanes, AirplaneService service, out int minTime, out int maxTime)
    {
        if (airplanes.Length == 0)
        {
            minTime = maxTime = 0;
            return false;
        }

        minTime = int.MaxValue;
        maxTime = int.MinValue;
        foreach (var airplane in airplanes)
        {
            int time = service.GetTotalTime(airplane);
            if (time < minTime) minTime = time;
            if (time > maxTime) maxTime = time;
        }
        return true;
    }

    static void SortAirplanesByDate(Airplane[] airplanes)
    {
        Array.Sort(airplanes, (a, b) => a.Start.ToDateTime().CompareTo(b.Start.ToDateTime()));
    }

    static void SortAirplanesByTotalTime(Airplane[] airplanes, AirplaneService service)
    {
        Array.Sort(airplanes, (a, b) => service.GetTotalTime(a).CompareTo(service.GetTotalTime(b)));
    }
}
