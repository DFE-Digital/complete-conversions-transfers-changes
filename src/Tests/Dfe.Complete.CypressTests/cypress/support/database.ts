import sql from "mssql";
import dotenv from "dotenv";
import { testUsers } from "cypress/constants/cypressConstants";
import { TestUser } from "cypress/constants/TestUser";

dotenv.config();

export async function setupDatabase() {
    const sqlConfig = {
        server: process.env.DB_HOST!,
        user: process.env.DB_USER,
        password: process.env.DB_PASSWORD,
        database: "complete",
        options: {
            encrypt: true,
            trustServerCertificate: true,
        },
    };

    try {
        const pool = await sql.connect(sqlConfig);

        for (const user of testUsers) {
            await pool.request().query(insertUserIfNotExists(user));
        }

        await pool.close();
        return null;
    } catch (err) {
        console.error("SQL error: ", err);
        throw err;
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
            , 0
            , 1
            , '${user.firstName}'
            , '${user.lastName}'
            , '${user.adId}'
            , 1
            , 0
            , null
            , '${user.team}'
            , null
            , 0
            , 0
            , null)
        END
        `;
}
