public static class LobbyCodeGenerator
{
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Generate(int length = 5)
    {
        char[] code = new char[length];
        for (int i = 0; i < length; i++)
        {
            code[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return new string(code);
    }
}