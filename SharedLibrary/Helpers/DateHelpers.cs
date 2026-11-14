using System.Globalization;

namespace SharedLibrary.Helpers;

// کلاس کمکی برای تبدیل تاریخ میلادی به شمسی
public static class DateHelpers
{
    // متدی اکستنشن برای تبدیل DateTime به رشته تاریخ شمسی
    public static string ToPersianDate(this DateTime? date,
        string dateFormat = "yyyy/MM/dd", string timeFormat = "HH:mm:ss", string dateTimeSeparator = " - ")
    {
        if (date == null)
        {
            return "";
        }
        DateTime input = date.GetValueOrDefault();
        // استفاده از تقویم فارسی برای استخراج اجزای تاریخ
        PersianCalendar pCal = new();
        int year = pCal.GetYear(input);          // سال شمسی
        int month = pCal.GetMonth(input);        // ماه شمسی
        int day = pCal.GetDayOfMonth(input);     // روز شمسی

        // استخراج ساعت، دقیقه و ثانیه از تاریخ ورودی
        int hour = input.Hour;
        int minute = input.Minute;
        int second = input.Second;

        // متغیرهای نهایی برای ذخیره رشته‌ی تاریخ و زمان فرمت شده
        string formattedDate = "";
        string formattedTime = "";

        // فرمت‌بندی تاریخ شمسی بر اساس الگوی ورودی
        formattedDate = dateFormat
            .Replace("yyyy", year.ToString("0000"))         // جایگزینی سال چهار رقمی
            .Replace("yy", (year % 100).ToString("00"))      // جایگزینی دو رقم آخر سال
            .Replace("MM", month.ToString("00"))             // جایگزینی ماه دو رقمی
            .Replace("M", month.ToString())                  // جایگزینی ماه یک یا دو رقمی
            .Replace("dd", day.ToString("00"))               // جایگزینی روز دو رقمی
            .Replace("d", day.ToString());                   // جایگزینی روز یک یا دو رقمی

        // اگر فرمت زمان خالی باشد، فقط تاریخ برگردانده می‌شود
        if (string.IsNullOrEmpty(timeFormat))
            return formattedDate;

        // فرمت‌بندی زمان بر اساس الگوی ورودی
        formattedTime = timeFormat
            .Replace("HH", hour.ToString("00"))              // جایگزینی ساعت دو رقمی
            .Replace("H", hour.ToString())                   // جایگزینی ساعت یک یا دو رقمی
            .Replace("mm", minute.ToString("00"))            // جایگزینی دقیقه دو رقمی
            .Replace("m", minute.ToString())                 // جایگزینی دقیقه یک یا دو رقمی
            .Replace("ss", second.ToString("00"))            // جایگزینی ثانیه دو رقمی
            .Replace("s", second.ToString());                // جایگزینی ثانیه یک یا دو رقمی

        // اگر فرمت تاریخ خالی باشد، فقط زمان برگردانده می‌شود
        if (string.IsNullOrEmpty(dateFormat))
            return formattedTime;

        // در نهایت ترکیب تاریخ و زمان با جداکننده مشخص‌شده
        return $"{formattedDate}{dateTimeSeparator}{formattedTime}";
    }

    public static DateTime? ToMiladiDateTime(this string input, string dateSeparator = "/")
    {
        if (input == null) return null;
        string[] dateValues = input.Split(dateSeparator);
        if (dateValues.Length != 3) return null;
        if (int.TryParse(dateValues[2], out int day) &&
            int.TryParse(dateValues[1], out int month) &&
            int.TryParse(dateValues[0], out int year))
        {
            PersianCalendar pCal = new();
            return pCal.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        else return null;
    }

    public static DateTime AddMonth(string shamsiDate, int dayOfMonth)
    {
        var dateArr = shamsiDate.Split("/");
        var month = Convert.ToInt32(dateArr[1]);
        var year = Convert.ToInt32(dateArr[0]);
        month += 1;
        if (month == 13)
        {
            month = 1;
            year += 1;
        }
        var installmentDate = $"{year}/{month:00}/{dayOfMonth:00}";

        var pc = new PersianCalendar();
        return new DateTime(year, month, dayOfMonth, pc);
    }
}
