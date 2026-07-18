using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Helpers
{
	public static class HashConvertor
	{
		public static string ConvertToHash<T>(T value)
		{
			var valueString = JsonSerializer.Serialize(value);
			byte[] bytes = Encoding.UTF8.GetBytes(valueString);
			byte[] hash = SHA256.HashData(bytes); 
			return Convert.ToHexString(hash).ToLowerInvariant();
		}
		public static string ConvertFromHash(string value)
		{
			var hashBytes = Convert.FromHexString(value);
			return Encoding.UTF8.GetString(hashBytes);
		}
	}
}
