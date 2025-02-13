# 📩 SMSInsights - SMS Rate Limiter Microservice

**SMSInsights** is a .NET Core microservice designed to act as a gatekeeper for sending SMS messages, ensuring that messages do not exceed the rate limits imposed by an external SMS provider. This helps businesses avoid unnecessary API costs while ensuring reliable message delivery.

---

## 🚀 Features
- ✅ Enforces per-number and per-account SMS rate limits.
- ✅ Prevents unnecessary API calls when limits are exceeded.
- ✅ Real-time rate limit tracking using in-memory or distributed caching.
- ✅ Scalable to handle high request volumes efficiently.

---

## 🛠 Tech Stack
- 🏗 **.NET Core 7+**
- 🏗 **ASP.NET Core Web API**
- 🏗 **Redis (for distributed rate limiting)**
- 🏗 **React (provides a dashboard client)**


---

## 🏷 Setup & Installation

### 1️⃣ Prerequisites
Ensure you have the following installed:
- 📌 [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- 📌 [Redis](https://redis.io/) (for distributed rate limiting)
- 📌 [Docker](https://www.docker.com/) (optional, for running Redis in a container)

### 2️⃣ Clone the Repository
```sh
git clone https://github.com/twalker-codes/SMSInsights.git
cd SMSInsights
```

### 3️⃣ Install Dependencies
Restore NuGet packages:
```sh
dotnet restore
```

### 4️⃣ Configure Environment Variables
Create an **`appsettings.json`** file in the `SMSInsights.API` project:
```json
{
  "ApplicationSettings": {
    "RateLimits": {
      "MaxMessagesPerSenderPerSec": 5,
      "MaxMessagesGlobalPerSec": 20
    },
    "Redis": {
      "ConnectionString": "localhost:6379"
    }
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/log-.txt", "rollingInterval": "Day" } }
    ]
  }
}
```

If running Redis in Docker:
```sh
docker run --name redis -d -p 6379:6379 redis
```

### 5️⃣ Run the Microservice
```sh
dotnet run --project SMSInsights.API
```

API will be available at: `http://localhost:5000`

---

## 🖥 Setting Up and Running the React Client (smsinsights-client)

### 1️⃣ Prerequisites
Ensure you have the following installed:
- 📌 [Node.js](https://nodejs.org/) (Recommended: Latest LTS version)
- 📌 [Yarn](https://yarnpkg.com/) or `npm` (for package management)

### 2️⃣ Navigate to the Client Directory
```sh
cd smsinsights-client
```

### 3️⃣ Install Dependencies
Using Yarn:
```sh
yarn install
```
Or using npm:
```sh
npm install
```

### 4️⃣ Configure API Endpoint
The API URL is stored in `config.js`. Update this file if necessary to point to the correct backend service:

```js
const API_BASE_URL = "http://localhost:5000";

const API_ENDPOINTS = {
  globalUsage: `${API_BASE_URL}/api/globalUsage`,
  senderUsage: `${API_BASE_URL}/api/senderUsage`,
  sendMessage: `${API_BASE_URL}/api/message/send`
};

export default API_ENDPOINTS;
```

Modify `API_BASE_URL` if your backend is running on a different host or port.

### 5️⃣ Run the Client
Using Yarn:
```sh
yarn start
```
Or using npm:
```sh
npm start
```
---

## 📝 API Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/message/send` | Sends an SMS message after checking rate limits. |
| `GET` | `/api/metrics/global` | Retrieves global SMS usage metrics. |
| `GET` | `/api/metrics/sender/{senderNumber}` | Retrieves usage metrics for a specific sender. |
| `GET` | `/api/health` | Provides a health check status of the service. |

Example Request for Sending an SMS:
```json
{
  "SenderPhoneNumber": "+15551234567",
  "ReceiverPhoneNumber": "+15557654321",
  "Message": "Hello, this is a test message."
}
```

Example Response:
```json
{
    "success": true,
    "message": "Message sent successfully."
}
```

The api will be available at:  `http://localhost:3000`
---

## 📦 NuGet Packages Used

| Package | Description | NuGet URL |
|---------|-------------|------------|
| `Microsoft.AspNetCore.OpenApi` | Provides APIs for annotating endpoints | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/9.0.2/) |
| `StackExchange.Redis` | Redis client for .NET | [NuGet](https://www.nuget.org/packages/StackExchange.Redis/) |
| `Serilog.AspNetCore ` | Serilog.Settings.Configuration | [Github](https://github.com/serilog/serilog-aspnetcore/) |
| `Serilog.Settings.Configuration` | Support for Serilog | [Github](https://github.com/serilog/serilog-settings-configuration/) |
| `Serilog.Sinks.Console` | Support for Serilog | [Github](https://github.com/serilog/serilog-sinks-console/) |
| `Serilog.Sinks.File` | Support for Serilog | [Github](https://github.com/serilog/serilog-sinks-file/) |
| `Swashbuckle.AspNetCore` | Swagger tools for doc | [Github](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) |
| `xunit` |  Testing framework | [Nuget](https://www.nuget.org/packages/xunit/2.9.3/license) |
| `NSubstitute` |  mocking framework | [Github](https://nsubstitute.github.io/) |
---

## 📌 Contributing
Feel free to fork the repository and submit pull requests. Contributions are welcome!

---

## 🏆 License
This project is licensed under the MIT License.
