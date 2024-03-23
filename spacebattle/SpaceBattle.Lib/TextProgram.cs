using Hwdtech;

public class ServerProgram
{
    public static void Main(int[] args)
    {
        var NumberOfTreads = args[0];
        Console.WriteLine("Запуск сервера\nСоздание и регистрация потоков...");
        IoC.Resolve<ICommand>("Game.Commands.StartServerCommand", NumberOfTreads).Execute();
        Console.WriteLine("Потоки запущены\nДля завершения работы сервера нажмите любую клавишу");
        Console.ReadKey();
        Console.WriteLine("Завершение работы сервера. Закрытие потоков...");
        IoC.Resolve<ICommand>("Game.Commands.StopServerCommand");
        Console.WriteLine("Все потоки закрыты. Завершение работы программы");
    }
}