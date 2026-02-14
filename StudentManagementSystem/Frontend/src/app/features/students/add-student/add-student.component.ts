import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-add-student',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container">
      <h2>Add New Student</h2>
      
      <form [formGroup]="studentForm" (ngSubmit)="onSubmit()" class="student-form">
        <div class="form-row">
          <div class="form-group">
            <label for="firstName">First Name *</label>
            <input 
              type="text" 
              id="firstName" 
              formControlName="firstName"
              class="form-control"
              [class.error]="isFieldInvalid('firstName')"
            >
            <div *ngIf="isFieldInvalid('firstName')" class="error-message">
              First name is required
            </div>
          </div>
          
          <div class="form-group">
            <label for="lastName">Last Name *</label>
            <input 
              type="text" 
              id="lastName" 
              formControlName="lastName"
              class="form-control"
              [class.error]="isFieldInvalid('lastName')"
            >
            <div *ngIf="isFieldInvalid('lastName')" class="error-message">
              Last name is required
            </div>
          </div>
        </div>
        
        <div class="form-row">
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
          
          <div class="form-group">
            <label for="studentId">Student ID *</label>
            <input 
              type="text" 
              id="studentId" 
              formControlName="studentId"
              class="form-control"
              [class.error]="isFieldInvalid('studentId')"
            >
            <div *ngIf="isFieldInvalid('studentId')" class="error-message">
              Student ID is required
            </div>
          </div>
        </div>
        
        <div class="form-group">
          <label for="status">Status</label>
          <select formControlName="status" class="form-control">
            <option value="Active">Active</option>
            <option value="Inactive">Inactive</option>
            <option value="Graduated">Graduated</option>
          </select>
        </div>
        
        <div class="form-actions">
          <button type="submit" [disabled]="studentForm.invalid || isLoading" class="btn-primary">
            {{ isLoading ? 'Adding...' : 'Add Student' }}
          </button>
          <button type="button" (click)="resetForm()" class="btn-secondary">Reset</button>
        </div>
      </form>
      
      <div *ngIf="message" [class]="messageType" class="message">
        {{ message }}
      </div>
    </div>
  `,
  styles: [`
    .container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .student-form {
      background: white;
      padding: 30px;
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
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
    
    .message {
      margin-top: 20px;
      padding: 15px;
      border-radius: 4px;
    }
    
    .success {
      background-color: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }
    
    .error {
      background-color: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }
    
    @media (max-width: 768px) {
      .form-row {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class AddStudentComponent {
  studentForm: FormGroup;
  isLoading = false;
  message = '';
  messageType = '';

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService
  ) {
    this.studentForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      studentId: ['', [Validators.required]],
      status: ['Active']
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.studentForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  onSubmit() {
    if (this.studentForm.valid) {
      this.isLoading = true;
      this.message = '';
      
      this.apiService.post('students', this.studentForm.value).subscribe({
        next: (response) => {
          this.message = 'Student added successfully!';
          this.messageType = 'success';
          this.resetForm();
          this.isLoading = false;
        },
        error: (error) => {
          this.message = 'Error adding student. Please try again.';
          this.messageType = 'error';
          this.isLoading = false;
        }
      });
    }
  }

  resetForm() {
    this.studentForm.reset();
    this.studentForm.patchValue({ status: 'Active' });
    this.message = '';
  }
}