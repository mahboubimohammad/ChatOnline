using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SharedLibrary.Helpers;

public static class StringHelpers
{
    /// <summary>
    /// حذف فاصله‌ها و تبدیل اعداد فارسی و عربی به معادل انگلیسی
    /// </summary>
    public static string NormalizeText(string? input)
    {
        // اگر ورودی خالی یا فقط شامل فاصله باشد، رشته خالی برگردان
        if (string.IsNullOrWhiteSpace(input)) return "";

        // دیکشنری نگاشت اعداد فارسی و عربی به انگلیسی
        var map = new Dictionary<char, char>
        {
            ['۰'] = '0',
            ['۱'] = '1',
            ['۲'] = '2',
            ['۳'] = '3',
            ['۴'] = '4',
            ['٤'] = '4', // عربی
            ['۵'] = '5',
            ['٥'] = '5', // عربی
            ['۶'] = '6',
            ['٦'] = '6', // عربی
            ['۷'] = '7',
            ['۸'] = '8',
            ['۹'] = '9',
        };

        var result = new char[input.Length]; // آرایه‌ای برای ذخیره رشته خروجی
        int index = 0;

        foreach (var c in input)
        {
            if (char.IsWhiteSpace(c)) continue; // حذف فاصله‌ها

            // اگر کاراکتر در دیکشنری بود، جایگزین کن؛ در غیر این صورت خود کاراکتر را استفاده کن
            result[index++] = map.TryGetValue(c, out var english) ? english : c;
        }

        // بازگرداندن رشته جدید بدون فاصله و با اعداد انگلیسی
        return new string(result, 0, index);
    }

    /// <summary>
    /// حذف فاصله‌ها و تبدیل اعداد انگلیسی به معادل فارسی
    /// </summary>
    public static string UnNormalizeText(string? input)
    {
        // اگر ورودی خالی یا فقط شامل فاصله باشد، رشته خالی برگردان
        if (string.IsNullOrWhiteSpace(input)) return "";

        // دیکشنری نگاشت اعداد فارسی و عربی به انگلیسی
        var map = new Dictionary<char, char>
        {
            ['0'] = '۰',
            ['1'] = '۱',
            ['2'] = '۲',
            ['3'] = '۳',
            ['4'] = '۴',
            ['5'] = '۵',
            ['6'] = '۶',
            ['7'] = '۷',
            ['8'] = '۸',
            ['9'] = '۹',
        };

        var result = new char[input.Length]; // آرایه‌ای برای ذخیره رشته خروجی
        int index = 0;

        foreach (var c in input)
        {
            if (char.IsWhiteSpace(c)) continue; // حذف فاصله‌ها

            // اگر کاراکتر در دیکشنری بود، جایگزین کن؛ در غیر این صورت خود کاراکتر را استفاده کن
            result[index++] = map.TryGetValue(c, out var persian) ? persian : c;
        }

        // بازگرداندن رشته جدید بدون فاصله و با اعداد انگلیسی
        return new string(result, 0, index);
    }

    /// <summary>
    /// نرمال‌سازی شماره موبایل به فرمت صفر ابتدایی (مثل 0912...)
    /// </summary>
    public static string NormalizeMobile(string mobile)
    {
        if (mobile.StartsWith("+98")) return "0" + mobile[3..];   // +98xxxx => 0xxxx
        if (mobile.StartsWith("0098")) return "0" + mobile[4..];  // 0098xxxx => 0xxxx
        if (mobile.StartsWith("98")) return "0" + mobile[2..];    // 98xxxx => 0xxxx
        return mobile; // اگر هیچکدام نبود، خود شماره را برگردان
    }

    /// <summary>
    /// تلاش برای استخراج شماره و مبلغ از یک رشته با فرمت number*amount
    /// </summary>
    public static bool TryExtract(string text, out string number, out decimal amount)
    {
        number = null!;
        amount = 0;

        if (string.IsNullOrWhiteSpace(text)) return false;

        var parts = text.Split('*');
        if (parts.Length != 2) return false; // رشته باید دقیقاً شامل یک * باشد

        number = parts[0];
        return decimal.TryParse(parts[1], out amount); // تلاش برای تبدیل بخش دوم به عدد اعشاری
    }

    /// <summary>
    /// تولید کد عددی تصادفی با طول مشخص
    /// </summary>
    public static string GenerateRandomCode(int length)
    {
        var rnd = new Random();
        // تولید عدد بین 10^(n-1) تا (10^n)-1 برای اطمینان از داشتن n رقم
        return rnd.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString();
    }

    public static bool IsValidIban(string iban)
    {
        if (string.IsNullOrEmpty(iban))
            return false;

        string ibanPattern = @"^IR\d{24}$";

        Regex regex = new(ibanPattern);
        return regex.IsMatch(iban);
    }

    public static bool IsValidMobile(this string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        phoneNumber = phoneNumber.Trim();

        // Define a regex pattern for valid mobile numbers.
        // Example: Iranian mobile numbers starting with +98 or 09 and 10 digits.
        var regex = new Regex(@"^(\+98|0)?9\d{9}$");

        return regex.IsMatch(phoneNumber);
    }

    public static bool IsValidWalletNumber(this string walletNumber)
    {
        if (string.IsNullOrWhiteSpace(walletNumber))
            return false;

        walletNumber = walletNumber.Trim();

        // باید دقیقا 9 رقم باشد
        if (!Regex.IsMatch(walletNumber, @"^\d{9}$"))
            return false;

        // جدا کردن prefix و accountId
        int prefix = int.Parse(walletNumber.Substring(0, 1));
        string accountIdPart = walletNumber.Substring(1);

        // تبدیل accountId به عدد برای محاسبه prefix
        if (!int.TryParse(accountIdPart, out int accountId))
            return false;

        int expectedPrefix = SecurityHelpers.CalculatePrefix(accountId);

        return prefix == expectedPrefix;
    }

    public static int ExtractAccountIdFromWalletNumber(string walletNumber)
    {
        if (string.IsNullOrWhiteSpace(walletNumber) || walletNumber.Length != 9)
            throw new ArgumentException("شماره کیف پول نامعتبر است.");

        string accountIdPart = walletNumber.Substring(1); // حذف prefix
        if (!int.TryParse(accountIdPart, out int accountId))
            throw new ArgumentException("شماره حساب نامعتبر است.");

        return accountId;
    }

    public static string GenerateNumericCode(int len)
    {
        var max = (int)Math.Pow(10, len);
        var num = RandomNumberGenerator.GetInt32(0, max);
        return num.ToString().PadLeft(len, '0');
    }

    public static string GenerateRandom(int length, string type)
    {
        string chars = type switch
        {
            "CouponCode" => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            "Number" => "0123456789",
            "Token" => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789=",
            "RefferalCode" => "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            _ => throw new ArgumentException("Invalid type")
        };
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[Random.Shared.Next(s.Length)])]);
    }
}
