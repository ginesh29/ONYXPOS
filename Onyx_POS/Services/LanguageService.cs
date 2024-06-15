using NPOI.XSSF.UserModel;

namespace Onyx_POS.Services
{
    public class LanguageService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _translations;
        public LanguageService()
        {
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/language_resource.xlsx");
            _translations = LoadTranslationsFromExcel(excelFilePath);
        }

        private Dictionary<string, Dictionary<string, string>> LoadTranslationsFromExcel(string excelFilePath)
        {
            var translations = new Dictionary<string, Dictionary<string, string>>();

            using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(fs);
                var sheet = workbook.GetSheetAt(0); // Assuming translations are in the first sheet

                int rowCount = sheet.LastRowNum + 1;
                int colCount = sheet.GetRow(0).LastCellNum;

                // Read headers (languages)
                var languages = new List<string>();
                for (int col = 1; col < colCount; col++)
                {
                    languages.Add(sheet.GetRow(0).GetCell(col).StringCellValue);
                }

                // Read translations
                for (int row = 1; row < rowCount; row++)
                {
                    var key = sheet.GetRow(row).GetCell(0).StringCellValue;
                    var translationsForThisKey = new Dictionary<string, string>();

                    for (int col = 1; col < colCount; col++)
                    {
                        var language = languages[col - 1];
                        var translation = sheet.GetRow(row).GetCell(col)?.StringCellValue;

                        translationsForThisKey[language] = translation;
                    }

                    translations[key] = translationsForThisKey;
                }
            }

            return translations;
        }

        public string GetTranslation(string key, string language = "en")
        {
            if (_translations.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(language, out var translation))
                {
                    return translation;
                }
            }

            return key; // Return key if translation not found
        }
    }
}
