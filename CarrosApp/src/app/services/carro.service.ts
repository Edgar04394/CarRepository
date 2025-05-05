import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Carro } from '../Models/Carro.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})


export class CarroService {
  private apiUrl = 'http://localhost:5126/api/Carros'; // Ajusta el puerto si es necesario

  constructor(private http: HttpClient) {}

  getAll(): Observable<Carro[]> {
    return this.http.get<Carro[]>(this.apiUrl);
  }

  getById(id: number): Observable<Carro> {
    return this.http.get<Carro>(`${this.apiUrl}/${id}`);
  }

  create(carro: Carro): Observable<Carro> {
    return this.http.post<Carro>(this.apiUrl, carro);
  }

  update(carro: Carro): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${carro.id}`, carro);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
