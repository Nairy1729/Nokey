using CareerCrafter.Models;
using System.Threading.Tasks;

namespace CareerCrafter.Repositories
{
    public interface IPersonRepository
    {
        Task<Person> GetPersonByIdAsync(string personId);
        Task<Person> GetPersonByEmailAsync(string email);
        Task<Person> GetPersonByPhoneNumberAsync(long phoneNumber);
        Task<Person> GetPersonByUserNameAsync(string userName);
        Task<bool> ExistsByEmailAsync(string email, string excludeId = null);
        Task<bool> ExistsByPhoneNumberAsync(long phoneNumber, string excludeId = null);
        Task<bool> ExistsByUserNameAsync(string userName, string excludeId = null);
        Task<Person> UpdatePersonAsync(Person updatedPerson);
        Task SaveAsync();
        Task DeletePersonAsync(string personId);
    }
}
