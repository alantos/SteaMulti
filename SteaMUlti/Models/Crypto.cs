using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace SteaMUlti.Models
{
	static class Crypto
	{

		public static byte[] Encrypt(string data)
		{
			if (data != null)
			{
				byte[] dataArray = System.Text.Encoding.UTF32.GetBytes(data);
				try
				{
					byte[] encryptedDataArray = ProtectedData.Protect(dataArray, null, DataProtectionScope.CurrentUser);
					return encryptedDataArray;
				}
				catch (CryptographicException e)
				{
					System.Windows.MessageBox.Show("Encrypting Error: " + e.Message);
				}
			}
			return null;
		}

		public static string Decrypt(byte[] data)
		{
			if (data != null)
			{
				try
				{
					byte[] encryptedDataArray = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
					return System.Text.Encoding.UTF32.GetString(encryptedDataArray);
				}
				catch (CryptographicException e)
				{
					System.Windows.MessageBox.Show("Decryption Error: " + e.Message);
				}
			}
			return null;
		}

		public static byte[] Hash(string password)
		{
			byte[] hash;
			Encoding u8 = Encoding.UTF8;

			SHA256 sha256 = SHA256Managed.Create();
			hash = sha256.ComputeHash(u8.GetBytes(password));

			return hash;
		}

		public static byte[] ComputeIV(byte[] hash)
		{
			byte[] cIV = new byte[16];

			for (int i = 0; i < 16; i++)
			{
				if (hash[i * 2] % 2 == 0)
					cIV[i] = hash[i * 2];
				else
					cIV[i] = hash[i * 2 + 1];
			}

			return cIV;
		}

		public static byte[] EncryptAes(string plainText, byte[] Key, byte[] IV)
		{
			// Check arguments. 
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("Key");
			byte[] encrypted;
			// Create an Aes object 
			// with the specified key and IV. 
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption. 
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{

							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}

			// Return the encrypted bytes from the memory stream. 
			return encrypted;

		}

		public static string DecryptAes(byte[] cipherText, byte[] Key, byte[] IV)
		{
			// Check arguments. 
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("Key");

			// Declare the string used to hold 
			// the decrypted text. 
			string plaintext = null;

			// Create an Aes object 
			// with the specified key and IV. 
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for decryption. 
				try
				{
					using (MemoryStream msDecrypt = new MemoryStream(cipherText))
					{
						using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (StreamReader srDecrypt = new StreamReader(csDecrypt))
							{

								// Read the decrypted bytes from the decrypting stream
								// and place them in a string.
								plaintext = srDecrypt.ReadToEnd();

							}
						}
					}
				}
				catch (CryptographicException)
				{
					System.Windows.MessageBox.Show("Invalid Password");
				}

			}

			return plaintext;

		}

	}
}
