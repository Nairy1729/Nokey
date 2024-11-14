using Microsoft.EntityFrameworkCore;
using Nokey.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nokey.models;
using Nokey.Authentication;

namespace Nokey.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Register a new user
        public async Task<Person> RegisterAsync(Person person)
        {
            person.CreatedAt = DateTime.UtcNow;
            person.UpdatedAt = DateTime.UtcNow;

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return person;
        }

        // Get user by email
        public async Task<Person> GetUserByEmailAsync(string email)
        {
            return await _context.Persons
                                 .Include(p => p.Profile)
                                 .FirstOrDefaultAsync(p => p.Email == email);
        }

        // Get user by ID
        public async Task<Person> GetUserByIdAsync(string id)
        {
            return await _context.Persons
                                 .Include(p => p.Profile)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Get all users
        public async Task<List<Person>> GetAllUsersAsync()
        {
            return await _context.Persons.ToListAsync();
        }

        // Update user profile
        public async Task<Person> UpdateProfileAsync(string id, Person updatedPerson)
        {
            var existingUser = await _context.Persons.FirstOrDefaultAsync(p => p.Id == id);

            if (existingUser == null) return null;

            existingUser.Fullname = updatedPerson.Fullname;
            existingUser.Email = updatedPerson.Email;
            existingUser.PhoneNumber = updatedPerson.PhoneNumber;
            existingUser.Profile = updatedPerson.Profile;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        // Delete user by ID
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _context.Persons.FirstOrDefaultAsync(p => p.Id == id);
            if (user == null) return false;

            _context.Persons.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
