using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.Api.Migrations
{
    public partial class Data3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BudgetCategoryIcons",
                columns: new[] { "BudgetCategoryIconId", "IconKey" },
                values: new object[,]
                {
                    { new Guid("c4b9bb46-e5bb-41ee-84c2-bf3fb4c406e9"), "mdi-car" },
                    { new Guid("31c0bbc0-fa68-4b64-b12e-cb925f6a0f78"), "mdi-babe-face" },
                    { new Guid("315369b7-9184-40d1-8569-58723f2d706b"), "mdi-paw" },
                    { new Guid("08ce6103-f529-4379-a863-89d3c9ef9b63"), "mdi-bandage" },
                    { new Guid("15a5771f-a5ff-4085-9377-60c1413cff43"), "mdi-human" },
                    { new Guid("0cffcd30-855a-4851-8ef9-420c3a0e8612"), "mdi-sofa" },
                    { new Guid("6945dcc7-b5bd-4108-a9f8-18535031ce80"), "mdi-memory" },
                    { new Guid("9471a661-26db-44ed-a462-fc2243ff1b6a"), "mdi-cellphone-android" },
                    { new Guid("2f043abc-3c97-4b00-80d7-bdc3df49f359"), "mdi-speaker" },
                    { new Guid("665e3fec-560d-45b3-9548-e9273334dbf8"), "mdi-sim" },
                    { new Guid("ccdd7d3e-b653-4ca0-8cd4-53b58cd02e66"), "mdi-silverware-fork-knife" },
                    { new Guid("521252a7-443c-4d4f-8f54-2796d8cbd977"), "mdi-food" },
                    { new Guid("caba0179-894e-4aa4-bd54-8793687915b8"), "mdi-gas-station" },
                    { new Guid("b2c5f1bb-92b7-4129-ae8f-e7db4bede237"), "mdi-hospital-building" },
                    { new Guid("8ea3c5a6-322e-4d72-a735-444fe60e767d"), "mdi-shopping" },
                    { new Guid("952917af-58ca-4497-b062-7d4ffb1f7002"), "mdi-glass-cocktail" },
                    { new Guid("cf037724-be64-454d-9d81-48fd7e24936a"), "mdi-filmstrip" },
                    { new Guid("389f16cc-7196-4eb0-aadc-4fa5d5f5a849"), "mdi-bike" },
                    { new Guid("bcc79ab5-1ba4-490b-8f7e-74b7917426d0"), "mdi-fridge" },
                    { new Guid("f506239c-78c8-49e3-bb4f-71e4e262be04"), "mdi-smoking" },
                    { new Guid("1c53a906-2005-42eb-8182-680f51d9af27"), "mdi-bus-articulated-front" },
                    { new Guid("2de31533-bb20-4901-9de9-6460c6b09b01"), "mdi-beach" },
                    { new Guid("6b8623c7-c719-4888-9ca5-4b57b63ab658"), "mdi-cart" },
                    { new Guid("a5d13a05-3fd2-4311-ab10-812fbf594c9a"), "mdi-train-car" },
                    { new Guid("48a0d93d-a6f4-4424-becf-fc23329a2a6a"), "mdi-wallet-travel" },
                    { new Guid("82c812ac-81e8-4af6-ad07-f06c9f21c7bd"), "mdi-wrench" },
                    { new Guid("25abbc59-132e-4eb7-92ca-de876069a867"), "mdi-basket" },
                    { new Guid("d32cdebe-75bd-427f-96a2-40a5cab8d893"), "mdi-gamepad" },
                    { new Guid("b480dd2c-e0e0-4d67-8e9d-a596162445c6"), "mdi-phone" },
                    { new Guid("1a84fe5c-c60a-4007-bfc7-fc4113283aa0"), "mdi-airplane" },
                    { new Guid("2c2796b3-8a8c-482d-b6a9-42ac5a23ba9a"), "mdi-motorbike" },
                    { new Guid("ab3fd2cd-dd91-40e9-81c7-e5ba29d9e410"), "mdi-currency-usd-circle-outline" },
                    { new Guid("8e49667b-6eda-415e-b7d5-65fae76a2d56"), "mdi-gamepad-square" },
                    { new Guid("91709684-a325-4acc-ad0d-715212bc28f4"), "mdi-laptop" },
                    { new Guid("f565696c-d7c1-4173-90ad-1c971e00af95"), "mdi-camera" },
                    { new Guid("9f4067c4-ea7d-4feb-94b3-d0eaabcf53de"), "mdi-city" },
                    { new Guid("66678e2a-6667-44b5-a5d3-4d306d5f1074"), "mdi-fire" },
                    { new Guid("83b0391d-78fa-46f3-a17f-4df666b65338"), "mdi-dumbbell" },
                    { new Guid("a4d70c56-66ac-404c-b137-7d0f71b47b1b"), "mdi-coffee" },
                    { new Guid("80b46725-e15a-4daf-8d41-615e560d162a"), "mdi-dice-5" },
                    { new Guid("f2d7effc-3c7a-4cc9-baa8-a28546bc3482"), "mdi-format-paint" },
                    { new Guid("9e60c8bf-d952-4f62-80f9-2c03a6e6c194"), "mdi-wallet-gift-card" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("08ce6103-f529-4379-a863-89d3c9ef9b63"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("0cffcd30-855a-4851-8ef9-420c3a0e8612"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("15a5771f-a5ff-4085-9377-60c1413cff43"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("1a84fe5c-c60a-4007-bfc7-fc4113283aa0"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("1c53a906-2005-42eb-8182-680f51d9af27"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("25abbc59-132e-4eb7-92ca-de876069a867"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("2c2796b3-8a8c-482d-b6a9-42ac5a23ba9a"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("2de31533-bb20-4901-9de9-6460c6b09b01"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("2f043abc-3c97-4b00-80d7-bdc3df49f359"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("315369b7-9184-40d1-8569-58723f2d706b"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("31c0bbc0-fa68-4b64-b12e-cb925f6a0f78"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("389f16cc-7196-4eb0-aadc-4fa5d5f5a849"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("48a0d93d-a6f4-4424-becf-fc23329a2a6a"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("521252a7-443c-4d4f-8f54-2796d8cbd977"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("665e3fec-560d-45b3-9548-e9273334dbf8"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("66678e2a-6667-44b5-a5d3-4d306d5f1074"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("6945dcc7-b5bd-4108-a9f8-18535031ce80"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("6b8623c7-c719-4888-9ca5-4b57b63ab658"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("80b46725-e15a-4daf-8d41-615e560d162a"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("82c812ac-81e8-4af6-ad07-f06c9f21c7bd"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("83b0391d-78fa-46f3-a17f-4df666b65338"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("8e49667b-6eda-415e-b7d5-65fae76a2d56"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("8ea3c5a6-322e-4d72-a735-444fe60e767d"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("91709684-a325-4acc-ad0d-715212bc28f4"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("9471a661-26db-44ed-a462-fc2243ff1b6a"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("952917af-58ca-4497-b062-7d4ffb1f7002"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("9e60c8bf-d952-4f62-80f9-2c03a6e6c194"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("9f4067c4-ea7d-4feb-94b3-d0eaabcf53de"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("a4d70c56-66ac-404c-b137-7d0f71b47b1b"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("a5d13a05-3fd2-4311-ab10-812fbf594c9a"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("ab3fd2cd-dd91-40e9-81c7-e5ba29d9e410"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("b2c5f1bb-92b7-4129-ae8f-e7db4bede237"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("b480dd2c-e0e0-4d67-8e9d-a596162445c6"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("bcc79ab5-1ba4-490b-8f7e-74b7917426d0"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("c4b9bb46-e5bb-41ee-84c2-bf3fb4c406e9"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("caba0179-894e-4aa4-bd54-8793687915b8"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("ccdd7d3e-b653-4ca0-8cd4-53b58cd02e66"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("cf037724-be64-454d-9d81-48fd7e24936a"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("d32cdebe-75bd-427f-96a2-40a5cab8d893"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("f2d7effc-3c7a-4cc9-baa8-a28546bc3482"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("f506239c-78c8-49e3-bb4f-71e4e262be04"));

            migrationBuilder.DeleteData(
                table: "BudgetCategoryIcons",
                keyColumn: "BudgetCategoryIconId",
                keyValue: new Guid("f565696c-d7c1-4173-90ad-1c971e00af95"));
        }
    }
}
