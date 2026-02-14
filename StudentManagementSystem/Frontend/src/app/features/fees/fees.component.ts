import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { FeeService, Fee, PaymentRequest } from '../../core/services/fee.service';

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
        <h3>Recent Fee Transactions</h3>
        <div class="transactions-summary">
          <div class="summary-card">
            <h4>Total Fees Collected</h4>
            <p class="amount">\${{totalCollected | number:'1.2-2'}}</p>
          </div>
          <div class="summary-card">
            <h4>Pending Amount</h4>
            <p class="amount pending">\${{totalPending | number:'1.2-2'}}</p>
          </div>
          <div class="summary-card">
            <h4>Total Fees</h4>
            <p class="amount">\${{totalFees | number:'1.2-2'}}</p>
          </div>
        </div>
        
        <div class="transactions-list" *ngIf="!loading">
          <div class="transaction" *ngFor="let fee of allFees">
            <div class="transaction-info">
              <h4>{{ fee.studentName }}</h4>
              <p>{{ fee.feeType }} - \${{ fee.amount | number:'1.2-2' }}</p>
              <p *ngIf="getStatusString(fee.status) === 'Paid' && fee.paymentDate">
                Paid: \${{ fee.paidAmount | number:'1.2-2' }} on {{ fee.paymentDate | date:'medium' }}
              </p>
              <small>Due: {{ fee.dueDate | date:'short' }} | {{ fee.academicYear }} {{ fee.semester }}</small>
            </div>
            <div class="transaction-actions">
              <div class="transaction-status" [ngClass]="getStatusString(fee.status).toLowerCase()">
                {{ getStatusString(fee.status) }}
              </div>
              <button 
                *ngIf="getStatusString(fee.status) !== 'Paid'" 
                (click)="payFee(fee)" 
                [disabled]="isProcessing"
                class="btn-pay">
                {{ isProcessing && processingFeeId === fee.id ? 'Processing...' : 'Pay Now' }}
              </button>
            </div>
          </div>
          
          <div *ngIf="allFees.length === 0" class="no-transactions">
            <p>No fee transactions found.</p>
          </div>
        </div>
        
        <div *ngIf="loading" class="loading">
          <div class="spinner"></div>
          <p>Loading fee data...</p>
        </div>
        
        <div *ngIf="error" class="error">
          <p>{{ error }}</p>
          <button (click)="loadFees()" class="btn-secondary">Retry</button>
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
    
    /* Summary Cards */
    .transactions-summary {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
      margin-bottom: 30px;
    }
    
    .summary-card {
      background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
      padding: 20px;
      border-radius: 12px;
      text-align: center;
      border-left: 4px solid #007bff;
    }
    
    .summary-card h4 {
      margin: 0 0 10px 0;
      font-size: 0.9rem;
      color: #6c757d;
      text-transform: uppercase;
    }
    
    .summary-card .amount {
      font-size: 1.8rem;
      font-weight: bold;
      margin: 0;
      color: #28a745;
    }
    
    .summary-card .amount.pending {
      color: #ffc107;
    }
    
    /* Loading and Error States */
    .loading, .error {
      text-align: center;
      padding: 40px;
    }
    
    .spinner {
      border: 3px solid #f3f3f3;
      border-top: 3px solid #007bff;
      border-radius: 50%;
      width: 40px;
      height: 40px;
      animation: spin 1s linear infinite;
      margin: 0 auto 15px;
    }
    
    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
    
    .error {
      background: #f8d7da;
      color: #721c24;
      border-radius: 8px;
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
    
    .transaction-info {
      flex: 1;
    }
    
    .transaction-actions {
      display: flex;
      align-items: center;
      gap: 10px;
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
    
    .transaction-status.paid {
      background-color: #d4edda;
      color: #155724;
    }
    
    .transaction-status.pending {
      background-color: #fff3cd;
      color: #856404;
    }
    
    .transaction-status.overdue {
      background-color: #f8d7da;
      color: #721c24;
    }
    
    .transaction-status.partiallypaid {
      background-color: #d1ecf1;
      color: #0c5460;
    }
    
    .transaction-status.cancelled {
      background-color: #e2e3e5;
      color: #495057;
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

    .btn-pay {
      background: #28a745;
      color: white;
      border: none;
      padding: 8px 16px;
      border-radius: 4px;
      font-size: 14px;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .btn-pay:hover:not(:disabled) {
      background: #218838;
    }

    .btn-pay:disabled {
      background: #6c757d;
      cursor: not-allowed;
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
  processingFeeId: string | null = null;
  loading = true;
  error: string | null = null;
  
  // Real fee data
  allFees: Fee[] = [];
  totalCollected = 0;
  totalPending = 0;
  totalFees = 0;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private feeService: FeeService
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
    this.loadFees();
    
    // Debug: Log form changes
    this.paymentForm.get('feeType')?.valueChanges.subscribe(value => {
      console.log('Fee Type changed to:', value);
      console.log('AllFees length:', this.allFees.length);
      console.log('Loading state:', this.loading);
    });
  }

  loadFees() {
    console.log('loadFees() called - setting loading to true');
    this.loading = true;
    this.error = null;
    
    this.feeService.getFees().subscribe({
      next: (response) => {
        console.log('API Response:', response);
        if (response.success && response.data) {
          this.allFees = response.data;
          this.calculateTotals();
          console.log('Fees loaded successfully:', this.allFees.length, 'fees');
          console.log('AllFees data:', this.allFees);
        } else {
          this.error = 'Failed to load fee data';
          console.error('Failed to load fee data:', response);
        }
        console.log('Setting loading to false');
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading fees:', error);
        this.error = `Failed to load fees: ${error.message || error}`;
        console.log('Setting loading to false after error');
        this.loading = false;
      }
    });
  }
  
  calculateTotals() {
    // Convert numeric status to string for compatibility
    this.totalCollected = this.allFees
      .filter(fee => this.getStatusString(fee.status) === 'Paid')
      .reduce((sum, fee) => sum + fee.paidAmount, 0);
      
    this.totalPending = this.allFees
      .filter(fee => {
        const statusStr = this.getStatusString(fee.status);
        return statusStr === 'Pending' || statusStr === 'Overdue';
      })
      .reduce((sum, fee) => sum + (fee.amount - fee.paidAmount), 0);
      
    this.totalFees = this.allFees.reduce((sum, fee) => sum + fee.amount, 0);
  }

  getStatusString(status: number | string): string {
    if (typeof status === 'string') return status;
    
    // Convert numeric enum to string
    const statusMap: { [key: number]: string } = {
      0: 'Pending',
      1: 'Paid', 
      2: 'Overdue',
      3: 'PartiallyPaid',
      4: 'Cancelled'
    };
    
    return statusMap[status] || 'Pending';
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

  payFee(fee: Fee) {
    this.isProcessing = true;
    this.processingFeeId = fee.id;
    
    const payment: PaymentRequest = {
      feeId: fee.id,
      paymentAmount: fee.amount - fee.paidAmount, // Pay remaining amount
      paymentMethod: 'Credit Card', // Default method
      transactionId: `TXN${Date.now()}` // Generate transaction ID
    };
    
    this.feeService.processPayment(payment).subscribe({
      next: (response) => {
        if (response.success) {
          console.log('Payment processed successfully:', response.data);
          // Reload fees to reflect the payment
          this.loadFees();
        } else {
          this.error = response.message || 'Payment failed';
        }
        this.isProcessing = false;
        this.processingFeeId = null;
      },
      error: (error) => {
        console.error('Payment error:', error);
        this.error = `Payment failed: ${error.message || error}`;
        this.isProcessing = false;
        this.processingFeeId = null;
      }
    });
  }

  processPayment() {
    if (this.paymentForm.valid && this.selectedStudent) {
      this.isProcessing = true;
      
      const paymentData = {
        studentId: this.selectedStudent.studentId,
        ...this.paymentForm.value
      };
      
      // Mock payment processing - replace with actual API call
      setTimeout(() => {
        // Reload fees after payment to reflect changes
        this.loadFees();
        
        this.isProcessing = false;
        this.paymentForm.reset();
        this.selectedStudent = null;
      }, 2000);
    }
  }
}