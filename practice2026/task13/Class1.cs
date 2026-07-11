using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace task13
{
    public class Subject
    {
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }
    }

    public class Student
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public List<Subject>? Grades { get; set; }
    }

    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "dd.MM.yyyy";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString()!, _format, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }

    public static class JsonStudentManager
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Converters = { new CustomDateTimeConverter() }
        };

        public static string Serialize(Student student)
        {
            return JsonSerializer.Serialize(student, Options);
        }

        public static Student DeserializeAndValidate(string json)
        {
            var student = JsonSerializer.Deserialize<Student>(json, Options);
            
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }
            if (string.IsNullOrWhiteSpace(student.FirstName))
            {
                throw new ArgumentException("FirstName cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(student.LastName))
            {
                throw new ArgumentException("LastName cannot be null or empty");
            }
            if (student.BirthDate > DateTime.Now)
            {
                throw new ArgumentException("BirthDate cannot be in the future");
            }
            if (student.Grades != null)
            {
                foreach (var grade in student.Grades)
                {
                    if (grade.Grade < 0 || grade.Grade > 100)
                    {
                        throw new ArgumentException("Grade must be between 0 and 100");
                    }
                }
            }

            return student;
        }

        public static void SaveToFile(string filePath, Student student)
        {
            string json = Serialize(student);
            File.WriteAllText(filePath, json);
        }

        public static Student LoadFromFileAndValidate(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return DeserializeAndValidate(json);
        }
    }
}
