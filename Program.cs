using SuiteTalkNS.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;

namespace SuiteTalkNS
{
	public class Program
	{
		static void Main(string[] args)
		{
			// Initialize configuration
			NetSuiteAccConfig config = new NetSuiteAccConfig();
			config.Validate();

			// Initialize NetSuite Service
			NetSuiteService service = new NetSuiteService();
			service.CookieContainer = new CookieContainer();
			service.Url = config.ServiceUrl;

			// Set up the token passport
			Program program = new Program();
			TokenPassport tokenPassport = program.CreateTokenPassport(config);
			service.tokenPassport = tokenPassport;

			// Create a Employee (after authentication)
			Employee employee = new Employee
			{
				customForm = new RecordRef { internalId = "147" }, // Replace with valid internalId
				firstName = "John",
				lastName = "Doe",
				email = "john.doe@example.com",
				mobilePhone = "1234567890",
				title = "Software Engineer",
				subsidiary = new RecordRef { internalId = "54" }, // Replace with valid internalId
				location = new RecordRef { internalId = "26" },  // Replace with valid internalId
				isInactive = false // Optional but useful for clarity
			};

			try
			{
				WriteResponse response = service.add(employee);
				if (response.status.isSuccess)
				{
					Console.WriteLine("Employee added successfully!");
					Console.WriteLine("Enter any Key to close");
					Console.ReadKey();
				}
				else
				{
					Console.WriteLine("Error: " + response.status.statusDetail[0].message);
					Console.WriteLine("Enter any Key to close");
					Console.ReadKey();
				}
			}
			catch (SoapException ex)
			{
				Console.WriteLine("SOAP Exception: " + ex.Message);
				Console.WriteLine("Detail: " + ex.Detail.InnerXml);
				Console.WriteLine("Enter any Key to close");
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
				Console.WriteLine("Enter any Key to close");
				Console.ReadKey();
			}
		}

		public TokenPassport CreateTokenPassport(NetSuiteAccConfig config)
		{
			string nonce = RandomString(14);
			string timestamp = DateTimeToUnixTimestamp(DateTime.UtcNow).ToString().Substring(0, 10);
			TokenPassportSignature signature = computeSignature(config.AccountId, config.ConsumerKey, config.ConsumerSecret, config.TokenId, config.TokenSecret, nonce, timestamp);

			TokenPassport tokenPassport = new TokenPassport()
			{
				account = config.AccountId,
				consumerKey = config.ConsumerKey,
				token = config.TokenId,
				nonce = nonce,
				timestamp = long.Parse(timestamp),
				signature = signature
			};

			return tokenPassport;
		}


		private static Random random = new Random();
		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
		public static double DateTimeToUnixTimestamp(DateTime dateTime)
		{
			DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
			return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
		}
		protected string computeShaHash(string baseString, string key, string algorithm)
		{
			string result = "";
			try
			{

				byte[] bytes = Encoding.ASCII.GetBytes(key);
				using (HMACSHA256 hmacSha256 = new HMACSHA256(bytes))
				{
					Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(baseString);
					result = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
				}

				return result;
			}
			catch (Exception Ex)
			{
				return result;
			}
		}
		public TokenPassportSignature computeSignature(string account, string consumerKey, string consumerSecret, string token, string tokenSecret, string nonce, string timeStamp)
		{
			string baseString = account + "&" + consumerKey + "&" + token + "&" + nonce + "&" + timeStamp;
			string key = consumerSecret + '&' + tokenSecret;
			string signature = computeShaHash(baseString, key, "HmacSHA256");

			TokenPassportSignature sign = new TokenPassportSignature();
			sign.algorithm = "HMAC_SHA256";
			sign.Value = signature;
			return sign;
		}
	}
}
