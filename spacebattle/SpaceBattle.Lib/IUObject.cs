namespace SpaceBattle.Lib
{
    public interface IUObject
    {
        public object getProperty(string key);
        public void setProperty(string key, object value);
        public void DeleteProperty(string name);
    }
}
