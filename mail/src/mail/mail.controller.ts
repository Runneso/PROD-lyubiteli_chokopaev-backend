import { IMailController } from "./interfaces/mail-controller.interface";
import { Controller, UsePipes } from "@nestjs/common";
import { GrpcMethod } from "@nestjs/microservices";
import { MailService } from "./mail.service";
import { ToAppDto } from "./dto/to-app.dto";

@Controller()
export class MailController implements IMailController {

    constructor(private readonly mailService: MailService) {}

    @GrpcMethod("MailService", "SendInvintationToApp")
    async sendInvintationsToApp(dto: ToAppDto): Promise<Empty> {
        return await this.mailService.sendInvintationToApp(dto);
    }

    @GrpcMethod("MailService", "SendInvintationToEvent")
    async sendInvintationsToEvent(dto: ToAppDto): Promise<Empty> {
        return await this.mailService.sendInvintationToEvent(dto);
    }

}