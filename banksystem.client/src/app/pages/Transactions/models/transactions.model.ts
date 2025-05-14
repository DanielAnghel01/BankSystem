export class TransactionModel {
  id!: number;
  senderAccountNumber!: string;
  reciverAccountNumber!: string;
  amount!: number;
  transactionType!: string;
  date!: string;
  details!: string;
}
