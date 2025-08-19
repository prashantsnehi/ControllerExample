using System.Globalization;
using System.Text;
using ControllerExample.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace ControllerExample.Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _persons;
        private Guid _serviceInstanceId;
        public Guid ServiceInstanceId { get { return _serviceInstanceId; } }
        public PersonService()
        {
            _serviceInstanceId = Guid.NewGuid();

            _persons = new List<Person>()
            {
                new() { PersonId = Guid.NewGuid(), PersonName = "Scott", Email = "scott@example.com", Phone = 123456789, Password = "P@$$w0rd", ConfirmPassword = "P@$$w0rd", Profession = "DBA" },
                new() { PersonId = Guid.NewGuid(), PersonName = "Tiger", Email = "tiger@example.com", Phone = 223456789, Password = "P@$$w0rd", ConfirmPassword = "P@$$w0rd", Profession = "DBA" },
                new() { PersonId = Guid.NewGuid(), PersonName = "Administrator", Email = "admin@example.com", Phone = 222456789, Password = "P@$$w0rd", ConfirmPassword = "P@$$w0rd", Profession = "DBA" },
                new() { PersonId = Guid.NewGuid(), PersonName = "SA", Email = "sa@example.com", Phone = 222256789, Password = "P@$$w0rd", ConfirmPassword = "P@$$w0rd", Profession = "DBA" }
            };
        }
        public Guid GetGuid() => ServiceInstanceId;
        public Person? GetPersonById(Guid guid) => _persons.FirstOrDefault(x => x.PersonId == guid);
        public List<Person> GetPersons() => _persons;
        public bool AddPerson(Person person)
        {
            if (person != null && !string.IsNullOrEmpty(person.PersonName) && !string.IsNullOrEmpty(person.Email))
            {
                person.PersonId = Guid.NewGuid();
                _persons.Add(person);
                return true;
            }
            else
            {
                return false;
                // throw new ArgumentException("Person cannot be null or have empty fields.");
            }
        }

        public async Task<MemoryStream> GenerateCsvAsync(List<Person> persons)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            var csv = new CsvWriter(writer);

            csv.WriteHeader<Person>();
            csv.NextRecord();
            csv.WriteRecords(persons);
            writer.Flush();
            memoryStream.Position = 0; // Reset the stream position to the beginning

            return await Task.FromResult(memoryStream);
        }

        public async Task<MemoryStream> GenerateCustomCsvAsync(List<Person> persons)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            var csvConfiguration = new CsvConfiguration()
            {
                Delimiter = ",",
                Encoding = Encoding.UTF8,
                HasHeaderRecord = true
            };

            var csv = new CsvWriter(writer);

            csv.WriteField("Sr.No");
            csv.WriteField(nameof(Person.PersonId));
            csv.WriteField(nameof(Person.PersonName));
            csv.WriteField(nameof(Person.Email));
            csv.WriteField(nameof(Person.Phone));
            csv.NextRecord();
            int index = 1;
            foreach (var person in persons)
            {
                csv.WriteField(index++);
                csv.WriteField(person.PersonId);
                csv.WriteField(person.PersonName);
                csv.WriteField(person.Email);
                csv.WriteField(person.Phone);
                csv.NextRecord();
            }

            writer.Flush();
            memoryStream.Position = 0; // Reset the stream position to the beginning

            return await Task.FromResult(memoryStream);
        }

    }

}