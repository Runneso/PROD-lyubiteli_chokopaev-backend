import { IMailService } from "./interfaces/mail-service.interface";
import { MailerService } from "@nestjs-modules/mailer";
import { Injectable } from "@nestjs/common";
import { ToAppDto } from "./dto/to-app.dto";

@Injectable()
export class MailService implements IMailService {

    constructor(private readonly mailerService: MailerService) {}

    async sendInvintationToApp(dto: ToAppDto): Promise<Empty> {
        try {
            await this.mailerService.sendMail({
                to: dto.email,
                subject: "Приглашение на олимпиаду",
                html: `
                    <div style="background-color: rgb(40, 39, 39);
                                border-radius: 15px">
                        <h1 style="
                                color: whitesmoke;
                                margin-left: 2%;
                                font-family: MailSans, Helvetica, Arial, sans-serif">
                                Здравствуйте!
                        </h1>
                        <p style="
                                color: whitesmoke;
                                margin-bottom: 1.5%;
                                margin-left: 2%;
                                padding-bottom: 1%;
                                font-family: MailSans, Helvetica, Arial, sans-serif">
                                &nbsp;&nbsp;&nbsp;Вам доступно событие "${dto.eventName}". Скачивайте приложение OlympTeam, регистрируйтесь и ищите себе команду 😉
                        </p>
                    </div>
                `
            });
        } catch (error) {}

        return {};
    }

    async sendInvintationToEvent(dto: ToAppDto): Promise<Empty> {
        try {
            await this.mailerService.sendMail({
                to: dto.email,
                subject: "Приглашение на олимпиаду",
                html: `
                    <div style="background-color: rgb(40, 39, 39);
                                border-radius: 15px">
                        <h1 style="
                                color: whitesmoke;
                                margin-left: 2%;
                                font-family: MailSans, Helvetica, Arial, sans-serif">
                                Здравствуйте!
                        </h1>
                        <p style="
                                color: whitesmoke;
                                margin-bottom: 1.5%;
                                margin-left: 2%;
                                padding-bottom: 1%;
                                font-family: MailSans, Helvetica, Arial, sans-serif">
                                &nbsp;&nbsp;&nbsp;Вам доступно событие "${dto.eventName}". Войдите в систему и ищите себе команду 😉
                        </p>
                    </div>
                `
            });
        } catch (error) {}

        return {};
    }

}