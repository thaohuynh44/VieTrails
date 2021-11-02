﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VieTrails_API.Migrations
{
    public partial class AddPictureToNationalParkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "NationalParks",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "NationalParks");
        }
    }
}
