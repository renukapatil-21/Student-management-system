import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-inquiries',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="container">
      <h2>Inquiry Management</h2>
      
      <!-- Add New Inquiry -->
      <div class="section">
        <h3>Add New Inquiry</h3>
        <form [formGroup]="inquiryForm" (ngSubmit)="submitInquiry()" class="inquiry-form">
          <div class="form-row">
            <div class="form-group">
              <label for="name">Full Name *</label>
              <input 
                type="text" 
                id="name" 
                formControlName="name"
                class="form-control"
                [class.error]="isFieldInvalid('name')"
              >
              <div *ngIf="isFieldInvalid('name')" class="error-message">
                Name is required
              </div>
            </div>
            
            <div class="form-group">
              <label for="email">Email *</label>
              <input 
                type="email" 
                id="email" 
                formControlName="email"
                class="form-control"
                [class.error]="isFieldInvalid('email')"
              >
              <div *ngIf="isFieldInvalid('email')" class="error-message">
                Valid email is required
              </div>
            </div>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <label for="phone">Phone Number</label>
              <input 
                type="tel" 
                id="phone" 
                formControlName="phone"
                class="form-control"
              >
            </div>
            
            <div class="form-group">
              <label for="course">Course of Interest</label>
              <select formControlName="course" class="form-control">
                <option value="">Select course</option>
                <option value="computer-science">Computer Science</option>
                <option value="business">Business Administration</option>
                <option value="engineering">Engineering</option>
                <option value="arts">Arts & Literature</option>
                <option value="science">Science</option>
                <option value="other">Other</option>
              </select>
            </div>
          </div>
          
          <div class="form-group">
            <label for="inquiryType">Inquiry Type *</label>
            <select formControlName="inquiryType" class="form-control" [class.error]="isFieldInvalid('inquiryType')">
              <option value="">Select inquiry type</option>
              <option value="admission">Admission Information</option>
              <option value="fees">Fee Structure</option>
              <option value="courses">Course Details</option>
              <option value="facilities">Campus Facilities</option>
              <option value="scholarships">Scholarships</option>
              <option value="general">General Inquiry</option>
            </select>
            <div *ngIf="isFieldInvalid('inquiryType')" class="error-message">
              Please select inquiry type
            </div>
          </div>
          
          <div class="form-group">
            <label for="message">Message *</label>
            <textarea 
              id="message" 
              formControlName="message"
              placeholder="Please describe your inquiry in detail"
              class="form-control"
              rows="4"
              [class.error]="isFieldInvalid('message')"
            ></textarea>
            <div *ngIf="isFieldInvalid('message')" class="error-message">
              Message is required
            </div>
          </div>
          
          <div class="form-actions">
            <button type="submit" [disabled]="inquiryForm.invalid || isSubmitting" class="btn-primary">
              {{ isSubmitting ? 'Submitting...' : 'Submit Inquiry' }}
            </button>
            <button type="button" (click)="resetForm()" class="btn-secondary">Reset</button>
          </div>
        </form>
      </div>
      
      <!-- Inquiry List -->
      <div class="section">
        <h3>Recent Inquiries</h3>
        
        <!-- Filters -->
        <div class="filters">
          <div class="filter-group">
            <label for="statusFilter">Filter by Status:</label>
            <select id="statusFilter" [(ngModel)]="selectedStatus" (change)="filterInquiries()" class="form-control">
              <option value="">All Statuses</option>
              <option value="new">New</option>
              <option value="in-progress">In Progress</option>
              <option value="resolved">Resolved</option>
              <option value="closed">Closed</option>
            </select>
          </div>
          
          <div class="filter-group">
            <label for="typeFilter">Filter by Type:</label>
            <select id="typeFilter" [(ngModel)]="selectedType" (change)="filterInquiries()" class="form-control">
              <option value="">All Types</option>
              <option value="admission">Admission</option>
              <option value="fees">Fees</option>
              <option value="courses">Courses</option>
              <option value="facilities">Facilities</option>
              <option value="scholarships">Scholarships</option>
              <option value="general">General</option>
            </select>
          </div>
        </div>
        
        <!-- Inquiries Table -->
        <div class="inquiries-table">
          <div class="table-header">
            <div class="col-name">Name</div>
            <div class="col-type">Type</div>
            <div class="col-status">Status</div>
            <div class="col-date">Date</div>
            <div class="col-actions">Actions</div>
          </div>
          
          <div *ngFor="let inquiry of filteredInquiries" class="table-row">
            <div class="col-name">
              <div class="inquiry-name">{{ inquiry.name }}</div>
              <div class="inquiry-email">{{ inquiry.email }}</div>
            </div>
            <div class="col-type">{{ inquiry.inquiryType }}</div>
            <div class="col-status">
              <span class="status-badge" [class]="inquiry.status.toLowerCase()">
                {{ inquiry.status }}
              </span>
            </div>
            <div class="col-date">{{ inquiry.date | date:'short' }}</div>
            <div class="col-actions">
              <button (click)="viewInquiry(inquiry)" class="btn-view">View</button>
              <button (click)="updateStatus(inquiry)" class="btn-update">Update</button>
            </div>
          </div>
          
          <div *ngIf="filteredInquiries.length === 0" class="no-inquiries">
            <p>No inquiries found matching the selected filters.</p>
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
    
    .inquiry-form {
      max-width: 800px;
    }
    
    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 20px;
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
      transition: border-color 0.3s;
    }
    
    .form-control:focus {
      outline: none;
      border-color: #007bff;
      box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
    }
    
    .form-control.error {
      border-color: #dc3545;
    }
    
    .error-message {
      color: #dc3545;
      font-size: 12px;
      margin-top: 5px;
    }
    
    .form-actions {
      display: flex;
      gap: 15px;
      margin-top: 30px;
    }
    
    .btn-primary, .btn-secondary {
      padding: 12px 24px;
      border: none;
      border-radius: 4px;
      font-weight: 600;
      cursor: pointer;
      transition: background-color 0.3s;
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
    
    .btn-secondary {
      background-color: #6c757d;
      color: white;
    }
    
    .btn-secondary:hover {
      background-color: #545b62;
    }
    
    .filters {
      display: flex;
      gap: 20px;
      margin-bottom: 30px;
      flex-wrap: wrap;
    }
    
    .filter-group {
      display: flex;
      flex-direction: column;
      min-width: 200px;
    }
    
    .filter-group label {
      margin-bottom: 5px;
      font-size: 14px;
    }
    
    .inquiries-table {
      border: 1px solid #dee2e6;
      border-radius: 4px;
      overflow: hidden;
    }
    
    .table-header, .table-row {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr 1fr 1fr;
      gap: 15px;
      padding: 15px;
      align-items: center;
    }
    
    .table-header {
      background-color: #f8f9fa;
      font-weight: 600;
      color: #495057;
      border-bottom: 1px solid #dee2e6;
    }
    
    .table-row {
      border-bottom: 1px solid #dee2e6;
    }
    
    .table-row:last-child {
      border-bottom: none;
    }
    
    .table-row:hover {
      background-color: #f8f9fa;
    }
    
    .inquiry-name {
      font-weight: 600;
      color: #333;
    }
    
    .inquiry-email {
      font-size: 12px;
      color: #6c757d;
    }
    
    .status-badge {
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: bold;
      text-transform: uppercase;
    }
    
    .status-badge.new {
      background-color: #fff3cd;
      color: #856404;
    }
    
    .status-badge.in-progress {
      background-color: #cce5ff;
      color: #0066cc;
    }
    
    .status-badge.resolved {
      background-color: #d4edda;
      color: #155724;
    }
    
    .status-badge.closed {
      background-color: #f8d7da;
      color: #721c24;
    }
    
    .btn-view, .btn-update {
      padding: 6px 12px;
      border: none;
      border-radius: 4px;
      font-size: 12px;
      cursor: pointer;
      margin-right: 5px;
    }
    
    .btn-view {
      background-color: #17a2b8;
      color: white;
    }
    
    .btn-view:hover {
      background-color: #138496;
    }
    
    .btn-update {
      background-color: #ffc107;
      color: #212529;
    }
    
    .btn-update:hover {
      background-color: #e0a800;
    }
    
    .no-inquiries {
      text-align: center;
      color: #6c757d;
      padding: 40px;
    }
    
    @media (max-width: 768px) {
      .form-row {
        grid-template-columns: 1fr;
      }
      
      .filters {
        flex-direction: column;
      }
      
      .table-header, .table-row {
        grid-template-columns: 1fr;
        gap: 10px;
      }
      
      .table-header {
        display: none;
      }
      
      .table-row {
        padding: 20px;
        border: 1px solid #dee2e6;
        margin-bottom: 10px;
        border-radius: 4px;
      }
    }
  `]
})
export class InquiriesComponent implements OnInit {
  inquiryForm: FormGroup;
  isSubmitting = false;
  inquiries: any[] = [];
  filteredInquiries: any[] = [];
  selectedStatus = '';
  selectedType = '';

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService
  ) {
    this.inquiryForm = this.fb.group({
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      course: [''],
      inquiryType: ['', [Validators.required]],
      message: ['', [Validators.required]]
    });
  }

  ngOnInit() {
    this.loadInquiries();
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.inquiryForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  submitInquiry() {
    if (this.inquiryForm.valid) {
      this.isSubmitting = true;
      
      // Mock submission - replace with actual API call
      setTimeout(() => {
        const newInquiry = {
          id: Date.now(),
          ...this.inquiryForm.value,
          status: 'New',
          date: new Date()
        };
        
        this.inquiries.unshift(newInquiry);
        this.filterInquiries();
        this.resetForm();
        this.isSubmitting = false;
      }, 1500);
    }
  }

  resetForm() {
    this.inquiryForm.reset();
  }

  loadInquiries() {
    // Mock data - replace with actual API call
    this.inquiries = [
      {
        id: 1,
        name: 'Alice Johnson',
        email: 'alice@example.com',
        phone: '+1234567890',
        course: 'Computer Science',
        inquiryType: 'admission',
        message: 'I would like to know about the admission process for Computer Science program.',
        status: 'New',
        date: new Date(2026, 1, 12)
      },
      {
        id: 2,
        name: 'Bob Wilson',
        email: 'bob@example.com',
        phone: '+0987654321',
        course: 'Business',
        inquiryType: 'fees',
        message: 'Can you provide information about the fee structure?',
        status: 'In Progress',
        date: new Date(2026, 1, 11)
      },
      {
        id: 3,
        name: 'Carol Davis',
        email: 'carol@example.com',
        phone: '+1122334455',
        course: 'Engineering',
        inquiryType: 'scholarships',
        message: 'Are there any scholarship opportunities available?',
        status: 'Resolved',
        date: new Date(2026, 1, 10)
      }
    ];
    
    this.filterInquiries();
  }

  filterInquiries() {
    this.filteredInquiries = this.inquiries.filter(inquiry => {
      const statusMatch = !this.selectedStatus || inquiry.status.toLowerCase().replace(' ', '-') === this.selectedStatus;
      const typeMatch = !this.selectedType || inquiry.inquiryType === this.selectedType;
      return statusMatch && typeMatch;
    });
  }

  viewInquiry(inquiry: any) {
    // Mock view functionality
    alert(`Viewing inquiry from ${inquiry.name}:\n\n${inquiry.message}`);
  }

  updateStatus(inquiry: any) {
    // Mock status update
    const statuses = ['New', 'In Progress', 'Resolved', 'Closed'];
    const currentIndex = statuses.indexOf(inquiry.status);
    const nextIndex = (currentIndex + 1) % statuses.length;
    inquiry.status = statuses[nextIndex];
    this.filterInquiries();
  }
}