# NetSuite Integration using .NET Framework

## Step-by-Step Guide

### **Step 1**
Install Microsoft Visual Studio .NET, version 2003 or higher, which includes the .NET Framework SDK (RECOMMENDED).

---

### **Step 2**
Start Microsoft Visual Studio .NET.

---

### **Step 3**
Create a new project and choose a template.  
Example: **Console Application**

![Example](https://github.com/user-attachments/assets/65e1cd4f-24a9-4733-890f-31b47bd1aff5)


---

### **Step 4**
After the project has been created, add a Web reference.

- Go to the **Project** menu > **Add Service Reference** > click the **Advanced** button.

![Add Service Reference](https://github.com/user-attachments/assets/1fa4f1d0-5c5f-416e-8c5c-31b3ba9b53e9)



- Click the **Add Web Reference** button.

![Add Web Reference](https://github.com/user-attachments/assets/12ac35c3-83b3-479c-9c9e-3383d37b2ee6)



---

### **Step 5**
In the **Add Web Reference** dialog, enter the SuiteTalk WSDL URL and click the **Go** icon.

Paste the following URL for SUITETALK WSDL:  
`https://webservices.netsuite.com/wsdl/v2024_2_0/netsuite.wsdl`

> **Note**: Version 2024_2_0 or below does not require an Application ID.

---

### **Step 6**
Visual Studio inspects the WSDL and displays a summary of the available operations.  
If security warnings are displayed, click **Yes** as many times as necessary to continue this process.

---

### **Step 7**
After the summary is displayed, click the **Add Reference** button to generate the classes. When this process is complete, `com.netsuite.webservices` is listed under **Web References** in the **Solution Explorer**.

![Web Reference in Solution Explorer](https://github.com/user-attachments/assets/9e3b4091-5550-4e21-a4d3-adcee7ea0ba7)



---

### **Step 8**
View all generated proxy classes by doing one of the following:

- Right-click the `com.netsuite.webservices` listing in the Solution Explorer and choose **View in Object Browser**.

---

### **Step 9**
Implement your application by writing your business logic using the generated .NET proxy classes.  

#### **Example Business Logic**:

```csharp
using System;
using NSIntegSoap.com.netsuite.webservices;

namespace NSIntegSoap
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
            service.CookieContainer = new System.Net.CookieContainer();
            service.Url = config.ServiceUrl;

            // Set up the token passport
            Program program = new Program();
            TokenPassport tokenPassport = program.CreateTokenPassport(config);
            service.tokenPassport = tokenPassport;

            // Create Employee Record
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
            TokenPassportSignature signature = ComputeSignature(
                config.AccountId, 
                config.ConsumerKey, 
                config.ConsumerSecret, 
                config.TokenId, 
                config.TokenSecret, 
                nonce, 
                timestamp
            );

            TokenPassport tokenPassport = new TokenPassport
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

        // Utility methods for signature generation (RandomString, DateTimeToUnixTimestamp, ComputeShaHash) can be added here...
    }
}
