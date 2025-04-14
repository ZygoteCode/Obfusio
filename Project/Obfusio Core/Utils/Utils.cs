using dnlib.DotNet;

public class Utils
{
    private static SuperRandom _random = new SuperRandom(2);
    private static char[] _characters = "ABCDEFGHIJKLMNOQPRSTUVWXYZ0123456789".ToCharArray();

    public static string GenerateName()
    {
        return _random.GetRandomString(_characters, 8);
    }

    public static bool IsValidManagedAssembly(string inputFilePath)
    {
        try
        {
            return ModuleDefMD.Load(inputFilePath).Types.Count > 0;
        }
        catch
        {

        }

        return false;
    }
}