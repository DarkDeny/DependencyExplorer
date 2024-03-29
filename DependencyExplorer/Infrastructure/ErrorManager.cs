﻿using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace DependencyExplorer.Infrastructure {
    public class ErrorManager {

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern int GetUserDefaultLocaleName(StringBuilder localeName, int bufferSize);

        private static string GetUserDefaultLocale()
        {
            const int localeNameMaxLength = 85;
            var sb = new StringBuilder(localeNameMaxLength);
            if (GetUserDefaultLocaleName(sb, localeNameMaxLength) == 0) {
                return "Unknown";
            }
            return sb.ToString();
        }

        public static void LogError(string productName, string version, string logFolder, string situation, string errorMessage) {
            var timeHappened = DateTime.Now;
            var date = String.Format("{0}{1}{2}", timeHappened.Year.ToString("D4"), timeHappened.Month.ToString("D2"), timeHappened.Day.ToString("D2"));
            var time = String.Format("{0}{1}{2}", timeHappened.Hour.ToString("D2"), timeHappened.Minute.ToString("D2"), timeHappened.Second.ToString("D2"));
            var timeStamp = String.Format("{0}-{1}", date, time);

            var environment = String.Format(
                "OS: {0}, Locale: {1}", Environment.OSVersion, GetUserDefaultLocale());

            var errorReport = String.Format(
                "Product: {0}\n  Timestamp: {1}\n  Version: {2}\n  {3}\n{4}",
                productName,
                timeStamp,
                version,
                environment,
                errorMessage);

            var errorFilename = String.Format("{0}-{1}.txt", situation, timeStamp);

            try {
                Directory.SetCurrentDirectory(logFolder);

                var errorFile = File.CreateText(errorFilename);
                errorFile.WriteLine(errorReport);
                errorFile.Close();
            } catch {
                MessageBox.Show(
                    "Something very strange happened - We were not able to create an error report.\n\nPlease contact us if problem persists!",
                    "Unrecoverable error");
            }
        }

        public static void LogException(string productName, string version, string logFolder, string situation, Exception ex) {
            var errorDetails = EnlistAllException(ex);
            LogError(productName, version, logFolder, situation, errorDetails);
        }

        private const string Spacing = "  ";

        private static string EnlistAllException(Exception exception) {
            var sb = new StringBuilder();
            var currentException = exception;
            var currentSpacings = String.Empty;
            while (null != currentException) {
                currentSpacings += Spacing;
                sb.AppendFormat(
                    "{0}Unhandled exception: ({1}) {2}\n{0}StackTrace:\n{3}\n\n",
                    currentSpacings,
                    currentException.GetType(),
                    currentException.Message,
                    currentException.StackTrace);
                currentException = currentException.InnerException;
            }

            return sb.ToString();
        }
    }
}
