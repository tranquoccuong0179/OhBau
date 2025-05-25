
# 📕 Redis Integration Guide for ASP.NET Core
# Redis Docker Image
````
docker run -d --cap-add sys_resource --name RE -p 8443:8443 -p 9443:9443 -p 12000:12000 redislabs/redis
````
## 🔧 1. Start Redis Using Docker

Khởi động container Redis trên cổng mặc định `6379`:

```bash
docker run -d --name redis -p 6379:6379 redis
```

Kiểm tra Redis hoạt động:

```bash
docker exec -it redis redis-cli ping
# Output: PONG
```

---

## 📦 2. Cài Đặt Redis Client cho .NET

Thêm package Redis vào dự án:

```bash
dotnet add package StackExchange.Redis
```

---

## ⚙️ 3. Cấu Hình Kết Nối Redis Trong ASP.NET Core

### 📁 appsettings.json

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### 🧩 Program.cs

Cấu hình Redis tại program.cs hoặc DIDI:

```csharp
using StackExchange.Redis;

var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped(typeof(IRedisService), typeof(RedisService<>));
```



## 🧪 4. Kiểm Tra Redis Bằng RedisInsight

1. Truy cập: [https://redis.com/redis-enterprise/redis-insight/](https://redis.com/redis-enterprise/redis-insight/)
2. Tải RedisInsight về và cài đặt
3. Mở app, chọn **Connect to Redis Database**
4. Nhập host: `localhost`, port: `6379`, nhấn **Connect**
5. Kiểm tra các key và dữ liệu được cache

---

## 🧼 5. Ghi Nhớ

- Redis chỉ hoạt động khi Redis server (hoặc container Docker) đang chạy.
- Đảm bảo port `6379` không bị chặn.
- Key nên có thời hạn (`expiry`) khi set để tránh giữ cache mãi mãi.
