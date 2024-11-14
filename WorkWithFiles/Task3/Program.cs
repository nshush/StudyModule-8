namespace Task3
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
                // Получаем размер папки до очистки
                long initialSize = GetDirectorySize(path);
                Console.WriteLine($"Размер директории до очистки: {initialSize} байт");

                // Выполняем очистку
                int filesDeleted = 0;
                long spaceFreed = ClearDirectory(path, ref filesDeleted);

                Console.WriteLine($"Удалено файлов: {filesDeleted}");
                Console.WriteLine($"Освобождено места: {spaceFreed} байт");


                long finalSize = GetDirectorySize(path);
                Console.WriteLine($"Размер директории после очистки: {finalSize} байт");
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

        static long ClearDirectory(string path, ref int filesDeleted)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Указанная директория не существует.");
            }

            long freedSpace = 0;
            DateTime cutoffTime = DateTime.Now - TimeSpan.FromMinutes(30);

            try
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    if (info.LastAccessTime < cutoffTime)
                    {
                        freedSpace += info.Length;
                        info.Delete();
                        filesDeleted++;
                        Console.WriteLine($"Удалён файл: {info.FullName}");
                    }
                }

                var directories = Directory.GetDirectories(path);
                foreach (var directory in directories)
                {
                    var dirInfo = new DirectoryInfo(directory);
                    if (dirInfo.LastAccessTime < cutoffTime)
                    {
                        freedSpace += GetDirectorySize(directory);
                        Directory.Delete(directory, true);
                        Console.WriteLine($"Удалена папка: {directory}");
                    }
                    else
                    {
                        freedSpace += ClearDirectory(directory, ref filesDeleted);
                    }
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

            return freedSpace;
        }
    }
}
