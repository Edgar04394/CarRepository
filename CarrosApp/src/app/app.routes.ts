import { Routes } from '@angular/router';
import { CarroListComponent } from './components/carro-list.component';
import { CarroFormComponent } from './components/carro-form.component';

export const routes: Routes = [
  { path: '', component: CarroListComponent },
  { path: 'nuevo', component: CarroFormComponent },
  { path: 'editar/:id', component: CarroFormComponent },
];
