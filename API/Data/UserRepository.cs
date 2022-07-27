using API.Entities;
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

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
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
