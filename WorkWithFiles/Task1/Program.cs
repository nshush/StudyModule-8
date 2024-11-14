namespace Task1
{
    using System;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите путь до папки: ");
            string path = Console.ReadLine();

            try
            {
                CleanDirectory(path);
                Console.WriteLine("Чистка завершена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        static void CleanDirectory(string path)
        {
            // Проверка на существование папки
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Ошибка: Указанная папка не существует.");
                return;
            }

            // Ттекущее время
            DateTime cutoffTime = DateTime.Now - TimeSpan.FromMinutes(30);

            var entries = Directory.GetFileSystemEntries(path);

            foreach (var entry in entries)
            {
                try
                {
                    var info = new FileInfo(entry);
                    if (info.Exists)
                    {
                        if (info.LastAccessTime < cutoffTime)
                        {
                            info.Delete();
                            Console.WriteLine($"Удалён файл: {info.FullName}");
                        }
                    }
                    else if (Directory.Exists(entry))
                    {
                        if (Directory.GetLastAccessTime(entry) < cutoffTime)
                        {
                            Directory.Delete(entry, true);
                            Console.WriteLine($"Удалена папка: {entry}");
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Нет доступа к элементу: {entry}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ошибка ввода-вывода при обработке {entry}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка при обработке {entry}: {ex.Message}");
                }
            }
        }
    }
}