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
- 🏗 **Entity Framework Core**

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
  "SmsProvider": {
    "MaxMessagesPerNumberPerSecond": 5,
    "MaxMessagesPerAccountPerSecond": 50
  },
  "ConnectionStrings": {
    "Redis": "localhost:6379"
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

## 📝 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/sms/can-send` | Check if an SMS can be sent without exceeding limits |
| `GET` | `/api/sms/stats` | Retrieve current SMS rate usage per number/account |

Example Request:
```json
{
  "phoneNumber": "+1234567890",
  "accountId": "account_123"
}
```

Example Response:
```json
{
  "canSend": true,
  "remainingQuota": 3
}
```
---

## 📦 NuGet Packages Used

| Package | Description | NuGet URL |
|---------|-------------|------------|
| `Microsoft.AspNetCore.Mvc.Core` | Core MVC framework for ASP.NET | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Core/) |
| `StackExchange.Redis` | Redis client for .NET | [NuGet](https://www.nuget.org/packages/StackExchange.Redis/) |

---

## 📌 Contributing
Feel free to fork the repository and submit pull requests. Contributions are welcome!

---

## 🏆 License
This project is licensed under the MIT License.
