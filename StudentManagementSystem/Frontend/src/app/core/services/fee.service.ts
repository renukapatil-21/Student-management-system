import { Injectable } from '@angular/core';
import { ApiService, ApiResponse } from './api.service';
import { Observable } from 'rxjs';

export interface Fee {
  id: string;
  studentId: string;
  studentName: string;
  feeType: string;
  amount: number;
  paidAmount: number;
  dueDate: string;
  paymentDate?: string;
  paymentMethod?: string;
  transactionId?: string;
  status: number | 'Pending' | 'Paid' | 'Overdue' | 'PartiallyPaid' | 'Cancelled'; // Allow both numeric and string
  academicYear: string;
  semester: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFeeRequest {
  studentId: string;
  studentName: string;
  feeType: string;
  amount: number;
  dueDate: string;
  academicYear: string;
  semester: string;
}

export interface PaymentRequest {
  feeId: string;
  paymentAmount: number;
  paymentMethod: string;
  transactionId: string;
}

@Injectable({
  providedIn: 'root'
})
export class FeeService {

  constructor(private apiService: ApiService) {}

  getFees(): Observable<ApiResponse<Fee[]>> {
    return this.apiService.get<Fee[]>('fees');
  }

  getFee(id: string): Observable<ApiResponse<Fee>> {
    return this.apiService.get<Fee>(`fees/${id}`);
  }

  getStudentFees(studentId: string): Observable<ApiResponse<Fee[]>> {
    return this.apiService.get<Fee[]>(`fees/student/${studentId}`);
  }

  getPendingFees(): Observable<ApiResponse<Fee[]>> {
    return this.apiService.get<Fee[]>('fees/pending');
  }

  getOverdueFees(): Observable<ApiResponse<Fee[]>> {
    return this.apiService.get<Fee[]>('fees/overdue');
  }

  createFee(fee: CreateFeeRequest): Observable<ApiResponse<Fee>> {
    return this.apiService.post<Fee>('fees', fee);
  }

  processPayment(payment: PaymentRequest): Observable<ApiResponse<Fee>> {
    return this.apiService.post<Fee>(`fees/${payment.feeId}/payment`, {
      Amount: payment.paymentAmount,
      PaymentMethod: payment.paymentMethod,
      TransactionId: payment.transactionId
    });
  }

  updateFee(id: string, fee: Partial<Fee>): Observable<ApiResponse<Fee>> {
    return this.apiService.put<Fee>(`fees/${id}`, fee);
  }

  deleteFee(id: string): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`fees/${id}`);
  }
}