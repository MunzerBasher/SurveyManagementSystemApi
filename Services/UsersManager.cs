namespace SurveyManagementSystemApi.Services
{
    public class UsersManager(
        UserManager<UserIdentity> userManager,
    IRoleService roleService,
    AppDbContext context
        ) : IUsersManager
    {
        private readonly UserManager<UserIdentity> _userManager = userManager;
        private readonly IRoleService _roleService = roleService;
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await (
            from u in _context.Users
               join ur in _context.UserRoles
               on u.Id equals ur.UserId
               join r in _context.Roles
               on ur.RoleId equals r.Id into roles
               where !roles.Any(x => x.Name == DefaultRoles.Member)
               select new
               {
                   u.Id,
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.IsDisabled,
                   Roles = roles.Select(x => x.Name!).ToList()
               }
                )
                .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
                .Select(u => new UserResponse
                {
                     Id =   u.Key.Id,
                     FirstName= u.Key.FirstName,
                     LastName =  u.Key.LastName,
                     Email = u.Key.Email,
                     IsDisabled = u.Key.IsDisabled,
                     Roles = u.SelectMany(x => x.Roles)
                })
               .ToListAsync(cancellationToken);

        public async Task<Result<UserResponse>> GetAsync(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result<UserResponse>.Failure<UserResponse>(new Error( "UserNotFound", StatusCodes.Status404NotFound));

            var userRoles = await _userManager.GetRolesAsync(user);

            var response = (user, userRoles).Adapt<UserResponse>();

            return Result<UserResponse>.Success(response);
        }
        public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExists)
                return Result<UserResponse>.Failure<UserResponse> (new Error(" DuplicatedEmail",StatusCodes.Status400BadRequest));

            var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result<UserResponse>.Failure<UserResponse>(new Error("InvalidRoles",StatusCodes.Status400BadRequest));

            var user = new UserIdentity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                NormalizedUserName = request.Email.ToUpper()
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, request.Roles);

                var response = new UserResponse
                {
                    Id = user.Id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    IsDisabled = user.IsDisabled,
                    Roles = request.Roles
                };

                return Result<UserResponse>.Success(response);
            }
            var error = result.Errors.First();

            return Result<UserResponse>.Failure<UserResponse>(new Error(error.Code, StatusCodes.Status400BadRequest));

        }


        public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id, cancellationToken);

            if (emailIsExists)
                return Result<UserResponse>.Failure<UserResponse>(new Error(" Duplicated Email", StatusCodes.Status400BadRequest));

            var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result<UserResponse>.Failure<UserResponse>(new Error("Invalid Roles", StatusCodes.Status400BadRequest));

            var user = new UserIdentity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                EmailConfirmed = true,
                NormalizedUserName = request.Email.ToUpper() 
            };
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _context.UserRoles
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await _userManager.AddToRolesAsync(user, request.Roles);

                return Result.Success();
            }
            var error = result.Errors.First();

            return Result<UserResponse>.Failure<UserResponse>(new Error(error.Code, StatusCodes.Status400BadRequest));


        }

        public async Task<Result> ToggleStatus(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(new Error("UserNotFound", StatusCodes.Status404NotFound));

            user.IsDisabled = !user.IsDisabled;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> Unlock(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(new Error("UserNotFound", StatusCodes.Status404NotFound));

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, StatusCodes.Status400BadRequest));
        }

             
    }
}