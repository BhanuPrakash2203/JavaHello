

using System;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Constants
{
    public static class Messages
    {
        public const string TITLE_ERROR = "Erreur";
        public const string TITLE_DONE = "Terminé";

        public const string MSG_EXCEPTION = "L'erreur suivante est survenue :";
        public const string TITLE_EXCEPTION = "Une erreur est survenue!";

        public const string MSG_FOLDER_DOES_NOT_EXISTS = "Le répertoire suivant est introuvable :";
        public const string MSG_NO_FILE_FOUND = "Le fichier suivant est introuvable :";

        public const string SELECT_REPORTS_FOLDER_TITLE = "Sélectionnez le répertoire contenant les fichiers de rapports (*.pdf) à comparer";
        public const string SELECT_OUTPUT_FOLDER_TITLE = "Sélectionnez le répertoire de destination des résultats";
        public const string SELECT_RESULTS_FOLDER_TITLE = "Sélectionnez le répertoire contenant les résultats de la comparaison";

        public const string MSG_COMPARISON_PROCESS_FINISHED = "Le processus de comparaison des rapports est terminé.";

        public static string GetMessageException(string message)
        {
            return $"{MSG_EXCEPTION}{Environment.NewLine}{Environment.NewLine}{message}";
        }

        public static string GetMessageFolderNotFound(string folder)
        {
            return $"{MSG_FOLDER_DOES_NOT_EXISTS}{Environment.NewLine}{folder}";
        }

        public static string GetMessageFileNotFound(string folder)
        {
            return $"{MSG_NO_FILE_FOUND}{Environment.NewLine}{folder}";
        }
    }
}
