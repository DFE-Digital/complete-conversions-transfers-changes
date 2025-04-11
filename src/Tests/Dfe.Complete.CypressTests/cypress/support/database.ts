import sql from "mssql";
import { testUsers } from "cypress/constants/cypressConstants";
import { TestUser } from "cypress/constants/TestUser";

export async function setupDatabase() {
    const sqlConfig = {
        server: process.env.dbHost!,
        user: process.env.dbUser,
        password: process.env.dbPassword,
        database: "complete",
        options: {
            encrypt: true,
            trustServerCertificate: true,
        },
    };

    console.log("Verifying users in complete.users table...");
    try {
        const pool = await sql.connect(sqlConfig);

        for (const user of testUsers) {
            await pool.request().query(insertUserIfNotExists(user));
        }

        await pool.close();
        console.log("Verified test users exist in complete.users table");
        return null;
    } catch (err) {
        console.warn("SQL error when verifying test users in complete.users table: ", err);
        console.warn("Tests may not run correctly if the tests users do not exist");
        return null;
    }
}

function insertUserIfNotExists(user: TestUser): string {
    return `
        IF NOT EXISTS (SELECT * FROM [complete].[users] WHERE '${user.adId}' = active_directory_user_id)
        BEGIN
        INSERT INTO [complete].[users] VALUES 
            (NEWID()
            , '${user.email}'
            , GETDATE()
            , GETDATE()
            , ${user.manageTeam}
            , 1
            , '${user.firstName}'
            , '${user.lastName}'
            , '${user.adId}'
            , ${user.assignToProject}
            , ${user.manageUserAccounts}
            , null
            , '${user.team}'
            , null
            , ${user.manageConversionURNs}
            , ${user.manageLocalAuthorities}
            , null)
        END
        `;
}
