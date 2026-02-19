import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TeamDto, CreateTeamDto, AddPlayerToTeamDto } from '../models/team.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TeamsService {
  private base = environment.apiBaseUrl.replace(/\/$/, '') + '/teams';

  constructor(private http: HttpClient) {}

  getAll(): Observable<TeamDto[]> {
    return this.http.get<TeamDto[]>(this.base);
  }

  getById(id: string): Observable<TeamDto> {
    return this.http.get<TeamDto>(`${this.base}/${id}`);
  }

  create(dto: CreateTeamDto): Observable<TeamDto> {
    return this.http.post<TeamDto>(this.base, dto);
  }

  addPlayer(dto: AddPlayerToTeamDto): Observable<TeamDto> {
    return this.http.post<TeamDto>(`${this.base}/add-player`, dto);
  }

  removePlayer(teamId: string, playerId: string): Observable<TeamDto> {
    return this.http.delete<TeamDto>(`${this.base}/${teamId}/players/${playerId}`);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
