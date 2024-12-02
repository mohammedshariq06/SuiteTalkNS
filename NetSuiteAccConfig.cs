using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiteTalkNS
{
	public class NetSuiteAccConfig
	{
		public string AccountId { get; set; }
		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
		public string TokenId { get; set; }
		public string TokenSecret { get; set; }
		public string ServiceUrl { get; set; }

		public NetSuiteAccConfig()
		{
			// Set default configuration values
			AccountId = "Your_AccountID";
			ConsumerKey = "Your_ConsumerKey";
			ConsumerSecret = "Your_ConsumerSecret";
			TokenId = "Your_TokenId";
			TokenSecret = "Your_TokenSecret";
			ServiceUrl = "https://<accountid>.suitetalk.api.netsuite.com/services/NetSuitePort_2024_2";
		}

		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(AccountId) ||
				string.IsNullOrWhiteSpace(ConsumerKey) ||
				string.IsNullOrWhiteSpace(ConsumerSecret) ||
				string.IsNullOrWhiteSpace(TokenId) ||
				string.IsNullOrWhiteSpace(TokenSecret) ||
				string.IsNullOrWhiteSpace(ServiceUrl))
			{
				throw new Exception("NetSuite configuration values are missing or incomplete.");
			}
		}
	}
}
