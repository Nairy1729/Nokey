using Nokey.models;
using Nokey.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nokey.Repositories
{
    public interface IUserRepository
    {
        Task<Person> RegisterAsync(Person person);
        Task<Person> GetUserByEmailAsync(string email);
        Task<Person> GetUserByIdAsync(int id);
        Task<List<Person>> GetAllUsersAsync();
        Task<Person> UpdateProfileAsync(int id, Person updatedPerson);
        Task<bool> DeleteUserAsync(int id);
    }
}
