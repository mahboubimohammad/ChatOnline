//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Localization;
//using Microsoft.Extensions.Options;
//using SharedLibrary.Common;
//using SharedLibrary.Helpers;
//using ChatOnlineApi.Application.Contracts;
//using ChatOnlineApi.Domain.Base;
//using ChatOnlineApi.Domain.Interfaces;
//using ChatOnlineApi.Infrastructure.Configuration;
//using ChatOnlineApi.Infrastructure.Security;
//using static SharedLibrary.Models.DTOs.SecurityHelpersDTO;
//using static ChatOnlineApi.Application.DTOs.AuthDtos;

//namespace ChatOnlineApi.Application.Services;

//public class BackofficeAuthService(
//        TokenService _tokens,
//        //IUserRepository _users,
//        IUnitOfWork _unitOfWork,
//        IOptions<LogOptions> logOpt,
//        IStringLocalizer<BackofficeAuthService> _localizer) : IBackofficeAuthService
//{
//    private readonly LogOptions _logOpt = logOpt.Value;

//    public async Task<ServiceResult<object>> LoginAsync(LoginRequest request)
//    {
//        if (string.IsNullOrEmpty(request.Username)) return ServiceResult<object>.Fail(_localizer["UsernameRequired"], 400);
//        if (string.IsNullOrEmpty(request.Password)) return ServiceResult<object>.Fail(_localizer["PasswordRequired"], 400);

//        try
//        {
//            //var user = await _users.Get(u => u.Username == request.Username && !u.IsDelete)
//            //                                    .Include(ip => ip.Position).ThenInclude(io => io.Organization).FirstOrDefaultAsync();

//            //if (user == null) return ServiceResult<object>.Fail(_localizer["InvalidCredentials"], 404);

//            //if (string.IsNullOrEmpty(user.Password)) return ServiceResult<object>.Fail(_localizer["UserPasswordNotFound"], 404);

//            //var isPasswordOk = SecurityHelpers.PasswordHasher.VerifyPassword(request.Password, user.Password);

//            //if (!user.IsActive)  return ServiceResult<object>.Fail(_localizer["UserInactive"], 403);
//            //if (!isPasswordOk) return ServiceResult<object>.Fail(_localizer["UsernameOrPasswordWrong"], 400);
//            //if (user.Position == null) return ServiceResult<object>.Fail(_localizer["PositionNotFound"], 404);
//            //if (user.Position.Organization == null) return ServiceResult<object>.Fail(_localizer["OrganizationNotFound"], 404);

//            // Generate token
//            var tokenPayload = new
//            {
//                user.Id,
//                Role = "Backoffice"
//            };

//            var (token, refreshToken) = _tokens.IssueTokens(new UserTokenClaims { Id = user.Id, Mobile = "", NationalCode = "" }, "Backoffice");

//            user.RememberMe = true;
//            user.RefreshToken = refreshToken;
//            user.LastLoginDateTime = DateTime.Now;
//            await _users.UpdateAsync(user);
//            await _unitOfWork.SaveChangesAsync();

//            LoginResponse response = new()
//            {
//                Email = user.Email ?? "",
//                FirstName = user.FirstName ?? "",
//                LastName = user.LastName ?? "",
//                Username = user.Username ?? "",
//                Mobile = user.Mobile ?? "",
//                Image = user.Image ?? "",
//                ImageThumb = user.ImageThumb ?? "",
//                PositionId = user.PositionId,
//                PositionTitle = user.Position.Title,
//                OrganizationId = user.Position.OrganizationId,
//                OrganizationTitle = user.Position.Organization.Title,
//                OrganizationIcon = user.Position.Organization.Icon,
//                Token = token,
//                RefreshToken = refreshToken
//            };

//            return ServiceResult<object>.Ok(response, _localizer["LoginSuccess"]);
//        }
//        catch (Exception ex)
//        {
//            SecurityHelpers.InsertToLog(_logOpt.InsertLog, "LoginAsync - " + ex.ToString());
//            return ServiceResult<object>.Fail(_localizer["InternalServerError"], 500, ex.Message);
//        }
//    }

//    public async Task<ServiceResult<object>> LogoutAsync(int? userId)
//    {
//        try
//        {
//            if ((userId == null) || (userId == 0))
//                return ServiceResult<object>.Fail(_localizer["InvalidToken"], 400);

//            var user = await _users.Get(u => u.Id == userId && !u.IsDelete).FirstOrDefaultAsync();

//            if (user == null)
//                return ServiceResult<object>.Fail(_localizer["UserNotFound"], 404);

//            user.RefreshToken = null;
//            await _users.UpdateAsync(user);
//            await _unitOfWork.SaveChangesAsync();

//            return ServiceResult<object>.Ok(_localizer["LogoutSuccess"]);
//        }
//        catch (Exception ex)
//        {
//            SecurityHelpers.InsertToLog(_logOpt.InsertLog, "LogoutAsync - " + ex.ToString());
//            return ServiceResult<object>.Fail(_localizer["InternalServerError"], 500, ex.Message);
//        }
//    }

//    public Task<ServiceResult<object>> CheckTokenAsync()
//    {
//        return Task.FromResult(ServiceResult<object>.Ok(_localizer["TokenValid"]));
//    }

//    public async Task<ServiceResult<object>> RefreshTokenAsync(RefreshTokenRequest request)
//    {
//        if (request == null)
//            return ServiceResult<object>.Fail(_localizer["InvalidRequestData"], 400);

//        if (string.IsNullOrEmpty(request.RefreshToken))
//            return ServiceResult<object>.Fail(_localizer["RefreshTokenRequired"], 400);

//        if (string.IsNullOrEmpty(request.Username))
//            return ServiceResult<object>.Fail(_localizer["UsernameRequired"], 400);

//        try
//        {
//            var user = await _users.Get(w => w.Username == request.Username && !w.IsDelete)
//                .Include(ip => ip.Position).ThenInclude(io => io.Organization).FirstOrDefaultAsync();

//            if (user == null) return ServiceResult<object>.Fail(_localizer["InvalidCredentials"], 404);
//            if (!user.IsActive) return ServiceResult<object>.Fail(_localizer["UserInactive"], 403);

//            if (user.RefreshToken != request.RefreshToken || !user.RememberMe)
//                return ServiceResult<object>.Fail(_localizer["InvalidRefreshToken"], 401);

//            var (token, newRefreshToken) = _tokens.IssueTokens(new UserTokenClaims { Id = user.Id, Mobile = "", NationalCode = "" }, "Backoffice");

//            user.RefreshToken = newRefreshToken;
//            await _users.UpdateAsync(user);
//            await _unitOfWork.SaveChangesAsync();

//            var response = new LoginResponse
//            {
//                Email = user.Email ?? "",
//                FirstName = user.FirstName ?? "",
//                LastName = user.LastName ?? "",
//                Username = user.Username ?? "",
//                Mobile = user.Mobile ?? "",
//                Image = user.Image ?? "",
//                ImageThumb = user.ImageThumb ?? "",
//                PositionId = user.PositionId,
//                PositionTitle = user.Position.Title,
//                OrganizationId = user.Position.OrganizationId,
//                OrganizationTitle = user.Position.Organization.Title,
//                OrganizationIcon = user.Position.Organization.Icon,
//                Token = token,
//                RefreshToken = newRefreshToken
//            };

//            return ServiceResult<object>.Ok(
//                    response
//                , _localizer["TokenRefreshed"]);
//        }
//        catch (Exception ex)
//        {
//            SecurityHelpers.InsertToLog(_logOpt.InsertLog, "RefreshTokenAsync - " + ex);
//            return ServiceResult<object>.Fail(_localizer["InternalServerError"], 500, ex.Message);
//        }
//    }
//}