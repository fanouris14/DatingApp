using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        //constructor
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        //end

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);

            //return await _context.Users
            //    .Where(x => x.UserName == username)
            //    .Select(user => new MemberDto
            //    {
            //        Id = user.Id,
            //        Username = user.UserName,
            //        PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            //        KnownAs = user.KnownAs,
            //        Created = user.Created,
            //        LastActive = user.LastActive,
            //        Gender = user.Gender,
            //        Introduction = user.Introduction,
            //        LookingFor = user.LookingFor,
            //        Interests = user.Interests,
            //        City = user.City,
            //        Country = user.Country,
            //        Photos = user.Photos.Select(e => new PhotoDto
            //        {
            //            Id = e.Id,
            //            IsMain = e.IsMain,
            //            Url = e.Url,
            //        }).ToList(),
            //    })
            //    .SingleOrDefaultAsync();

        }

        public async Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams)
        {
            //[OLD WAY - BEFORE PAGINATION]
            //return await _context.Users
            //    .Include(p => p.Photos)
            //    .ToListAsync();

            //[OLD WAY - NOT WORKING FOR FILTERING
            //var query = _context.Users
            //    .Include(p => p.Photos)
            //    .AsNoTracking(); //only for display purposes, not editable;

            //[filter]first init and make it Queryable so i can edit it in other vars
            var queryEdited = _context.Users.Include(p => p.Photos).AsQueryable();

            //[filter] remove own user and filter by gender
            queryEdited = queryEdited.Where(u => u.UserName != userParams.CurrentUsername);
            queryEdited = queryEdited.Where(u => u.Gender == userParams.Gender);

            //[filter] by age
            var minDob = DateTime.Today.AddYears(-userParams.maxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.minAge - 1);

            queryEdited = queryEdited.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            //[shorting] by last time active
            queryEdited = userParams.OrderBy switch //new switch syntax without the need of break; and case;
            {
                "created" => queryEdited.OrderByDescending(u => u.Created),
                _ => queryEdited.OrderByDescending(u => u.LastActive) //default value
            };

            var query = queryEdited.AsNoTracking();

            return await PagedList<AppUser>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
