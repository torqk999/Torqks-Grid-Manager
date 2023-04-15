#pragma warning disable CS0246 // The type or namespace name 'Malware' could not be found (are you missing a using directive or an assembly reference?)
using Malware.MDKUtilities;
#pragma warning restore CS0246 // The type or namespace name 'Malware' could not be found (are you missing a using directive or an assembly reference?)

namespace IngameScript.MDK
{
    public class TestBootstrapper
    {
        // All the files in this folder, as well as all files containing the file ".debug.", will be excluded
        // from the build process. You can use this to create utilites for testing your scripts directly in 
        // Visual Studio.

        static TestBootstrapper()
        {
            // Initialize the MDK utility framework
            MDKUtilityFramework.Load();
        }

        public static void Main()
        {
            // In order for your program to actually run, you will need to provide a mockup of all the facilities 
            // your script uses from the game, since they're not available outside of the game.

            // Create and configure the desired program.
            var program = MDKFactory.CreateProgram<Program>();
            MDKFactory.Run(program);
        }
    }
}