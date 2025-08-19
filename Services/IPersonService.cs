using ControllerExample.Models;

namespace ControllerExample.Services
{
    public interface IPersonService
    {
        Guid ServiceInstanceId { get; }
        List<Person> GetPersons();
        Person? GetPersonById(Guid guid);
        Guid GetGuid();
        bool AddPerson(Person person);

        Task<MemoryStream> GenerateCsvAsync(List<Person> persons);
        Task<MemoryStream> GenerateCustomCsvAsync(List<Person> persons);
    }
}