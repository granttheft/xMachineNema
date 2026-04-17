using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class Quality_CoreSkeleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "quality");

            migrationBuilder.CreateTable(
                name: "quality_checks",
                schema: "quality",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lot_batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    check_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    check_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    check_status = table.Column<int>(type: "integer", nullable: false),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quality_checks", x => x.id);
                    table.ForeignKey(
                        name: "FK_quality_checks_lot_batches_lot_batch_id",
                        column: x => x.lot_batch_id,
                        principalSchema: "mes",
                        principalTable: "lot_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quality_checks_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quality_checks_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalSchema: "mes",
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quality_checks_user_accounts_approved_by",
                        column: x => x.approved_by,
                        principalSchema: "auth",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "nonconformances",
                schema: "quality",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quality_check_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nc_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    nc_status = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nonconformances", x => x.id);
                    table.ForeignKey(
                        name: "FK_nonconformances_quality_checks_quality_check_id",
                        column: x => x.quality_check_id,
                        principalSchema: "quality",
                        principalTable: "quality_checks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quality_measurements",
                schema: "quality",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quality_check_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parameter_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    measured_value = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    target_value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    min_value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    max_value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    result = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quality_measurements", x => x.id);
                    table.ForeignKey(
                        name: "FK_quality_measurements_quality_checks_quality_check_id",
                        column: x => x.quality_check_id,
                        principalSchema: "quality",
                        principalTable: "quality_checks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quality_dispositions",
                schema: "quality",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nonconformance_id = table.Column<Guid>(type: "uuid", nullable: false),
                    disposition_type = table.Column<int>(type: "integer", nullable: false),
                    decision_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    decided_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quality_dispositions", x => x.id);
                    table.ForeignKey(
                        name: "FK_quality_dispositions_nonconformances_nonconformance_id",
                        column: x => x.nonconformance_id,
                        principalSchema: "quality",
                        principalTable: "nonconformances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quality_dispositions_user_accounts_decided_by",
                        column: x => x.decided_by,
                        principalSchema: "auth",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nonconformances_quality_check_id",
                schema: "quality",
                table: "nonconformances",
                column: "quality_check_id");

            migrationBuilder.CreateIndex(
                name: "IX_quality_checks_approved_by",
                schema: "quality",
                table: "quality_checks",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_quality_checks_lot_batch_id",
                schema: "quality",
                table: "quality_checks",
                column: "lot_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_quality_checks_machine_id",
                schema: "quality",
                table: "quality_checks",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_quality_checks_production_order_id",
                schema: "quality",
                table: "quality_checks",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_quality_checks_tenant_id_check_time",
                schema: "quality",
                table: "quality_checks",
                columns: new[] { "tenant_id", "check_time" });

            migrationBuilder.CreateIndex(
                name: "IX_quality_dispositions_decided_by",
                schema: "quality",
                table: "quality_dispositions",
                column: "decided_by");

            migrationBuilder.CreateIndex(
                name: "IX_quality_dispositions_nonconformance_id",
                schema: "quality",
                table: "quality_dispositions",
                column: "nonconformance_id");

            migrationBuilder.CreateIndex(
                name: "IX_quality_measurements_quality_check_id",
                schema: "quality",
                table: "quality_measurements",
                column: "quality_check_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quality_dispositions",
                schema: "quality");

            migrationBuilder.DropTable(
                name: "quality_measurements",
                schema: "quality");

            migrationBuilder.DropTable(
                name: "nonconformances",
                schema: "quality");

            migrationBuilder.DropTable(
                name: "quality_checks",
                schema: "quality");
        }
    }
}
