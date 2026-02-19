import { Routes } from '@angular/router';
import { PlayersComponent } from './players/players.component';
import { TeamsComponent } from './teams/teams.component';
import { MatchResultsComponent } from './match-results/match-results.component';

export const routes: Routes = [
  { path: 'players', component: PlayersComponent },
  { path: 'teams', component: TeamsComponent },
  { path: 'match-results', component: MatchResultsComponent },
  { path: '', redirectTo: 'players', pathMatch: 'full' }
];
