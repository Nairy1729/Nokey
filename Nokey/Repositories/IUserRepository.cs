﻿using Nokey.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nokey.Repositories
{
    public interface IUserRepository
    {
        Task<Person> RegisterAsync(Person person);
        Task<Person> GetUserByEmailAsync(string email);
        Task<Person> GetUserByIdAsync(string id);
        Task<List<Person>> GetAllUsersAsync();
        Task<Person> UpdateProfileAsync(string id, Person updatedPerson);
        Task<bool> DeleteUserAsync(string id);
    }
}
