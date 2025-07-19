namespace SurveyManagementSystemApi.Service
{
    
    public class LoginService : IAuthService
    {
        public LoginService(UserManager<UserIdentity> userManager, 
            IJwtToken jwtToken,SignInManager<UserIdentity> signInManager, ILogger<IAuthService> logger,
            IHttpContextAccessor httpContextAccessor, IEmailSender emailSender, AppDbContext context)
        {
            _jwtToken = jwtToken;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
           _httpContextAccessor = httpContextAccessor;
        _emailSender = emailSender;
            _context = context;
        }
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _context;
        private readonly SignInManager<UserIdentity> _signInManager;
        private readonly ILogger<IAuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly double _RefreshTokenExpires;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly IJwtToken _jwtToken;
       

        
        public async Task<Result<AuthResponse>> GetReFreshTokenAsnyc(ReFreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
        {
            var UserId = _jwtToken.ValidateToken(refreshTokenRequest.token);
            if (UserId is null)
                return Result<AuthResponse>.Failure<AuthResponse>(new Error("Invalid token ", StatusCodes.Status400BadRequest));
            var user = await _userManager.FindByIdAsync(UserId);         
            if (user is null)
                return Result<AuthResponse>.Failure<AuthResponse>(new Error("Invalid token ", StatusCodes.Status400BadRequest));
            var RefreshTokens = user.RefreshTokens.SingleOrDefault(x => x.IsActive  && x.Token == refreshTokenRequest.token);          
            if (RefreshTokens is null)
                return Result<AuthResponse>.Failure<AuthResponse>(new Error("Invalid RefreshTokens  ", StatusCodes.Status400BadRequest));
            RefreshTokens!.ExpiresOn = DateTime.UtcNow;
            _userManager?.UpdateAsync(user);
            LoginRequest loginRequest = new LoginRequest { Email = user.Email!, Password = user.PasswordHash! };
            var (roles, permissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            string newtoken = _jwtToken.GenerateToken(user, roles, permissions);

            var newrefreshToken = GeneratorRefreshToken();
            var refreshTokenExpires = DateTime.UtcNow.AddDays(_RefreshTokenExpires);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newrefreshToken,
                ExpiresOn = refreshTokenExpires,
            });
            await _userManager!.UpdateAsync(user);
            return Result<AuthResponse>.Success(new AuthResponse { RefreshToken = newrefreshToken, Token = newtoken, ExpiresIn = 10, FirstName = user.FirstName, LastName = user.LastName });
        }

        public async Task<Result<string>> Register(RegisterRequest registerRequest, CancellationToken cancellationToken= default)
        {

            var email = await _userManager.Users.AnyAsync(x => x.Email == registerRequest.Email);
            if (email)
                return Result<string>.Failure<string>(new   Error("Duplicated Email",StatusCodes.Status409Conflict));

            var user = new UserIdentity
            {
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
            }
            ;
            user.UserName = registerRequest.Email;
            var result = await _userManager.CreateAsync(user,registerRequest.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                _logger.LogInformation($"before encoding  { code}");
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                _logger.LogInformation($"after encoding  {code}");
                await SendConfirmationEmail(user, code);
                return Result<string>.Success("");
            }
            var error = result.Errors.First();
            return Result<string>.Failure<string>(new Error(error.Description, StatusCodes.Status400BadRequest));

        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == loginRequest.Email);
            if (user is null)
            {
                return Result<AuthResponse>.Failure<AuthResponse>(new Error("Invalid UserName Or Password ",StatusCodes.Status400BadRequest));
            }
            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return Result<AuthResponse>.Failure<AuthResponse>(new Error("Invalid UserName Or Password ", StatusCodes.Status400BadRequest));

            if( ! await _signInManager.UserManager.IsEmailConfirmedAsync(user))
                Result<AuthResponse>.Failure<AuthResponse>(new Error("Confirmed Your Email ", StatusCodes.Status409Conflict));
            //await _signInManager.PasswordSignInAsync(user,loginRequest.Password,false,false); 
            var (roles, permissions) = await  GetUserRolesAndPermissions(user,cancellationToken); 
            string token = _jwtToken.GenerateToken(user, roles, permissions);

            var refreshToken = GeneratorRefreshToken();
            var refreshTokenExpires = DateTime.UtcNow.AddDays(_RefreshTokenExpires);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpires,
            });
            await _userManager.UpdateAsync(user);
            return Result<AuthResponse>.Success(new AuthResponse {RefreshToken = refreshToken, Token = token,ExpiresIn = 10,FirstName = user.FirstName,LastName= user.LastName });
      
        }

        public async Task<Result> ConformEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if(user is null)
                return Result.Failure(new Error("Invalid UserName Or Password ", StatusCodes.Status400BadRequest));

            var code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            }catch(FormatException e)
            {
                return Result.Failure(new Error(e.Message, StatusCodes.Status400BadRequest));
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
            {
                BackgroundJob.Enqueue(() => SendConfirmationEmail(user, code ));
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default)   
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(new Error("",StatusCodes.Status400BadRequest));    

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        private async Task SendConfirmationEmail(UserIdentity user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
                templateModel: new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                }
            );

            await _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody);

        }

        
        public async Task<Result> SendResetPasswordCodeAsync(string email)
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return Result.Success();

            if (!user.EmailConfirmed)
                return Result.Failure(new Error("Invalid Code", StatusCodes.Status409Conflict));

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Reset code: {code}", code);

            await SendResetPasswordEmail(user, code);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !user.EmailConfirmed)
                return Result.Failure(new Error( "Invalid Code", StatusCodes.Status409Conflict));

            IdentityResult result;

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, StatusCodes.Status401Unauthorized));
        }

        private async Task SendResetPasswordEmail(UserIdentity user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                { "{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
                }
            );

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Change Password", emailBody));

            await Task.CompletedTask;
        }

        
        private static string GeneratorRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(UserIdentity user, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await (from r in _context.Roles
                                         join p in _context.RoleClaims
                                         on r.Id equals p.RoleId
                                         where userRoles.Contains(r.Name!)
                                         select p.ClaimValue!)
                                         .Distinct()
                                         .ToListAsync(cancellationToken);

            return (userRoles, userPermissions);
        }

    }

}