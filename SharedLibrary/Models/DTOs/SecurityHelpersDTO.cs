namespace SharedLibrary.Models.DTOs;

public class SecurityHelpersDTO
{
    public class EncodedIdResponse
    {
        public bool IsValid { get; set; }
        public int Id { get; set; }
        public int GivenPrefix { get; set; }
        public int ExpectedPrefix { get; set; }
    }

    public class UserTokenClaims
    {
        public int Id { get; set; }
        public string? Mobile { get; set; }
        public string? NationalCode { get; set; }
    }

    public class InternalServiceCredentials
    {
        public string Name { get; init; }
        public string Secret { get; init; }
    }

    public class InternalServiceIntegratedResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class InternalTokenResponse
    {
        public string accessToken { get; set; }
    }

    public class SharedHttpClientParams
    {
        public string IdentityBaseUrl { get; set; }
        public string WalletBaseUrl { get; set; }
        public string TransferBaseUrl { get; set; }
        public string UploadBaseUrl { get; set; }
        public string ExternalBaseUrl { get; set; }
        public string LoanBaseUrl { get; set; }
        public string LoyaltyClubBaseUrl { get; set; }
        public string QRBaseUrl { get; set; }
        public string CardBaseUrl { get; set; }
        public string PurchaseBaseUrl { get; set; }
    }

}
