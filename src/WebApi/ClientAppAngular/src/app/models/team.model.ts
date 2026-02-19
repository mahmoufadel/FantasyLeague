import { PlayerDto } from './player.model';

export interface TeamDto {
  id: string;
  name: string;
  managerName: string;
  budget: number;
  totalPoints: number;
  players: PlayerDto[];
}

export interface CreateTeamDto {
  name: string;
  managerName: string;
}

export interface AddPlayerToTeamDto {
  teamId: string;
  playerId: string;
}
