namespace Task4
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    [Serializable]
    public class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal AverageScore { get; set; }

        public override string ToString()
        {
            return $"{Name}, {DateOfBirth.ToShortDateString()}, {AverageScore}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите путь к бинарному файлу: ");
            string binaryFilePath = Console.ReadLine();

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string studentsDir = Path.Combine(desktopPath, "Students");

            Directory.CreateDirectory(studentsDir);

            try
            {
                List<Student> students = LoadStudentsFromBinaryFile(binaryFilePath);
                SaveStudentsToFiles(students, studentsDir);
                Console.WriteLine("Данные успешно загружены и сохранены по группам.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        static List<Student> LoadStudentsFromBinaryFile(string filePath)
        {
            List<Student> students = new List<Student>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                try
                {
                    while (fileStream.Position < fileStream.Length)
                    {
                        string name = reader.ReadString();
                        string group = reader.ReadString();
                        long dateTimeBinary = reader.ReadInt64();
                        DateTime dateOfBirth = DateTime.FromBinary(dateTimeBinary);
                        decimal averageScore = reader.ReadDecimal();

                        Student student = new Student
                        {
                            Name = name,
                            Group = group,
                            DateOfBirth = dateOfBirth,
                            AverageScore = averageScore
                        };

                        students.Add(student);
                    }
                }
                catch (EndOfStreamException) {  }
                catch (Exception ex)
                {
                    throw new IOException("Ошибка при чтении бинарного файла.", ex);
                }
            }

            return students;
        }

        static void SaveStudentsToFiles(List<Student> students, string directory)
        {
            Dictionary<string, List<string>> groupStudents = new Dictionary<string, List<string>>();

            foreach (var student in students)
            {
                if (!groupStudents.ContainsKey(student.Group))
                {
                    groupStudents[student.Group] = new List<string>();
                }
                groupStudents[student.Group].Add(student.ToString());
            }

            foreach (var group in groupStudents)
            {
                string groupFilePath = Path.Combine(directory, $"{group.Key}.txt");
                File.WriteAllLines(groupFilePath, group.Value);
            }
        }
    }
}
