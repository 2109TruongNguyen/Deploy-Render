# 1️⃣ Base image chứa ASP.NET Core runtime (Chạy ứng dụng)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7166
EXPOSE 5153

# Thêm biến môi trường để bật HTTPS
ENV ASPNETCORE_URLS="https://+:7166;http://+:5153"
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# 2️⃣ Build image chứa .NET SDK (Dùng để build app)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 3️⃣ Copy file .csproj trước để tận dụng cache
COPY NZWalksAPI/NZWalksAPI.csproj NZWalksAPI/
COPY RepositoryLayer/RepositoryLayer.csproj RepositoryLayer/
COPY ServiceLayer/ServiceLayer.csproj ServiceLayer/

# 4️⃣ Restore dependencies
RUN dotnet restore "NZWalksAPI/NZWalksAPI.csproj"

# 5️⃣ Copy toàn bộ source code
COPY . .

# 6️⃣ Build ứng dụng
WORKDIR "/src/NZWalksAPI"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# 7️⃣ Publish ứng dụng
FROM build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish --p:UseAppHost=false

# 8️⃣ Final image để chạy ứng dụng
FROM base AS final
WORKDIR /app

# 🔹 Tạo thư mục Images và HTTPS để tránh lỗi
RUN mkdir -p /app/Images/ && mkdir -p /https/

# **Copy chứng chỉ SSL vào container**
COPY ./localhost.pfx /https/localhost.pfx

# **Thêm biến môi trường để chỉ định chứng chỉ**
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/localhost.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="000000"

# Copy code đã build
COPY --from=publish /app/publish .

# 9️⃣ Khởi động ứng dụng (Giữ container sống nếu lỗi)
CMD ["dotnet", "NZWalksAPI.dll"]
