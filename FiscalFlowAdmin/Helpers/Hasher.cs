using System.Security.Cryptography;
using System.Text;

namespace FiscalFlowAdmin.Helpers
{
    public static class Hasher
    {
        // 1. Генерация случайной строки (как в Django)
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

        // 2. Установка пароля (хеширование, как в Django)
        public static string SetPassword(string plainPassword)
        {
            // Устанавливаем количество итераций (в вашем случае это 870000)
            int iterations = 870000;
            // Генерируем соль
            string salt = GetRandomString(); 

            // Хешируем пароль
            string hashedPassword = HashPassword(plainPassword, salt, iterations);

            // Формируем строку в стиле Django
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
            // Для примера: выводим, как будет выглядеть парол ьс фиксированной солью (отладка)
            string debugHash = Hasher.SetPassword("Asd32!", "FixedTestSalt");
            Console.WriteLine("Debug hash: " + debugHash);

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
                return parts[2]; // Возвращаем соль
            }
            throw new FormatException("Неверный формат хеша пароля");
        }

        public static string ExtractHashedPasswordFromHash(string passwordHash)
        {
            // Разделяем строку хеша пароля по $
            string[] parts = passwordHash.Split('$');
            if (parts.Length == 4)
            {
                return parts[3]; // Возвращаем хеш пароля
            }
            throw new FormatException("Неверный формат хеша пароля");
        }

        //--------------------------------------------------------------------------------
        // Методы для ШИФРОВАНИЯ и ДЕШИФРОВАНИЯ (AES-256 в режиме CBC)
        //--------------------------------------------------------------------------------

        /// <summary>
        /// Шифрует строку с помощью AES-256 (CBC). 
        /// Возвращает Base64-строку, где сначала идут (IV + зашифрованные данные).
        /// </summary>
        /// <param name="plainText">Открытый текст, который нужно зашифровать.</param>
        /// <param name="key">Ключ шифрования (должен быть 32 байта в Base64). 
        /// В реальном приложении следует хранить его в защищённом месте.</param>
        /// <returns>Base64-представление (IV + CipherText)</returns>
        [Obsolete("Obsolete")]
        public static string EncryptString(string plainText, string key)
        {
            // Конвертируем Base64-ключ в байты
            byte[] keyBytes = Convert.FromBase64String(key);

            // Генерируем случайный IV
            byte[] iv = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }

            // Создаём AES
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Создаём шифратор
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                    // Склеиваем IV + EncryptedData и кодируем в Base64
                    byte[] combinedData = new byte[iv.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(iv, 0, combinedData, 0, iv.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, combinedData, iv.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(combinedData);
                }
            }
        }

        /// <summary>
        /// Расшифровывает строку, зашифрованную методом EncryptString().
        /// </summary>
        /// <param name="cipherTextBase64">Base64-строка (IV + CipherText).</param>
        /// <param name="key">Ключ шифрования (32 байта в Base64).</param>
        /// <returns>Исходная (дешифрованная) строка.</returns>
        public static string DecryptString(string cipherTextBase64, string key)
        {
            // Конвертируем Base64-ключ в байты
            byte[] keyBytes = Convert.FromBase64String(key);

            // Декодируем Base64-текст, где хранятся (IV + EncryptedData)
            byte[] combinedData = Convert.FromBase64String(cipherTextBase64);

            // IV будет в первых 16 байтах
            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[combinedData.Length - iv.Length];

            Buffer.BlockCopy(combinedData, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(combinedData, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            // Создаём AES
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Создаём дешифратор
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}