import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { StudentsListComponent } from './features/students/students-list.component';
import { AddStudentComponent } from './features/students/add-student/add-student.component';
import { FeesComponent } from './features/fees/fees.component';
import { InquiriesComponent } from './features/inquiries/inquiries.component';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'students', component: StudentsListComponent },
  { path: 'add-student', component: AddStudentComponent },
  { path: 'fees', component: FeesComponent },
  { path: 'inquiries', component: InquiriesComponent },
  { path: '**', redirectTo: '/dashboard' }
];
