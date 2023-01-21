using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(u=>u.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users
                .Include(p=> p.Photos)
                .SingleOrDefaultAsync(u=> u.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p=> p.Photos)
                .ToListAsync();
        }

        public async Task<AppUser> GetUsersByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UserExistAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName== username.ToLower());
        }

        public async Task AddUserAsync(AppUser user)
        {
            await _context.Users.AddAsync(user);
        }


        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync()>0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}