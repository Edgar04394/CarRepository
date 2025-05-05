import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarroService } from '../services/carro.service';
import { Router, RouterModule } from '@angular/router';
import { Carro } from '../Models/Carro.model';
import { HttpClientModule, provideHttpClient } from '@angular/common/http'; // <-- agrega esto
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-carro-list',
  standalone: true,
  imports: [CommonModule, RouterModule, HttpClientModule],
  providers: [CarroService], 
  templateUrl: './carro-list.component.html',
  styleUrls: ['./carro-list.component.css']
})
export class CarroListComponent {
  carros: Carro[] = [];

  constructor(private carroService: CarroService, private router: Router, private cdRef: ChangeDetectorRef) {
    this.cargarCarros();
  }

  getColorCode(colorName: string): string {
    // Mapeo extendido de nombres de colores a códigos hexadecimales
    const colorMap: {[key: string]: string} = {
      // Colores básicos
      'naranja': '#FFA500',
      'azul': '#0000FF',
      'amarillo': '#FFFF00',
      'rojo': '#FF0000',
      'verde': '#008000',
      'negro': '#000000',
      'blanco': '#FFFFFF',
      'gris': '#808080',
      'beige': '#F5F5DC',
      'veige': '#F5F5DC', // Alternativa para beige
      'plateado': '#C0C0C0',
      'dorado': '#FFD700',
      
      // Nuevos colores solicitados
      'morado': '#800080',
      'lila': '#C8A2C8',
      'rosa': '#FFC0CB',
      'rosado': '#FFC0CB', // Alternativa para rosa
      'cafe': '#A52A2A',
      'marrón': '#A52A2A', // Alternativa para café
      'turquesa': '#40E0D0',
      'verde lima': '#00FF00',
      'verde oliva': '#808000',
      'vino': '#722F37',
      'azul marino': '#000080',
      'azul cielo': '#87CEEB',
      'cyan': '#00FFFF',
      'magenta': '#FF00FF',
      'coral': '#FF7F50',
      'salmón': '#FA8072',
      'oro': '#FFD700',
      'bronce': '#CD7F32'
    };

    // Normalizar el nombre del color
    const normalizedColor = colorName.toLowerCase()
      .trim()
      .normalize('NFD') // Eliminar acentos
      .replace(/[\u0300-\u036f]/g, '') // Eliminar diacríticos
      .replace(/\s+/g, ' '); // Normalizar espacios

    // 1. Buscar coincidencia exacta
    if (colorMap[normalizedColor]) {
      return colorMap[normalizedColor];
    }

    // 2. Buscar coincidencias parciales (por ejemplo "azul claro" encontrará "azul")
    const foundColor = Object.keys(colorMap).find(key => 
      normalizedColor.includes(key)
    );
    if (foundColor) {
      return colorMap[foundColor];
    }

    // 3. Si es un código hexadecimal válido, usarlo directamente
    if (/^#([0-9A-F]{3}){1,2}$/i.test(normalizedColor)) {
      return normalizedColor;
    }

    // 4. Generar un color aleatorio si no se reconoce
    return this.generateRandomColor();
  }

  // Método para generar colores aleatorios como fallback
  private generateRandomColor(): string {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

  cargarCarros() {
    this.carroService.getAll().subscribe({
      next: (data) => {
        this.carros = data;
        this.cdRef.detectChanges(); // Forzar actualización
      }
    });
  }

  crearCarro() {
    this.router.navigate(['/nuevo']);
  }

  editarCarro(id: number) {
    this.router.navigate(['/editar', id]);
  }

  eliminarCarro(id: number) {
    if (confirm('¿Estás seguro de eliminar este carro?')) {
      this.carroService.delete(id).subscribe(() => {
        this.cargarCarros();
      });
    }
  }
}
