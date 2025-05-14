import { BankAccount } from './bank-account.models';
import { User } from './user.models';

export class UserProfileModel {
  user!: User;
  bankAccounts!: BankAccount[];
}
