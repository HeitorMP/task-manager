
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TaskListComponent } from './components/task-list/task-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, TaskListComponent],
  template: `
    <router-outlet></router-outlet>  <!-- Aqui será carregado o conteúdo da rota -->
  `,
})
export class AppComponent {}
