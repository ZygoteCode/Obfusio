using dnlib.DotNet;

namespace Obfusio.Engine.Helpers
{
    public class Utils
    {
        public static SuperRandom Random { get; private set; } = new SuperRandom(1);
        private static char[] _characters = "ABCDEFGHIJKLMNOQPRSTUVWXYZ0123456789".ToCharArray();

        public static string GenerateName()
        {
            return Random.GetRandomString(_characters, 8);
        }

        public static bool IsValidManagedAssembly(string inputFilePath)
        {
            try
            {
                return ModuleDefMD.Load(inputFilePath).Types.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}