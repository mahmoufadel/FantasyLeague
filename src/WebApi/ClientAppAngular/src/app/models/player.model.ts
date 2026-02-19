export interface PlayerDto {
  id: string;
  name: string;
  position: string;
  club: string;
  price: number;
  points: number;
  goalsScored: number;
  assists: number;
  cleanSheets: number;
}

export interface CreatePlayerDto {
  name: string;
  position: string;
  club: string;
  price: number;
}

export interface UpdatePlayerStatsDto {
  goalsScored: number;
  assists: number;
  cleanSheets: number;
}
