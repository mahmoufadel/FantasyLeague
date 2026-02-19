import { Component, OnInit,signal,ChangeDetectionStrategy,effect  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlayersService } from '../services/players.service';
import { PlayerDto } from '../models/player.model';
import { firstValueFrom } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-players',
  imports: [CommonModule, FormsModule],
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlayersComponent implements OnInit {
  players = signal<PlayerDto[]>([]);

  loading = false;
  error: string | null = null;


  constructor(private playersService: PlayersService) { 
effect(() => {
    console.log('players changed:', this.players().length);
  });

  }

  async ngOnInit(): Promise<void> {
   
    await this.loadPlayers();
  }

  async loadPlayers(): Promise<void> {
    this.loading = true;
    this.error = null;
    try {
      const data = await firstValueFrom(this.playersService.getAll());
      this.loading = false;
      this.players.set( data);
      console.log('Players loaded:', this.players);
    } catch (err: any) {
      this.error = err?.message ?? 'Failed to load players';
    } finally {
      this.loading = false;
    }
  }

  async deletePlayer(id: string) {
    if (!confirm('Delete player?')) return;
    try {
      await firstValueFrom(this.playersService.delete(id));
      this.players.set(this.players().filter(p => p.id !== id));
    } catch {
      alert('Failed to delete player');
    }
  }
}
