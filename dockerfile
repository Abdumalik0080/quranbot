# Build bosqichi: SDK bilan ilovani quramiz
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Manba kodni konteynerga nusxalash
COPY . .

# Kutubxonalarni tiklash va ilovani release rejimida publish qilish
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Runtime bosqichi: faqat ishga tushirish uchun yengil rasm
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Build natijasini nusxalash
COPY --from=build /app/out .

# Botni ishga tushirish
ENTRYPOINT ["dotnet", "Quranbot.dll"]
