# 1. Build bosqichi: SDK rasmi yordamida ilovani quramiz
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Manba kodni konteynerga nusxalash
COPY . .

# Kutubxonalarni tiklash va ilovani release rejimida publish qilish (loyiha nomi aniq ko‘rsatilgan)
RUN dotnet restore Quranbot.csproj
RUN dotnet publish Quranbot.csproj -c Release -o out --no-restore

# 2. Runtime bosqichi: faqat ishga tushirish uchun yengil rasm
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Build natijasini nusxalash
COPY --from=build /app/out .

# Botni ishga tushirish
ENTRYPOINT ["dotnet", "Quranbot.dll"]
