using System.Security.Cryptography;
using System.Text;

namespace FiscalFlowAdmin.Helpers;

public static class Hasher
    {
        // Метод для генерации случайной строки, аналогичный get_random_string() из Django
        public static string GetRandomString(int length = 12)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var data = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(data);
            }

            var result = new StringBuilder(length);
            foreach (byte b in data)
            {
                // Используем модуль для обеспечения корректного индекса в массиве символов
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }

        public static string SetPassword(string plainPassword)
        {
            // Устанавливаем количество итераций (в вашем случае это 870000)
            int iterations = 870000;
            // Генерируем соль с использованием метода, аналогичного Django
            string salt = GetRandomString(); // Используем новый метод генерации соли

            // Хешируем пароль
            string hashedPassword = HashPassword(plainPassword, salt, iterations);

            // Формируем строку для сохранения в БД в нужном формате
            return $"pbkdf2_sha256${iterations}${salt}${hashedPassword}";
        }

        public static string SetPassword(string plainPassword, string salt)
        {
            int iterations = 870000;

            string hashedPassword = HashPassword(plainPassword, salt, iterations);

            return $"pbkdf2_sha256${iterations}${salt}${hashedPassword}";
        }
        public static string HashPassword(string password, string salt, int iterations)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(
                password,
                Encoding.UTF8.GetBytes(salt),
                iterations,
                HashAlgorithmName.SHA256))
            {
                byte[] hash = rfc2898.GetBytes(32); // 32 байта для SHA-256
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string password, string hashedPasswordFromDb)
        {
            
            string hashedPassword = Hasher.SetPassword("Asd32!", "FixedTestSalt");
            Console.WriteLine(hashedPassword);
            // Извлекаем информацию из хеша пароля
            var parts = hashedPasswordFromDb.Split('$');
            if (parts.Length != 4)
            {
                throw new FormatException("Неверный формат хеша пароля");
            }

            string algorithm = parts[0];
            int iterations = int.Parse(parts[1]);
            string salt = parts[2];
            string storedHash = parts[3];

            // Убедимся, что алгоритм - pbkdf2_sha256
            if (algorithm != "pbkdf2_sha256")
            {
                throw new NotSupportedException($"Алгоритм {algorithm} не поддерживается");
            }

            // Генерируем хеш для пароля с использованием полученной соли и итераций
            string computedHash = HashPassword(password, salt, iterations);

            // Сравниваем хеши
            return storedHash == computedHash;
        }

        public static string ExtractSaltFromHash(string passwordHash)
        {
            // Разделяем строку хеша пароля по $
            string[] parts = passwordHash.Split('$');
            if (parts.Length == 4)
            {
                return parts[2]; // Возвращаем соль, которая находится на 3 позиции (индекс 2)
            }
            throw new FormatException("Неверный формат хеша пароля");
        }

        public static string ExtractHashedPasswordFromHash(string passwordHash)
        {
            // Разделяем строку хеша пароля по $
            string[] parts = passwordHash.Split('$');
            if (parts.Length == 4)
            {
                return parts[3]; // Возвращаем сам хеш пароля (индекс 3)
            }
            throw new FormatException("Неверный формат хеша пароля");
        }
    }