import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

// Item with id

export interface TaskResponse {
  tasks: TaskItem[];
}

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  status: string;
}

// input dont have id
export interface TaskInput {
  title: string;
  description: string;
  status: string;
}

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private apiUrl = 'http://localhost:5170/tasks'; // URL da sua API

  constructor(private http: HttpClient, private authService: AuthService) {}

  // Função para obter o cabeçalho de autenticação
  private getAuthHeaders() {
    const token = this.authService.getToken(); // Obtém o token JWT
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`, // Define o cabeçalho Authorization com o token
      'Content-Type': 'application/json',
    });
  }


  getTasks(): Observable<TaskResponse> {
    return this.http.get<TaskResponse>(this.apiUrl, { headers: this.getAuthHeaders() });
  }


  getTaskById(id: number): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }


  addTask(task: TaskInput): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.apiUrl, task, { headers: this.getAuthHeaders() });
  }


  updateTask(id: number, task: TaskItem): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.apiUrl}/${id}`, task, { headers: this.getAuthHeaders() });
  }


  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }
}
