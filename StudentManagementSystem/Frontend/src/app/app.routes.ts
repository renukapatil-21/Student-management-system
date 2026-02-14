import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { StudentsListComponent } from './features/students/students-list.component';
import { AddStudentComponent } from './features/students/add-student/add-student.component';
import { FeesComponent } from './features/fees/fees.component';
import { InquiriesComponent } from './features/inquiries/inquiries.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'students', component: StudentsListComponent },
  { path: 'add-student', component: AddStudentComponent },
  { path: 'fees', component: FeesComponent },
  { path: 'inquiries', component: InquiriesComponent },
  { path: '**', redirectTo: '/login' }
];
