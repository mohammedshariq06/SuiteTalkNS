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

![Example](https://github.com/user-attachments/assets/4f354965-979f-416e-9157-32720a980078)

---

### **Step 4**
After the project has been created, add a Web reference.

- Go to the **Project** menu > **Add Service Reference** > click the **Advanced** button.

![Add Service Reference](https://github.com/user-attachments/assets/f2c67165-8d27-473f-a3ea-a62b78a3efbf)


- Click the **Add Web Reference** button.

![Add Web Reference](https://github.com/user-attachments/assets/5633a8ae-58e0-4e8d-815e-854bb97a3b4d)


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

![Web Reference in Solution Explorer](https://github.com/user-attachments/assets/e0ada1cd-e83a-45e1-ba10-5c606083a545)


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
