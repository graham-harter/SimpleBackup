using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SimpleBackup
{
    class Program
    {
        private const string backupDefinitionFilename = "SimpleBackup.xml";

        private string destinationFolderName;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);

            ProgramControlHelper.ExitApplication("Completed.");
        }

        private void Run(string[] args)
        {
            try
            {
                logger.Info("==================================================================");

                ParseArgs(args);

                logger.Info("SimpleBackup called to create backup directory \"{0}\"", destinationFolderName);

                CheckBackupDefinitionFileExists();

                var definitionReader = new BackupDefinitionReader();
                var definition = definitionReader.Read(backupDefinitionFilename);

                // Exit if the destination folder already exists.
                CheckDestinationFolderDoesNotExist();

                CreateDestinationFolder();

                logger.Info("The following files failed to copy:");

                // Execute the backup
                var executor = new BackupExecutor(destinationFolderName, definition);
                executor.ExecuteBackup();

                logger.Info("  {0} files failed to copy.", executor.NumberOfFilesWhichFailedToCopy);
            }
            catch (Exception ex)
            {
                var message = ProgramControlHelper.BuildErrorMessageWithException(
                    "SimpleBackup: Uncaught exception: {0}", ex);
                ProgramControlHelper.ExitApplicationWithError(
                    message,
                    (int)ApplicationExitCode.UncaughtException);
            }
        }

        private void ParseArgs(string[] args)
        {
            if (args.Length == 0)
            {
                ProgramControlHelper.ExitApplicationWithError(
                    "Usage: SimpleBackup {dest-folder-path}",
                    (int)ApplicationExitCode.InvalidCmdLineArguments);
            }

            destinationFolderName = args[0];
        }

        private void CheckBackupDefinitionFileExists()
        {
            if (!File.Exists(backupDefinitionFilename))
            {
                ProgramControlHelper.ExitApplicationWithError(
                    string.Format("Error: Backup definition file \"{0}\" not found.", backupDefinitionFilename),
                    (int)ApplicationExitCode.BackupDefinitionFileNotFound);
            }
        }

        private void CheckDestinationFolderDoesNotExist()
        {
            if (Directory.Exists(destinationFolderName))
            {
                ProgramControlHelper.ExitApplicationWithError(
                    string.Format("Error: Folder \"{0}\" already exists. Please specify a folder which does not exist.", destinationFolderName),
                    (int)ApplicationExitCode.DestinationRootFolderAlreadyExists);
            }
        }

        private void CreateDestinationFolder()
        {
            try
            {
                Directory.CreateDirectory(destinationFolderName);
            }
            catch (Exception ex)
            {
                ProgramControlHelper.ExitApplicationWithError(
                    string.Format("Error creating destination folder \"{0}\": {1}", destinationFolderName, ex.Message),
                    (int)ApplicationExitCode.ErrorCreatingDestinationRootFolder);
            }
        }
    }
}
