import { ToAppDto } from "../dto/to-app.dto";

export interface IMailService {
    sendInvintationToApp(dto: ToAppDto): Promise<Empty>;
    sendInvintationToEvent(dto: ToAppDto): Promise<Empty>;
}