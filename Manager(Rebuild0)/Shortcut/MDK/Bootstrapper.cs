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
#pragma warning disable CS0103 // The name 'MDKUtilityFramework' does not exist in the current context
            MDKUtilityFramework.Load();
#pragma warning restore CS0103 // The name 'MDKUtilityFramework' does not exist in the current context
        }

        public static void Main()
        {
            // In order for your program to actually run, you will need to provide a mockup of all the facilities 
            // your script uses from the game, since they're not available outside of the game.

            // Create and configure the desired program.
#pragma warning disable CS0103 // The name 'MDKFactory' does not exist in the current context
            var program = MDKFactory.CreateProgram<Program>();
#pragma warning restore CS0103 // The name 'MDKFactory' does not exist in the current context
#pragma warning disable CS0103 // The name 'MDKFactory' does not exist in the current context
            MDKFactory.Run(program);
#pragma warning restore CS0103 // The name 'MDKFactory' does not exist in the current context
        }
    }
}