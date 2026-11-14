namespace ChatOnlineApi.Infrastructure.Configuration;

public class JwtOptions
{
    public string JwtKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int ExpiresInMinutes { get; set; }
}

public class SeriLogOptions
{
    public bool InsertLog { get; set; }
    public bool InsertInfoLog { get; set; }
    public string LogFilePath { get; set; }
    public int LogFileSizeMB { get; set; }
    public string LogFileName { get; set; }
}

public class SwaggerOptions
{
    public string BackofficeSwaggerAddress { get; set; } = "/swagger/backoffice/swagger.json";
    public string SwaggerCssAddress { get; set; } = "/swagger-ui/css/custom.css";
}

public class LogOptions
{
    public bool InsertLog { get; set; } = false;
}
public sealed class YaghutOptions
{
    /// <summary>"Soap" (default) or "Rest" — selects which IYaghutService implementation is used.</summary>
    public string Protocol { get; set; } = "Soap";
    public string RestBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// REST path per operation (key = ExternalServiceNames.Yaghut* constant), relative to RestBaseUrl.
    /// Paths are taken from the official "Rest URL" entries of the Yaghut service documentation
    /// (yaghut.day24.ir/yaghut/dey/doc); override in appsettings if the deployment differs.
    /// </summary>
    public Dictionary<string, string> RestEndpoints { get; set; } = new()
    {
        ["YaghutNormalTransfer"] = "deposit/transfer",
        ["YaghutStatement"] = "deposit/statement",
        ["YaghutDepositBalance"] = "deposit/getDepositBalance",
        ["YaghutDepositNumberToIban"] = "depositToIban",
        ["YaghutIssueSayadCheque"] = "sayadCheque/issue",
        ["YaghutKartablIssueSayadCheque"] = "sayadCheque/kartablIssue",
        ["YaghutTransferSayadCheque"] = "sayadCheque/transfer",
        ["YaghutRejectSayadChequeTransfer"] = "sayadCheque/rejectTransfer",
        ["YaghutAcceptOrRejectSayadCheque"] = "sayadCheque/acceptOrReject",
        ["YaghutInquirySayadChequeByIssuer"] = "sayadCheque/inquiryIssuer",
        ["YaghutInquirySayadChequeByHolder"] = "sayadCheque/inquiryHolder",
        ["YaghutInquirySayadChequeTransfer"] = "sayadCheque/inquiryTransfer",
        ["YaghutInquirySayadChequeReceiversName"] = "sayadCheque/inquiryChequeReceiversName",
        ["YaghutGetDepositBySayadId"] = "sayadCheque/getDepositBySayadId"
    };
    public string ServiceUrl { get; set; } = "https://yaghut.day24.ir/yaghut/soap/soap_dey";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string CertificatePath { get; set; } = "Resources/Certificates/yaghout-10.5.34.29-client.p12";
    public string CertificatePassword { get; set; } = string.Empty;
    public int TokenAliveInMinutes { get; set; } = 10;
}
