using AutoMapper;
using AutoMapper.QueryableExtensions;
using Chat.Application.Features.Accounts.Query.GetAllBlockedUser;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Helpers.FileSettings;
using Chat.Application.Helpers.Paginations;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Chat.Infrastructe.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Chat.Infrastructe.Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHost;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserRespository(
            ApplicationDbContext dbContext, UserManager<AppUser> userManager, IWebHostEnvironment webHost,
            IHttpContextAccessor httpContextAccessor, IMapper mapper

            )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _webHost = webHost;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<AppUser?> GetUserByEmail(string email)
        {
            var user = await _userManager.Users.Include(p => p.Photos)
                                               .SingleOrDefaultAsync(u => u.Email == email);
            return user;
        }
        public async Task<AppUser?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.Id == id);
            return user;
        }
        public async Task<AppUser?> GetUserByNameAsync(string userName)
        {
            return await _userManager.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.UserName == userName);

        }


        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var users = await _userManager.Users
                    .Include(u => u.Photos)
                    .ToListAsync();
            return users;
        }
        public async Task UpdateUser(AppUser user)
        {
            user.LastActive = DateTime.Now;
            await _userManager.UpdateAsync(user);
            _dbContext.SaveChanges();
        }
        public async Task<bool> UploadFile(IFormFile file, string pathName)
        {
            if (file is not null)
            {
                var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim is not null)
                {
                    var user = await _userManager.FindByIdAsync(userIdClaim);
                    if (user is not null)
                    {
                        Photo photo = new();
                        photo.Url = ManageFile.UploadPhoto(_webHost, file, pathName);
                        photo.AppUserId = userIdClaim;
                        await _dbContext.AddAsync(photo);
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;

        }
        public async Task<bool> RemoveFile(int id)
        {
            var currentPhoto = await _dbContext.Photos.FindAsync(id);
            if (currentPhoto is not null)
            {
                _dbContext.Photos.Remove(currentPhoto);
                await _dbContext.SaveChangesAsync();
                ManageFile.RemovePhoto(_webHost, currentPhoto.Url);
                return true;
            }
            return false;
        }

        public async Task<bool> SetMainPhoto(int id)
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is not null)
            {
                var user = await _userManager.FindByIdAsync(userIdClaim);

                if (user is not null)
                {
                    var photosUser = await _dbContext.Photos.Where(u => u.AppUserId == user.Id).ToListAsync();


                    foreach (var photo in photosUser)
                    {
                        photo.IsMain = false;
                    }
                    _dbContext.Photos.UpdateRange(photosUser);
                    await _dbContext.SaveChangesAsync();
                }
            }

            var currentPhoto = await _dbContext.Photos.FindAsync(id);

            if (currentPhoto is not null)
            {
                currentPhoto.IsMain = true;
                _dbContext.Photos.Update(currentPhoto);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<Pagination<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var currentUser = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(currentUser);

            var users = _dbContext.Users.Include(p => p.Photos).AsQueryable();

            //// Filter by the specified gender if provided; otherwise, use the opposite gender of the current user
            //if (!string.IsNullOrEmpty(userParams.Gender))
            //{
            //    users = users.Where(u => u.Gender == userParams.Gender);
            //}
            //else
            //{
            //    // Assuming there are only two genders (male and female)
            //    var oppositeGender = userParams?.Gender?.ToLower() == "male" ? "female" : "male";
            //    users = users.Where(u => u.Gender == oppositeGender);
            //}

            // Calculate age range for date of birth filter
            var today = DateTime.Today;
            var minDateOfBirth = today.AddYears(-userParams.MaxAge);
            var maxDateOfBirth = today.AddYears(-userParams.MinAge).AddDays(1); // Add one day to include the upper bound

            users = users.Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth < maxDateOfBirth);

            // Exclude the current user from the list
            users = users.Where(u => u.UserName != user!.UserName);

            // Apply sorting based on the provided order
            users = userParams.OrderBy.ToLower() switch
            {
                "lastactive" => users.OrderByDescending(u => u.LastActive),
                "created" => users.OrderByDescending(u => u.Created),
                _ => users.OrderByDescending(u => u.LastActive),
            };

            // Ensure that pageNumber is not negative
            var pageNumber = userParams.PageNumber < 1 ? 1 : userParams.PageNumber;

            var memberDtos = users.Select(user => _mapper.Map<MemberDto>(user));

            return Pagination<MemberDto>.Create(memberDtos.AsQueryable(), pageNumber, userParams.PageSize);
        }
        public async Task<bool> SoftDeleteUserAndLockout()
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is not null)
            {
                var user = await _userManager.FindByIdAsync(userIdClaim);
                if (user is not null)
                {
                    // Lock the user out before soft delete
                    await _userManager.SetLockoutEnabledAsync(user, true);

                    // Set lockout end date (optional duration)
                    var lockoutEndDate = DateTimeOffset.UtcNow.AddMinutes(30); // Lockout for 30 minutes
                    await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

                    // Soft delete logic (replace with your desired implementation)
                    // user.IsDeleted = true; // Assuming an IsDeleted property
                    // await _userManager.UpdateAsync(user);

                    // Return success if both operations are successful
                    return true;
                }
            }
            return false;
        }


    }

}
