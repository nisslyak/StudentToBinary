using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace StudentToBinary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;

            string filePath = "";
            if (args.Length > 0)
            {
                filePath = args[0];
            }
            else
            {
                Console.WriteLine("The path to the directory has not been provided. Please try again.");
                return;
            }

            string Content;

            if (File.Exists(filePath))
            {
                List<Student> students = CreateListOfStudents(filePath);

                var studentDirectory = CreateStudentsDirectory();
                RecordListOfStudents(studentDirectory, students);
            }
        }

        public static List<Student> CreateListOfStudents(string filePath)
        {
            List<Student> students = new List<Student>();

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        Student student = new Student
                        {
                            Name = reader.ReadString(),
                            Group = reader.ReadString(),
                            DateOfBirth = DateTime.FromBinary(reader.ReadInt64()),
                            AverageMark = reader.ReadDecimal()
                        };
                        students.Add(student);
                    }
                }
                Console.WriteLine("The data from the binary file has been extracted.");
                return students;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an issue when processing the binary file: {ex.Message}");
                return new List<Student>();
            }
        }

        static string CreateStudentsDirectory()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string studentsDirectoryPath = Path.Combine(desktopPath, "Students");

            if (!Directory.Exists(studentsDirectoryPath))
            {
                Directory.CreateDirectory(studentsDirectoryPath);
                Console.WriteLine($"The folder Student was created: {studentsDirectoryPath}");
            }
            else
            {
                Console.WriteLine($"The folder Student already exists: {studentsDirectoryPath}");
            }
            return studentsDirectoryPath;
        }

        public static void RecordListOfStudents(string filePath, List<Student> students)
        {
            if (students.Count > 0)
            {
                var groupedStudents = students.GroupBy(s => s.Group);

                foreach (var group in groupedStudents)
                {
                    string groupFileName = $"{group.Key}.txt";
                    string groupFilePath = Path.Combine(filePath, groupFileName);

                    using (StreamWriter writer = new StreamWriter(groupFilePath))
                    {
                        foreach (var student in group)
                        {
                            writer.WriteLine($"{student.Name}, {student.DateOfBirth:dd.MM.yyyy}, {student.AverageMark}");
                        }
                    }
                    Console.WriteLine($"File for the group: {groupFileName} has been created.");
                }
            }
            else
            {
                Console.WriteLine("The list of students is empty. Nothing to record.");
            }
            
        }

        [Serializable]
        public class Student
        {
            public string Name { get; set; }
            public string Group { get; set; }
            public DateTime DateOfBirth { get; set; }
            public decimal AverageMark { get; set; }
        }
    }
    
}
