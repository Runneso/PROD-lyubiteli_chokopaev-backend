import { RpcException } from "@nestjs/microservices";

export class ValidationException extends RpcException {

    messages: string[];

    constructor(response: string[]) {
        super(response);
        this.messages = response;
    }

}