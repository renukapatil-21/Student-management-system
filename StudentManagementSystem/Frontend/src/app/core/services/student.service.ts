import { Injectable } from '@angular/core';
import { ApiService, ApiResponse, ApiPagedResponse } from './api.service';
import { Observable } from 'rxjs';

export interface Student {
  id: string;
  studentId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  gender: string;
  address: string;
  department: string;
  course: string;
  academicYear: string;
  admissionDate: string;
  status: string;
  guardianName?: string;
  guardianPhone?: string;
  guardianEmail?: string;
  bloodGroup?: string;
  emergencyContact?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateStudentRequest {
  studentId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  gender: string;
  address: string;
  department: string;
  course: string;
  academicYear: string;
  admissionDate: string;
  guardianName?: string;
  guardianPhone?: string;
  guardianEmail?: string;
  bloodGroup?: string;
  emergencyContact?: string;
}

export interface UpdateStudentRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  address: string;
  department: string;
  course: string;
  academicYear: string;
  status: string;
  guardianName?: string;
  guardianPhone?: string;
  guardianEmail?: string;
  bloodGroup?: string;
  emergencyContact?: string;
}

export interface StudentSearchCriteria {
  studentId?: string;
  name?: string;
  email?: string;
  department?: string;
  academicYear?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class StudentService {

  constructor(private apiService: ApiService) {}

  getStudents(criteria?: StudentSearchCriteria): Observable<ApiPagedResponse<Student>> {
    return this.apiService.getPagedData<Student>('students', criteria);
  }

  getStudent(id: string): Observable<ApiResponse<Student>> {
    return this.apiService.get<Student>(`students/${id}`);
  }

  createStudent(student: CreateStudentRequest): Observable<ApiResponse<Student>> {
    return this.apiService.post<Student>('students', student);
  }

  updateStudent(id: string, student: UpdateStudentRequest): Observable<ApiResponse<Student>> {
    return this.apiService.put<Student>(`students/${id}`, student);
  }

  deleteStudent(id: string): Observable<ApiResponse<void>> {
    return this.apiService.delete<void>(`students/${id}`);
  }
}