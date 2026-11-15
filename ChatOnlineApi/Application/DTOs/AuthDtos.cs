namespace ChatOnlineApi.Application.DTOs;

public class AuthDtos
{
    #region Merchant & Client
    public class InitialSignUpResponse
    {
        public bool IsRegistered { get; set; }
        public bool PassCodeEnabled { get; set; }
        public Guid OtpSessionId { get; set; }
        public int OtpExpiresInSec { get; set; }
    }

    public class RegisterAccountRequest
    {
        public string Mobile { get; set; } = default!;
        public string ConfirmCode { get; set; } = default!;
        public Guid OtpSessionId { get; set; }
        public string? NationalCode { get; set; }
        public string? Reagent { get; set; }
        /// <summary>Set by the Merchant-branded pay page (same opaque code it passed to InitialSignUp). When present the
        /// platform-branded welcome SMS is suppressed — this buyer came in through a Merchant's QR and must not learn
        /// which platform is behind it.</summary>
        public string? MerchantQrCode { get; set; }
        public string AppVersion { get; set; } = string.Empty;
        public string DeviceId { get; set; } = default!;
        public string DeviceType { get; set; } = default!;
        public string Browser { get; set; } = default!;
        public string? PNEndpoint { get; set; }
        public string? PNP256Dh { get; set; }
        public string? PNAuth { get; set; }
    }

    public class VerifyPassCodeRequest
    {
        public Guid ChallengeId { get; set; }
        public string PassCode { get; set; } = default!;
    }

    public class SetPassCodeRequest
    {
        public bool? EnablePassCode { get; set; }      // true=فعال، false=غیرفعال، null=بدون تغییر
        public string? PassCode { get; set; }          // کد جدید (برای setup یا change یا disable)
        public string? OldPassCode { get; set; }       // برای تغییر لازم است
    }

    public class TokensResponse
    {
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public bool? PassCodeEnabled { get; set; }
    }
    #endregion

    #region Backoffice
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ImageThumb { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string PositionTitle { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public string OrganizationTitle { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string RoleTitle { get; set; } = string.Empty;
        public string OrganizationIcon { get; set; } = string.Empty;
        public string CreationDateTime { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string Username { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenAppRequest
    {
        public string Mobile { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class ThirdPartyTokenResponse
    {
        public string TokenValue { get; set; } = string.Empty;
    }

    public class GetTokenResponse
    {
        public class Data
        {
            public string Access_Token { get; set; } = string.Empty;
            public long Expires_in { get; set; }
        }
        public bool HasError { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public Data TokenData { get; set; } = new();
    }
    #endregion
}
