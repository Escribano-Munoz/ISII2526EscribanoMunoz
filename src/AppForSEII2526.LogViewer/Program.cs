using System;
using LogViewer;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- LOG VIEWER MENU ---");
        Console.WriteLine("1. Solo ERRORES (log.error)");
        Console.WriteLine("2. Solo INFORMACION (log.info)");
        Console.WriteLine("3. Solo WARNINGS (log.warning)");
        Console.WriteLine("4. Solo DEBUG (log.debug)");
        Console.WriteLine("5. Solo CRITICAL (log.critical)");
        Console.WriteLine("6. TODOS los logs");
        Console.WriteLine("7. SALIR Y CERRAR MENU");

        string topicPattern;

        while (true)
        {
            Console.Write("\nSelecciona una opcion (1-7): ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    topicPattern = "log.error";
                    break;
                case "2":
                    topicPattern = "log.info";
                    break;
                case "3":
                    topicPattern = "log.warning";
                    break;
                case "4":
                    topicPattern = "log.debug";
                    break;
                case "5":
                    topicPattern = "log.critical";
                    break;
                case "6":
                    topicPattern = "#";
                    break;
                case "7":
                    Console.WriteLine("Saliendo del menu. No se iniciara el LogViewer.");
                    return;
                default:
                    Console.WriteLine("Opcion invalida. Intenta de nuevo.");
                    continue;
            }

            break;
        }

        Console.WriteLine($"\nIniciando LogViewer con topic: {topicPattern}");
        Console.WriteLine("Presiona ENTER para salir.\n");

        var subscriber = new Subscriber(topicPattern);
        subscriber.Start();
    }
}