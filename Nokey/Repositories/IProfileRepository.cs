using CareerCrafter.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerCrafter.Repositories
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
