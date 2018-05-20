using System.Windows;

namespace MVVMSample {

    static class Strings {

        public const string app_title_separator = " – ";

        public const string app_settings_file = "Settings.xml";
        public const string app_db_name_one = "TableOne";
        public const string app_db_name_two = "TableTwo";

        public static string app_title => GetResource(nameof(app_title));

        public static string label_error => GetResource(nameof(label_error));
        public static string label_table_one => GetResource(nameof(label_table_one));
        public static string label_table_two => GetResource(nameof(label_table_two));

        public static string label_error_database_corrupted => GetResource(nameof(label_error_database_corrupted));
        public static string label_error_file_not_found => GetResource(nameof(label_error_file_not_found));

        public static string file_filter => GetResource(nameof(file_filter));

        public static string third_party_libraries_usage => GetResource(nameof(third_party_libraries_usage));

        public static string GetResource(string id) {
            string res = Application.Current.Resources[id] as string;
            return res ?? string.Empty;
        }
    }
}
