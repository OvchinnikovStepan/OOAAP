namespace SpaceBattle.Lib;

public interface IUObject
{
    void DeleteProperty(string name);
    void setProperty(string key, object value);
    object getProperty(string key);
}
