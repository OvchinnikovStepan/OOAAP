// using Hwdtech;
// using SpaceBattle.Lib;

// public class ServerProgram
// {
//     public static void Main(int[] args)
//     {
//         var NumberOfTreads = args[0];
//         new InitCommand().Execute();
//         Console.WriteLine("Запуск сервера\nСоздание и регистрация потоков...");
//         IoC.Resolve<Hwdtech.ICommand>("Game.Commands.StartServerCommand", NumberOfTreads).Execute();
//         Console.WriteLine("Потоки запущены\nДля завершения работы сервера нажмите любую клавишу");
//         Console.ReadKey();
//         Console.WriteLine("Завершение работы сервера. Закрытие потоков...");
//         IoC.Resolve<Hwdtech.ICommand>("Game.Commands.StopServerCommand");
//         Console.WriteLine("Все потоки закрыты. Завершение работы программы");
//     }
// }
