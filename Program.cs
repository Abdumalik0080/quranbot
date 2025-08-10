using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var botToken = "8410113085:AAFTqGNA3e6rgc7BZt04rF-Fhq-m0LXMpaw";
var botClient = new TelegramBotClient(botToken);
var httpClient = new HttpClient();
var random = new Random();

var userState = new Dictionary<long, string>();
var userSura = new Dictionary<long, int>();

// Kuniga 1 marta tasodifiy oyat cheklovi uchun
var userLastRandomRequest = new Dictionary<long, DateTime>();

var suraOyats = new Dictionary<int, int>
{
    { 1, 7 }, { 2, 286 }, { 3, 200 }, { 4, 176 }, { 5, 120 }, { 6, 165 },
    { 7, 206 }, { 8, 75 }, { 9, 129 }, { 10, 109 }, { 11, 123 }, { 12, 111 },
    { 13, 43 }, { 14, 52 }, { 15, 99 }, { 16, 128 }, { 17, 111 }, { 18, 110 },
    { 19, 98 }, { 20, 135 }, { 21, 112 }, { 22, 78 }, { 23, 118 }, { 24, 64 },
    { 25, 77 }, { 26, 227 }, { 27, 93 }, { 28, 88 }, { 29, 69 }, { 30, 60 },
    { 31, 34 }, { 32, 30 }, { 33, 73 }, { 34, 54 }, { 35, 45 }, { 36, 83 },
    { 37, 182 }, { 38, 88 }, { 39, 75 }, { 40, 85 }, { 41, 54 }, { 42, 53 },
    { 43, 89 }, { 44, 59 }, { 45, 37 }, { 46, 35 }, { 47, 38 }, { 48, 29 },
    { 49, 18 }, { 50, 45 }, { 51, 60 }, { 52, 49 }, { 53, 62 }, { 54, 55 },
    { 55, 78 }, { 56, 96 }, { 57, 29 }, { 58, 22 }, { 59, 24 }, { 60, 13 },
    { 61, 14 }, { 62, 11 }, { 63, 11 }, { 64, 18 }, { 65, 12 }, { 66, 12 },
    { 67, 30 }, { 68, 52 }, { 69, 52 }, { 70, 44 }, { 71, 28 }, { 72, 28 },
    { 73, 20 }, { 74, 56 }, { 75, 40 }, { 76, 31 }, { 77, 50 }, { 78, 40 },
    { 79, 46 }, { 80, 42 }, { 81, 29 }, { 82, 19 }, { 83, 36 }, { 84, 25 },
    { 85, 22 }, { 86, 17 }, { 87, 19 }, { 88, 26 }, { 89, 30 }, { 90, 20 },
    { 91, 15 }, { 92, 21 }, { 93, 11 }, { 94, 8 }, { 95, 8 }, { 96, 19 },
    { 97, 5 }, { 98, 8 }, { 99, 8 }, { 100, 11 }, { 101, 11 }, { 102, 8 },
    { 103, 3 }, { 104, 9 }, { 105, 5 }, { 106, 4 }, { 107, 7 }, { 108, 3 },
    { 109, 6 }, { 110, 3 }, { 111, 5 }, { 112, 4 }, { 113, 5 }, { 114, 6 }
};

var suraNamesUz = new Dictionary<int, string>
{
    {1, "Al-Fotiha"},
    {2, "Al-Baqara"},
    {3, "Oli Imron"},
    {4, "An-Niso"},
    {5, "Al-Moida"},
    {6, "Al-An'am"},
    {7, "Al-A'rof"},
    {8, "Al-Anfal"},
    {9, "At-Tavba"},
    {10, "Yunus"},
    {11, "Hud"},
    {12, "Yusuf"},
    {13, "Ar-Ro'd"},
    {14, "Ibrohim"},
    {15, "Al-Hijr"},
    {16, "An-Nahl"},
    {17, "Al-Isro"},
    {18, "Al-Kahf"},
    {19, "Maryam"},
    {20, "Taha"},
    {21, "Al-Anbiyo"},
    {22, "Al-Haj"},
    {23, "Al-Mu'minun"},
    {24, "An-Nur"},
    {25, "Al-Furqon"},
    {26, "Ash-Shu'aro"},
    {27, "An-Naml"},
    {28, "Al-Qasas"},
    {29, "Al-Ankabut"},
    {30, "Ar-Rum"},
    {31, "Luqmon"},
    {32, "As-Sajda"},
    {33, "Al-Ahzab"},
    {34, "Saba"},
    {35, "Fatir"},
    {36, "Ya-Sin"},
    {37, "As-Saffat"},
    {38, "Sad"},
    {39, "Az-Zumar"},
    {40, "Gafir"},
    {41, "Fussilat"},
    {42, "Ash-Shura"},
    {43, "Az-Zukhruf"},
    {44, "Ad-Duxan"},
    {45, "Al-Cadiyah"},
    {46, "Al-Ahqaf"},
    {47, "Muhammad"},
    {48, "Al-Fath"},
    {49, "Al-Hujurot"},
    {50, "Qof"},
    {51, "Ad-Dhariyat"},
    {52, "At-Tur"},
    {53, "An-Najm"},
    {54, "Al-Qamar"},
    {55, "Ar-Rahman"},
    {56, "Al-Vaqi'a"},
    {57, "Al-Hadid"},
    {58, "Al-Mujodala"},
    {59, "Al-Hashr"},
    {60, "Al-Mumtahina"},
    {61, "As-Saff"},
    {62, "Al-Cunfuz"},
    {63, "Al-Munofiqun"},
    {64, "At-Taghabun"},
    {65, "At-Tolq"},
    {66, "At-Tahrim"},
    {67, "Al-Molok"},
    {68, "Al-Qalam"},
    {69, "Al-Haqqa"},
    {70, "Al-Maarij"},
    {71, "Nuh"},
    {72, "Al-Jinn"},
    {73, "Al-Muzzammil"},
    {74, "Al-Muddassir"},
    {75, "Al-Qiyoma"},
    {76, "Al-Insan"},
    {77, "Al-Mursalat"},
    {78, "An-Naba"},
    {79, "An-Nazi'at"},
    {80, "Abasa"},
    {81, "At-Takwir"},
    {82, "Al-Infitar"},
    {83, "Al-Mutaffifin"},
    {84, "Al-Inshiqoq"},
    {85, "Al-Buroj"},
    {86, "At-Tariq"},
    {87, "Al-Ala"},
    {88, "Al-Gashiya"},
    {89, "Al-Fajr"},
    {90, "Al-Balad"},
    {91, "Ash-Shams"},
    {92, "Al-Layl"},
    {93, "Ad-Duha"},
    {94, "Ash-Sharh"},
    {95, "At-Tin"},
    {96, "Al-Alaq"},
    {97, "Al-Qadr"},
    {98, "Al-Bayyina"},
    {99, "Az-Zalzala"},
    {100, "Al-Adiyat"},
    {101, "Al-Qori'a"},
    {102, "At-Takathur"},
    {103, "Al-Asr"},
    {104, "Al-Humaza"},
    {105, "Al-Fil"},
    {106, "Quraysh"},
    {107, "Al-Maun"},
    {108, "Al-Kavsar"},
    {109, "Al-Kafirun"},
    {110, "An-Nosr"},
    {111, "Al-Masad"},
    {112, "Al-Ikhlos"},
    {113, "Al-Falaq"},
    {114, "An-Nos"}
};

botClient.StartReceiving(async (bot, update, ct) =>
{
    if (update.Message is { Text: "/start" } startMsg)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Tasodifiy oyat tanlash" },
            new KeyboardButton[] { "Oyatni izlab topish" }
        })
        { ResizeKeyboard = true };

        await bot.SendMessage(
            chatId: startMsg.Chat.Id,
            text: $"Assalomu alaykum, {startMsg.From.FirstName}! 😊\nMen sizga Qur'on oyatlarini olib bera olaman.\nKerakli tugmani bosing.",
            replyMarkup: keyboard
        );
    }
    else if (update.Message is { Text: "Tasodifiy oyat tanlash" } randomMsg)
    {
        var chatId = randomMsg.Chat.Id;

        if (userLastRandomRequest.TryGetValue(chatId, out DateTime lastRequest))
        {
            if ((DateTime.UtcNow - lastRequest).TotalHours < 24)
            {
                await bot.SendMessage(
                    chatId: chatId,
                    text: "⚠️ Siz bugun allaqachon tasodifiy oyat tanladingiz. Iltimos, keyinroq urinib ko'ring."
                );
                return;
            }
        }

        userLastRandomRequest[chatId] = DateTime.UtcNow;

        int sura = random.Next(1, 115);
        int oyat = random.Next(1, suraOyats[sura] + 1);

        var arabRes = await httpClient.GetFromJsonAsync<QuranResponse>(
            $"https://api.alquran.cloud/v1/ayah/{sura}:{oyat}/ar");

        string suraName = suraNamesUz.ContainsKey(sura) ? suraNamesUz[sura] : "Noma'lum sura";

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("O'zbekcha talqini", $"{sura}:{oyat}")
        });

        await bot.SendMessage(
            chatId: chatId,
            text: $"📖 *Sura:* {sura} — {suraName}\n*Oyat:* {oyat}\n\n{arabRes?.Data?.Text}",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: inlineKeyboard
        );
    }
    else if (update.Message is { Text: "Oyatni izlab topish" } searchMsg)
    {
        userState[searchMsg.Chat.Id] = "sura";
        await bot.SendMessage(
            chatId: searchMsg.Chat.Id,
            text: "🔢 Sura raqamini kiriting:"
        );
    }
    else if (update.Message is { Text: var input } userMsg)
    {
        if (userState.ContainsKey(userMsg.Chat.Id))
        {
            if (userState[userMsg.Chat.Id] == "sura")
            {
                if (int.TryParse(input, out int suraNum) && suraOyats.ContainsKey(suraNum))
                {
                    userSura[userMsg.Chat.Id] = suraNum;
                    userState[userMsg.Chat.Id] = "oyat";
                    await bot.SendMessage(
                        chatId: userMsg.Chat.Id,
                        text: "🔢 Oyat raqamini kiriting:"
                    );
                }
                else
                {
                    await bot.SendMessage(
                        chatId: userMsg.Chat.Id,
                        text: "❌ Noto‘g‘ri sura raqami! 1–114 oralig‘ida kiriting."
                    );
                }
            }
            else if (userState[userMsg.Chat.Id] == "oyat")
            {
                int suraNum = userSura[userMsg.Chat.Id];
                if (int.TryParse(input, out int oyatNum) && oyatNum >= 1 && oyatNum <= suraOyats[suraNum])
                {
                    var arabRes = await httpClient.GetFromJsonAsync<QuranResponse>(
                        $"https://api.alquran.cloud/v1/ayah/{suraNum}:{oyatNum}/ar");

                    string suraName = suraNamesUz.ContainsKey(suraNum) ? suraNamesUz[suraNum] : "Noma'lum sura";

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("O'zbekcha talqini", $"{suraNum}:{oyatNum}")
                    });

                    await bot.SendMessage(
                        chatId: userMsg.Chat.Id,
                        text: $"📖 *Sura:* {suraNum} — {suraName}\n*Oyat:* {oyatNum}\n\n{arabRes?.Data?.Text}",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: inlineKeyboard
                    );

                    userState.Remove(userMsg.Chat.Id);
                    userSura.Remove(userMsg.Chat.Id);
                }
                else
                {
                    await bot.SendMessage(
                        chatId: userMsg.Chat.Id,
                        text: $"❌ Noto‘g‘ri oyat raqami! 1–{suraOyats[suraNum]} oralig‘ida kiriting."
                    );
                }
            }
        }
    }
    else if (update.CallbackQuery is { Data: var data } callback)
    {
        var parts = data.Split(':');
        int sura = int.Parse(parts[0]);
        int oyat = int.Parse(parts[1]);

        var uzRes = await httpClient.GetFromJsonAsync<QuranResponse>(
            $"https://api.alquran.cloud/v1/ayah/{sura}:{oyat}/uz.sodik");

        string suraName = suraNamesUz.ContainsKey(sura) ? suraNamesUz[sura] : "Noma'lum sura";

        string message = $"📖 *Sura:* {sura} — {suraName}\n🕋 *Oyat:* {oyat}\n\n🇺🇿 {uzRes?.Data?.Text}";

        await bot.SendMessage(
            chatId: callback.Message.Chat.Id,
            text: message,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );
    }
}, (_, _, _) => Task.CompletedTask);

Console.WriteLine("Bot ishga tushdi...");
Console.ReadLine();

public class QuranResponse
{
    public QuranData? Data { get; set; }
}

public class QuranData
{
    public string? Text { get; set; }
}
