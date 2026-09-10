using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RazorpayPaymentId",
                table: "Orders",
                newName: "PaymentOrderId");

            migrationBuilder.RenameColumn(
                name: "RazorpayOrderId",
                table: "Orders",
                newName: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentOrderId",
                table: "Orders",
                newName: "RazorpayPaymentId");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Orders",
                newName: "RazorpayOrderId");
        }
    }
}
