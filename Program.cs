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
}

class Airplane
{
    public string StartCity { get; set; }
    public string FinishCity { get; set; }
    public Date StartDate { get; set; }
    public Date FinishDate { get; set; }

    public Airplane() { }

    public Airplane(string startCity, string finishCity, Date startDate, Date finishDate)
    {
        StartCity = startCity;
        FinishCity = finishCity;
        StartDate = startDate;
        FinishDate = finishDate;
    }

    public Airplane(Airplane other)
    {
        StartCity = other.StartCity;
        FinishCity = other.FinishCity;
        StartDate = new Date(other.StartDate);
        FinishDate = new Date(other.FinishDate);
    }

    public int GetTotalTime()
    {
        return (int)(FinishDate.ToDateTime() - StartDate.ToDateTime()).TotalMinutes;
    }

    public bool IsArrivingToday()
    {
        return StartDate.Year == FinishDate.Year && StartDate.Month == FinishDate.Month && StartDate.Day == FinishDate.Day;
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
                    PrintAirplanes(airplanes);
                    break;
                case "2":
                    int min, max;
                    if (GetAirplaneInfo(airplanes, out min, out max))
                        Console.WriteLine($"Найкоротший час: {min} хв, Найдовший час: {max} хв");
                    else
                        Console.WriteLine("Немає достатніх даних для аналізу.");
                    break;
                case "3":
                    SortAirplanesByDate(airplanes);
                    PrintAirplanes(airplanes);
                    break;
                case "4":
                    SortAirplanesByTotalTime(airplanes);
                    PrintAirplanes(airplanes);
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
        int n;
        if (!int.TryParse(input, out n) || n <= 0)
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
            string[] parts = Console.ReadLine().Split();
            if (parts.Length == 5)
            {
                int year, month, day, hours, minutes;
                if (int.TryParse(parts[0], out year) &&
                    int.TryParse(parts[1], out month) &&
                    int.TryParse(parts[2], out day) &&
                    int.TryParse(parts[3], out hours) &&
                    int.TryParse(parts[4], out minutes))
                {
                    return new Date(year, month, day, hours, minutes);
                }
            }
            Console.Write("Невірний формат! Введіть ще раз: ");
        }
    }

    static void PrintAirplane(Airplane airplane)
    {
        Console.WriteLine($"{airplane.StartCity} -> {airplane.FinishCity}, Час польоту: {airplane.GetTotalTime()} хв, Прибуття в цей же день: {airplane.IsArrivingToday()}");
    }

    static void PrintAirplanes(Airplane[] airplanes)
    {
        for (int i = 0; i < airplanes.Length; i++)
        {
            PrintAirplane(airplanes[i]);
        }
    }

    static bool GetAirplaneInfo(Airplane[] airplanes, out int minTime, out int maxTime)
    {
        if (airplanes.Length == 0)
        {
            minTime = maxTime = 0;
            return false;
        }

        minTime = int.MaxValue;
        maxTime = int.MinValue;
        for (int i = 0; i < airplanes.Length; i++)
        {
            int time = airplanes[i].GetTotalTime();
            if (time < minTime) minTime = time;
            if (time > maxTime) maxTime = time;
        }
        return true;
    }

    static void SortAirplanesByDate(Airplane[] airplanes)
    {
        Array.Sort(airplanes, delegate (Airplane a, Airplane b)
        {
            return b.StartDate.ToDateTime().CompareTo(a.StartDate.ToDateTime());
        });
    }

    static void SortAirplanesByTotalTime(Airplane[] airplanes)
    {
        Array.Sort(airplanes, delegate (Airplane a, Airplane b)
        {
            return a.GetTotalTime().CompareTo(b.GetTotalTime());
        });
    }
}