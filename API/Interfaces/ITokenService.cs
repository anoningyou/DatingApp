
using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateTocken(AppUser user);
        
    }
}