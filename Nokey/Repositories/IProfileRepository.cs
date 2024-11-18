using Microsoft.AspNetCore.Http;
using Nokey.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nokey.Repositories
{
    public interface IProfileRepository
    {
        Profile GetProfileByUserId(string userId);
        Task<Profile> CreateOrUpdateProfileAsync(string userId, Profile profile, IFormFile resume, IFormFile profilePhoto);
        bool DeleteProfile(string userId);
        byte[] GetResume(string userId);
        string GetProfilePhotoPath(string userId);
    }
}
