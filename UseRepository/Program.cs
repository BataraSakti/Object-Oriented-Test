using FormulatrixRepositoryManager;

class Program {
    static void Main(string[] args){
        RepositoryManager repository = new RepositoryManager();

        repository.Initialized();

        repository.Register("json1", "{\"key_1\": \"value json1\"}", ItemType.JSON);
        var json1 = repository.Retrieve("json1");
        Console.WriteLine($"Retreived JSON item: {json1}");

        repository.Register("xml1", "<root><key>xml1Value</key></root>", ItemType.XML);
        var xml1 = repository.Retrieve("xml1");
        Console.WriteLine($"Retreieved XML item: {xml1}");
    }
}




