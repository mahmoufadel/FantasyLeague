import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeamsService } from '../services/teams.service';
import { TeamDto } from '../models/team.model';
import { PlayersService } from '../services/players.service';
import { PlayerDto } from '../models/player.model';
import { firstValueFrom } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-teams',
  imports: [CommonModule, FormsModule],
  templateUrl: './teams.component.html',
  styleUrls: ['./teams.component.scss']
})
export class TeamsComponent implements OnInit {
  teams = signal<TeamDto[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  // for create form
  newName = signal('');
  newManager = signal('');

  // players to add
  availablePlayers = signal<PlayerDto[]>([]);

  constructor(private teamsService: TeamsService, private playersService: PlayersService) {}

  async ngOnInit(): Promise<void> {
    await this.loadAll();
    await this.loadPlayers();
  }

  async loadAll() {
    this.loading.set(true);
    this.error.set(null);
    try {
      const data = await firstValueFrom(this.teamsService.getAll());
      this.loading.set(false);
      this.teams.set(data);
      console.log('Teams loaded:', this.teams());
    } catch (err: any) {
      this.error.set(err?.message ?? 'Failed to load teams');
    } finally {
      this.loading.set(false);
    }
  }

  async loadPlayers() {
    try {
      const data = await firstValueFrom(this.playersService.getAll());
      this.availablePlayers.set(data);
    } catch {
      // ignore
    }
  }

  async createTeam() {
    try {
      const dto = { name: this.newName(), managerName: this.newManager() };
      const team = await firstValueFrom(this.teamsService.create(dto));
      this.teams.set([...(this.teams()), team]);
      this.newName.set('');
      this.newManager.set('');
    } catch (err: any) {
      alert(err?.message ?? 'Failed to create team');
    }
  }

  async addPlayer(teamId: string, playerId: string) {
    try {
      const dto = { teamId, playerId };
      const team = await firstValueFrom(this.teamsService.addPlayer(dto));
      // replace team in teams list
      this.teams.set(this.teams().map(t => t.id === team.id ? team : t));
    } catch (err: any) {
      alert(err?.message ?? 'Failed to add player');
    }
  }

  async removePlayer(teamId: string, playerId: string) {
    try {
      const team = await firstValueFrom(this.teamsService.removePlayer(teamId, playerId));
      this.teams.set(this.teams().map(t => t.id === team.id ? team : t));
    } catch (err: any) {
      alert(err?.message ?? 'Failed to remove player');
    }
  }

  async deleteTeam(id: string) {
    if (!confirm('Delete team?')) return;
    try {
      await firstValueFrom(this.teamsService.delete(id));
      this.teams.set(this.teams().filter(t => t.id !== id));
    } catch {
      alert('Failed to delete team');
    }
  }
}
