namespace LogViewer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== LogViewer ===");

        var subscriber = new Subscriber();

        try
        {
            subscriber.Start();
            Console.WriteLine("Presiona Enter para salir...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}