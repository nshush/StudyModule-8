namespace Task2
{
    using System;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите путь к директории: ");
            string path = Console.ReadLine();

            try
            {
                long size = GetDirectorySize(path);
                Console.WriteLine($"Размер директории: {size} байт");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        static long GetDirectorySize(string path)
        {
            // Валидация пути
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Указанная директория не существует.");
            }

            long size = 0;

            try
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    size += new FileInfo(file).Length;
                }

                var directories = Directory.GetDirectories(path);
                foreach (var directory in directories)
                {
                    size += GetDirectorySize(directory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет доступа к директории: {path}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка ввода-вывода при обработке {path}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при обработке {path}: {ex.Message}");
            }

            return size;
        }
    }
}
