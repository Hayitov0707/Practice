using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using task13;

namespace task13tests
{
    public class JsonStudentManagerTests
    {
        [Fact]
        public void Serialize_ShouldIgnoreNullValuesAndFormatDate()
        {
            var student = new Student
            {
                FirstName = "Ivan",
                LastName = null,
                BirthDate = new DateTime(2005, 5, 15),
                Grades = new List<Subject> { new Subject { Name = "Math", Grade = 95 } }
            };

            string json = JsonStudentManager.Serialize(student);

            Assert.Contains("\"FirstName\": \"Ivan\"", json);
            Assert.Contains("\"BirthDate\": \"15.05.2005\"", json);
            Assert.DoesNotContain("LastName", json);
        }

        [Fact]
        public void Deserialize_ShouldThrowExceptionOnInvalidData()
        {
            string invalidJson = "{\"FirstName\":\"\",\"LastName\":\"Petrov\",\"BirthDate\":\"10.10.2004\"}";

            Assert.Throws<ArgumentException>(() => JsonStudentManager.DeserializeAndValidate(invalidJson));
        }

        [Fact]
        public void SaveAndLoadFromFile_ShouldPersistDataCorrectly()
        {
            var student = new Student
            {
                FirstName = "Oleg",
                LastName = "Khayitov",
                BirthDate = new DateTime(2006, 1, 1),
                Grades = new List<Subject> { new Subject { Name = "IT", Grade = 100 } }
            };
            string tempFile = Path.GetTempFileName();

            try
            {
                JsonStudentManager.SaveToFile(tempFile, student);
                var loaded = JsonStudentManager.LoadFromFileAndValidate(tempFile);

                Assert.Equal("Oleg", loaded.FirstName);
                Assert.Equal("Khayitov", loaded.LastName);
                Assert.Equal(100, loaded.Grades![0].Grade);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
