using FormulatrixRepositoryManager;

namespace RepositoryManagerTest;

public class RepositoryManagerTest
{
    private RepositoryManager repository;

    [SetUp]
    public void Setup(){
        repository = new RepositoryManager();
        repository.Initialized();
    }

    [Test]
    public void JsonRegisterAndRetreived_Success(){
        repository.Register("jsonTest", "{\"test_key\": \"test_value\"}", ItemType.JSON);
        var result = repository.Retrieve("jsonTest");

        Assert.That(result, Is.EqualTo("{\"test_key\": \"test_value\"}"));
    }

    [Test]
    public void XmlRegisterAndRetreived_Success(){
        repository.Register("XMLtest", "<root><key>xml_test</key></root>", ItemType.XML);
        var result = repository.Retrieve("XMLtest");

        Assert.That(result, Is.EqualTo("<root><key>xml_test</key></root>"));
    }

    [Test]
    public void JsonGetType_Success(){
        repository.Register("jsonTest", "{\"test_key\": \"test_value\"}", ItemType.JSON);
        var result = repository.GetType("jsonTest");

        Assert.That(result, Is.EqualTo((int)ItemType.JSON));
    }

    [Test]
    public void XmlGetType_Success(){
         repository.Register("XMLtest", "<root><key>xml_test</key></root>", ItemType.XML);
         var result = repository.GetType("XMLtest");

          Assert.That(result, Is.EqualTo((int)ItemType.XML));
    }

    [Test]
    public void DeregisterItem_Success(){
        repository.Register("jsonToRemove", "{\"test_key\": \"json_to_remove\"}", ItemType.JSON);
        repository.Deregister("jsonToRemove");

        Assert.Throws<KeyNotFoundException>(()=> repository.Retrieve("jsonToRemove"));
    }

    [Test]
    public void RegisterDuplicateItem_Exception(){
        repository.Register("jsonDuplicate", "{\"test_key\": \"duplicate_json\"}", ItemType.JSON);

        Assert.Throws<InvalidOperationException>(()=> repository.Register("jsonDuplicate", "{\"test_key\": \"duplicate_json\"}", ItemType.JSON));
    }

    [Test]
    public void JsonValidateInvalidItem_Exception(){
        Assert.Throws<ArgumentException>(() => repository.Register("invalidJsonItem", "invalidJsonContent", ItemType.JSON));
    }

    [Test]
    public void XmlValidateInvalidItem_Exception(){
        Assert.Throws<ArgumentException>(() => repository.Register("invalidXmlItem", "invalidXmlContent", ItemType.XML));
    }
}