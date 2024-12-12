using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FiscalFlowAdmin.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authentication_user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    password = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_superuser = table.Column<bool>(type: "boolean", nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    first_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    last_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_confirmed_email = table.Column<bool>(type: "boolean", nullable: true),
                    is_staff = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authentication_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_currency",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    value = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_currency", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_transactioncategory",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_income = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_transactioncategory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reminders_notification",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    send_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminders_notification", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "authentication_profile",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    image = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authentication_profile", x => x.id);
                    table.ForeignKey(
                        name: "FK_authentication_profile_authentication_user_user_id",
                        column: x => x.user_id,
                        principalTable: "authentication_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "authentication_devicetoken",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    profile_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authentication_devicetoken", x => x.id);
                    table.ForeignKey(
                        name: "FK_authentication_devicetoken_authentication_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "authentication_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_bill",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_main = table.Column<bool>(type: "boolean", nullable: false),
                    balance = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                    currency_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_bill", x => x.id);
                    table.ForeignKey(
                        name: "FK_finances_app_bill_authentication_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "authentication_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_finances_app_bill_finances_app_currency_currency_id",
                        column: x => x.currency_id,
                        principalTable: "finances_app_currency",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_dailyreport",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    total_income = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    total_expense = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    profile_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_dailyreport", x => x.id);
                    table.ForeignKey(
                        name: "FK_finances_app_dailyreport_authentication_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "authentication_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reminders_notification_profiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notification_id = table.Column<long>(type: "bigint", nullable: false),
                    profile_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminders_notification_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_reminders_notification_profiles_authentication_profile_prof~",
                        column: x => x.profile_id,
                        principalTable: "authentication_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reminders_notification_profiles_reminders_notification_noti~",
                        column: x => x.notification_id,
                        principalTable: "reminders_notification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_credit",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    term = table.Column<int>(type: "integer", nullable: false),
                    interest_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    paid_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    remaining_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    bill_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_credit", x => x.id);
                    table.ForeignKey(
                        name: "FK_finances_app_credit_finances_app_bill_bill_id",
                        column: x => x.bill_id,
                        principalTable: "finances_app_bill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_debt",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    debtor = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    bill_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_debt", x => x.id);
                    table.ForeignKey(
                        name: "FK_finances_app_debt_finances_app_bill_bill_id",
                        column: x => x.bill_id,
                        principalTable: "finances_app_bill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_monthlyexpense",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    next_payment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    period = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    bill_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_monthlyexpense", x => x.id);
                    table.ForeignKey(
                        name: "FK_finances_app_monthlyexpense_finances_app_bill_bill_id",
                        column: x => x.bill_id,
                        principalTable: "finances_app_bill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_transaction",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    bill_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_finances_app_transaction_finances_app_bill_bill_id",
                        column: x => x.bill_id,
                        principalTable: "finances_app_bill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_finances_app_transaction_finances_app_transactioncategory_c~",
                        column: x => x.category_id,
                        principalTable: "finances_app_transactioncategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finances_app_dailycategoryexpense",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    expense_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    daily_report_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_finances_app_dailycategoryexpense", x => x.id);
                    table.ForeignKey(
                        name: "FK_finances_app_dailycategoryexpense_finances_app_dailyreport_~",
                        column: x => x.daily_report_id,
                        principalTable: "finances_app_dailyreport",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_finances_app_dailycategoryexpense_finances_app_transactionc~",
                        column: x => x.category_id,
                        principalTable: "finances_app_transactioncategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "authentication_devicetoken_profile_id_0fbaea76",
                table: "authentication_devicetoken",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "authentication_profile_user_id_key",
                table: "authentication_profile",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "authentication_user_email_key",
                table: "authentication_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "finances_ap_currenc_17b09c_idx",
                table: "finances_app_bill",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_profile_a16c59_idx",
                table: "finances_app_bill",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_bill_currency_id_88fc4021",
                table: "finances_app_bill",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_bill_profile_id_f1838735",
                table: "finances_app_bill",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_bill_id_444031_idx",
                table: "finances_app_credit",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_credit_bill_id_d22a52cd",
                table: "finances_app_credit",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_categor_1843f5_idx",
                table: "finances_app_dailycategoryexpense",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_daily_r_36cd3e_idx",
                table: "finances_app_dailycategoryexpense",
                column: "daily_report_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_dailycategoryexpense_category_id_c1b2af05",
                table: "finances_app_dailycategoryexpense",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_dailycategoryexpense_daily_report_id_ee07bcdc",
                table: "finances_app_dailycategoryexpense",
                column: "daily_report_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_profile_af7197_idx",
                table: "finances_app_dailyreport",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_dailyreport_profile_id_cdac02f8",
                table: "finances_app_dailyreport",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_bill_id_83d405_idx",
                table: "finances_app_debt",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_debt_bill_id_c93ab690",
                table: "finances_app_debt",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_bill_id_746c78_idx",
                table: "finances_app_monthlyexpense",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_monthlyexpense_bill_id_37680f98",
                table: "finances_app_monthlyexpense",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_bill_id_f7f665_idx",
                table: "finances_app_transaction",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_ap_categor_75baae_idx",
                table: "finances_app_transaction",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_transaction_bill_id_9a0a9218",
                table: "finances_app_transaction",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "finances_app_transaction_category_id_113b376f",
                table: "finances_app_transaction",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "reminders_notification_p_notification_id_profile__a95e6e84_uniq",
                table: "reminders_notification_profiles",
                columns: new[] { "notification_id", "profile_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "reminders_notification_profiles_notification_id_3654fc2b",
                table: "reminders_notification_profiles",
                column: "notification_id");

            migrationBuilder.CreateIndex(
                name: "reminders_notification_profiles_profile_id_b218a596",
                table: "reminders_notification_profiles",
                column: "profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authentication_devicetoken");

            migrationBuilder.DropTable(
                name: "finances_app_credit");

            migrationBuilder.DropTable(
                name: "finances_app_dailycategoryexpense");

            migrationBuilder.DropTable(
                name: "finances_app_debt");

            migrationBuilder.DropTable(
                name: "finances_app_monthlyexpense");

            migrationBuilder.DropTable(
                name: "finances_app_transaction");

            migrationBuilder.DropTable(
                name: "reminders_notification_profiles");

            migrationBuilder.DropTable(
                name: "finances_app_dailyreport");

            migrationBuilder.DropTable(
                name: "finances_app_bill");

            migrationBuilder.DropTable(
                name: "finances_app_transactioncategory");

            migrationBuilder.DropTable(
                name: "reminders_notification");

            migrationBuilder.DropTable(
                name: "authentication_profile");

            migrationBuilder.DropTable(
                name: "finances_app_currency");

            migrationBuilder.DropTable(
                name: "authentication_user");
        }
    }
}
