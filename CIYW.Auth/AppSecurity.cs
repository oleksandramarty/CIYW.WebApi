using System.Security.Cryptography;
using System.Text;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;

namespace CIYW.Auth;

    public class AppSecurity
    {
        public static string GenerateSalt()
        {
            int saltLength = 64;

            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] salt = new byte[saltLength];
            try
            {
                random.GetNonZeroBytes(salt);
            }
            catch (Exception e)
            {
                throw new LoggerException($"An error occurred while generating a new salt: [{e.GetError()}]", 500, null, null,"");
            }
            finally
            {
                random.Dispose();
            }

            string saltToBase64 = Convert.ToBase64String(salt);

            return saltToBase64;
        }

        public static string GetSHA256(string data)
        {
            if (data == null)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA256 hash, hashing data was not transmitted", 400);
            }

            byte[] byteList = null;

            try
            {
                byteList = Encoding.UTF8.GetBytes(data);
            }
            catch (Exception e)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA256 hash, incorrect data was transmitted: [{e.GetError()}]", 400);
            }

            return GetSHA256(byteList);
        }

        public static string GetSHA256(byte[] data)
        {
            if (data == null)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA256 hash, hashing data was not transmitted", 400);
            }

            SHA256Managed hashObject = new SHA256Managed();

            byte[] hashValue;

            try
            {
                hashValue = hashObject.ComputeHash(data);
            }
            catch (Exception e)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA25 hash6: [{e.GetError()}]", 400);
            }
            finally
            {
                hashObject.Dispose();
            }

            string hash = "";

            foreach (byte x in hashValue)
            {
                hash += string.Format("{0:x2}", x);
            }

            return hash;
        }

        public static string GetSHA512(string data)
        {
            if (data == null)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA512 hash, the hashing data was not transmitted", 400);
            }

            byte[] byteList = null;

            try
            {
                byteList = Encoding.UTF8.GetBytes(data);
            }
            catch (Exception e)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA512 hash, incorrect data was transmitted: [{e.GetError()}]", 400);
            }

            return GetSHA512(byteList);
        }

        public static string GetSHA512(byte[] data)
        {
            if (data == null)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA512 hash, hashing data was not transmitted", 400);
            }

            SHA512Managed hashObject = new SHA512Managed();

            byte[] hashValue;

            try
            {
                hashValue = hashObject.ComputeHash(data);
            }
            catch (Exception e)
            {
                throw new LoggerException($"An error occurred while retrieving the SHA512 hash: [{e.GetError()}]", 400);
            }
            finally
            {
                hashObject.Dispose();
            }

            string hash = "";

            foreach (byte x in hashValue)
            {
                hash += string.Format("{0:x2}", x);
            }

            return hash;
        }

        public static bool GetSHA512AndSalt(string source, out string hash, out string salt, out string error)
        {
            try
            {
                hash = null;
                salt = null;
                error = null;

                if (source == null)
                {
                    error = "No input for hashing provided";
                    return false;
                }

                salt = GenerateSalt();

                byte[] saltBytes = Convert.FromBase64String(salt);
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] sourceAndSaltBytes = new byte[saltBytes.Length + sourceBytes.Length];
                sourceBytes.CopyTo(sourceAndSaltBytes, 0);
                saltBytes.CopyTo(sourceAndSaltBytes, sourceBytes.Length);
                hash = GetSHA512(sourceAndSaltBytes);

                return true;
            }
            catch (Exception e)
            {
                hash = null;
                salt = null;
                error = e.Message;
                return false;
            }
        }
    }