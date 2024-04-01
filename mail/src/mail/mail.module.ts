import { Module } from "@nestjs/common";
import { MailController } from "./mail.controller";
import { MailService } from "./mail.service";
import { MailerModule } from "@nestjs-modules/mailer";
import { config } from "dotenv";
config({ path: ".env" })

@Module({
    controllers: [MailController],
    providers: [MailService],
    imports: [
        MailerModule.forRoot({
            transport: {
                host: process.env.MAIL_HOST, 
                port: process.env.MAIL_PORT,
                secure: process.env.IS_SECURE,
                auth: {
                    user: process.env.MAIL_USER,
                    pass: process.env.MAIL_PASSWORD,
                },
            },
            defaults: {
                from: `"OlympTeam" <${process.env.MAIL_USER}>`,
            },
        }),
    ],
})
export class MailModule {}