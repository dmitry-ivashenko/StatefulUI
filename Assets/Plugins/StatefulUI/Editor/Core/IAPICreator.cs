namespace StatefulUI.Editor.Core
{
    public interface IAPICreator
    {
        public string CreateAPI(string prefix = "");
        
        public string CreateDocs(string prefix = "");
    }
}