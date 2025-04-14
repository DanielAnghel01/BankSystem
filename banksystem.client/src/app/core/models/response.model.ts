export class Response<T> {
  public status!: number;
  public isSuccess!: boolean;
  public content!: T;
  public errorMessage!: string;
}
