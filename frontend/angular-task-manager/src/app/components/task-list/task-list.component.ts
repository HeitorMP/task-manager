import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../task.service';  // Task service
import { TaskItem, TaskInput } from '../../task.service';  // Task model
import { CommonModule } from '@angular/common';  // To use *ngFor
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../auth.service';
import { Router } from '@angular/router'

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css'],
})

export class TaskListComponent implements OnInit {
  tasks: TaskItem[] = []; // Lista de tarefas
  selectedTask: TaskItem | null = null; // Tarefa selecionada para edição
  newTask: TaskInput = { title: '', description: '', status: 'Pending' }; // Nova tarefa para adicionar
  filteredTasks: any[] = [];

  constructor(
    private taskService: TaskService,
    private authService: AuthService,
    private router: Router
  ) {
    this.loadTasks();
  }

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']); // Redirecionar para login caso não esteja autenticado
    } else {
      this.getTasks();
    }
  }

  getTasks(): void {
  this.taskService.getTasks().subscribe((response) => {
    console.log('Fetched tasks:', response); // Verifique a resposta da API
    if (response && response.tasks) {
      this.tasks = response.tasks; // Acessa a chave 'tasks' do objeto retornado
      this.filteredTasks = this.tasks;  // Inicializa a lista filtrada com as tarefas
    }
  });
}

  addTask(): void {
    if (!this.newTask.title || !this.newTask.description) {
      alert('Please fill in all fields');
      return;
    }

    this.taskService.addTask(this.newTask).subscribe((task) => {
      this.tasks.push(task); // Adiciona a tarefa à lista local após ser criada
      this.filteredTasks = [...this.tasks];  // Atualiza também a lista filtrada
      this.newTask = { title: '', description: '', status: 'Pending' }; // Limpa os campos
    });
  }


  editTask(id: number): void {
    this.selectedTask = this.tasks.find((task) => task.id === id) || null;
  }

  updateTask(): void {
    if (this.selectedTask) {
      this.taskService.updateTask(this.selectedTask.id, this.selectedTask).subscribe(() => {
        this.getTasks();
        this.selectedTask = null;
      });
    }
  }

  deleteTask(id: number): void {
    // Primeiro, remova a tarefa da lista local antes de fazer a requisição ao backend
    this.tasks = this.tasks.filter(task => task.id !== id);
    
    // Agora, faça a requisição para o backend
    this.taskService.deleteTask(id).subscribe(
      () => {
        console.log(`Task with ID ${id} deleted successfully`);
        this.getTasks();  // Recarregar a lista de tarefas para garantir que o estado do backend esteja refletido no frontend
      },
      (error) => {
        console.error('Error deleting task:', error);
        // Reverter a remoção da tarefa caso haja erro
        this.loadTasks();  // Recarregar as tarefas caso a exclusão falhe
      }
    );
  }
  

  filterTasks(status: string) {
    console.log('Current tasks:', this.tasks);
    if (status === '') {
      this.filteredTasks = this.tasks;
    } else {
      this.filteredTasks = this.tasks.filter(task => task.status === status);
    }
  }
  

  loadTasks() {
    this.taskService.getTasks().subscribe(
      (response) => {
        console.log('Fetched tasks:', response);
        if (response && response.tasks) {
          this.tasks = response.tasks;
          this.filteredTasks = this.tasks;
  
          
          if (this.tasks.length === 0) {
            console.log('No tasks available');
            
          }
        }
      },
      (error) => {
       
        if (error.status === 404 && error.error && error.error === 'No tasks found with status: ') {
          console.log('No tasks found');
          this.tasks = [];
          this.filteredTasks = [];
        } else {
          console.error('Error fetching tasks:', error);
        }
      }
    );
  }
  
  
  logout(): void {
    this.authService.logout();
  }
}
