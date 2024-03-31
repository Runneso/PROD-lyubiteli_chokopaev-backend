import { ToAppDto } from "../dto/to-app.dto";

export interface IMailController {
    sendInvintationsToApp(dto: ToAppDto): Promise<Empty>;
    sendInvintationsToEvent(dto: ToAppDto): Promise<Empty>;
}