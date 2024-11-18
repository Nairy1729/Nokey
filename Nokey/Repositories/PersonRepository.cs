using Microsoft.EntityFrameworkCore;
using Nokey.Authentication;
using Nokey.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Nokey.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person> GetPersonByIdAsync(string personId)
        {
            return await _context.Set<Person>().FindAsync(personId);
        }

        public async Task<Person> GetPersonByEmailAsync(string email)
        {
            return await _context.Set<Person>().FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<Person> GetPersonByPhoneNumberAsync(long phoneNumber)
        {
            return await _context.Set<Person>().FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
        }

        public async Task<Person> GetPersonByUserNameAsync(string userName)
        {
            return await _context.Set<Person>().FirstOrDefaultAsync(p => p.UserName == userName);
        }

        public async Task<bool> ExistsByEmailAsync(string email, string excludeId = null)
        {
            return await _context.Set<Person>().AnyAsync(p => p.Email == email && p.Id != excludeId);
        }

        public async Task<bool> ExistsByPhoneNumberAsync(long phoneNumber, string excludeId = null)
        {
            return await _context.Set<Person>().AnyAsync(p => p.PhoneNumber == phoneNumber && p.Id != excludeId);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName, string excludeId = null)
        {
            return await _context.Set<Person>().AnyAsync(p => p.UserName == userName && p.Id != excludeId);
        }

        public async Task<Person> UpdatePersonAsync(Person updatedPerson)
        {
            var person = await GetPersonByIdAsync(updatedPerson.Id);
            if (person == null)
                return null;

            person.Fullname = updatedPerson.Fullname;
            person.Email = updatedPerson.Email;
            person.PhoneNumber = updatedPerson.PhoneNumber;
            person.UserName = updatedPerson.UserName;
            person.UpdatedAt = updatedPerson.UpdatedAt;

            _context.Entry(person).State = EntityState.Modified;

            return person;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(string personId)
        {
            var person = await GetPersonByIdAsync(personId);
            if (person != null)
            {
                person.IsDeleted = true;
                _context.Entry(person).State = EntityState.Modified;
                await SaveAsync();
            }
        }
    }
}
