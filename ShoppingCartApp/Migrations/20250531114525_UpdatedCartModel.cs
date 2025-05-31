﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingCartApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCartModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Carts",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Carts");
        }
    }
}
