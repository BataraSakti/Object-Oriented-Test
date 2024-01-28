using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Xml;


namespace FormulatrixRepositoryManager;

public enum ItemType {
    JSON = 1,
    XML = 2
}

/*Interface to extend in case other type of storage is needed*/
public interface IRepository{
    void Register(string itemName, string itemContent, ItemType itemType);
    string Retrieve(string itemName);
    int GetType(string itemName);
    void Deregister(string itemName);
}


public class RepositoryManager : IRepository
{

    /*private class members*/

    /*
    Use a in-memory storage, other storage type (database, file base, etc) would need
      another class implementation that extends the IRepository Interface
    */
    private readonly ConcurrentDictionary<string, Tuple<string, ItemType>> repository;

    private readonly object _lock = new object(); //implement lock to prevent concurrent access from multi-threading env.

    private bool isInitialized;

    //Constructor
    public RepositoryManager(){
        repository = new ConcurrentDictionary<string, Tuple<string, ItemType>>();
    }

    public void Initialized(){
        
        lock(_lock){
            if (!isInitialized){
                repository.Clear(); //start fresh
                isInitialized = true;
            } else {
                throw new InvalidOperationException("Initialized() - Repository has been initialized");
            }
        }
    }

    /*Interface Methods*/

    public void Register(string itemName, string itemContent, ItemType itemType){
       if (!isInitialized){
            throw new InvalidOperationException("Register() -  Repository is not initialized");
       }

       lock(_lock){

            if (repository.TryAdd(itemName, Tuple.Create(itemContent, itemType))){
                ValidateItemContent(itemContent, itemType);
            } else {
                throw new InvalidOperationException($"Item '{itemName}' already exists.");
            }
       }
    }

    public string Retrieve(string itemName){
        if (!isInitialized){
            throw new InvalidOperationException("Retrieve() -  Repository is not initialized");
       }

       lock (_lock){
            if (repository.TryGetValue(itemName, out var item)){
                return item.Item1;
            } else {
                throw new KeyNotFoundException($"Retrieve() - Item with name '{itemName}' doesn't exists.");
            }
       }
       
    }

    public int GetType(string itemName){
        if (!isInitialized){
            throw new InvalidOperationException("GetType() -  Repository is not initialized");
       }

        lock (_lock){
            if (repository.TryGetValue(itemName, out var item)){
                return (int)item.Item2;
            } else {
                throw new KeyNotFoundException($"GetType() - Item with name '{itemName}' doesn't exists.");
            }
       }
    }

    public void Deregister(string itemName){
        if (!isInitialized){
            throw new InvalidOperationException("Deregister() -  Repository is not initialized");
       }

       lock (_lock) {
            if (repository.TryRemove(itemName, out _)){
                //item removed
            } else {
                throw new KeyNotFoundException($"Deregister() - Item with name '{itemName}' doesn't exists.");
            }
       }
    }

    /*End of Interface Methods*/

    //Helper method for item validation
    private void ValidateItemContent(string itemContent, ItemType itemType){
        switch(itemType){
            case ItemType.JSON:{
                try {
                    JsonConvert.DeserializeObject(itemContent);
                } catch (JsonException jex){
                    throw new ArgumentException($"Invalid JSON format: {jex.Message}", nameof(itemContent));
                }
                break;
            }
            case ItemType.XML:{
                try{
                    new XmlDocument().LoadXml(itemContent);
                } catch (XmlException xmlEx){
                    throw new ArgumentException($"Invalid XML format: {xmlEx.Message}", nameof(itemContent));
                }
                break;
            }
            default : {
                throw new ArgumentException($"Unsupported Itemtype: {itemType}", nameof(itemType));
               
            }
        }
    }
}
