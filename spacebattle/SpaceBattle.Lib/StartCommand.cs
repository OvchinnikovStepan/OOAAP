namespace SpaceBattle.Lib;

public interface IStartCommand{
    IUObject Target {get;}
    IDictionary <string,object> Properties {get;}
}