import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PlayerDto, CreatePlayerDto, UpdatePlayerStatsDto } from '../models/player.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PlayersService {
  private base = environment.apiBaseUrl.replace(/\/$/, '') + '/players';

  constructor(private http: HttpClient) {}

  getAll(): Observable<PlayerDto[]> {
    return this.http.get<PlayerDto[]>(this.base);
  }

  getById(id: string): Observable<PlayerDto> {
    return this.http.get<PlayerDto>(`${this.base}/${id}`);
  }

  getByPosition(position: string): Observable<PlayerDto[]> {
    return this.http.get<PlayerDto[]>(`${this.base}/position/${encodeURIComponent(position)}`);
  }

  create(dto: CreatePlayerDto): Observable<PlayerDto> {
    return this.http.post<PlayerDto>(this.base, dto);
  }

  updateStats(id: string, dto: UpdatePlayerStatsDto): Observable<PlayerDto> {
    return this.http.put<PlayerDto>(`${this.base}/${id}/stats`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
