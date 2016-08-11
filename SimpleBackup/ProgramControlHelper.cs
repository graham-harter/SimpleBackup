using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SimpleBackup
{
    /// <summary>
    /// Class which helps with program control flow operations, e.g., application exit.
    /// </summary>
    class ProgramControlHelper
    {
        private const string PressEnterToCloseText = "Press Enter to close this window.";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void ExitApplication(string message)
        {
            Console.WriteLine(string.Concat(message, " ", PressEnterToCloseText));
            Console.ReadLine();
        }

        public static void ExitApplicationWithError(string errorMessage, int exitReturnCode)
        {
            // Validate arguments.
            if (errorMessage == null) throw new ArgumentNullException("errorMessage");

            logger.Error(errorMessage);

            Console.Error.WriteLine(errorMessage);
            Console.Error.WriteLine(PressEnterToCloseText);
            Console.ReadLine();
            Environment.Exit(exitReturnCode);
        }

        /// <summary>
        /// Build an error message string including a stack trace.
        /// </summary>
        /// <param name="errorMessageFormatString">
        /// Format string for the error message. If you want the exception
        /// message included in the output, then insert <c>{0}</c> in the
        /// format string.
        /// </param>
        /// <param name="ex">
        /// The exception to output.
        /// </param>
        /// <returns>
        /// Error message string including a stack trace.
        /// </returns>
        public static string BuildErrorMessageWithException(
            string errorMessageFormatString,
            Exception ex)
        {
            // Validate arguments.
            if (errorMessageFormatString == null) throw new ArgumentNullException("errorMessageFormatString");
            if (ex == null) throw new ArgumentNullException("ex");

            StringBuilder sb = new StringBuilder(
                string.Format(errorMessageFormatString, ex.Message));
            sb.AppendLine();
            sb.Append(ex.StackTrace);

            return sb.ToString();
        }
    }
}
