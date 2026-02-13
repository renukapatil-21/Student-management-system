import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService, User } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
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

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  getGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good morning';
    if (hour < 17) return 'Good afternoon';
    return 'Good evening';
  }
}