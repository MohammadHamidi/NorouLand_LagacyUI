public interface INameStorage
{
    void SaveName(string name);
    string LoadName();
    void DeleteName();
    bool HasName();
}