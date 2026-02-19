import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatchResultsService } from '../services/match-results.service';
import { TeamsService } from '../services/teams.service';
import { firstValueFrom } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-match-results',
  imports: [CommonModule, FormsModule],
  templateUrl: './match-results.component.html',
  styleUrls: ['./match-results.component.scss']
})
export class MatchResultsComponent {
  homeTeam = signal<string | null>(null);
  awayTeam = signal<string | null>(null);
  homeScore = signal<number>(0);
  awayScore = signal<number>(0);
  matchDate = signal<string | null>(null);

  teams = signal<any[]>([]);
  loading = signal(false);

  constructor(private matchService: MatchResultsService, private teamsService: TeamsService) {}

  async ngOnInit() {
    this.loading.set(true);
    try {
      const data = await firstValueFrom(this.teamsService.getAll());
      this.teams.set(data);
    } catch {
      // ignore
    } finally {
      this.loading.set(false);
    }
  }

  async submit() {
    if (!this.homeTeam() || !this.awayTeam()) {
      alert('Please select both teams');
      return;
    }

    const dto = {
      homeTeamId: this.homeTeam()!,
      awayTeamId: this.awayTeam()!,
      homeScore: this.homeScore(),
      awayScore: this.awayScore(),
      matchDate: this.matchDate() ?? new Date().toISOString()
    };

    try {
      this.loading.set(true);
      const result = await firstValueFrom(this.matchService.create(dto));
      alert('Match result created');
      // reset
      this.homeScore.set(0);
      this.awayScore.set(0);
      this.matchDate.set(null);
    } catch (err: any) {
      alert(err?.message ?? 'Failed to create match result');
    } finally {
      this.loading.set(false);
    }
  }
}
