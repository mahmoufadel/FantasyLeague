import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MatchResultDto, CreateMatchResultDto } from '../models/match.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MatchResultsService {
  private base = environment.apiBaseUrl.replace(/\/$/, '') + '/matchresults';

  constructor(private http: HttpClient) {}

  create(dto: CreateMatchResultDto): Observable<MatchResultDto> {
    return this.http.post<MatchResultDto>(this.base, dto);
  }
}
