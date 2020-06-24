using System;
using System.Security.Cryptography;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    class PasswordEncryptor
    {
        private readonly byte[] salt;

        public PasswordEncryptor()
        {
            salt = new byte[] { 5, 58, 169, 86, 157, 249, 57, 136, 245, 222, 4, 198, 65, 88, 148, 209 };
        }

        public string EnctyptPassword(string password)
        {
            //1. Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            //2. Combine the salt and password bytes for later use
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //3. Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            //4. Return hashed password
            pbkdf2.Dispose();
            return savedPasswordHash;
        }

        public string DecryptPassword(string hashedPassword, string password)
        {
            //1. Fetch the stored value
            string savedPasswordHash = hashedPassword;

            //2. Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

            //3. Get the salt 
            //salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            //4. Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            //5. Compare the results 
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    throw new UnauthorizedAccessException();

            //6. Return decrypted password
            pbkdf2.Dispose();
            return password;
        }
    }
}
