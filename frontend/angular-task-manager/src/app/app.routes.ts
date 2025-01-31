import { Routes } from '@angular/router';
import { TaskListComponent } from './components/task-list/task-list.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';

export const routes: Routes = [
  { path: '', component: LoginComponent }, // ✅ Agora a página inicial é o login
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }, // ✅ Adicionando a rota de registro
  { path: 'tasks', component: TaskListComponent }
];
