using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;


namespace Onyx_POS
{
    public static class ExtensionMethod
    {
        public static dynamic StringToJsonObject<T>(this object jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString.ToString());
        }
        public static string JsonObjectToString(this object jsonObject)
        {
            return jsonObject is not null ? JsonSerializer.Serialize(jsonObject) : string.Empty;
        }
        public static bool Between(this int num, int lower, int upper, bool inclusive = true)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }
        public static bool Between(this double num, int lower, int upper, bool inclusive = true)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }
        public static bool Between(this long num, int lower, int upper, bool inclusive = true)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }
        public static bool BetweenDate(this DateTime date, DateTime start, DateTime end)
        {
            return start <= date && date <= end;
        }
        public static bool IsBetweenTime(this DateTime date, TimeSpan start, TimeSpan end)
        {
            var time = date.TimeOfDay;
            return start <= time && time <= end;
        }
        public static int NagativeToZero(this int value)
        {
            return value > 0 ? value : 0;
        }
        public static double NagativeToZero(this double value)
        {
            return value > 0 ? value : 0;
        }
        public static long NagativeToZero(this long value)
        {
            return value > 0 ? value : 0;
        }
        public static Guid ToGuid(this string stringValue)
        {
            return stringValue != null ? new Guid(stringValue) : Guid.Empty;
        }
        public static string ToCurrency(this int amount)
        {
            int multiplier = amount > 0 ? 1 : -1;
            string prefix = amount < 0 ? "-" : string.Empty;
            return $"{prefix}{string.Format("{0:C0}", amount * multiplier)}";
        }
        public static string FormatNagativeAmount(this int amount)
        {
            string sign = amount > 0 ? "" : "-";
            string result = string.Format("{0:C0}", amount);
            if (amount < 0)
                result = $"{sign}{string.Format("{0:C0}", Math.Abs(amount))}";
            return result;
        }
        public static string BoolToYesNo(this bool value)
        {
            if (value)
                return "Yes";
            else
                return "No";
        }
        public static string Encrypt(this string password)
        {
            string encryptedstring;
            byte[] data;
            data = Encoding.ASCII.GetBytes(password);
            encryptedstring = Convert.ToBase64String(data);
            return encryptedstring;
        }
        public static string Decrypt(this string hash)
        {
            byte[] data;
            string decryptedstring;
            data = Convert.FromBase64String(hash);
            decryptedstring = Encoding.ASCII.GetString(data);
            return decryptedstring;
        }
        public static string ConvertToSlug(this string text)
        {
            string lowercaseText = text.ToLower();
            string withoutAmpersand = lowercaseText.Replace("&", "");
            string slug = withoutAmpersand.Replace(" ", "-");
            return slug;
        }
        public static bool FileExist(this string filename, string foldername, string CoCd)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/uploads/{CoCd}/{foldername}/{filename}");
            return File.Exists(path);
        }
        public static bool IsNotEmptyDate(this DateTime date)
        {
            if (date != DateTime.MinValue && date.Year != 1900 && date.Year != 0001)
                return true;
            else
                return false;
        }
        public static string GetDateRange(DateTime? startDate, DateTime? endDate)
        {
            var format = GetDateFormat();
            if (startDate == null || endDate == null)
                return string.Empty;
            return $"{Convert.ToDateTime(startDate).ToString(format)} - {Convert.ToDateTime(endDate).ToString(format)}";
        }
        public static string GetDateFormat() => CommonSetting.DisplayDateFormat;
        public static int GetDaysBetweenDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
                return 0;
            return (Convert.ToDateTime(endDate) - Convert.ToDateTime(startDate)).Days + 1;
        }
        public static string GetRelativeTime(DateTime past, DateTime present)
        {
            TimeSpan timeDifference = present - past;
            if (timeDifference.TotalSeconds < 60)
                return "Just now";
            else if (timeDifference.TotalMinutes < 60)
                return $"{(int)timeDifference.TotalMinutes} minutes ago";
            else if (timeDifference.TotalHours < 24)
                return $"{(int)timeDifference.TotalHours} hours ago";
            else if (timeDifference.TotalDays < 7)
                return $"{(int)timeDifference.TotalDays} day(s) ago";
            else if (timeDifference.TotalDays < 30)
            {
                int weeks = (int)(timeDifference.TotalDays / 7);
                return $"{weeks} week(s) ago";
            }
            else if (timeDifference.TotalDays < 365)
            {
                int months = (int)(timeDifference.TotalDays / 30);
                return $"{months} month(s) ago";
            }
            else
            {
                int years = (int)(timeDifference.TotalDays / 365);
                return $"{years} year(s) ago";
            }
        }
        public static string NumberToWords(decimal number)
        {
            if (number == 0)
                return "Zero";

            string[] units = ["", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"];
            string[] teens = ["Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"];
            string[] tens = ["", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"];

            string words = "";

            if ((int)(number / 1000000000) > 0)
            {
                words += NumberToWords((int)(number / 1000000000)) + " Billion ";
                number %= 1000000000;
            }

            if ((int)(number / 1000000) > 0)
            {
                words += NumberToWords((int)(number / 1000000)) + " Million ";
                number %= 1000000;
            }

            if ((int)(number / 1000) > 0)
            {
                words += NumberToWords((int)(number / 1000)) + " Thousand ";
                number %= 1000;
            }

            if ((int)(number / 100) > 0)
            {
                words += NumberToWords((int)(number / 100)) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                if (number < 10)
                    words += units[(int)number] + " ";
                else if (number < 20)
                    words += teens[(int)number - 10] + " ";
                else
                {
                    words += tens[(int)number / 10] + " ";
                    if ((int)number % 10 > 0)
                        words += units[(int)number % 10] + " ";
                }
            }

            return words.Trim();
        }

        public static string FormatDate(this DateTime date)
        {
            var dateFormat = CommonSetting.DisplayDateFormat;
            return date.IsNotEmptyDate() ? date.ToString(dateFormat) : string.Empty;
        }
        public static string GetDecimalFormat(int decimalPlaces)
        {
            return "0." + new string('0', decimalPlaces);
        }
        public static string GetOrdinal(this int number)
        {
            if (number % 100 >= 11 && number % 100 <= 13)
                return "th";
            return (number % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };
        }
        public static string ConvertDaysToYearsMonthsDays(int totalDays)
        {
            DateTime startDate = new(1, 1, 1);
            DateTime endDate = startDate.AddDays(totalDays);

            int years = endDate.Year - startDate.Year;
            int months = endDate.Month - startDate.Month;
            int days = endDate.Day - startDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(endDate.AddMonths(-1).Year, endDate.AddMonths(-1).Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }
            string result = "";
            if (years > 0)
            {
                result += $"{years} year{(years > 1 ? "s" : "")}";
            }

            if (months > 0)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }
                result += $"{months} month{(months > 1 ? "s" : "")}";
            }

            if (days > 0)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }
                result += $"{days} day{(days > 1 ? "s" : "")}";
            }
            return result;
        }
        public static Dictionary<string, decimal> GetDynamicListTotal(this List<dynamic> list)
        {
            var totals = new Dictionary<string, decimal>();
            foreach (var row in list)
            {
                var rowDict = (IDictionary<string, object>)row;
                foreach (var key in rowDict.Keys)
                {
                    if (decimal.TryParse(rowDict[key]?.ToString(), out decimal value))
                    {
                        if (totals.ContainsKey(key))
                        {
                            totals[key] += value;
                        }
                        else
                        {
                            totals[key] = value;
                        }
                    }
                }
            }
            return totals;
        }
        public static bool IsImage(this string filename)
        {
            string[] imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".ico"];
            var extension = Path.GetExtension(filename);
            return Array.Exists(imageExtensions, ext => ext == extension);
        }
        public static bool IsVideo(this string filename)
        {
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv", ".webm" };
            var extension = Path.GetExtension(filename.ToLower());
            return Array.Exists(videoExtensions, ext => ext == extension);
        }
    }
}