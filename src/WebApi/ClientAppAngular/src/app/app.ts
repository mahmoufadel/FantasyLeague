import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PlayersComponent } from './players/players.component';
import { TeamsComponent } from './teams/teams.component';
import { MatchResultsComponent } from './match-results/match-results.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('ClientAppAngular');
}
