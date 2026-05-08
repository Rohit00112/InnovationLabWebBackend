using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnovationLab.Landing.Migrations
{
    /// <inheritdoc />
    public partial class AutoCloseTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create a trigger function that closes registration when max teams is reached
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION landing.auto_close_event_registration()
                RETURNS TRIGGER AS $$
                BEGIN
                    -- Check if this event has reached max number of teams
                    IF (SELECT COUNT(*) FROM landing.""EventRegistrations"" 
                        WHERE ""EventId"" = NEW.""EventId"" AND ""DeletedAt"" IS NULL AND ""Type"" = 1) -- Type 1 = Team
                        >= (SELECT ""MaxNumberOfTeams"" FROM landing.""Events"" WHERE ""Id"" = NEW.""EventId"")
                    THEN
                        -- Close registration for this event
                        UPDATE landing.""Events"" 
                        SET ""IsRegistrationOpen"" = false, ""UpdatedAt"" = CURRENT_TIMESTAMP
                        WHERE ""Id"" = NEW.""EventId"";
                    END IF;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Create trigger on EventRegistrations insert
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS event_registration_auto_close_trigger 
                ON landing.""EventRegistrations"";
                
                CREATE TRIGGER event_registration_auto_close_trigger
                AFTER INSERT ON landing.""EventRegistrations""
                FOR EACH ROW
                EXECUTE FUNCTION landing.auto_close_event_registration();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS event_registration_auto_close_trigger ON landing.\"EventRegistrations\";");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS landing.auto_close_event_registration();");
        }
    }
}
