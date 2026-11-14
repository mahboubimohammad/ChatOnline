using Newtonsoft.Json;
using Serilog;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static SharedLibrary.Models.DTOs.SecurityHelpersDTO;

namespace SharedLibrary.Helpers;

public static class SecurityHelpers
{
    private static readonly Random _rnd = new();

    // متدی برای رمزنگاری یک رشته با استفاده از AES و کلید متنی
    public static string EncryptString(string plainText, string pass)
    {
        var key = GenerateKey(pass); // تولید کلید از رمز عبور
        byte[] iv = new byte[16];   // بردار اولیه خالی (صفر)
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key); // تنظیم کلید رمزنگاری
            aes.IV = iv;                           // تنظیم بردار اولیه

            // ایجاد شیء رمزنگار
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // نوشتن داده‌های رمزنگاری‌شده در حافظه
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new((Stream)cryptoStream))
            {
                streamWriter.Write(plainText); // نوشتن متن ساده در جریان رمزنگاری‌شده
            }

            array = memoryStream.ToArray(); // تبدیل به آرایه بایت رمزنگاری‌شده
        }

        return Convert.ToBase64String(array); // بازگرداندن رشته رمزنگاری‌شده به صورت Base64
    }

    // متدی برای رمزگشایی رشته‌ای که با متد بالا رمزنگاری شده
    public static string DecryptString(string cipherText, string pass)
    {
        var key = GenerateKey(pass); // تولید کلید از رمز عبور
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText); // تبدیل متن رمزنگاری‌شده از Base64 به بایت

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream memoryStream = new(buffer);
        using CryptoStream cryptoStream = new((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new((Stream)cryptoStream);
        return streamReader.ReadToEnd(); // خواندن و بازگرداندن متن اصلی
    }

    // متدی برای تولید کلید رمزنگاری از یک رمز عبور
    private static string GenerateKey(string pass)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(pass)); // رمز عبور به Base64
        var str = 0;

        // افزایش طول کلید تا حداقل 32 کاراکتر
        while (key.Length < 32)
        {
            key += str;
            str++;
        }

        return key;
    }

    public sealed class EncodedIdResponse
    {
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public int GivenPrefix { get; set; }
        public int ExpectedPrefix { get; set; }
    }

    // رمزگشایی و اعتبارسنجی شناسه‌های رمزگذاری‌شده عددی
    public static EncodedIdResponse DecodeAndValidate(int encodedId)
    {
        string encodedStr = encodedId.ToString().PadLeft(9, '0'); // رشته ۹ رقمی

        int givenPrefix = int.Parse(encodedStr[..1]); // پیشوند
        int id = int.Parse(encodedStr[1..]);          // شناسه اصلی

        int expectedPrefix = CalculatePrefix(id);

        return new EncodedIdResponse
        {
            Id = id,
            IsValid = givenPrefix == expectedPrefix,
            GivenPrefix = givenPrefix,
            ExpectedPrefix = expectedPrefix
        };
    }

    // متدهای کمکی برای تولید رمز و تقسیم
    public static (string FirstPrivateKey, string SecondPrivateKey) GenerateAndSplitPassword()
    {
        // تولید رمز 30 کاراکتری
        string generatedPassword = GenerateRandomPassword(30);

        // تقسیم به دو بخش: 12 کاراکتر اول و 18 کاراکتر بعدی
        string firstPart = generatedPassword.Substring(0, 12);
        string secondPart = generatedPassword.Substring(12, 18);

        Console.WriteLine($"Generated Password: {generatedPassword}");
        Console.WriteLine($"First Part (12 chars): {firstPart}");
        Console.WriteLine($"Second Part (18 chars): {secondPart}");
        Console.WriteLine($"First Length: {firstPart.Length}");
        Console.WriteLine($"Second Length: {secondPart.Length}");

        return (firstPart, secondPart);
    }
    private static string GenerateRandomPassword(int length)
    {
        // فقط حروف کوچک برای سادگی (می‌توانید کاراکترهای مورد نظر خود را اضافه کنید)
        const string validChars = "abcdefghijklmnopqrstuvwxyz0123456789/";
        Random random = new();
        var chars = new char[length];

        for (int i = 0; i < length; i++)
        {
            chars[i] = validChars[random.Next(validChars.Length)];
        }

        return new string(chars);
    }

    // محاسبه پیشوند (۱ تا ۹)
    public static int CalculatePrefix(int id)
    {
        int[] weights = { 2, 3, 4, 5, 6, 7 };
        int sum = 0;
        string idStr = id.ToString();
        int weightIndex = 0;

        for (int i = idStr.Length - 1; i >= 0; i--)
        {
            int digit = idStr[i] - '0';
            sum += digit * weights[weightIndex];
            weightIndex = (weightIndex + 1) % weights.Length;
        }

        int prefix = (sum % 9) + 1;
        return prefix;
    }

    // ساخت شناسه عمومی (۹ رقمی)
    public static string BuildPublicId(int id)
    {
        int prefix = CalculatePrefix(id);
        string idPart = id.ToString().PadLeft(8, '0');
        return $"{prefix}{idPart}"; // ۹ رقمی
    }
    // متدی برای ماسک‌کردن شماره موبایل به‌صورت سریع
    public static string MaskMobileFast(string mobile)
    {
        // جابجایی بخش انتهایی و ابتدایی با چهار ستاره در وسط
        return string.Concat(mobile.AsSpan(7), "****", mobile.AsSpan(0, 4));
    }

    public static void InsertToLog(bool insertLog, string logText, string type = "Error", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string? caller = null)
    {
        try
        {
            if (insertLog)
                switch (type)
                {
                    case "Error":
                        Log.Error(logText, lineNumber + " at line " + lineNumber + " (" + caller + ")" + "\n");
                        break;
                    case "Info":
                        Log.Information(logText, lineNumber + " at line " + lineNumber + " (" + caller + ")" + "\n");
                        break;
                    default:
                        break;
                }
        }
        catch (Exception)
        {

        }
    }

    public static void InsertToLog(object insertLog, string v)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// متدی برای بررسی اعتبار کد ملی ایرانی
    /// </summary>
    public static bool IsValidNationalCode(this string nationalCode)
    {
        // بررسی خالی بودن
        if (String.IsNullOrEmpty(nationalCode))
            return false;

        // بررسی طول 10 رقمی
        if (nationalCode.Length != 10)
            return false;

        // بررسی فقط عدد بودن
        var regex = new Regex(@"\d{10}");
        if (!regex.IsMatch(nationalCode))
            return false;

        // بررسی یکنواخت نبودن تمام ارقام
        var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
        if (allDigitEqual.Contains(nationalCode)) return false;

        // محاسبه رقم کنترلی با استفاده از الگوریتم کد ملی
        var chArray = nationalCode.ToCharArray();
        var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
        var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
        var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
        var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
        var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
        var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
        var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
        var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
        var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
        var a = Convert.ToInt32(chArray[9].ToString());

        var b = num0 + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
        var c = b % 11;

        // بررسی صحت رقم کنترل
        return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
    }

    /// <summary>
    /// متدی برای بررسی اعتبار شناسه اکانت
    /// </summary>
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

    /// <summary>
    /// متدی برای بررسی اعتبار موبایل 
    /// </summary>
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

    /// <summary>
    /// متدی برای بررسی اعتبار ایمیل
    /// </summary>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var address = new System.Net.Mail.MailAddress(email.Trim());
            return address.Address == email.Trim();
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool IsValidIban(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return false;

        iban = iban.Replace(" ", "").ToUpper();

        // طول باید 26 کاراکتر باشه
        if (iban.Length != 26)
            return false;

        // باید با IR شروع بشه
        if (!iban.StartsWith("IR"))
            return false;

        // کل شبا باید فقط حرف یا عدد باشه
        if (!Regex.IsMatch(iban, "^[A-Z0-9]+$"))
            return false;

        return true;
    }

    public static bool IbanAndBankMatched(string iban, string ibanDigit)
    {
        iban = iban.Replace(" ", "").ToUpper();
        var ibanWithoutPrefix = iban.Substring(4);
        var bankCodeFromIban = ibanWithoutPrefix[..3];
        return bankCodeFromIban == ibanDigit;
    }

    public static string GetBankDigit(string iban)
    {
        iban = iban.Replace(" ", "").ToUpper();
        var ibanWithoutPrefix = iban.Substring(4);
        var bankCodeFromIban = ibanWithoutPrefix[..3];
        return bankCodeFromIban;
    }

    #region AtmWithdrawal
    // ===================== ساخت توکن 30 رقمی =====================
    /// <summary>
    /// ساخت توکن 30 رقمی با ساختار:
    /// [DigitA(1)] [RandX(digitA)] [Req6(6=DB5+'0')] [WalletId(9)] [AmtTrim(2)] ["8"(1)] [RandY(11 - digitA)]
    /// مثال DigitA=7: 7 + 7رقم + 18رقم ثابت + 4رقم = 30
    /// </summary>
    public static string Build(
        int userId,
        decimal amountRial,
        string requestCodeFromDb,
        int? digitAOpt = null,
        string? walletIdOverride9 = null
    )
    {
        // 1) DigitA: اگر مشخص نشد، 1..9 تصادفی
        int digitA = digitAOpt.HasValue
            ? Math.Max(1, Math.Min(9, digitAOpt.Value))
            : RandomNumberGenerator.GetInt32(1, 10);
        string dA = digitA.ToString();

        // 2) RandX: به طول digitA
        string randX = NextDigitsExact(digitA);

        // 3) RequestCode(6) = (5 رقم از DB) + "0"
        string req6 = BuildRequestCode(requestCodeFromDb);

        // 4) WalletId(9): یا Override یا از userId
        string walletId = !string.IsNullOrWhiteSpace(walletIdOverride9)
            ? walletIdOverride9
            : BuildPublicId(userId);

        // 5) AmountTrimmed(2): دو رقم از مبلغ پس از حذف صفرهای انتهایی
        string amt2 = BuildAmountTrimmed2(amountRial);

        // 6) DigitB: طبق خواستهٔ شما ثابت "8"
        const string digitB = "8";

        // 7) RandY: باقیمانده تا 30
        // طول میانی ثابت = 18 (Req6(6)+WalletId(9)+Amt2(2)+DigitB(1))
        int midLen = 18;
        int usedLeft = 1 + digitA;      // DigitA + RandX(digitA)
        int used = usedLeft + midLen;   // تا قبل از RandY
        int randYLen = 30 - used;
        if (randYLen < 0) throw new InvalidOperationException("DigitA خیلی بزرگ است؛ طول 30 رعایت نمی‌شود.");
        string randY = NextDigitsExact(randYLen);

        return dA + randX + req6 + walletId + amt2 + digitB + randY;
    }

    // ===================== ابزارهای کمکی =====================

    /// <summary>
    /// تولید دقیقاً length رقم تصادفی (ایمن).
    /// </summary>
    private static string NextDigitsExact(int length)
    {
        if (length <= 0) return string.Empty;
        StringBuilder sb = new();
        while (sb.Length < length)
            sb.Append(RandomNumberGenerator.GetInt32(0, 10000).ToString("D4"));
        return sb.ToString(0, length);
    }

    /// <summary>
    /// ساخت RequestCode شش‌رقمی از DB (5 رقم) + "0".
    /// مثال: "46929" → "469290"
    /// </summary>
    private static string BuildRequestCode(string requestCodeFromDb)
    {
        string digits = new((requestCodeFromDb ?? string.Empty).Where(char.IsDigit).ToArray());
        digits = digits.Length > 5 ? digits[^5..] : digits.PadLeft(5, '0');
        return digits + "0"; // 6 رقم
    }

    /// <summary>
    /// AmountTrimmed(2): مبلغ پس از حذف صفرهای انتهایی؛
    /// اگر کم‌تر از 2 رقم شد، PadLeft؛ اگر بیشتر بود، دو رقم آخر.
    /// 9,900,000 → "99"
    /// </summary>
    private static string BuildAmountTrimmed2(decimal amountRial)
    {
        long rial = (long)Math.Truncate(Math.Abs(amountRial));
        while (rial > 0 && rial % 10 == 0) rial /= 10;
        string s = rial.ToString();
        if (s.Length >= 2) return s[^2..];
        return s.PadLeft(2, '0');
    }

    /// <summary>
    /// تبدیل WalletId قدیمی (prefix(1)+userId(8) مثل 200001007) به جدید (010072000).
    /// </summary>
    public static string ConvertOldToNewWalletId(string oldWalletId)
    {
        if (string.IsNullOrWhiteSpace(oldWalletId) || oldWalletId.Length != 9 || !oldWalletId.All(char.IsDigit))
            throw new ArgumentException("WalletId قدیمی باید 9 رقمی و فقط عدد باشد.");

        char prefix = oldWalletId[0];                 // مثلا '2'
        string userStr = oldWalletId.Substring(1, 8); // "00001007"
        string part1 = userStr.Substring(3, 2); // "01"
        string part2 = userStr.Substring(5, 3); // "007"
        return part1 + part2 + prefix + "000";  // "010072000"
    }

    public static UserTokenClaims? GetUserFromToken(string? encryptedToken)
    {
        if (string.IsNullOrEmpty(encryptedToken))
            return null;

        try
        {
            var decryptedJson = SecurityHelpers.DecryptString(encryptedToken, "B3h!n3H88P@rd@7");
            var userData = JsonConvert.DeserializeObject<UserTokenClaims>(decryptedJson);

            if (userData == null || userData.Id <= 0)
                return null;

            return userData;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    public sealed class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }

    #region PasswordHasher
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using var derive = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256);
            var salt = derive.Salt;
            var key = derive.GetBytes(32);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
        }

        public static bool VerifyPassword(string password, string hash)
        {
            var parts = hash.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var key = Convert.FromBase64String(parts[1]);

            using var derive = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var newKey = derive.GetBytes(32);

            return newKey.SequenceEqual(key);
        }
    }
    #endregion
}

