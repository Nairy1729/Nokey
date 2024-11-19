using Microsoft.AspNetCore.Http;
using Nokey.Authentication;
using Nokey.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nokey.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfileRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Profile GetProfileByUserId(string userId)
        {
            return _dbContext.Profiles.FirstOrDefault(p => p.PersonId == userId);
        }

        public async Task<Profile> CreateOrUpdateProfileAsync(string userId, Profile profile, IFormFile resume, IFormFile profilePhoto)
        {
            var existingProfile = _dbContext.Profiles.FirstOrDefault(p => p.PersonId == userId);

            if (existingProfile == null)
            {
                existingProfile = new Profile { PersonId = userId };
                _dbContext.Profiles.Add(existingProfile);
            }

            existingProfile.Bio = profile.Bio;
            existingProfile.Skills = profile.Skills;

            if (resume != null)
            {
                if (Path.GetExtension(resume.FileName).ToLower() != ".pdf")
                    throw new InvalidOperationException("Resume must be a PDF file.");

                using var ms = new MemoryStream();
                await resume.CopyToAsync(ms);
                existingProfile.Resume = ms.ToArray();
                existingProfile.ResumeFileName = resume.FileName;
            }

            if (profilePhoto != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePhoto.FileName)}";
                var filePath = Path.Combine("Uploads/ProfilePhotos", fileName);

                Directory.CreateDirectory("Uploads/ProfilePhotos");
                using var stream = new FileStream(filePath, FileMode.Create);
                await profilePhoto.CopyToAsync(stream);

                existingProfile.ProfilePhoto = fileName;
            }

            _dbContext.SaveChanges();
            return existingProfile;
        }

        public bool DeleteProfile(string userId)
        {
            var profile = _dbContext.Profiles.FirstOrDefault(p => p.PersonId == userId);

            if (profile == null)
                return false;

            _dbContext.Profiles.Remove(profile);
            _dbContext.SaveChanges();

            return true;
        }

        public byte[] GetResume(string userId)
        {
            var profile = _dbContext.Profiles.FirstOrDefault(p => p.PersonId == userId);
            return profile?.Resume;
        }

        public string GetProfilePhotoPath(string userId)
        {
            var profile = _dbContext.Profiles.FirstOrDefault(p => p.PersonId == userId);
            if (profile?.ProfilePhoto == null) return null;

            return Path.Combine("Uploads/ProfilePhotos", profile.ProfilePhoto);
        }
    }
}
