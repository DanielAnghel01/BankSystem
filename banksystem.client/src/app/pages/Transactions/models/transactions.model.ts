export class TransactionModel {
  id!: number;
  senderAccountNumber!: number;
  reciverAccountNumber!: number;
  amount!: number;
  transactionType!: string;
  date!: string;
  details!: string;
}
