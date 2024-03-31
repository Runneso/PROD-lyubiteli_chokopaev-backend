import { IsEmail, IsNotEmpty } from "class-validator";

export class ToAppDto {

    @IsEmail({}, { message: "Incorrect email" })
    readonly email: string;

    @IsNotEmpty({ message: "Incorrect name" })
    readonly eventName: string;
    
}