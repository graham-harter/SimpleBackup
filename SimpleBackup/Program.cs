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

                var destinationFolderPath = ComposeDestinationFolderPath(definition, destinationFolderName);

                logger.Info("Backup destination folder is: \"{0}\"", destinationFolderPath);

                // Exit if the destination folder already exists.
                CheckDestinationFolderDoesNotExist(destinationFolderPath);

                CreateDestinationFolder(destinationFolderPath);

                logger.Info("The following files and folders failed to copy:");

                // Execute the backup
                var executor = new BackupExecutor(destinationFolderPath, definition);
                executor.ExecuteBackup();

                LogNumberOfFailures(executor);
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
                    "Usage: SimpleBackup {dest-folder-name}",
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

        private static string ComposeDestinationFolderPath(BackupDefinition definition, string destinationFolderName)
        {
            // The destination folder path is constructed from the destination root folder
            // in the XML definition, plus the destination folder name specifed in the cmd-line.
            var result = Path.Combine(
                definition.DestinationRootPath,
                destinationFolderName
                );
            return result;
        }

        private static void CheckDestinationFolderDoesNotExist(string destinationFolderPath)
        {
            if (Directory.Exists(destinationFolderPath))
            {
                ProgramControlHelper.ExitApplicationWithError(
                    string.Format("Error: Folder \"{0}\" already exists. Please specify a folder which does not exist.", destinationFolderPath),
                    (int)ApplicationExitCode.DestinationFolderAlreadyExists);
            }
        }

        private static void CreateDestinationFolder(string destinationFolderPath)
        {
            try
            {
                Directory.CreateDirectory(destinationFolderPath);
            }
            catch (Exception ex)
            {
                ProgramControlHelper.ExitApplicationWithError(
                    string.Format("Error creating destination folder \"{0}\": {1}", destinationFolderPath, ex.Message),
                    (int)ApplicationExitCode.ErrorCreatingDestinationFolder);
            }
        }

        private static void LogNumberOfFailures(BackupExecutor executor)
        {
            var failures = executor.GetExecutionFailures();

            if (failures.Count == 0)
            {
                logger.Info("There were no execution failures.");
            }
            else
            {
                logger.Info("The following execution failures occurred:");
                var failureMessageProvider = new BackupExecutionFailureMessageProvider();
                foreach (var failure in failures)
                {
                    string format = failureMessageProvider.GetFailureTypeFormatString(failure.Type);
                    logger.Info(string.Concat("  ", string.Format(format, failure.NumberOfFailures)));
                }
            }
        }
    }
}
