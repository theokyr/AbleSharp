namespace AbleSharp.SDK.Utils;

public static class IdGenerator
{
    private static int _nextId = 1;

    public static string GetNextId()
    {
        return (_nextId++).ToString();
    }

    public static int GetLastId()
    {
        return _nextId;
    }

    public static void Reset()
    {
        _nextId = 1;
    }
}