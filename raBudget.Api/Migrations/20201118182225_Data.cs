using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.Api.Migrations
{
    public partial class Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Currency",
                table: "Currency");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "Currencies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "CurrencyCode");

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "CurrencyCode", "Code", "EnglishName", "NativeName", "Symbol" },
                values: new object[,]
                {
                    { 8, "ALL", "Albanian Lek", "Leku shqiptar", "Lekë" },
                    { 760, "SYP", "Syrian Pound", "ليرة سورية", "ل.س.‏" },
                    { 756, "CHF", "Swiss Franc", "Schweizer Franken", "CHF" },
                    { 752, "SEK", "Swedish Krona", "ruoŧŧa kruvdno", "kr" },
                    { 710, "ZAR", "South African Rand", "Suid-Afrikaanse rand", "R" },
                    { 704, "VND", "Vietnamese Dong", "Đồng Việt Nam", "₫" },
                    { 702, "SGD", "Singapore Dollar", "Singapore Dollar", "$" },
                    { 682, "SAR", "Saudi Riyal", "ريال سعودي", "ر.س.‏" },
                    { 646, "RWF", "Rwandan Franc", "RWF", "RF" },
                    { 643, "RUB", "Russian Ruble", "RUB", "RUB" },
                    { 764, "THB", "Thai Baht", "บาท", "฿" },
                    { 634, "QAR", "Qatari Rial", "ريال قطري", "ر.ق.‏" },
                    { 604, "PEN", "Peruvian Sol", "sol peruano", "S/" },
                    { 600, "PYG", "Paraguayan Guarani", "guaraní paraguayo", "Gs." },
                    { 590, "PAB", "Panamanian Balboa", "balboa panameño", "B/." },
                    { 586, "PKR", "Pakistani Rupee", "روپئیہ", "ر" },
                    { 578, "NOK", "Norwegian Krone", "norske kroner", "kr" },
                    { 558, "NIO", "Nicaraguan Córdoba", "córdoba nicaragüense", "C$" },
                    { 554, "NZD", "New Zealand Dollar", "New Zealand Dollar", "$" },
                    { 524, "NPR", "Nepalese Rupee", "नेपाली रूपैयाँ", "नेरू" },
                    { 512, "OMR", "Omani Rial", "ريال عماني", "ر.ع.‏" },
                    { 608, "PHP", "Philippine Piso", "Philippine Piso", "₱" },
                    { 504, "MAD", "Moroccan Dirham", "درهم مغربي", "د.م.‏" },
                    { 780, "TTD", "Trinidad & Tobago Dollar", "Trinidad & Tobago Dollar", "$" },
                    { 788, "TND", "Tunisian Dinar", "دينار تونسي", "د.ت.‏" },
                    { 981, "GEL", "Georgian Lari", "ქართული ლარი", "₾" },
                    { 980, "UAH", "Ukrainian Hryvnia", "українська гривня", "₴" },
                    { 978, "EUR", "Euro", "euro", "€" },
                    { 977, "BAM", "Bosnia-Herzegovina Convertible Mark", "Конвертибилна марка", "КМ" },
                    { 975, "BGN", "Bulgarian Lev", "Български лев", "лв." },
                    { 972, "TJS", "Tajikistani Somoni", "Сомонӣ", "сом." },
                    { 971, "AFN", "Afghan Afghani", "افغانی افغانستان", "؋" },
                    { 952, "XOF", "West African CFA Franc", "Mbuuɗu Seefaa BCEAO", "CFA" },
                    { 949, "TRY", "Turkish Lira", "Türk Lirası", "₺" },
                    { 784, "AED", "United Arab Emirates Dirham", "درهم إماراتي", "د.إ.‏" },
                    { 946, "RON", "Romanian Leu", "leu românesc", "RON" },
                    { 941, "RSD", "Serbian Dinar", "Српски динар", "RSD" },
                    { 901, "TWD", "New Taiwan Dollar", "新台幣", "$" },
                    { 886, "YER", "Yemeni Rial", "ريال يمني", "ر.ي.‏" },
                    { 860, "UZS", "Uzbekistani Som", "Ўзбекистон сўм", "сўм" },
                    { 858, "UYU", "Uruguayan Peso", "peso uruguayo", "$" },
                    { 840, "USD", "US Dollar", "US ᎠᏕᎳ", "$" },
                    { 826, "GBP", "British Pound", "Punt Prydain", "£" },
                    { 818, "EGP", "Egyptian Pound", "جنيه مصري", "ج.م.‏" },
                    { 807, "MKD", "Macedonian Denar", "Македонски денар", "ден" },
                    { 944, "AZN", "Azerbaijani Manat", "AZN", "₼" },
                    { 985, "PLN", "Polish Zloty", "złoty polski", "zł" },
                    { 496, "MNT", "Mongolian Tugrik", "төгрөг", "₮" },
                    { 462, "MVR", "Maldivian Rufiyaa", "MVR", "ރ." },
                    { 214, "DOP", "Dominican Peso", "peso dominicano", "RD$" },
                    { 208, "DKK", "Danish Krone", "dansk krone", "kr." },
                    { 203, "CZK", "Czech Koruna", "česká koruna", "Kč" },
                    { 191, "HRK", "Croatian Kuna", "hrvatska kuna", "HRK" },
                    { 188, "CRC", "Costa Rican Colón", "colón costarricense", "₡" },
                    { 170, "COP", "Colombian Peso", "peso colombiano", "$" },
                    { 156, "CNY", "Chinese Yuan", "ཡུ་ཨན་", "¥" },
                    { 152, "CLP", "Chilean Peso", "CLP", "CLP" },
                    { 144, "LKR", "Sri Lankan Rupee", "ශ්‍රී ලංකා රුපියල", "රු." },
                    { 230, "ETB", "Ethiopian Birr", "ETB", "Br" },
                    { 124, "CAD", "Canadian Dollar", "Canadian Dollar", "$" },
                    { 96, "BND", "Brunei Dollar", "Dolar Brunei", "$" },
                    { 84, "BZD", "Belize Dollar", "Belize Dollar", "$" },
                    { 68, "BOB", "Bolivian Boliviano", "boliviano", "Bs" },
                    { 51, "AMD", "Armenian Dram", "հայկական դրամ", "֏" },
                    { 50, "BDT", "Bangladeshi Taka", "বাংলাদেশী টাকা", "৳" },
                    { 48, "BHD", "Bahraini Dinar", "دينار بحريني", "د.ب.‏" },
                    { 36, "AUD", "Australian Dollar", "Australian Dollar", "$" },
                    { 32, "ARS", "Argentine Peso", "peso argentino", "$" },
                    { 12, "DZD", "Algerian Dinar", "دينار جزائري", "د.ج.‏" },
                    { 116, "KHR", "Cambodian Riel", "រៀល​កម្ពុជា", "៛" },
                    { 484, "MXN", "Mexican Peso", "peso mexicano", "$" },
                    { 320, "GTQ", "Guatemalan Quetzal", "quetzal", "Q" },
                    { 344, "HKD", "Hong Kong Dollar", "Hong Kong Dollar", "HK$" },
                    { 458, "MYR", "Malaysian Ringgit", "Malaysian Ringgit", "RM" },
                    { 446, "MOP", "Macanese Pataca", "澳門元", "MOP$" },
                    { 434, "LYD", "Libyan Dinar", "دينار ليبي", "د.ل.‏" },
                    { 422, "LBP", "Lebanese Pound", "جنيه لبناني", "ل.ل.‏" },
                    { 418, "LAK", "Laotian Kip", "ລາວ ກີບ", "₭" },
                    { 417, "KGS", "Kyrgystani Som", "Кыргызстан сому", "сом" },
                    { 414, "KWD", "Kuwaiti Dinar", "دينار كويتي", "د.ك.‏" },
                    { 410, "KRW", "South Korean Won", "대한민국 원", "₩" },
                    { 404, "KES", "Kenyan Shilling", "Shilingi ya Kenya", "Ksh" },
                    { 340, "HNL", "Honduran Lempira", "lempira hondureño", "L" },
                    { 400, "JOD", "Jordanian Dinar", "دينار أردني", "د.أ.‏" },
                    { 392, "JPY", "Japanese Yen", "日本円", "￥" },
                    { 388, "JMD", "Jamaican Dollar", "Jamaican Dollar", "$" },
                    { 376, "ILS", "Israeli New Shekel", "שקל חדש", "₪" },
                    { 368, "IQD", "Iraqi Dinar", "دينار عراقي", "د.ع.‏" },
                    { 364, "IRR", "Iranian Rial", "ریال ایران", "ریال" },
                    { 360, "IDR", "Indonesian Rupiah", "Rupiah Indonesia", "Rp" },
                    { 356, "INR", "Indian Rupee", "ভাৰতীয় ৰুপী", "₹" },
                    { 352, "ISK", "Icelandic Króna", "íslensk króna", "ISK" },
                    { 348, "HUF", "Hungarian Forint", "magyar forint", "Ft" },
                    { 398, "KZT", "Kazakhstani Tenge", "Қазақстан теңгесі", "₸" },
                    { 986, "BRL", "Brazilian Real", "Real brasileiro", "R$" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 188);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 214);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 230);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 320);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 340);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 344);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 348);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 352);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 356);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 360);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 364);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 368);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 376);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 388);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 392);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 398);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 400);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 404);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 410);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 414);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 417);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 418);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 422);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 434);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 446);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 458);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 462);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 484);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 496);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 504);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 512);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 524);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 554);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 558);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 578);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 586);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 590);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 600);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 604);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 608);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 634);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 643);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 646);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 682);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 702);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 704);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 710);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 752);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 756);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 760);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 764);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 780);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 784);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 788);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 807);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 818);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 826);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 840);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 858);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 860);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 886);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 901);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 941);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 944);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 946);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 949);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 952);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 971);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 972);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 975);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 977);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 978);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 980);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 981);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 985);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: 986);

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currency");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currency",
                table: "Currency",
                column: "CurrencyCode");
        }
    }
}
