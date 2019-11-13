using Codecamp.Data;
using Codecamp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface IUserBusinessLogic
    {
        Task<bool> UserExists(string CodecampUserId);
        Task<CodecampUser> GetUser(string CodecampUserId);

        Task<bool> UpdateUser(CodecampUser user);
    }

    public class UserBusinessLogic : IUserBusinessLogic
    {
        private CodecampDbContext _context;

        public UserBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExists(string CodecampUserId)
        {
            return await _context.CodecampUsers.AnyAsync(c => c.Id == CodecampUserId);
        }

        public async Task<CodecampUser> GetUser(string CodecampUserId)
        {
            return await _context.CodecampUsers.FirstOrDefaultAsync(c =>
                c.Id == CodecampUserId);
        }


        public async Task<bool> UpdateUser(CodecampUser user)
        {
            try
            {
                _context.CodecampUsers.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(user.Id))
                    return false;
                else
                    throw;
            }
        }
    }
}
