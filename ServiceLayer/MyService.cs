using RepositoryLayer;

namespace ServiceLayer;

public class MyService : IMyService
{
    private readonly IMyRepository _repository;

    public MyService(IMyRepository repository)
    {
        _repository = repository;
    }
    
    public int Add(int a, int b)
    {
        var result = a + b;
        
        _repository.Log($"Addition of {a} and {b} is {result}");
        return result;
    }
}