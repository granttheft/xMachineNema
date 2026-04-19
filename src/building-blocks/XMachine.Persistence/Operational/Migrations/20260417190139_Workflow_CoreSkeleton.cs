using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class Workflow_CoreSkeleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "workflow");

            migrationBuilder.CreateTable(
                name: "workflow_definitions",
                schema: "workflow",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workflow_instances",
                schema: "workflow",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_state = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_instances", x => x.id);
                    table.ForeignKey(
                        name: "FK_workflow_instances_workflow_definitions_workflow_definition~",
                        column: x => x.workflow_definition_id,
                        principalSchema: "workflow",
                        principalTable: "workflow_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workflow_steps",
                schema: "workflow",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence_no = table.Column<int>(type: "integer", nullable: false),
                    role_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    approval_mode = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_steps", x => x.id);
                    table.ForeignKey(
                        name: "FK_workflow_steps_workflow_definitions_workflow_definition_id",
                        column: x => x.workflow_definition_id,
                        principalSchema: "workflow",
                        principalTable: "workflow_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_actions",
                schema: "workflow",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    action_by = table.Column<Guid>(type: "uuid", nullable: true),
                    action_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_actions", x => x.id);
                    table.ForeignKey(
                        name: "FK_workflow_actions_user_accounts_action_by",
                        column: x => x.action_by,
                        principalSchema: "auth",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_workflow_actions_workflow_instances_workflow_instance_id",
                        column: x => x.workflow_instance_id,
                        principalSchema: "workflow",
                        principalTable: "workflow_instances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workflow_actions_workflow_steps_workflow_step_id",
                        column: x => x.workflow_step_id,
                        principalSchema: "workflow",
                        principalTable: "workflow_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workflow_actions_action_by",
                schema: "workflow",
                table: "workflow_actions",
                column: "action_by");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_actions_tenant_id_workflow_instance_id_action_time",
                schema: "workflow",
                table: "workflow_actions",
                columns: new[] { "tenant_id", "workflow_instance_id", "action_time" });

            migrationBuilder.CreateIndex(
                name: "IX_workflow_actions_workflow_instance_id",
                schema: "workflow",
                table: "workflow_actions",
                column: "workflow_instance_id");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_actions_workflow_step_id",
                schema: "workflow",
                table: "workflow_actions",
                column: "workflow_step_id");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_definitions_tenant_id_workflow_type",
                schema: "workflow",
                table: "workflow_definitions",
                columns: new[] { "tenant_id", "workflow_type" });

            migrationBuilder.CreateIndex(
                name: "IX_workflow_instances_tenant_id_reference_type_reference_id",
                schema: "workflow",
                table: "workflow_instances",
                columns: new[] { "tenant_id", "reference_type", "reference_id" });

            migrationBuilder.CreateIndex(
                name: "IX_workflow_instances_workflow_definition_id",
                schema: "workflow",
                table: "workflow_instances",
                column: "workflow_definition_id");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_steps_tenant_id_workflow_definition_id_sequence_no",
                schema: "workflow",
                table: "workflow_steps",
                columns: new[] { "tenant_id", "workflow_definition_id", "sequence_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workflow_steps_workflow_definition_id",
                schema: "workflow",
                table: "workflow_steps",
                column: "workflow_definition_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workflow_actions",
                schema: "workflow");

            migrationBuilder.DropTable(
                name: "workflow_instances",
                schema: "workflow");

            migrationBuilder.DropTable(
                name: "workflow_steps",
                schema: "workflow");

            migrationBuilder.DropTable(
                name: "workflow_definitions",
                schema: "workflow");
        }
    }
}
