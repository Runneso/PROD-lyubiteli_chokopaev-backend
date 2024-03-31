import { ArgumentMetadata, PipeTransform } from "@nestjs/common";
import { plainToInstance } from "class-transformer";
import { validate } from "class-validator";
import { ValidationException } from "src/exceptions/validation.exception";

export class ValidationPipe implements PipeTransform {
    
    async transform(value: any, { metatype }: ArgumentMetadata) {
        if (!metatype || !this.toValidate(metatype)) {
          return value;
        }
        const object = plainToInstance(metatype, value);
        const errors = await validate(object);
        if (errors.length != 0) {
            const messages = errors.map((error) => {
                return `${error.property}: ${Object.values(error.constraints).join(", ")}`;
            });
            throw new ValidationException(messages);
        }
        return value;
    }
    
    private toValidate(metatype: Function): boolean {
        const types: Function[] = [String, Boolean, Number, Array, Object];
        return !types.includes(metatype);
    }

}