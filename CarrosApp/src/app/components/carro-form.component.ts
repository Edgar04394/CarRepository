import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarroService } from '../services/carro.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Carro } from '../Models/Carro.model';

@Component({
  selector: 'app-carro-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: `./carro-form.component.html`,
  styleUrls: ['./carro-form.component.css']
})

export class CarroFormComponent {
  carro: Carro = {
    id: 0,
    marca: '',
    modelo: '',
    anio: 0,
    color: '',
    precio: 0
  };

  esEditar = false;

  constructor(
    private carroService: CarroService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.esEditar = true;
      this.carroService.getById(+id).subscribe(carro => {
        this.carro = carro;
      });
    }
  }

  guardar() {
    if (this.esEditar) {
      this.carroService.update(this.carro).subscribe(() => {
        this.router.navigate(['/']);
      });
    } else {
      this.carroService.create(this.carro).subscribe(() => {
        this.router.navigate(['/']);
      });
    }
  }
}
