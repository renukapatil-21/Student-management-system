import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService, DashboardStats, RecentActivity } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard-container">
      <h1>Student Management Dashboard</h1>
      
      <!-- Loading State -->
      <div *ngIf="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Loading dashboard data...</p>
      </div>
      
      <!-- Error State -->
      <div *ngIf="error && !loading" class="error-state">
        <div class="error-message">
          <span class="error-icon">⚠️</span>
          <p>{{error}}</p>
          <button (click)="loadDashboardData()" class="retry-btn">Try Again</button>
        </div>
      </div>
      
      <!-- Dashboard Content -->
      <div *ngIf="!loading && !error">
        <!-- Stats Cards -->
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon">👥</div>
            <div class="stat-content">
              <h3>Total Students</h3>
              <p class="stat-number">{{dashboardStats?.totalStudents || 0}}</p>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon">📚</div>
            <div class="stat-content">
              <h3>New Admissions</h3>
              <p class="stat-number">{{dashboardStats?.newAdmissions || 0}}</p>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon">💰</div>
            <div class="stat-content">
              <h3>Fees Collected</h3>
              <p class="stat-number">\${{(dashboardStats?.feesCollected || 0) | number:'1.2-2'}}</p>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon">❓</div>
            <div class="stat-content">
              <h3>Pending Inquiries</h3>
              <p class="stat-number">{{dashboardStats?.pendingInquiries || 0}}</p>
            </div>
          </div>
        </div>
      
      <!-- Action Cards -->
      <div class="actions-section">
        <h2>Quick Actions</h2>
        <div class="actions-grid">
          <a routerLink="/add-student" class="action-card">
            <div class="action-icon">➕</div>
            <div class="action-content">
              <h3>Add New Student</h3>
              <p>Register a new student to the system</p>
            </div>
          </a>
          
          <a routerLink="/students" class="action-card">
            <div class="action-icon">👁️</div>
            <div class="action-content">
              <h3>View Students</h3>
              <p>Browse and manage existing students</p>
            </div>
          </a>
          
          <a routerLink="/fees" class="action-card">
            <div class="action-icon">💳</div>
            <div class="action-content">
              <h3>Manage Fees</h3>
              <p>Handle fee payments and billing</p>
            </div>
          </a>
          
          <a routerLink="/inquiries" class="action-card">
            <div class="action-icon">📧</div>
            <div class="action-content">
              <h3>Handle Queries</h3>
              <p>Respond to student inquiries</p>
            </div>
          </a>
        </div>
      </div>
      
      <!-- Recent Activity -->
      <div class="recent-activity">
        <h2>Recent Activity</h2>
        <div class="activity-list">
          <div class="activity-item" *ngFor="let activity of recentActivities">
            <div class="activity-icon" [ngClass]="'icon-' + activity.type">
              {{getActivityIcon(activity.type)}}
            </div>
            <div class="activity-content">
              <p>{{activity.message}}</p>
              <small>{{activity.time}}</small>
            </div>
          </div>
        </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    
    h1 {
      color: #333;
      text-align: center;
      margin-bottom: 40px;
      font-size: 2.5rem;
    }
    
    h2 {
      color: #555;
      margin-bottom: 20px;
      font-size: 1.8rem;
    }
    
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 20px;
      margin-bottom: 40px;
    }
    
    .stat-card {
      background: white;
      padding: 25px;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);
      display: flex;
      align-items: center;
      gap: 20px;
      border-left: 4px solid #007bff;
    }
    
    .stat-icon {
      font-size: 3rem;
      opacity: 0.8;
    }
    
    .stat-content h3 {
      margin: 0 0 10px 0;
      color: #666;
      font-size: 1rem;
      font-weight: 500;
    }
    
    .stat-number {
      font-size: 2rem;
      font-weight: bold;
      color: #007bff;
      margin: 0;
    }
    
    .actions-section {
      margin-bottom: 40px;
    }
    
    .actions-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 20px;
    }
    
    .action-card {
      background: white;
      padding: 30px;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);
      display: flex;
      align-items: center;
      gap: 20px;
      text-decoration: none;
      color: inherit;
      transition: all 0.3s ease;
      border: 2px solid transparent;
    }
    
    .action-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 25px rgba(0,0,0,0.15);
      border-color: #007bff;
    }
    
    .action-icon {
      font-size: 2.5rem;
      background: #f8f9fa;
      padding: 15px;
      border-radius: 50%;
      min-width: 70px;
      text-align: center;
    }
    
    .action-content h3 {
      margin: 0 0 10px 0;
      color: #333;
      font-size: 1.2rem;
    }
    
    .action-content p {
      margin: 0;
      color: #666;
      font-size: 0.9rem;
    }
    
    .recent-activity {
      background: white;
      padding: 30px;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }
    
    .activity-list {
      display: flex;
      flex-direction: column;
      gap: 15px;
    }
    
    .activity-item {
      display: flex;
      align-items: center;
      gap: 15px;
      padding: 15px;
      background: #f8f9fa;
      border-radius: 8px;
      border-left: 3px solid #28a745;
    }
    
    .activity-icon {
      font-size: 1.5rem;
      min-width: 40px;
      text-align: center;
    }
    
    .icon-admission {
      color: #28a745;
    }
    
    .icon-payment {
      color: #007bff;
    }
    
    .icon-inquiry {
      color: #ffc107;
    }
    
    .icon-update {
      color: #6c757d;
    }
    
    .activity-content p {
      margin: 0 0 5px 0;
      color: #333;
    }
    
    .activity-content small {
      color: #666;
      font-size: 0.8rem;
    }
    
    /* Loading and Error States */
    .loading-state, .error-state {
      text-align: center;
      padding: 60px 20px;
    }
    
    .spinner {
      border: 4px solid #f3f3f3;
      border-top: 4px solid #007bff;
      border-radius: 50%;
      width: 50px;
      height: 50px;
      animation: spin 1s linear infinite;
      margin: 0 auto 20px;
    }
    
    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
    
    .error-message {
      background: #f8d7da;
      color: #721c24;
      padding: 20px;
      border-radius: 8px;
      border: 1px solid #f1aeb5;
      display: inline-block;
    }
    
    .error-icon {
      font-size: 2rem;
      display: block;
      margin-bottom: 10px;
    }
    
    .retry-btn {
      background: #dc3545;
      color: white;
      border: none;
      padding: 8px 16px;
      border-radius: 4px;
      cursor: pointer;
      margin-top: 10px;
    }
    
    .retry-btn:hover {
      background: #c82333;
    }
    
    @media (max-width: 768px) {
      .dashboard-container {
        padding: 10px;
      }
      
      h1 {
        font-size: 2rem;
        margin-bottom: 20px;
      }
      
      .stats-grid, .actions-grid {
        grid-template-columns: 1fr;
      }
      
      .stat-card, .action-card {
        padding: 20px;
      }
      
      .action-icon {
        min-width: 60px;
        padding: 12px;
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  dashboardStats: DashboardStats | null = null;
  recentActivities: RecentActivity[] = [];
  loading = true;
  error: string | null = null;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.loadDashboardData();
    
    // Add a timeout fallback in case API calls take too long
    setTimeout(() => {
      if (this.loading) {
        console.warn('Dashboard data loading timeout - showing fallback data');
        this.dashboardStats = {
          totalStudents: 0,
          newAdmissions: 0,
          feesCollected: 0,
          pendingInquiries: 0,
          lastUpdated: new Date().toISOString()
        };
        this.recentActivities = [
          { type: 'info', message: 'Unable to load recent activities. Please check API connection.', time: 'Just now', timestamp: new Date().toISOString() }
        ];
        this.loading = false;
        this.error = 'API connection timeout - showing default data';
      }
    }, 5000); // 5 second timeout
  }

  loadDashboardData() {
    this.loading = true;
    this.error = null;

    console.log('Loading dashboard data...');

    // Load dashboard statistics
    this.dashboardService.getDashboardStats().subscribe({
      next: (response) => {
        console.log('Dashboard stats response:', response);
        if (response.success && response.data) {
          this.dashboardStats = response.data;
          console.log('Dashboard stats loaded successfully:', this.dashboardStats);
        } else {
          this.error = 'Failed to load dashboard statistics';
          console.error('Dashboard stats response unsuccessful:', response);
        }
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        this.error = `Failed to load dashboard statistics: ${error.message || error}`;
        // Fallback to default values
        this.dashboardStats = {
          totalStudents: 0,
          newAdmissions: 0,
          feesCollected: 0,
          pendingInquiries: 0,
          lastUpdated: new Date().toISOString()
        };
      }
    });

    // Load recent activities
    this.dashboardService.getRecentActivities().subscribe({
      next: (response) => {
        console.log('Recent activities response:', response);
        if (response.success && response.data) {
          this.recentActivities = response.data;
          console.log('Recent activities loaded successfully:', this.recentActivities);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading recent activities:', error);
        // Keep default activities as fallback
        this.recentActivities = [
          { type: 'info', message: 'Welcome to Student Management System', time: 'Just now', timestamp: new Date().toISOString() }
        ];
        this.loading = false;
      }
    });
  }

  getActivityIcon(type: string): string {
    switch(type) {
      case 'admission': return '👤';
      case 'payment': return '💰';
      case 'inquiry': return '❓';
      case 'update': return '✏️';
      default: return '📝';
    }
  }
}