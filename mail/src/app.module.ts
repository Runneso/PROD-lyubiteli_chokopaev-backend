import { Module } from "@nestjs/common";
import { MailModule } from "./mail/mail.module";
import { ConfigModule } from "@nestjs/config";

@Module({
    imports: [
        ConfigModule.forRoot({
            envFilePath: ".env",
            isGlobal: true
        }),
        MailModule,
    ],
})
export class AppModule {}