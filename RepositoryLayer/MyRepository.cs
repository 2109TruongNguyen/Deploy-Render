namespace RepositoryLayer;

public class MyRepository : IMyRepository
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}