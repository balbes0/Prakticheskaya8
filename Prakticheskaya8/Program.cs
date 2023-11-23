using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;

[Serializable]
public class User
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public int CharactersPerSecond { get; set; }
}

public static class Leaderboard
{
    private static List<User> leaderboard;

    static Leaderboard()
    {
        leaderboard = new List<User>();
    }

    public static void AddUserToLeaderboard(User user)
    {
        leaderboard.Add(user);
        SaveLeaderboard();
    }

    public static void DisplayLeaderboard()
    {
        Console.WriteLine("Таблица рекордов:");
        Console.WriteLine("Имя\tСимволов в минуту\tСимволов в секунду");

        foreach (var user in leaderboard)
        {
            Console.WriteLine($"{user.Name}\t{user.CharactersPerMinute}\t\t\t{user.CharactersPerSecond}");
        }
    }

    private static void SaveLeaderboard()
    {
        string json = JsonConvert.SerializeObject(leaderboard, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("leaderboard.json", json);
    }

    public static void LoadLeaderboard()
    {
        if (File.Exists("leaderboard.json"))
        {
            string json = File.ReadAllText("leaderboard.json");
            leaderboard = JsonConvert.DeserializeObject<List<User>>(json);
        }
    }
}

public class TypingTest
{
    private static readonly string TextToType = "Быстрый барсук плавно прокрался сквозь заросли темного леса. Ветер свистел в ветвях, создавая мелодию природы. Барсук был умелым охотником и ловко избегал опасности. Его густая шерсть блестела на солнце, словно золото. Вдали раздавался тихий шорох, и барсук замер, напрягая свое внимание. Внезапно он рванулся вперёд, стремительно мчась сквозь заросли. В его глазах горел огонь страсти и решимости. Барсук был в поисках приключений и готов пройти через любые трудности. Его быстрые лапы неустанно двигались, оставляя за собой следы на мягкой почве лесной дорожки. Вскоре он исчез в густом зеленом покрове, оставив лишь слабый налет загадки в воздухе.";

    public static void StartTest(string userName)
    {
        Console.Clear();
        Console.WriteLine($"Привет {userName}!\nВаш текст для теста на скоропечатание:\n{TextToType}\nНажмите Enter, чтобы начать тест.");

        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

        Console.Clear();
        Console.WriteLine("Ваш текст для теста на скорость печатания, время пошло у вас есть 1 минута!:");
        Console.WriteLine();
        Console.Write(TextToType);
        Console.WriteLine();

        Stopwatch stopwatch = new Stopwatch();
        bool typingAllowed = true;

        Thread timerThread = new Thread(() =>
        {
            stopwatch.Start();
            Thread.Sleep(60000);
            typingAllowed = false;
            stopwatch.Stop();
        });

        timerThread.Start();

        int charactersTyped = 0;

        while (typingAllowed && charactersTyped < TextToType.Length)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key != ConsoleKey.Enter)
            {
                if (keyInfo.KeyChar == TextToType[charactersTyped])
                {
                    charactersTyped++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(keyInfo.KeyChar);
                    Console.ResetColor();
                }
            }
        }

        double elapsedMinutes = stopwatch.Elapsed.TotalMinutes;
        double charactersPerMinute = charactersTyped / elapsedMinutes;
        double charactersPerSecond = charactersTyped / stopwatch.Elapsed.TotalSeconds;

        Console.WriteLine($"\n\nТест завершен!");
        Console.WriteLine($"Символов в минуту: {charactersPerMinute}");
        Console.WriteLine($"Символов в секунду: {charactersPerSecond}");

        User user = new User
        {
            Name = userName,
            CharactersPerMinute = (int)charactersPerMinute,
            CharactersPerSecond = (int)charactersPerSecond
        };

        Leaderboard.AddUserToLeaderboard(user);
    }
}

class Program
{
    static void Main()
    {
        Leaderboard.LoadLeaderboard();

        while (true)
        {
            Console.Clear();
            Console.Write("Введите ваше имя: ");
            string userName = Console.ReadLine();

            TypingTest.StartTest(userName);

            Leaderboard.DisplayLeaderboard();

            Console.WriteLine("\nЖелаете пройти тест еще раз? (Enter) или нажмите ESC для выхода.");

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
            } while (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Escape);

            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}