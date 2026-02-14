import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard-container">
      <h1>Student Management Dashboard</h1>
      
      <!-- Stats Cards -->
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-icon">👥</div>
          <div class="stat-content">
            <h3>Total Students</h3>
            <p class="stat-number">{{dashboardStats.totalStudents}}</p>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">📚</div>
          <div class="stat-content">
            <h3>New Admissions</h3>
            <p class="stat-number">{{dashboardStats.newAdmissions}}</p>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">💰</div>
          <div class="stat-content">
            <h3>Fees Collected</h3>
            <p class="stat-number">\${{dashboardStats.feesCollected | number}}</p>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">❓</div>
          <div class="stat-content">
            <h3>Pending Inquiries</h3>
            <p class="stat-number">{{dashboardStats.pendingInquiries}}</p>
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
  dashboardStats = {
    totalStudents: 1250,
    newAdmissions: 45,
    feesCollected: 125000,
    pendingInquiries: 12
  };

  recentActivities = [
    { type: 'admission', message: 'New student John Doe admitted to Computer Science', time: '2 hours ago' },
    { type: 'payment', message: 'Fee payment of $2500 received from Sarah Smith', time: '4 hours ago' },
    { type: 'inquiry', message: 'New inquiry from parent about admission process', time: '6 hours ago' },
    { type: 'update', message: 'Student profile updated for Mike Johnson', time: '8 hours ago' }
  ];

  constructor() {}

  ngOnInit() {
    // Component initialization
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