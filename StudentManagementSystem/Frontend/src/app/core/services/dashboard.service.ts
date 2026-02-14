import { Injectable } from '@angular/core';
import { ApiService, ApiResponse } from './api.service';
import { Observable } from 'rxjs';

export interface DashboardStats {
  totalStudents: number;
  newAdmissions: number;
  feesCollected: number;
  pendingInquiries: number;
  lastUpdated: string;
}

export interface RecentActivity {
  type: string;
  message: string;
  time: string;
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private apiService: ApiService) {}

  getDashboardStats(): Observable<ApiResponse<DashboardStats>> {
    return this.apiService.get<DashboardStats>('dashboard/stats');
  }

  getRecentActivities(): Observable<ApiResponse<RecentActivity[]>> {
    return this.apiService.get<RecentActivity[]>('dashboard/recent-activities');
  }
}