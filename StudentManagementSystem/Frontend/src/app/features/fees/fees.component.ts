import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-fees',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container">
      <h2>Fee Management</h2>
      
      <!-- Fee Structure Section -->
      <div class="section">
        <h3>Fee Structure</h3>
        <div class="fee-structure">
          <div class="fee-card">
            <h4>Tuition Fee</h4>
            <p class="amount">$2,000 / semester</p>
            <p class="description">Academic courses and instruction</p>
          </div>
          
          <div class="fee-card">
            <h4>Library Fee</h4>
            <p class="amount">$100 / year</p>
            <p class="description">Access to library resources</p>
          </div>
          
          <div class="fee-card">
            <h4>Lab Fee</h4>
            <p class="amount">$300 / semester</p>
            <p class="description">Laboratory equipment and materials</p>
          </div>
          
          <div class="fee-card">
            <h4>Sports Fee</h4>
            <p class="amount">$150 / year</p>
            <p class="description">Sports facilities and activities</p>
          </div>
        </div>
      </div>
      
      <!-- Fee Payment Section -->
      <div class="section">
        <h3>Fee Payment</h3>
        <form [formGroup]="paymentForm" (ngSubmit)="processPayment()" class="payment-form">
          <div class="form-row">
            <div class="form-group">
              <label for="studentSearch">Student ID or Name</label>
              <input 
                type="text" 
                id="studentSearch" 
                formControlName="studentSearch"
                placeholder="Enter student ID or name"
                class="form-control"
              >
            </div>
            <button type="button" (click)="searchStudent()" class="btn-search">Search</button>
          </div>
          
          <div *ngIf="selectedStudent" class="student-info">
            <h4>Student Information</h4>
            <p><strong>Name:</strong> {{ selectedStudent.firstName }} {{ selectedStudent.lastName }}</p>
            <p><strong>ID:</strong> {{ selectedStudent.studentId }}</p>
            <p><strong>Status:</strong> {{ selectedStudent.status }}</p>
          </div>
          
          <div class="form-group">
            <label for="feeType">Fee Type</label>
            <select formControlName="feeType" class="form-control">
              <option value="">Select fee type</option>
              <option value="tuition">Tuition Fee</option>
              <option value="library">Library Fee</option>
              <option value="lab">Lab Fee</option>
              <option value="sports">Sports Fee</option>
              <option value="other">Other</option>
            </select>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <label for="amount">Amount</label>
              <input 
                type="number" 
                id="amount" 
                formControlName="amount"
                placeholder="Enter amount"
                class="form-control"
              >
            </div>
            
            <div class="form-group">
              <label for="paymentMethod">Payment Method</label>
              <select formControlName="paymentMethod" class="form-control">
                <option value="">Select payment method</option>
                <option value="cash">Cash</option>
                <option value="card">Credit/Debit Card</option>
                <option value="bank">Bank Transfer</option>
                <option value="online">Online Payment</option>
              </select>
            </div>
          </div>
          
          <div class="form-group">
            <label for="remarks">Remarks (Optional)</label>
            <textarea 
              id="remarks" 
              formControlName="remarks"
              placeholder="Any additional notes"
              class="form-control"
              rows="3"
            ></textarea>
          </div>
          
          <div class="form-actions">
            <button type="submit" [disabled]="paymentForm.invalid || isProcessing" class="btn-primary">
              {{ isProcessing ? 'Processing...' : 'Process Payment' }}
            </button>
          </div>
        </form>
      </div>
      
      <!-- Recent Transactions -->
      <div class="section">
        <h3>Recent Transactions</h3>
        <div class="transactions-list">
          <div class="transaction" *ngFor="let transaction of recentTransactions">
            <div class="transaction-info">
              <h4>{{ transaction.studentName }}</h4>
              <p>{{ transaction.feeType }} - {{ transaction.amount | currency }}</p>
              <small>{{ transaction.date | date:'medium' }}</small>
            </div>
            <div class="transaction-status" [class]="transaction.status.toLowerCase()">
              {{ transaction.status }}
            </div>
          </div>
          
          <div *ngIf="recentTransactions.length === 0" class="no-transactions">
            <p>No recent transactions found.</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .section {
      margin-bottom: 40px;
      background: white;
      padding: 30px;
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }
    
    h2, h3 {
      color: #333;
      margin-bottom: 20px;
    }
    
    .fee-structure {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 20px;
    }
    
    .fee-card {
      background: #f8f9fa;
      padding: 20px;
      border-radius: 8px;
      border: 1px solid #dee2e6;
      text-align: center;
    }
    
    .fee-card h4 {
      margin: 0 0 10px 0;
      color: #495057;
    }
    
    .amount {
      font-size: 24px;
      font-weight: bold;
      color: #007bff;
      margin: 10px 0;
    }
    
    .description {
      color: #6c757d;
      font-size: 14px;
    }
    
    .payment-form {
      max-width: 600px;
    }
    
    .form-row {
      display: grid;
      grid-template-columns: 1fr auto;
      gap: 15px;
      align-items: end;
      margin-bottom: 20px;
    }
    
    .form-group {
      margin-bottom: 20px;
    }
    
    label {
      display: block;
      margin-bottom: 5px;
      font-weight: 600;
      color: #333;
    }
    
    .form-control {
      width: 100%;
      padding: 12px;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 14px;
    }
    
    .form-control:focus {
      outline: none;
      border-color: #007bff;
      box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
    }
    
    .btn-search, .btn-primary {
      padding: 12px 24px;
      border: none;
      border-radius: 4px;
      font-weight: 600;
      cursor: pointer;
      transition: background-color 0.3s;
    }
    
    .btn-search {
      background-color: #28a745;
      color: white;
      height: fit-content;
    }
    
    .btn-search:hover {
      background-color: #218838;
    }
    
    .btn-primary {
      background-color: #007bff;
      color: white;
    }
    
    .btn-primary:hover:not(:disabled) {
      background-color: #0056b3;
    }
    
    .btn-primary:disabled {
      background-color: #6c757d;
      cursor: not-allowed;
    }
    
    .student-info {
      background: #e9ecef;
      padding: 15px;
      border-radius: 4px;
      margin: 20px 0;
    }
    
    .student-info h4 {
      margin: 0 0 10px 0;
      color: #495057;
    }
    
    .student-info p {
      margin: 5px 0;
      color: #6c757d;
    }
    
    .transactions-list {
      max-height: 400px;
      overflow-y: auto;
    }
    
    .transaction {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 15px;
      border: 1px solid #dee2e6;
      border-radius: 4px;
      margin-bottom: 10px;
    }
    
    .transaction-info h4 {
      margin: 0 0 5px 0;
      color: #333;
    }
    
    .transaction-info p {
      margin: 0;
      color: #6c757d;
    }
    
    .transaction-status {
      padding: 5px 15px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: bold;
      text-transform: uppercase;
    }
    
    .transaction-status.completed {
      background-color: #d4edda;
      color: #155724;
    }
    
    .transaction-status.pending {
      background-color: #fff3cd;
      color: #856404;
    }
    
    .no-transactions {
      text-align: center;
      color: #6c757d;
      padding: 40px;
    }
    
    @media (max-width: 768px) {
      .form-row {
        grid-template-columns: 1fr;
      }
      
      .fee-structure {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class FeesComponent implements OnInit {
  paymentForm: FormGroup;
  selectedStudent: any = null;
  isProcessing = false;
  recentTransactions: any[] = [];

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService
  ) {
    this.paymentForm = this.fb.group({
      studentSearch: [''],
      feeType: [''],
      amount: [''],
      paymentMethod: [''],
      remarks: ['']
    });
  }

  ngOnInit() {
    this.loadRecentTransactions();
  }

  searchStudent() {
    const searchTerm = this.paymentForm.get('studentSearch')?.value;
    if (searchTerm) {
      // Mock search - replace with actual API call
      this.selectedStudent = {
        firstName: 'John',
        lastName: 'Doe',
        studentId: 'STU001',
        status: 'Active'
      };
    }
  }

  processPayment() {
    if (this.paymentForm.valid && this.selectedStudent) {
      this.isProcessing = true;
      
      const paymentData = {
        studentId: this.selectedStudent.studentId,
        ...this.paymentForm.value
      };
      
      // Mock payment processing
      setTimeout(() => {
        this.recentTransactions.unshift({
          studentName: `${this.selectedStudent.firstName} ${this.selectedStudent.lastName}`,
          feeType: this.paymentForm.get('feeType')?.value,
          amount: this.paymentForm.get('amount')?.value,
          date: new Date(),
          status: 'Completed'
        });
        
        this.isProcessing = false;
        this.paymentForm.reset();
        this.selectedStudent = null;
      }, 2000);
    }
  }

  loadRecentTransactions() {
    // Mock data - replace with actual API call
    this.recentTransactions = [
      {
        studentName: 'Jane Smith',
        feeType: 'Tuition Fee',
        amount: 2000,
        date: new Date(2026, 1, 10),
        status: 'Completed'
      },
      {
        studentName: 'Bob Johnson',
        feeType: 'Lab Fee',
        amount: 300,
        date: new Date(2026, 1, 9),
        status: 'Pending'
      }
    ];
  }
}