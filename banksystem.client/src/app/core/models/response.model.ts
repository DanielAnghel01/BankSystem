export class Response<T> {
  public statusCode!: number;
  public isSuccess!: boolean;
  public content!: T;
  public errorMessage!: string;
}
