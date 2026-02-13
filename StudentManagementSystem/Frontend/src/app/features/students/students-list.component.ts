import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-students-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container">
      <h2>Students List</h2>
      
      <!-- Debug Info -->
      <div class="debug-info" style="background: #f8f9fa; padding: 10px; margin: 10px 0; border-radius: 4px; font-family: monospace; font-size: 12px;">
        <p><strong>Debug:</strong> Students count: {{ students?.length || 0 }} | Loading: {{ isLoading }} | Error: {{ error || 'none' }}</p>
      </div>
      
      <!-- Loading State -->
      <div *ngIf="isLoading" class="loading" style="text-align: center; padding: 40px;">
        <p>Loading students...</p>
      </div>
      
      <!-- Students Grid -->
      <div *ngIf="!isLoading && students && students.length > 0" class="students-grid">
        <div *ngFor="let student of students; let i = index" class="student-card">
          <h3>{{ student.firstName }} {{ student.lastName }}</h3>
          <p><strong>Student ID:</strong> {{ student.studentId }}</p>
          <p><strong>Email:</strong> {{ student.email }}</p>
          <p><strong>Status:</strong> {{ student.status }}</p>
        </div>
      </div>
      
      <!-- No Data -->
      <div *ngIf="!isLoading && (!students || students.length === 0) && !error" class="no-data">
        <p>No students found.</p>
        <small>API URL: http://localhost:5286/api/students</small>
      </div>
      
      <!-- Error State -->
      <div *ngIf="error" class="error">
        <p>Error loading students: {{ error }}</p>
      </div>
    </div>
  `,
  styles: [`
    .container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .students-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 20px;
      margin-top: 20px;
    }
    
    .student-card {
      background: white;
      border: 1px solid #ddd;
      border-radius: 8px;
      padding: 20px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      transition: transform 0.2s;
    }
    
    .student-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.15);
    }
    
    .student-card h3 {
      margin: 0 0 10px 0;
      color: #333;
    }
    
    .student-card p {
      margin: 5px 0;
      color: #666;
    }
    
    .no-data, .error {
      text-align: center;
      padding: 40px;
      background: #f5f5f5;
      border-radius: 8px;
      margin-top: 20px;
    }
    
    .error {
      background: #ffe6e6;
      color: #d63384;
    }
  `]
})
export class StudentsListComponent implements OnInit {
  students: any[] = [];
  error: string = '';
  isLoading: boolean = true;

  constructor(
    private apiService: ApiService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadStudents();
  }

  loadStudents() {
    this.isLoading = true;
    this.error = '';
    
    this.apiService.get('/students').subscribe({
      next: (response: any) => {
        console.log('API Response received, processing...');
        
        // Handle the API response format: {success: true, data: [...], message: "..."}
        if (response && response.success && response.data && Array.isArray(response.data)) {
          this.students = response.data;
          console.log('Successfully loaded', this.students.length, 'students');
        } else {
          console.error('Unexpected API response format:', response);
          this.students = [];
        }
        
        this.isLoading = false;
        
        // Explicitly trigger change detection
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading students:', err);
        this.error = err.message || 'Failed to load students';
        this.students = [];
        this.isLoading = false;
        
        // Explicitly trigger change detection
        this.cdr.detectChanges();
      }
    });
  }
}