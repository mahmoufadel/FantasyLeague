export interface MatchResultDto {
  matchId: string;
  matchDate: string; // ISO string
  homeTeamId: string;
  awayTeamId: string;
  homeScore: number;
  awayScore: number;
}

export interface CreateMatchResultDto {
  homeTeamId: string;
  awayTeamId: string;
  homeScore: number;
  awayScore: number;
  matchDate?: string | null; // ISO string or null
}
