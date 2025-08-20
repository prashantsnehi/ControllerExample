using System.Globalization;
using System.Text;
using ControllerExample.Models;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

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
        public async Task<MemoryStream> GenerateExcelAsync()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage(memoryStream))
            {
                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context for EPPlus
                ExcelPackage.License.SetNonCommercialPersonal("Prashant");
                var worksheet = package.Workbook.Worksheets.Add("Persons");
                worksheet.Cells[1, 1].Value = "Sr.No";
                worksheet.Cells[1, 2].Value = nameof(Person.PersonId);
                worksheet.Cells[1, 3].Value = nameof(Person.PersonName);
                worksheet.Cells[1, 4].Value = nameof(Person.Email);
                worksheet.Cells[1, 5].Value = nameof(Person.Phone);

                using (ExcelRange headerCells = worksheet.Cells[1, 1, 1, 5])
                {
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerCells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    headerCells.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerCells.AutoFitColumns();
                }

                int rowIndex = 2;
                int index = 1;
                foreach (var person in _persons)
                {
                    worksheet.Cells[rowIndex, 1].Value = index++;
                    worksheet.Cells[rowIndex, 2].Value = person.PersonId;
                    worksheet.Cells[rowIndex, 3].Value = person.PersonName;
                    worksheet.Cells[rowIndex, 4].Value = person.Email;
                    worksheet.Cells[rowIndex, 5].Value = person.Phone;
                    rowIndex++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                await package.SaveAsync();
            }

            memoryStream.Position = 0; // Reset the stream position to the beginning
            return await Task.FromResult(memoryStream);
        }
        public async Task<int> UpdateContriesAsync(IFormFile file)
        {
            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset the stream position to the beginning

            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int updatedCount = 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    var countryName = worksheet.Cells[row, 1].Text;
                    if (!string.IsNullOrEmpty(countryName))
                    {
                        // Simulate updating the country in the database
                        // UpdateCountryInDatabase(countryName);
                        updatedCount++;
                    }
                }

                await package.SaveAsync();
                return updatedCount;
            }
        }
        public async Task<string[]> GetContriesAsync(IFormFile file)
        {
            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset the stream position to the beginning

            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                var countries = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var countryName = worksheet.Cells[row, 1].Text;
                    if (!string.IsNullOrEmpty(countryName))
                    {
                        countries.Add(countryName);
                    }
                }

                return await Task.FromResult(countries.ToArray());
            }
        }
    }
}